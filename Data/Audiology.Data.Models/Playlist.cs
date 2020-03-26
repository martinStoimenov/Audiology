namespace Audiology.Data.Models
{
    using System.ComponentModel.DataAnnotations;

    using Audiology.Data.Common.Models;

    public class Playlist : BaseDeletableModel<int>
    {
        [Required]
        public string Name { get; set; }

        [Required]
        public string UserId { get; set; }

        public virtual ApplicationUser User { get; set; }

        public int SongId { get; set; }

        public virtual Song Song { get; set; }
    }
}
