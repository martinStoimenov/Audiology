namespace Audiology.Data.Models
{
    using System.Collections.Generic;
    using System.ComponentModel.DataAnnotations;

    using Audiology.Data.Common.Models;
    using Audiology.Data.Models.Enumerations;

    public class Song : BaseDeletableModel<int>
    {
        public Song()
        {
            this.Favourites = new HashSet<Favourites>();
        }

        [Required]
        [MaxLength(50)]
        public string Name { get; set; }

        [MaxLength(100)]
        public string Description { get; set; }

        [MaxLength(100)]
        public string Producer { get; set; }

        public int? AlbumId { get; set; }

        public virtual Album Album { get; set; }

        [Required]
        public string UserId { get; set; }

        public virtual ApplicationUser User { get; set; }

        [Required]
        public Genre Genre { get; set; }

        [Required]
        public int Year { get; set; }

        [MaxLength(500)]
        public string SongArtUrl { get; set; }

        public virtual ICollection<Favourites> Favourites { get; set; }
    }
}
