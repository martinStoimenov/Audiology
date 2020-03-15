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
            this.Artists = new HashSet<ApplicationUser>();
        }

        [Required]
        [MaxLength(50)]
        public string Name { get; set; }

        [MaxLength(100)]
        public string Description { get; set; }

        public int? AlbumId { get; set; }

        public virtual Album Album { get; set; }

        public int? LyricsId { get; set; }

        public virtual Lyrics Lyrics { get; set; }

        [Required]
        public Genre Genre { get; set; }

        [Required]
        public int Year { get; set; }

        public virtual ICollection<ApplicationUser> Artists { get; set; }
    }
}
