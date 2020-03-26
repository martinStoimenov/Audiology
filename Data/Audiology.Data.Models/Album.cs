namespace Audiology.Data.Models
{
    using System;
    using System.Collections.Generic;
    using System.ComponentModel.DataAnnotations;
    using System.ComponentModel.DataAnnotations.Schema;

    using Audiology.Data.Common.Models;
    using Audiology.Data.Models.Enumerations;

    public class Album : BaseDeletableModel<int>
    {
        public Album()
        {
            this.Songs = new HashSet<Song>();
            this.UsersAlbum = new HashSet<UsersAlbum>();
            this.Playlists = new HashSet<Playlist>();
        }

        [Required]
        [MaxLength(200)]
        public string Name { get; set; }

        [Required]
        [MaxLength(150)]
        public string Producer { get; set; }

        [MaxLength(500)]
        public string CoverUrl { get; set; }

        [MaxLength(1000)]
        public string Description { get; set; }

        [Required]
        public Genre Genre { get; set; }

        [Required]
        [Column(TypeName = "date")]
        public DateTime? ReleaseDate { get; set; }

        public virtual ICollection<UsersAlbum> UsersAlbum { get; set; }

        public virtual ICollection<Song> Songs { get; set; }

        public virtual ICollection<Playlist> Playlists { get; set; }
    }
}
