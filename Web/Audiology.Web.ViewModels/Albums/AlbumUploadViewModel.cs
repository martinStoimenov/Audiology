using System;
using System.ComponentModel.DataAnnotations;

using Audiology.Data.Models;
using Audiology.Data.Models.Enumerations;
using Audiology.Services.Mapping;

namespace Audiology.Web.ViewModels.Albums
{
    public class AlbumUploadViewModel : IMapFrom<Album>
    {

        [Required]
        [MaxLength(200)]
        public string Name { get; set; }

        [Required]
        [MaxLength(150)]
        public string Producer { get; set; }

        [MaxLength(500)]
        [Display(Name = "Cover Url")]
        public string CoverUrl { get; set; }

        [MaxLength(1000)]
        public string Description { get; set; }

        [Required]
        [EnumDataType(typeof(Genre))]
        public Genre Genre { get; set; }

        [Required]
        [DataType(DataType.Date)]
        public DateTime? ReleaseDate { get; set; }
    }
}
