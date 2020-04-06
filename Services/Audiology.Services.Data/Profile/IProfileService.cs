
namespace Audiology.Services.Data.Profile
{
    using System.Threading.Tasks;

    public interface IProfileService
    {
        Task<T> GetUserAsync<T>(string userId);
    }
}