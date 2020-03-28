namespace Audiology.Web.ViewComponents
{
    using Audiology.Services.Data.Songs;
    using Audiology.Web.ViewModels.Songs;
    using Microsoft.AspNetCore.Mvc;

    public class NewestSongsViewComponent : ViewComponent
    {
        private readonly ISongsServcie songsServcie;

        public NewestSongsViewComponent(ISongsServcie songsServcie)
        {
            this.songsServcie = songsServcie;
        }

        public IViewComponentResult Invoke()
        {
            var songs = this.songsServcie.GetNewestSongs<SongListViewModel>();  // make it async and to take only 3 items

            return this.View(songs);
        }
    }
}
