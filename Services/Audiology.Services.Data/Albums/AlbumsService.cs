namespace Audiology.Services.Data.Albums
{
    using System;
    using System.Collections.Generic;
    using System.Linq;
    using System.Threading.Tasks;

    using Audiology.Data.Common.Repositories;
    using Audiology.Data.Models;
    using Audiology.Services.Mapping;
    using Audiology.Web.ViewModels.Albums;
    using Audiology.Web.ViewModels.Songs;
    using Microsoft.AspNetCore.Identity;

    public class AlbumsService : IAlbumsService
    {
        private readonly IRepository<Album> repository;
        private readonly IRepository<UsersAlbum> usersAlbumRepository;
        private readonly IRepository<Song> songRepository;

        public AlbumsService(IRepository<Album> repository, IRepository<UsersAlbum> usersAlbumRepository, IRepository<Song> songRepository)
        {
            this.repository = repository;
            this.usersAlbumRepository = usersAlbumRepository;
            this.songRepository = songRepository;
        }

        public IEnumerable<T> GetAllForUser<T>(string userId)
        {
            var result = this.repository.All().Where(a => a.UsersAlbum.Any(ua => ua.UserId == userId)).To<T>().ToList();

            return result;
        }

        public async Task<int> AddAsync(string name, string coverUrl, string description, string producer, string userId, DateTime? releaseDate)
        {
            var album = new Album
            {
                Name = name,
                CoverUrl = coverUrl,
                Description = description,
                Producer = producer,
                ReleaseDate = releaseDate,
            };
            // Add check for the album name
            await this.repository.AddAsync(album);
            await this.repository.SaveChangesAsync();

            var usersAlbum = new UsersAlbum
            {
                AlbumId = album.Id,
                UserId = userId,
            };

            await this.usersAlbumRepository.AddAsync(usersAlbum);
            await this.usersAlbumRepository.SaveChangesAsync();

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

        public AlbumViewModel GetCurrentAlbumById(int albumId)
        {
            var album = this.repository.All().Where(a => a.Id == albumId).To<AlbumViewModel>().FirstOrDefault();

            if (album.Songs.Count() > 0)
            {
                var songs = this.songRepository.All().Where(s => s.AlbumId == albumId).To<SongListViewModel>().ToList();
                album.Songs = songs;
            }

            return album;
        }
    }
}
