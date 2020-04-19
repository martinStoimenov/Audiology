namespace Audiology.Web.Controllers
{
    using System.Threading.Tasks;

    using Audiology.Common;
    using Audiology.Data.Models;
    using Audiology.Services.Data.Profile;
    using Audiology.Web.ViewModels.Profile;
    using Microsoft.AspNetCore.Identity;
    using Microsoft.AspNetCore.Mvc;

    public class ProfileController : Controller
    {
        private readonly IProfileService service;
        private readonly UserManager<ApplicationUser> userManager;

        public ProfileController(IProfileService service, UserManager<ApplicationUser> userManager)
        {
            this.service = service;
            this.userManager = userManager;
        }

        public async Task<IActionResult> Index(string id)
        {
            var user = await this.service.GetUserAsync<ArtistProfileViewModel>(id);
            var requestedUser = await this.service.GetUserAsync(id);
            var desc = await this.service.GetArtistDescription(user.UserName);

            if (await this.userManager.IsInRoleAsync(requestedUser, GlobalConstants.ArtistRoleName))
            {
                user.Description = desc;
                return this.View("ArtistProfile", user);
            }

            return this.View();
        }
    }
}