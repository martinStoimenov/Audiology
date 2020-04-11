namespace Audiology.Web.ViewComponents
{
    using System.Threading.Tasks;

    using Audiology.Services.Data.Albums;
    using Audiology.Web.ViewModels.Albums;
    using Microsoft.AspNetCore.Mvc;

    public class NewestAlbumsViewComponent : ViewComponent
    {
        private readonly IAlbumsService service;

        public NewestAlbumsViewComponent(IAlbumsService service)
        {
            this.service = service;
        }

        public async Task<IViewComponentResult> InvokeAsync()
        {
            var albums = await this.service.NewestAlbumsAsync<AlbumsListViewModel>();

            return this.View(albums);
        }
    }
}
