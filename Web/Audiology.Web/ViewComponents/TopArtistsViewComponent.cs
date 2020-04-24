namespace Audiology.Web.ViewComponents
{
    using System.Threading.Tasks;
    using Audiology.Services.Data.Favourites;
    using Audiology.Services.Data.Songs;
    using Audiology.Web.ViewModels.Profile;
    using Microsoft.AspNetCore.Mvc;

    public class TopArtistsViewComponent : ViewComponent
    {
        private readonly ISongsServcie service;
        private readonly IFavouritesService favouritesService;

        public TopArtistsViewComponent(ISongsServcie service, IFavouritesService favouritesService)
        {
            this.service = service;
            this.favouritesService = favouritesService;
        }

        public async Task<IViewComponentResult> InvokeAsync()
        {
            var artists = await this.service.GetTopArtistsByFavsCount<TopArtistsViewModel>(5);
            foreach (var artist in artists)
            {
                artist.TotalFavCount = await this.favouritesService.TotalFavsForArtist(artist.Id);
            }

            return this.View(artists);
        }
    }
}
