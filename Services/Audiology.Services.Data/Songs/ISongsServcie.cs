namespace Audiology.Services.Data.Songs
{
    using System.Collections.Generic;
    using System.Threading.Tasks;

    using Audiology.Web.ViewModels.Songs;
    using Microsoft.AspNetCore.Http;

    public interface ISongsServcie
    {
        public Task UploadAsync(IFormFile input, string username);

        public Task<IEnumerable<SongListViewModel>> GetAllSongsForUserAsync(string userId);

        public IEnumerable<T> GetAll<T>(int? count = null);
    }
}