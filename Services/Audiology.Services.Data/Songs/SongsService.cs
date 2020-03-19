namespace Audiology.Services.Data.Songs
{
    using System.Collections.Generic;
    using System.IO;
    using System.Linq;
    using System.Threading.Tasks;

    using Audiology.Data;
    using Audiology.Data.Common.Repositories;
    using Audiology.Data.Models;
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

        public async Task UploadAsync(IFormFile input, string username)
        {
            //var dotIndex = input.FileName.IndexOf('.');
            //var fileExtension = input.FileName.Substring(dotIndex, input.FileName.Length);

            //if (input.ToString().ToLower() != fileExtension.ToLower())
            //{
                
            //}


            // Add folder with username
            string webRootPath = this.env.WebRootPath + "\\Songs\\";

            if (!Directory.Exists(webRootPath + username))
            {
                Directory.CreateDirectory(webRootPath + username);
            }

            var filePath = Path.Combine(webRootPath, username, input.FileName);

            using (var fileStream = new FileStream(filePath, FileMode.Create))
            {
                await input.CopyToAsync(fileStream);
            }
        }
    }
}
