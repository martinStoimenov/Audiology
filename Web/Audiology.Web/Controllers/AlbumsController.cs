namespace Audiology.Web.Controllers
{
    using System;
    using System.Collections.Generic;
    using System.Linq;
    using System.Threading.Tasks;

    using Audiology.Data.Models;
    using Audiology.Services.Data.Albums;
    using Audiology.Services.Data.Songs;
    using Audiology.Web.ViewModels.Albums;
    using Audiology.Web.ViewModels.Songs;
    using Microsoft.AspNetCore.Identity;
    using Microsoft.AspNetCore.Mvc;

    public class AlbumsController : Controller
    {
        private readonly IAlbumsService albumsService;
        private readonly ISongsServcie songsService;
        private readonly UserManager<ApplicationUser> userManager;

        public AlbumsController(IAlbumsService albumsService, ISongsServcie songsService, UserManager<ApplicationUser> userManager)
        {
            this.albumsService = albumsService;
            this.songsService = songsService;
            this.userManager = userManager;
        }

        public IActionResult Index()
        {
            string userId = this.userManager.GetUserId(this.User);
            var viewModel = this.albumsService.GetAllForUser<AlbumsListViewModel>(userId);

            return this.View(viewModel);
        }

        public IActionResult Upload()
        {
            return this.View();
        }

        [HttpPost]
        public async Task<IActionResult> Upload(AlbumUploadViewModel input)
        {
            var userId = this.userManager.GetUserId(this.User);

            var albumId = await this.albumsService.AddAsync(input.Name, input.CoverUrl, input.Description, input.Producer, userId, input.ReleaseDate);

            return this.RedirectToAction(nameof(this.ById), new { id = albumId });
        }

        public async Task<IActionResult> ById(int id)
        {
            var album = this.albumsService.GetCurrentAlbumById<AlbumViewModel>(id);

            var songs = await this.songsService.GetSongsByAlbumAsync<SongListViewModel>(id);

            album.Songs = songs;

            return this.View(album);
        }

        public async Task<IActionResult> Edit(int id)
        {
            var album = this.albumsService.GetCurrentAlbumById<AlbumEditViewModel>(id);

            return this.View(album);
        }

        [HttpPost]
        public async Task<IActionResult> Edit(AlbumEditViewModel input)
        {
            var albumId = await this.albumsService.EditAlbumAsync(input.Id, input.Name, input.Description, input.Producer, input.CoverUrl, input.Genre, input.ReleaseDate);

            return this.RedirectToAction(nameof(this.ById), new { id = albumId});
        }
    }
}