namespace Audiology.Data.Models
{
    using System.ComponentModel.DataAnnotations;

    using Audiology.Data.Common.Models;

    public class Comment : BaseDeletableModel<int>
    {
        public string UserId { get; set; }

        public ApplicationUser User { get; set; }

        public int SongId { get; set; }

        public Song Song { get; set; }

        [MaxLength(1000)]
        public string Content { get; set; }
    }
}
