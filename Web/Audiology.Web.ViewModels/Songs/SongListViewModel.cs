namespace Audiology.Web.ViewModels.Songs
{
    using System.Linq;

    using Audiology.Data.Models;
    using Audiology.Data.Models.Enumerations;
    using Audiology.Services.Mapping;
    using AutoMapper;

    public class SongListViewModel : IMapFrom<Song> //IHaveCustomMappings
    {
        public int Id { get; set; }

        public string Name { get; set; }

        public string SongDuration { get; set; }

        public string SongArtUrl { get; set; }

        public string AlbumCoverUrl { get; set; }

        public int? Year { get; set; }

        public Genre Genre { get; set; }

        public int FavouritesCount { get; set; }

/*        public void CreateMappings(IProfileExpression configuration)
        {
            configuration.CreateMap<Song, SongListViewModel>()
                .ForMember(x => x.FavouritesCount, options => options.MapFrom(s => s.Favourites.Count())); // debug to see if it returns the right value      Try with .Sum()
        }*/
    }
}
