namespace Audiology.Web.ViewComponents
{
    using System.Threading.Tasks;

    using Audiology.Services.Data.Albums;
    using Audiology.Web.ViewModels.Albums;
    using Microsoft.AspNetCore.Mvc;

    public class AllAlbumsForArtistViewComponent : ViewComponent
    {
        private readonly IAlbumsService service;

        public AllAlbumsForArtistViewComponent(IAlbumsService service)
        {
            this.service = service;
        }

        public async Task<IViewComponentResult> InvokeAsync(string userId)
        {
            var albums = await this.service.GetAllForUser<AlbumsListViewModel>(userId);

            return this.View(albums);
        }
    }
}
