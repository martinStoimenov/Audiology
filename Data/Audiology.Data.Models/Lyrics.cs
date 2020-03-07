namespace Audiology.Data.Models
{
    using System.ComponentModel.DataAnnotations;

    using Audiology.Data.Common.Models;

    public class Lyrics : BaseDeletableModel<int>
    {
        [Required]
        public string SongId { get; set; }

        public virtual Song Song { get; set; }

        [Required]
        [MaxLength(700)]
        public string Text { get; set; }
    }
}
