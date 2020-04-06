namespace Audiology.Web.ViewModels.Songs
{
    using System;
    using System.Collections.Generic;
    using System.ComponentModel.DataAnnotations;
    using System.Linq;
    using Audiology.Data.Models;
    using Audiology.Data.Models.Enumerations;
    using Audiology.Services.Mapping;
    using Audiology.Web.ViewModels.Albums;
    using Audiology.Web.ViewModels.Playlists;
    using AutoMapper;

    public class SongViewModel : IMapTo<Song>, IMapFrom<Song>, IHaveCustomMappings
    {
        public int Id { get; set; }

        public string FileExtension { get; set; }

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

        [EnumDataType(typeof(Genre))]
        public Genre Genre { get; set; }

        [Range(1, 2020)]
        public int Year { get; set; }

        [MaxLength(100)]
        public string Featuring { get; set; }

        [MaxLength(100)]
        public string WrittenBy { get; set; }

        [MaxLength(500)]
        public string YoutubeUrl { get; set; }

        [MaxLength(500)]
        public string SoundcloudUrl { get; set; }

        [MaxLength(500)]
        public string InstagramPostUrl { get; set; }

        public string AlbumName { get; set; }

        public DateTime? AlbumReleaseDate { get; set; }

        public decimal AlbumDuration { get; set; }

        public string AlbumProducer { get; set; }

        public string AlbumCoverUrl { get; set; }

        public string UserId { get; set; }

        public string UserUserName { get; set; }

        public IEnumerable<PlaylistChooseViewModel> Playlists { get; set; }

        public void CreateMappings(IProfileExpression configuration)
        {
            configuration.CreateMap<Album, SongViewModel>().ForMember(sv => sv.AlbumReleaseDate, opt => opt.MapFrom(a => a.ReleaseDate.Value.ToString("dd-MM-yyyy")));

            configuration.CreateMap<Song, SongViewModel>().ForMember(sv => sv.Playlists, opt => opt.MapFrom(src => src.PlaylistsSongs.Select(x => new PlaylistChooseViewModel
            {
                UserId = this.UserId,
            })));
        }
    }
}
