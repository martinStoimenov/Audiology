namespace Audiology.Data.Models
{
    using System.Collections.Generic;
    using System.ComponentModel.DataAnnotations;

    using Audiology.Data.Common.Models;

    public class Playlist : BaseDeletableModel<int>
    {
        public Playlist()
        {
            this.Songs = new HashSet<Song>();
        }

        [Required]
        public string Name { get; set; }

        public virtual ICollection<Song> Songs { get; set; }
    }
}
