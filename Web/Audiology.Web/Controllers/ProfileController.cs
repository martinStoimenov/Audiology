namespace Audiology.Web.Controllers
{
    using Audiology.Common;
    using Audiology.Services.Data.Profile;
    using Audiology.Web.ViewModels.Profile;
    using Microsoft.AspNetCore.Mvc;
    using System.Threading.Tasks;

    public class ProfileController : Controller
    {
        private readonly IProfileService service;

        public ProfileController(IProfileService service)
        {
            this.service = service;
        }

        public async Task<IActionResult> Index(string Id)
        {
            var user = await this.service.GetUserAsync<ArtistProfileViewModel>(Id);

            if (this.User.IsInRole(GlobalConstants.ArtistRoleName))
            {
                return this.View("ArtistProfile", user);
            }

            return this.View();
        }
    }
}