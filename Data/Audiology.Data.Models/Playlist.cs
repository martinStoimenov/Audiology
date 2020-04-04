namespace Audiology.Data.Models
{
    using System;
    using System.Collections.Generic;
    using System.ComponentModel.DataAnnotations;

    using Audiology.Data.Common.Models;

    public class Playlist : BaseDeletableModel<int>
    {
        public Playlist()
        {
            this.PlaylistsSongs = new HashSet<PlaylistsSongs>();
        }

        [Required]
        [MaxLength(30)]
        public string Name { get; set; }

        public string UserId { get; set; }

        public ApplicationUser User { get; set; }

        public virtual ICollection<PlaylistsSongs> PlaylistsSongs { get; set; }

        public bool IsPrivate { get; set; }

    }
}
