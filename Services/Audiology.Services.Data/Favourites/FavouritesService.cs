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

        public FavouritesService(IDeletableEntityRepository<Favourites> repository, IDeletableEntityRepository<Song> songRepository)
        {
            this.repository = repository;
            this.songRepository = songRepository;
        }

        public int GetCount(int? songId, int? albumId)
        {
            var favourites = this.repository.All().Where(f => f.SongId == songId && f.AlbumId == albumId).Select(f => f.Id);

            return favourites.Count();
        }

        public async Task FavouritedAsync(int? songId, int? albumId, string userId)
        {
            var favourite = await this.repository.All().Where(f => f.SongId == songId && f.AlbumId == albumId && f.UserId == userId).FirstOrDefaultAsync();
            var song = await this.songRepository.All().Where(s => s.Id == songId).FirstOrDefaultAsync();

            if (favourite == null)
            {
                var liked = new Favourites
                {
                    SongId = songId,
                    AlbumId = albumId,
                    UserId = userId,
                };

                await this.repository.AddAsync(liked);

                if (song.FavouritesCount == null)
                {
                    song.FavouritesCount = 0;
                }

                song.FavouritesCount += 1;
            }
            else if (favourite.SongId == songId && favourite.AlbumId == albumId)
            {
                this.repository.HardDelete(favourite);
                song.FavouritesCount -= 1;
            }

            this.songRepository.Update(song);

            await this.songRepository.SaveChangesAsync();
            await this.repository.SaveChangesAsync();
        }

        public async Task<IEnumerable<T>> GetAllAsync<T>(string userId)
        {
            var allFavourited = await this.repository.All().Where(f => f.UserId == userId).To<T>().ToListAsync();

            return allFavourited;
        }
    }
}
