namespace Audiology.Web.ViewModels.Favourites
{
    using System;
    using System.Collections.Generic;
    using System.Linq;
    using System.Net;
    using System.Text.RegularExpressions;
    using Audiology.Data.Models;
    using Audiology.Data.Models.Enumerations;
    using Audiology.Services.Mapping;
    using Audiology.Web.ViewModels.Albums;
    using Audiology.Web.ViewModels.Songs;
    using AutoMapper;

    public class FavouritesViewModel : IMapFrom<Favourites>
    {
        public int Id { get; set; }

        public string UserId { get; set; }

        public int SongId { get; set; }

        public int SongFavouritesCount { get; set; }

        public string SongName { get; set; }

        public string SongDescription { get; set; }

        public string SongProducer { get; set; }

        public string SongYear { get; set; }

        public string SongSongArtUrl { get; set; }

        public string SongAlbumCoverUrl { get; set; }

        public Genre SongGenre { get; set; }

        public string SongUserUserName { get; set; }

        public string SongFeaturing { get; set; }

        public int AlbumId { get; set; }

        public string AlbumCoverUrl { get; set; }

        public int AlbumFavouritesCount { get; set; }

        public string AlbumName { get; set; }

        public string AlbumProducer { get; set; }

        public string AlbumDescription { get; set; }

        public DateTime ReleaseDate { get; set; }

        public string ShortDescription
        {
            get
            {
                var content = WebUtility.HtmlDecode(Regex.Replace(this.AlbumDescription, @"<[^>]+>", string.Empty));
                return content.Length > 57
                        ? content.Substring(0, 57) + "..."
                        : content;
            }
        }

        public string AlbumUserUserName { get; set; }
    }
}
