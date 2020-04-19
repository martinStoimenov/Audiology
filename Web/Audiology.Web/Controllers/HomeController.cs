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

        [HttpGet]
        public async Task<IActionResult> Search(string search)
        {
            if (search != string.Empty)
            {
                var result = await this.songsServcie.Search<SearchSongsViewModel>(search);
                return this.View(result);
            }

            return this.NotFound();
        }
    }
}
