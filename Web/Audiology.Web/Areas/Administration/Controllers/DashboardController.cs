namespace Audiology.Web.Areas.Administration.Controllers
{
    using System.Linq;
    using System.Threading.Tasks;

    using Audiology.Data;
    using Audiology.Data.Models;
    using Audiology.Services.Data;
    using Audiology.Services.Data.Administration;
    using Audiology.Web.ViewModels.Administration.Dashboard;
    using Microsoft.AspNetCore.Identity;
    using Microsoft.AspNetCore.Mvc;
    using Microsoft.EntityFrameworkCore;

    public class DashboardController : AdministrationController
    {
        private readonly UserManager<ApplicationUser> userManager;
        private readonly IAdminService service;

        public DashboardController(
            UserManager<ApplicationUser> userManager,
            IAdminService service)
        {
            this.userManager = userManager;
            this.service = service;
        }

        public async Task<IActionResult> Index()
        {
            var users = await this.userManager.GetUsersInRoleAsync(Common.GlobalConstants.UserRoleName);
            var artists = await this.userManager.GetUsersInRoleAsync(Common.GlobalConstants.ArtistRoleName);

            var viewModel = new IndexViewModel
            {
                Users = users,
                Artists = artists,
                Songs = await this.service.AllSongs<SongDropDownViewModel>(),
            };

            return this.View(viewModel);
        }

        public async Task<IActionResult> Edit(string id)
        {
            var user = await this.userManager.FindByIdAsync(id);

            return this.View(user);
        }

        [HttpPost]
        public async Task<IActionResult> Edit(ApplicationUser user)
        {
            await this.service.EditUserAsync(user.Id, user.ProfilePicUrl, user.FirstName, user.LastName, user.UserName, user.Email, user.Birthday, user.InstagramUrl, user.FacebookUrl, user.YouTubeUrl, user.TwitterUrl, user.SondcloudUrl, user.Gender);

            return this.RedirectToAction(nameof(this.Index));
        }

        [HttpPost]
        public async Task<IActionResult> Delete(string id)
        {
            var user = await this.userManager.FindByIdAsync(id);
            var roles = await this.userManager.GetRolesAsync(user);
            var role = roles[0];

            await this.userManager.RemoveFromRoleAsync(user, role);
            await this.userManager.DeleteAsync(user);

            return this.RedirectToAction(nameof(this.Index));
        }

        [HttpGet]
        public async Task<IActionResult> Lyrics(int songId)
        {
            var lyrics = await this.service.GetLyricsForSong(songId);

            return this.Json(lyrics);
        }

        [HttpPost]
        public async Task<IActionResult> EditLyrics(string text, int id)
        {
            await this.service.EditLyricsAsync(text, id);

            return this.RedirectToAction(nameof(this.Index));
        }

        [HttpPost]
        public async Task<IActionResult> DeleteLyrics(int id)
        {
            await this.service.DeleteLyricsAsync(id);

            return this.RedirectToAction(nameof(this.Index));
        }
    }
}
