namespace Audiology.Services.Data.Favourites
{
    using System.Collections.Generic;
    using System.Linq;
    using System.Threading.Tasks;

    using Audiology.Data.Common.Repositories;
    using Audiology.Data.Models;
    using Audiology.Services.Mapping;
    using Microsoft.EntityFrameworkCore;

    public class FavouritesService : IFavouritesService
    {
        private readonly IDeletableEntityRepository<Favourites> repository;
        private readonly IDeletableEntityRepository<Song> songRepository;
        private readonly IDeletableEntityRepository<Album> albumRepository;

        public FavouritesService(
            IDeletableEntityRepository<Favourites> repository,
            IDeletableEntityRepository<Song> songRepository,
            IDeletableEntityRepository<Album> albumRepository)
        {
            this.repository = repository;
            this.songRepository = songRepository;
            this.albumRepository = albumRepository;
        }

        public int GetCount(int? songId, int? albumId)
        {
            var favourites = this.repository.All().Where(f => f.SongId == songId && f.AlbumId == albumId).Select(f => f.Id);

            return favourites.Count();
        }

        public async Task FavouritedAsync(int? songId, int? albumId, string userId)
        {
            var favourite = await this.repository.All().Where(f => f.SongId == songId && f.AlbumId == albumId && f.UserId == userId).FirstOrDefaultAsync();

            if (favourite == null)
            {
                var liked = new Favourites
                {
                    SongId = songId,
                    AlbumId = albumId,
                    UserId = userId,
                };

                await this.repository.AddAsync(liked);

                if (songId != null)
                {
                    var song = await this.songRepository.All().Where(s => s.Id == songId).FirstOrDefaultAsync();
                    if (song.FavouritesCount == null)
                    {
                        song.FavouritesCount = 0;
                    }

                    song.FavouritesCount += 1;
                }

                if (albumId != null)
                {
                    var album = await this.albumRepository.All().Where(a => a.Id == albumId).FirstOrDefaultAsync();

                    album.FavouritesCount += 1;
                }
            }
            else if (favourite.SongId == songId && favourite.AlbumId == albumId)
            {
                if (songId != null)
                {
                    var song = await this.songRepository.All().Where(s => s.Id == songId).FirstOrDefaultAsync();

                    song.FavouritesCount -= 1;

                    this.songRepository.Update(song);
                    await this.songRepository.SaveChangesAsync();
                }

                if (albumId != null)
                {
                    var album = await this.albumRepository.All().Where(a => a.Id == albumId).FirstOrDefaultAsync();

                    album.FavouritesCount -= 1;

                    this.albumRepository.Update(album);
                    await this.albumRepository.SaveChangesAsync();
                }

                this.repository.HardDelete(favourite);
            }

            await this.repository.SaveChangesAsync();
        }

        public async Task<IEnumerable<T>> GetAllAsync<T>(string userId)
        {
            var allFavourited = await this.repository.All().Where(f => f.UserId == userId).To<T>().ToListAsync();

            return allFavourited;
        }
    }
}
