namespace Audiology.Services.Data.Albums
{
    using System;
    using System.Collections.Generic;
    using System.Linq;
    using System.Threading.Tasks;

    using Audiology.Data.Common.Repositories;
    using Audiology.Data.Models;
    using Audiology.Data.Models.Enumerations;
    using Audiology.Services.Data.Songs;
    using Audiology.Services.Mapping;
    using Audiology.Web.ViewModels.Albums;
    using Audiology.Web.ViewModels.Songs;
    using Microsoft.AspNetCore.Identity;
    using Microsoft.EntityFrameworkCore;

    public class AlbumsService : IAlbumsService
    {
        private readonly IRepository<Album> repository;
        private readonly IRepository<Song> songRepository;
        private readonly ISongsServcie songsServcie;

        public AlbumsService(
            IRepository<Album> repository,
            IRepository<Song> songRepository,
            ISongsServcie songsServcie)
        {
            this.repository = repository;
            this.songRepository = songRepository;
            this.songsServcie = songsServcie;
        }

        public async Task<IEnumerable<T>> GetAllForUser<T>(string userId)
        {
            var result = await this.repository.All().Where(a => a.UserId == userId).OrderByDescending(a => a.CreatedOn).To<T>().ToListAsync();

            return result;
        }

        public async Task<int> AddAsync(string name, string coverUrl, string description, string producer, string userId, DateTime? releaseDate)
        {
            var album = new Album
            {
                UserId = userId,
                Name = name,
                CoverUrl = coverUrl,
                Description = description,
                Producer = producer,
                ReleaseDate = releaseDate,
            };
            // Add check for the album name
            await this.repository.AddAsync(album);
            await this.repository.SaveChangesAsync();

            return album.Id;
        }

        public async Task<int> EditAlbumAsync(int id, string name, string description, string producer, string coverUrl, Enum genre, DateTime? releaseDate)
        {
            var album = await this.repository.All().Where(a => a.Id == id).FirstOrDefaultAsync();

            if (album != null)
            {
                if (name.Length <= 200 && name != null)
                {
                    album.Name = name;
                }

                if (description.Length <= 1000 && description != null)
                {
                    album.Description = description;
                }

                if (producer.Length <= 150 && producer != null)
                {
                    album.Producer = producer;
                }

                if (coverUrl.Length <= 500 && coverUrl != null)
                {
                    album.CoverUrl = coverUrl;
                }

                if (Enum.IsDefined(typeof(Genre), genre))
                {
                    album.Genre = (Genre)genre;
                }

                DateTime temp;
                if (DateTime.TryParse(releaseDate.ToString(), out temp))
                {
                    album.ReleaseDate = temp;
                }
            }

            this.repository.Update(album);
            await this.repository.SaveChangesAsync();

            return album.Id;
        }

        public IEnumerable<T> GetAll<T>(int? count = null)
        {
            IQueryable<Album> query =
                this.repository.All().OrderBy(x => x.CreatedOn).ThenBy(x => x.ModifiedOn);
            if (count.HasValue)
            {
                query = query.Take(count.Value);
            }

            return query.To<T>().ToList();
        }

        public T GetCurrentAlbumById<T>(int albumId)
        {
            var album = this.repository.All().Where(a => a.Id == albumId).To<T>().FirstOrDefault();

            return album;
        }

        public async Task DeleteAlbum(int albumId)
        {
            var album = await this.repository.All().Where(a => a.Id == albumId).FirstOrDefaultAsync();
            var songsIds = await this.songRepository.All().Where(s => s.AlbumId == albumId).Select(s => s.Id).ToListAsync();

            if (album != null)
            {
                foreach (var songId in songsIds)
                {
                    await this.songsServcie.DeleteSong(songId);
                }

                this.repository.Delete(album);
                await this.repository.SaveChangesAsync();
            }
        }

        public async Task<IEnumerable<T>> NewestAlbumsAsync<T>()
        {
            var albums = await this.repository.All().OrderByDescending(a => a.CreatedOn).Take(12).To<T>().ToListAsync();

            return albums;
        }

        public async Task<IEnumerable<T>> TopAlbumsForUser<T>(string userId)
        {
            var albums = await this.repository.All().Where(a => a.UserId == userId).OrderByDescending(a => a.FavouritesCount).Take(3).To<T>().ToListAsync();

            return albums;
        }
    }
}
