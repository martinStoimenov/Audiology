namespace Audiology.Web.Controllers
{
    using System.Diagnostics;
    using Audiology.Services.Data.Songs;
    using Audiology.Web.ViewModels;
    using Audiology.Web.ViewModels.Songs;
    using Microsoft.AspNetCore.Authorization;
    using Microsoft.AspNetCore.Mvc;

    [AllowAnonymous]
    public class HomeController : BaseController
    {
        private readonly ISongsServcie songsServcie;

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
    }
}
