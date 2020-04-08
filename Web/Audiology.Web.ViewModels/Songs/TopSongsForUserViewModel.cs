namespace Audiology.Web.ViewModels.Songs
{
    using Audiology.Data.Models;
    using Audiology.Services.Mapping;

    public class TopSongsForUserViewModel : IMapFrom<Song>
    {
        public int Id { get; set; }

        public string Name { get; set; }

        public string UserUserName { get; set; }

        public string SongArtUrl { get; set; }
    }
}
