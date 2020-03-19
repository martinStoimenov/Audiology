namespace Audiology.Data.Models
{
    using System.ComponentModel.DataAnnotations;

    public class UsersAlbum
    {
        [Key]
        public string UserId { get; set; }

        public virtual ApplicationUser User { get; set; }

        [Key]
        public int AlbumId { get; set; }

        public virtual Album Album { get; set; }
    }
}
