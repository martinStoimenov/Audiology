namespace Audiology.Web.Controllers
{
    using System;
    using System.Collections.Generic;
    using System.IO;
    using System.Linq;
    using System.Net.Http;
    using System.Threading.Tasks;

    using Audiology.Data.Models;
    using Audiology.Services.Data.Albums;
    using Audiology.Services.Data.Comments;
    using Audiology.Services.Data.Playlists;
    using Audiology.Services.Data.Songs;
    using Audiology.Web.ViewModels.Albums;
    using Audiology.Web.ViewModels.Comments;
    using Audiology.Web.ViewModels.Playlists;
    using Audiology.Web.ViewModels.Songs;
    using Microsoft.AspNetCore.Http;
    using Microsoft.AspNetCore.Identity;
    using Microsoft.AspNetCore.Mvc;

    public class SongsController : Controller
    {
        private readonly ISongsServcie songsService;
        private readonly IAlbumsService albumsService;
        private readonly UserManager<ApplicationUser> userManager;
        private readonly IPlaylistsService playlistsService;
        private readonly ICommentsService commentsService;

        public SongsController(
            ISongsServcie songsService,
            IAlbumsService albumsService,
            UserManager<ApplicationUser> userManager,
            IPlaylistsService playlistsService,
            ICommentsService commentsService)
        {
            this.songsService = songsService;
            this.albumsService = albumsService;
            this.userManager = userManager;
            this.playlistsService = playlistsService;
            this.commentsService = commentsService;
        }

        public async Task<IActionResult> ById(int id)
        {
            string userId = this.userManager.GetUserId(this.User);

            var albums = await this.albumsService.GetAllForUser<AlbumDropDownViewModel>(userId);

            var song = await this.songsService.GetSong<SongViewModel>(id);

            int dotIndex = song.Name.LastIndexOf('.');
            string fileExtension = song.Name.Substring(dotIndex + 1);
            string songName = song.Name.Substring(0, dotIndex);

            song.Playlists = await this.playlistsService.GetAllPlaylistsAsync<PlaylistChooseViewModel>(userId);
            song.Albums = albums;
            song.FileExtension = fileExtension;
            song.Name = songName;

            return this.View(song);
        }

        public async Task<IActionResult> Index()
        {
            string userName = this.User.Identity.Name;
            string userId = this.userManager.GetUserId(this.User);
            var songs = await this.songsService.GetAllSongsForUserAsync<SongListViewModel>(userId);

            foreach (var song in songs)
            {
                var songDuration = this.songsService.GetMediaDuration(song.Name, userName);
                song.SongDuration = songDuration;
            }

            return this.View(songs);
        }

        public async Task<IActionResult> Upload()
        {
            var albums = await this.albumsService.GetAllForUser<AlbumDropDownViewModel>(this.userManager.GetUserId(this.User));
            var viewModel = new SongUploadViewModel
            {
                Albums = albums,
            };
            return this.View(viewModel);
        }

        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Upload(SongUploadViewModel input) // Add validation for error 404.13 data length
        {
            if (!this.ModelState.IsValid)
            {
                return this.View(input);
            }

            string userId = this.userManager.GetUserId(this.User);

            var songId = await this.songsService.UploadAsync(input.Song, this.User.Identity.Name, input.Name, input.Description, input.Producer, input.AlbumId, input.Genre, input.Year, userId, input.SongArtUrl, input.Featuring, input.WrittenBy, input.YoutubeUrl, input.SoundcloudUrl, input.InstagramPostUrl);

            return this.RedirectToAction(nameof(this.ById), new { id = songId });
        }

        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Edit(SongViewModel input)
        {
            if (!this.ModelState.IsValid)
            {
                return this.RedirectToAction(nameof(this.ById), new { id = input.Id });
            }

            var songId = await this.songsService.EditSongAsync(input.Id, input.UserUserName, input.Name, input.Description, input.AlbumId, input.Producer, input.SongArtUrl, input.Genre, input.Year, input.Featuring, input.WrittenBy, input.YoutubeUrl, input.SoundcloudUrl, input.InstagramPostUrl);
            return this.RedirectToAction(nameof(this.ById), new { id = songId });
        }

        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Delete(int id)
        {
            await this.songsService.DeleteSong(id);

            return this.RedirectToAction(nameof(this.Index));
        }

        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Comment(CommentViewModel input)
        {
            if (!this.ModelState.IsValid)
            {
                return this.BadRequest();
            }

            await this.commentsService.AddComment(input.UserId, input.SongId, input.Content);

            return this.RedirectToAction(nameof(this.ById), new { id = input.SongId });
        }

        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> DeleteComment(CommentViewModel input)
        {
            await this.commentsService.Delete(input.UserId, input.SongId, input.Id);

            return this.RedirectToAction(nameof(this.ById), new { id = input.SongId });
        }
    }
}
