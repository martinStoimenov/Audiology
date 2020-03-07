namespace Audiology.Data.Models
{
    using Audiology.Data.Common.Models;

    public class Favourites : BaseDeletableModel<int>
    {
        public int UserId { get; set; }

        public ApplicationUser User { get; set; }

        public int SongId { get; set; }

        public virtual Song Song { get; set; }

        public int AlbumId { get; set; }

        public virtual Album Album { get; set; }
    }
}
