namespace Audiology.Web.ViewModels
{
    using System;
    using System.Collections.Generic;
    using System.Net;
    using System.Text;
    using System.Text.RegularExpressions;
    using Audiology.Data.Models;
    using Audiology.Data.Models.Enumerations;
    using Audiology.Services.Mapping;

    public class SearchSongsViewModel : IMapFrom<Song>
    {
        public int Id { get; set; }

        public string UserId { get; set; }

        public int SongFavouritesCount { get; set; }

        public string Name { get; set; }

        public string Description { get; set; }

        public string Producer { get; set; }

        public string Year { get; set; }

        public string SongArtUrl { get; set; }

        public string SongAlbumCoverUrl { get; set; }

        public Genre SongGenre { get; set; }

        public string UserUserName { get; set; }

        public string Featuring { get; set; }

        public int AlbumId { get; set; }

        public string AlbumCoverUrl { get; set; }

        public int AlbumFavouritesCount { get; set; }

        public string AlbumName { get; set; }

        public string AlbumProducer { get; set; }

        public string AlbumDescription { get; set; }

        public DateTime ReleaseDate { get; set; }

        public string Extension
        {
            get
            {
                var dotIndex = this.Name.LastIndexOf(".");
                return this.Name.Substring(dotIndex);
            }
        }

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
