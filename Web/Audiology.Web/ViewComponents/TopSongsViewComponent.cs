namespace Audiology.Web.ViewComponents
{
    using System.Threading.Tasks;

    using Audiology.Services.Data.Songs;
    using Audiology.Web.ViewModels.Songs;
    using Microsoft.AspNetCore.Mvc;

    public class TopSongsViewComponent : ViewComponent
    {
        private readonly ISongsServcie service;

        public TopSongsViewComponent(ISongsServcie service)
        {
            this.service = service;
        }

        public async Task<IViewComponentResult> InvokeAsync()
        {
            var songs = await this.service.GetTopFavouritedSongs<SongListViewModel>(10);

            return this.View(songs);
        }
    }
}
