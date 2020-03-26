namespace Audiology.Web.ViewModels.Songs
{
    using System;
    using System.Collections.Generic;
    using System.ComponentModel.DataAnnotations;

    using Audiology.Data.Models;
    using Audiology.Data.Models.Enumerations;
    using Audiology.Services.Mapping;
    using Audiology.Web.ViewModels.Albums;
    using Microsoft.AspNetCore.Http;

    public class SongUploadViewModel : IMapTo<Song>
    {
        [Required]
        [MaxLength(50)]
        public string Name { get; set; }

        [MaxLength(100)]
        public string Description { get; set; }

        [Display(Name = "Album")]
        public int? AlbumId { get; set; }

        public IEnumerable<AlbumDropDownViewModel> Albums { get; set; }

        [MaxLength(100)]
        public string Producer { get; set; }

        [MaxLength(500)]
        [Url]
        public string SongArtUrl { get; set; }

        [Required]
        [EnumDataType(typeof(Genre))]
        public Genre Genre { get; set; }

        [Required]
        [Range(1, 2020)]
        public int Year { get; set; }

        [Required]
        public IFormFile Song { get; set; }   // Add validation for fileSize and extension
    }
}
