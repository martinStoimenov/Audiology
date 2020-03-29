namespace Audiology.Web.ViewModels.Songs
{
    using Audiology.Data.Models;
    using Audiology.Services.Mapping;

    public class SongsByAlbumViewModel : IMapFrom<Song>
    {
        public int Id { get; set; }

        public string Name { get; set; }
    }
}
