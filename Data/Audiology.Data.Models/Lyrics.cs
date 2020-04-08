namespace Audiology.Data.Models
{
    using System.ComponentModel.DataAnnotations;

    using Audiology.Data.Common.Models;

    public class Lyrics : BaseDeletableModel<int>
    {
        public int SongId { get; set; }

        public Song Song { get; set; }

        [Required]
        [MaxLength(5000)]
        public string Text { get; set; }
    }
}
