namespace Audiology.Web.ViewModels.Songs
{
    using Audiology.Data.Models;
    using Audiology.Services.Mapping;
    using AutoMapper;

    public class SongListViewModel : IMapFrom<Song>
    {
        public int Id { get; set; }

        public string Name { get; set; }

        public string FileExtension
        {
            get
            {
                int dotIndex = this.Name.LastIndexOf('.');
                string fileExtension = this.Name.Substring(dotIndex + 1);
                return fileExtension;
            }
        }

        public string UserUserName { get; set; }

        public string UserId { get; set; }

        public string SongArtUrl { get; set; }

        public string AlbumCoverUrl { get; set; }

        public int? Year { get; set; }

        public int FavouritesCount { get; set; }

        public string PlaylistName { get; set; }
    }
}
