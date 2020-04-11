namespace Audiology.Web.ViewComponents
{
    using System.Threading.Tasks;

    using Audiology.Services.Data.Songs;
    using Audiology.Web.ViewModels.Songs;
    using Microsoft.AspNetCore.Mvc;

    public class AllSongsForArtistViewComponent : ViewComponent
    {
        private readonly ISongsServcie service;

        public AllSongsForArtistViewComponent(ISongsServcie service)
        {
            this.service = service;
        }

        public async Task<IViewComponentResult> InvokeAsync(string userId)
        {
            var songs = await this.service.GetAllSongsForUserAsync<SongListViewModel>(userId);

            return this.View(songs);
        }
    }
}
