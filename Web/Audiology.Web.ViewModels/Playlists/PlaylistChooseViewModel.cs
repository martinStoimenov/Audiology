namespace Audiology.Web.ViewModels.Playlists
{
    using System.ComponentModel.DataAnnotations;

    using Audiology.Data.Models;
    using Audiology.Services.Mapping;
    using AutoMapper;

    public class PlaylistChooseViewModel : IMapTo<Playlist>, IMapFrom<Playlist>
    {
        public int Id { get; set; }

        public string UserId { get; set; }

        [MaxLength(30)]
        public string Name { get; set; }

        public bool IsPrivate { get; set; }
    }
}
