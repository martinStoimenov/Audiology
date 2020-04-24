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
    using Microsoft.AspNetCore.Authorization;
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

        public async Task<IActionResult> Index()
        {
            string userId = this.userManager.GetUserId(this.User);
            var viewModel = await this.albumsService.GetAllForUser<AlbumsListViewModel>(userId);

            return this.View(viewModel);
        }

        public IActionResult Upload()
        {
            return this.View();
        }

        [HttpPost]
        [Authorize(Roles = "Artist")]
        public async Task<IActionResult> Upload(AlbumUploadViewModel input)
        {
            if (!this.ModelState.IsValid)
            {
                return this.View();
            }

            var userId = this.userManager.GetUserId(this.User);

            var albumId = await this.albumsService.AddAsync(input.Name, input.CoverUrl, input.Description, input.Producer, userId, input.ReleaseDate, input.Genre);

            return this.RedirectToAction(nameof(this.ById), new { id = albumId });
        }

        public async Task<IActionResult> ById(int id)
        {
            if (!this.ModelState.IsValid)
            {
                return this.View();
            }

            var album = this.albumsService.GetCurrentAlbumById<AlbumViewModel>(id);

            var songs = await this.songsService.GetSongsByAlbumAsync<SongListViewModel>(id);

            album.Songs = songs;

            return this.View(album);
        }

        [HttpPost]
        [Authorize(Roles = "Administrator,Artist")]
        public async Task<IActionResult> Edit(AlbumViewModel input)
        {
            if (!this.ModelState.IsValid)
            {
                return this.View();
            }

            var albumId = await this.albumsService.EditAlbumAsync(input.Id, input.Name, input.Description, input.Producer, input.CoverUrl, input.Genre, input.ReleaseDate);

            return this.RedirectToAction(nameof(this.ById), new { id = albumId });
        }

        [HttpPost]
        [Authorize(Roles = "Administrator,Artist")]
        public async Task<IActionResult> Delete(int id)
        {
            if (!this.ModelState.IsValid)
            {
                return this.View();
            }

            await this.albumsService.DeleteAlbum(id);

            return this.RedirectToAction(nameof(this.Index));
        }
    }
}