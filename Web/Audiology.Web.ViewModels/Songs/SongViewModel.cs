namespace Audiology.Web.ViewModels.Songs
{
    using Audiology.Data.Models;
    using Audiology.Data.Models.Enumerations;
    using Audiology.Services.Mapping;

    public class SongViewModel : IMapFrom<Song>
    {
        public string Name { get; set; }

        public string Description { get; set; }

        public string Producer { get; set; }

        public string AlbumCoverUrl { get; set; }

        public int? AlbumId { get; set; }

        public string UserId { get; set; }

        public string UserUserName { get; set; }

        public Genre Genre { get; set; }

        public int Year { get; set; }

        public string SongArtUrl { get; set; }
    }
}
