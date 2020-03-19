namespace Audiology.Web.ViewModels.Songs
{
    using Audiology.Data.Models;
    using Audiology.Data.Models.Enumerations;
    using Audiology.Services.Mapping;
    using AutoMapper;
    using Microsoft.AspNetCore.Http;
    using System.Collections.Generic;

    public class SongListViewModel : IHaveCustomMappings
    {
        // List of all the songs like a newsfeed
        public string Name { get; set; }

        public double SongDuration { get; set; }

        public int? Year { get; set; }

        public Genre Genre { get; set; }

        public IEnumerable<SongModel> Songs { get; set; }

        public void CreateMappings(IProfileExpression configuration)
        {
            configuration.CreateMap<Song, SongListViewModel>()
                .ForMember(x => x.Songs, options =>
                {
                    // Get the song from disk here
                    //options.MapFrom(p => p.Name.(v => (int)v.Type));
                });
        }
    }
}
