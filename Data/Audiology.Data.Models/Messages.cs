namespace Audiology.Data.Models
{
    using System.ComponentModel.DataAnnotations;

    using Audiology.Data.Common.Models;

    public class Messages : BaseDeletableModel<int>
    {
        public string ArtistId { get; set; }

        public string UserId { get; set; }

        public virtual ApplicationUser User { get; set; }

        [Required]
        [MaxLength(1000)]
        public string Text { get; set; }
    }
}
