namespace Audiology.Data.Models
{
    using Audiology.Data.Common.Models;

    public class PlaylistsSongs : BaseDeletableModel<int>
    {
        public int PlaylistId { get; set; }

        public virtual Playlist Playlist { get; set; }

        public int SongId { get; set; }

        public virtual Song Song { get; set; }
    }
}
