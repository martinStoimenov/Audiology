namespace Audiology.Services.Data.Songs
{
    using System;
    using System.Collections.Generic;
    using System.IO;
    using System.Linq;
    using System.Threading.Tasks;

    using Audiology.Data;
    using Audiology.Data.Common.Repositories;
    using Audiology.Data.Models;
    using Audiology.Data.Models.Enumerations;
    using Audiology.Services.Mapping;
    using Audiology.Web.ViewModels.Songs;
    using Microsoft.AspNetCore.Hosting;
    using Microsoft.AspNetCore.Http;
    using Microsoft.AspNetCore.Identity;
    using Microsoft.EntityFrameworkCore;

    public class SongsService : ISongsServcie
    {
        private readonly IHostingEnvironment env;
        private readonly UserManager<ApplicationUser> userManager;
        private readonly IRepository<Song> songRepository;

        public SongsService(
            IHostingEnvironment env,
            UserManager<ApplicationUser> userManager,
            IRepository<Song> songRepository)
        {
            this.env = env;
            this.userManager = userManager;
            this.songRepository = songRepository;
        }

        public IEnumerable<T> GetAll<T>(int? count = null)
        {
            IQueryable<Song> query =
                this.songRepository.All().OrderBy(x => x.CreatedOn).ThenBy(x => x.ModifiedOn);
            if (count.HasValue)
            {
                query = query.Take(count.Value);
            }

            return query.To<T>().ToList();
        }

        public Task<IEnumerable<SongListViewModel>> GetAllSongsForUserAsync(string userId)
        {
            return null;
        }

        public async Task UploadAsync(IFormFile input, string username, string songName, string description,  int? albumId, Enum genre, int year)
        {
            var dotIndex = input.FileName.LastIndexOf('.');
            var fileExtension = input.FileName.Substring(dotIndex);
            var originalFileName = input.FileName.Substring(0, dotIndex);

            string webRootPath = this.env.WebRootPath + "\\Songs\\";

            if (!Directory.Exists(webRootPath + username))
            {
                Directory.CreateDirectory(webRootPath + username);
            }

            if (songName.Length > 50 || songName == null)
            {
                throw new ArgumentOutOfRangeException("Song name cannot be more than 50 characters!");
            }

            if (description.Length > 100)
            {
                throw new ArgumentOutOfRangeException("Description cannot be more than 100 characters!");
            }

            if (year > 2020)
            {
                throw new ArgumentOutOfRangeException("Year cannot be more than the current one!");
            }

            string name = songName + fileExtension;

            var filePath = Path.Combine(webRootPath, username, name);

            using (var fileStream = new FileStream(filePath, FileMode.Create))
            {
                await input.CopyToAsync(fileStream);
            }

            var song = new Song
            {
                Name = name,
                Description = description,
                Year = year,
                AlbumId = albumId,
                Genre = (Genre)genre,
            };

            await this.songRepository.AddAsync(song);
            await this.songRepository.SaveChangesAsync();
        }
    }
}
