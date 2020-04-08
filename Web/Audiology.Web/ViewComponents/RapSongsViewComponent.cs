namespace Audiology.Web.ViewComponents
{
    using System.Threading.Tasks;

    using Audiology.Data.Models.Enumerations;
    using Audiology.Services.Data.Songs;
    using Audiology.Web.ViewModels.Songs;
    using Microsoft.AspNetCore.Mvc;

    public class RapSongsViewComponent : ViewComponent
    {
        private readonly ISongsServcie service;

        public RapSongsViewComponent(ISongsServcie service)
        {
            this.service = service;
        }

        public async Task<IViewComponentResult> InvokeAsync()
        {
            var songs = await this.service.GetTopSongsByGenre<SongListViewModel>(Genre.Rap);

            return this.View(songs);
        }
    }
}
