namespace Audiology.Web.ViewComponents
{
    using System.Threading.Tasks;

    using Audiology.Services.Data.Songs;
    using Audiology.Web.ViewModels.Songs;
    using Microsoft.AspNetCore.Mvc;

    public class SongsByAlbumViewComponent : ViewComponent
    {
        private readonly ISongsServcie service;

        public SongsByAlbumViewComponent(ISongsServcie service)
        {
            this.service = service;
        }

        public async Task<IViewComponentResult> InvokeAsync(int? albumId)
        {
             var songs = await this.service.GetSongsByAlbumAsync<SongsByAlbumViewModel>(albumId);

             return await Task.FromResult<IViewComponentResult>(this.View(songs));
        }
    }
}
