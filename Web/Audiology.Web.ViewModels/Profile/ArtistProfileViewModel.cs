namespace Audiology.Web.ViewModels.Profile
{
    using System;

    using Audiology.Data.Models;
    using Audiology.Data.Models.Enumerations;
    using Audiology.Services.Mapping;

    public class ArtistProfileViewModel : IMapFrom<ApplicationUser>
    {
        public string Id { get; set; }

        public string UserName { get; set; }

        public string FirstName { get; set; }

        public string LastName { get; set; }

        public string ProfilePicUrl { get; set; }

        public string InstagramUrl { get; set; }

        public string FacebookUrl { get; set; }

        public string TwitterUrl { get; set; }

        public string YouTubeUrl { get; set; }

        public string SondcloudUrl { get; set; }

        public Gender Gender { get; set; }

        public DateTime? Birthday { get; set; }

        public string AlbumName { get; set; }
    }
}
