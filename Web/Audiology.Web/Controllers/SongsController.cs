namespace Audiology.Web.Controllers
{
    using System;
    using System.Collections.Generic;
    using System.IO;
    using System.Linq;
    using System.Threading.Tasks;
    using Audiology.Data.Models;
    using Audiology.Services.Data.Albums;
    using Audiology.Services.Data.Songs;
    using Audiology.Web.ViewModels.Albums;
    using Audiology.Web.ViewModels.Songs;
    using Microsoft.AspNetCore.Http;
    using Microsoft.AspNetCore.Identity;
    using Microsoft.AspNetCore.Mvc;

    public class SongsController : Controller
    {
        private readonly ISongsServcie songsService;
        private readonly IAlbumsService albumsService;
        private readonly UserManager<ApplicationUser> userManager;

        public SongsController(ISongsServcie songsService, IAlbumsService albumsService, UserManager<ApplicationUser> userManager)
        {
            this.songsService = songsService;
            this.albumsService = albumsService;
            this.userManager = userManager;
        }

        public ActionResult ById(int id)
        {
            return this.View();
        }

        // GET: Songs
        public ActionResult Index()
        {
            string userName = this.User.Identity.Name;
            string userId = this.userManager.GetUserId(this.User);
            var songs = this.songsService.GetAllSongsForUserAsync<SongListViewModel>(userId);

            foreach (var song in songs)
            {
                var songDuration = this.songsService.GetMediaDuration(song.Name, userName);
                song.SongDuration = songDuration;
            }

            return this.View(songs);
        }

        public ActionResult All()
        {
            string userName = this.User.Identity.Name;
            string userId = this.userManager.GetUserId(this.User);
            var songs = this.songsService.GetAllSongsForUserAsync<SongListViewModel>(userId);

            foreach (var song in songs)
            {
                var songDuration = this.songsService.GetMediaDuration(song.Name, userName);
                song.SongDuration = songDuration;
            }

            return this.View(songs);
        }

        // GET: Songs/Details/5
        public ActionResult Details(int id)
        {
            return this.View();
        }

        // GET: Songs/Upload
        public ActionResult Upload()
        {
            var albums = this.albumsService.GetAllForUser<AlbumDropDownViewModel>(this.userManager.GetUserId(this.User));
            var viewModel = new SongUploadViewModel
            {
                Albums = albums,
            };
            return this.View(viewModel);
        }

        // POST: Songs/Upload
        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Upload(SongUploadViewModel input) // Add validation for error 404.13 data length
        {
            if (!this.ModelState.IsValid)
            {
                return this.View(input);
            }

            string userId = this.userManager.GetUserId(this.User);

            var songId = await this.songsService.UploadAsync(input.Song, this.User.Identity.Name, input.Name, input.Description, input.AlbumId, input.Genre, input.Year, userId, input.SongArtUrl);

            return this.RedirectToAction(nameof(this.ById), new { id = songId });
        }

        // GET: Songs/Edit/5
        public ActionResult Edit(int id)
        {
            return this.View();
        }

        // POST: Songs/Edit/5
        [HttpPost]
        [ValidateAntiForgeryToken]
        public ActionResult Edit(int id, IFormCollection collection)
        {
            try
            {
                // TODO: Add update logic here
                return this.RedirectToAction(nameof(this.Index));
            }
            catch
            {
                return this.View();
            }
        }

        // GET: Songs/Delete/5
        public ActionResult Delete(int id)
        {
            return this.View();
        }

        // POST: Songs/Delete/5
        [HttpPost]
        [ValidateAntiForgeryToken]
        public ActionResult Delete(int id, IFormCollection collection)
        {
            try
            {
                // TODO: Add delete logic here
                return RedirectToAction(nameof(Index));
            }
            catch
            {
                return this.View();
            }
        }
    }
}
