namespace Audiology.Web.Controllers
{
    using System.Diagnostics;
    using System.Linq;
    using System.Threading.Tasks;

    using Audiology.Data.Common.Repositories;
    using Audiology.Data.Models;
    using Audiology.Services.Data.Songs;
    using Audiology.Web.ViewModels;
    using Audiology.Web.ViewModels.Songs;
    using Microsoft.AspNetCore.Authorization;
    using Microsoft.AspNetCore.Mvc;
    using Microsoft.EntityFrameworkCore;

    [AllowAnonymous]
    public class HomeController : BaseController
    {
        private readonly ISongsServcie songsServcie;
        private readonly IRepository<ApplicationUser> userRepo;

        public HomeController(ISongsServcie songsServcie)
        {
            this.songsServcie = songsServcie;
        }

        public IActionResult Index()
        {
            var songsView = this.songsServcie.GetAll<SongListViewModel>();

            return this.View(songsView);
        }

        public IActionResult Privacy()
        {
            return this.View();
        }

        [ResponseCache(Duration = 0, Location = ResponseCacheLocation.None, NoStore = true)]
        public IActionResult Error()
        {
            return this.View(
                new ErrorViewModel { RequestId = Activity.Current?.Id ?? this.HttpContext.TraceIdentifier });
        }

        [HttpPost]
        public async Task<IActionResult> Profile(string userId)
        {
            return this.View();
        }
    }
}
