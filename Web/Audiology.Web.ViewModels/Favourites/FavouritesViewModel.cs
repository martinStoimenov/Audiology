namespace Audiology.Web.ViewModels.Favourites
{
    using System.Collections.Generic;
    using System.Linq;
    using Audiology.Data.Models;
    using Audiology.Data.Models.Enumerations;
    using Audiology.Services.Mapping;
    using Audiology.Web.ViewModels.Songs;
    using AutoMapper;

    public class FavouritesViewModel : IMapFrom<Favourites>
    {
        public int Id { get; set; }

        public string UserId { get; set; }

        public int AlbumId { get; set; }

        public int SongId { get; set; }

        public string SongName { get; set; }

        public string SongDescription { get; set; }

        public string SongProducer { get; set; }

        public string SongYear { get; set; }

        public string SongSongArtUrl { get; set; }

        public string SongAlbumCoverUrl { get; set; }

        public Genre SongGenre { get; set; }

        public string SongUserUserName { get; set; }

        public string SongFeaturing { get; set; }

        public string SongSoundcloudUrl { get; set; }

        public string SongWrittenBy { get; set; }

        public string SongYoutubeUrl { get; set; }
    }
}
