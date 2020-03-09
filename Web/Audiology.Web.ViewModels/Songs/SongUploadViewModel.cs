namespace Audiology.Web.ViewModels.Songs
{
    using System;
    using System.ComponentModel.DataAnnotations;

    using Audiology.Data.Models;
    using Audiology.Data.Models.Enumerations;
    using Audiology.Services.Mapping;
    using Microsoft.AspNetCore.Http;

    public class SongUploadViewModel : IMapFrom<Song>
    {
        [Required]
        [MaxLength(50)]
        public string Name { get; set; }

        [Required]
        [Display(Name = "File Extension")]
        public FileExtension FileExtension { get; set; }

        [MaxLength(100)]
        public string Description { get; set; }

        public Album Album { get; set; }

        public Lyrics Lyrics { get; set; }

        [Required]
        [EnumDataType(typeof(Genre))]
        public Genre Genre { get; set; }

        [Required]
        [Range(1, 2020)]
        public int Year { get; set; }

        [Required]
        public IFormFile Song { get; set; }
    }
}
