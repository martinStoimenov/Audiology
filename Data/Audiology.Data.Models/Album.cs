namespace Audiology.Data.Models
{
    using System.Collections.Generic;
    using System.ComponentModel.DataAnnotations;

    using Audiology.Data.Common.Models;

    public class Album : BaseDeletableModel<int>
    {
        public Album()
        {
            this.Songs = new HashSet<Song>();
            this.UsersAlbum = new HashSet<UsersAlbum>();
            this.Artists = new HashSet<ApplicationUser>();
        }

        [Required]
        [MaxLength(50)]
        public string Name { get; set; }

        [Required]
        public string Producer { get; set; }

        [MaxLength(150)]
        public string CoverUrl { get; set; }

        [MaxLength(100)]
        public string Description { get; set; }

        public virtual ICollection<UsersAlbum> UsersAlbum { get; set; }

        public virtual ICollection<Song> Songs { get; set; }

        public virtual ICollection<ApplicationUser> Artists { get; set; }
    }
}
