namespace Audiology.Web.ViewComponents
{
    using System.Threading.Tasks;

    using Audiology.Services.Data.Songs;
    using Audiology.Web.ViewModels.Songs;
    using Microsoft.AspNetCore.Mvc;

    public class SongsByUserViewComponent : ViewComponent
    {
        private readonly ISongsServcie service;

        public SongsByUserViewComponent(ISongsServcie service)
        {
            this.service = service;
        }

        public async Task<IViewComponentResult> InvokeAsync(string userId)
        {
            var songs = await this.service.GetTopSongsForUserAsync<TopSongsForUserViewModel>(userId, 5);

            return this.View(songs);
        }
    }
}
