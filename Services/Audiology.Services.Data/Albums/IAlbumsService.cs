namespace Audiology.Services.Data.Albums
{
    using System.Collections.Generic;
    using System.Linq;
    using System.Threading.Tasks;

    using Audiology.Web.ViewModels.Albums;

    public interface IAlbumsService
    {
        public IEnumerable<T> GetAllForUser<T>(string userId);

        public Task AddAsync(string name, string coverUrl, string description, string producer, string userId);
    }
}
