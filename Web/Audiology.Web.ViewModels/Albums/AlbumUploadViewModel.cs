using System.ComponentModel.DataAnnotations;

namespace Audiology.Web.ViewModels.Albums
{
    public class AlbumUploadViewModel
    {

        [Required]
        [MaxLength(50)]
        public string Name { get; set; }

        [Required]
        public string Producer { get; set; }

        [MaxLength(150)]
        [Display(Name = "Cover Url")]
        public string CoverUrl { get; set; }

        [MaxLength(100)]
        public string Description { get; set; }
    }
}
