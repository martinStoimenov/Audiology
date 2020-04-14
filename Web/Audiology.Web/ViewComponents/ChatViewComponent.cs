namespace Audiology.Web.ViewComponents
{
    using System.Threading.Tasks;

    using Microsoft.AspNetCore.Mvc;

    public class ChatViewComponent : ViewComponent
    {
        public async Task<IViewComponentResult> InvokeAsync(string artistId)
        {
            this.ViewData["ArtistId"] = artistId;

            return this.View();
        }
    }
}
