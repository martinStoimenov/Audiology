namespace Audiology.Web.ViewModels.Playlists
{
    using Audiology.Data.Models;
    using Audiology.Services.Mapping;

    public class PlaylistViewModel : IMapFrom<Playlist>
    {
        public int Id { get; set; }

        public string UserId { get; set; }

        public string Name { get; set; }

        public bool IsPrivate { get; set; }
        // Add prop for image
    }
}
