﻿namespace Audiology.Web.ViewModels.Albums
{
    using System.Collections.Generic;

    using Audiology.Data.Models;
    using Audiology.Services.Mapping;
    using Audiology.Web.ViewModels.Songs;
    using AutoMapper;

    public class AlbumViewModel : IMapFrom<Album>, IHaveCustomMappings
    {
        public string Name { get; set; }

        public string Producer { get; set; }

        public string CoverUrl { get; set; }

        public string Description { get; set; }

        public string Genre { get; set; }

        public string ReleaseDate { get; set; }

        public IEnumerable<SongListViewModel> Songs { get; set; }

        public void CreateMappings(IProfileExpression configuration)
        {
            configuration.CreateMap<Album, AlbumViewModel>().ForMember(av => av.Songs, opt => opt.Ignore());
        }
    }
}
