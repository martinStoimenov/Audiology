namespace Audiology.Data.Models
{
    using System.ComponentModel.DataAnnotations;

    using Audiology.Data.Common.Models;
    using Audiology.Data.Models.Enumerations;

    public class Song : BaseDeletableModel<int>
    {
        [Required]
        [MaxLength(50)]
        public string Name { get; set; }

        [Required]
        public FileExtension FileExtension { get; set; }

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
    }
}
