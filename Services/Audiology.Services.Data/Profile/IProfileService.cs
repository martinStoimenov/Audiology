
namespace Audiology.Services.Data.Profile
{
    using Audiology.Data.Models;
    using System.Threading.Tasks;

    public interface IProfileService
    {
        Task<T> GetUserAsync<T>(string userId);

        Task<ApplicationUser> GetUserAsync(string userId);

        Task<string> GetArtistDescription(string artist);
    }
}