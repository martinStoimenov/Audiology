namespace Audiology.Web.ViewModels.Albums
{
    using System;
    using System.Collections.Generic;
    using System.ComponentModel.DataAnnotations;
    using Audiology.Data.Models;
    using Audiology.Data.Models.Enumerations;
    using Audiology.Services.Mapping;
    using Audiology.Web.ViewModels.Songs;
    using AutoMapper;

    public class AlbumViewModel : IMapFrom<Album>, IMapTo<Album>
    {
        public int Id { get; set; }

        [MaxLength(200)]
        public string Name { get; set; }

        [MaxLength(150)]
        public string Producer { get; set; }

        [MaxLength(500)]
        [Display(Name = "Cover Url")]
        public string CoverUrl { get; set; }

        [MaxLength(1000)]
        public string Description { get; set; }

        [EnumDataType(typeof(Genre))]
        public Genre Genre { get; set; }

        [DataType(DataType.Date)]
        public DateTime? ReleaseDate { get; set; }

        public string UsersAlbumUserId { get; set; }

        public IEnumerable<SongListViewModel> Songs { get; set; }
    }
}
