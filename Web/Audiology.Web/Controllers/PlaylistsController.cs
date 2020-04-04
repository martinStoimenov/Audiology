﻿namespace Audiology.Web.Controllers
{
    using System.Threading.Tasks;

    using Audiology.Data.Models;

    using Audiology.Services.Data.Playlists;
    using Audiology.Web.ViewModels.Playlists;
    using Audiology.Web.ViewModels.Songs;
    using Microsoft.AspNetCore.Identity;
    using Microsoft.AspNetCore.Mvc;

    public class PlaylistsController : Controller
    {
        private readonly IPlaylistsService service;
        private readonly UserManager<ApplicationUser> userManager;

        public PlaylistsController(IPlaylistsService service, UserManager<ApplicationUser> userManager)
        {
            this.service = service;
            this.userManager = userManager;
        }

        [HttpPost]
        public async Task<IActionResult> Create(string name, int songId, bool isPrivate)
        {
            var userId = this.userManager.GetUserId(this.User);

            await this.service.CreateAsync(name, userId, songId, isPrivate);

            return this.RedirectToAction("Index", "Songs");
        }

        [HttpPost]
        public async Task<IActionResult> Add(int playlistId, int songId)
        {
            var userId = this.userManager.GetUserId(this.User);

            await this.service.AddAsync(userId, playlistId, songId);

            return this.RedirectToAction("Index", "Songs");
        }

        public async Task<IActionResult> Index()
        {
            var userId = this.userManager.GetUserId(this.User);

            var playlists = await this.service.GetAllPlaylistsAsync<PlaylistViewModel>(userId);

            return this.View(playlists);
        }

        public async Task<IActionResult> ById(int id)
        {
            var userId = this.userManager.GetUserId(this.User);

            var songsInPlaylist = await this.service.GetAllSongsInPlaylistAsync<SongListViewModel>(userId, id);

            return this.View(songsInPlaylist);
        }
    }
}
