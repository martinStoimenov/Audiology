namespace Audiology.Web.ViewModels.Profile
{
    using Audiology.Data.Models;
    using Audiology.Services.Mapping;

    public class TopArtistsViewModel : IMapFrom<ApplicationUser>
    {
        public string Id { get; set; }

        public string UserName { get; set; }

        public string FirstName { get; set; }

        public string LastName { get; set; }

        public string ProfilePicUrl { get; set; }

        public int TotalFavCount { get; set; }
    }
}
