namespace Audiology.Web.ViewComponents
{
    using System.Threading.Tasks;

    using Audiology.Data.Models.Enumerations;
    using Audiology.Services.Data.Songs;
    using Audiology.Web.ViewModels.Songs;
    using Microsoft.AspNetCore.Mvc;

    public class MoreSongsFromGenreViewComponent : ViewComponent
    {
        private readonly ISongsServcie service;

        public MoreSongsFromGenreViewComponent(ISongsServcie service)
        {
            this.service = service;
        }

        public async Task<IViewComponentResult> InvokeAsync(Genre genre)
        {
            var songs = await this.service.GetRandomSongsFromGenre<TopSongsForUserViewModel>(genre);

            return this.View(songs);
        }
    }
}
