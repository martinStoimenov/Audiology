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
        }

        [Required]
        [MaxLength(50)]
        public string Name { get; set; }

        public virtual ICollection<UsersAlbum> UsersAlbum { get; set; }

        public virtual ICollection<Song> Songs { get; set; }
    }
}
