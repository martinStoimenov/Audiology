namespace Audiology.Web.Controllers
{
    using System;
    using System.Collections.Generic;
    using System.Linq;
    using System.Threading.Tasks;
    using Audiology.Web.ViewModels.Albums;
    using Microsoft.AspNetCore.Mvc;

    public class AlbumsController : Controller
    {
        public IActionResult Upload()
        {
            return this.View();
        }

        [HttpPost]
        public async Task<IActionResult> Upload(AlbumUploadViewModel input)
        {
            return this.Content("Success");
        }
    }
}