namespace Audiology.Web.Controllers
{
    using System.Threading.Tasks;

    using Audiology.Data.Models;
    using Audiology.Services.Data.Favourites;
    using Audiology.Web.ViewModels.Favourites;
    using Microsoft.AspNetCore.Authorization;
    using Microsoft.AspNetCore.Identity;
    using Microsoft.AspNetCore.Mvc;

    [Route("api/[controller]")]
    [ApiController]
    public class FavouritesController : Controller
    {
        private readonly IFavouritesService favouritesService;
        private readonly UserManager<ApplicationUser> userManager;

        public FavouritesController(IFavouritesService favouritesService, UserManager<ApplicationUser> userManager)
        {
            this.favouritesService = favouritesService;
            this.userManager = userManager;
        }

        // Request {"userId":"43452d61-619b-48db-b859-3146df33f3df","songId":2,"albumId":9}       works without authentication in postman
        [HttpPost]
        public async Task<ActionResult<FavouritesOutputModel>> Post(FavouritesInputViewModel input)
        {
            string userId = this.userManager.GetUserId(this.User);
            await this.favouritesService.FavouritedAsync(input.SongId, input.AlbumId, userId);
            var favouritesCount = this.favouritesService.GetCount(input.SongId, input.AlbumId);

            return new FavouritesOutputModel { FavouritesCount = favouritesCount };
        }

        [HttpGet]
        public async Task<IActionResult> Index()
        {
            var userId = this.userManager.GetUserId(this.User);

            var all = await this.favouritesService.GetAllAsync<FavouritesViewModel>(userId);

            return this.View(all);
        }
    }
}