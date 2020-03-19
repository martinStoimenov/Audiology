namespace Audiology.Web.Controllers
{
    using System;
    using System.Collections.Generic;
    using System.Linq;
    using System.Threading.Tasks;
    using Audiology.Data.Models;
    using Audiology.Services.Data.Albums;
    using Audiology.Web.ViewModels.Albums;
    using Microsoft.AspNetCore.Identity;
    using Microsoft.AspNetCore.Mvc;

    public class AlbumsController : Controller
    {
        private readonly IAlbumsService albumsRepository;
        private readonly UserManager<ApplicationUser> userManager;

        public AlbumsController(IAlbumsService albumsRepository, UserManager<ApplicationUser> userManager)
        {
            this.albumsRepository = albumsRepository;
            this.userManager = userManager;
        }

        public IActionResult Index()
        {
            return this.View();
        }

        public IActionResult Upload()
        {
            return this.View();
        }

        [HttpPost]
        public async Task<IActionResult> Upload(AlbumUploadViewModel input)
        {
            var userId = this.userManager.GetUserId(this.User);

            await this.albumsRepository.AddAsync(input.Name, input.CoverUrl, input.Description, input.Producer, userId);

            return this.RedirectToAction("Index");
        }
    }
}