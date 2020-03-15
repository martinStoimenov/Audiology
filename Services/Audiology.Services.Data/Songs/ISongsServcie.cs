namespace Audiology.Services.Data.Songs
{
    using System.Threading.Tasks;

    using Microsoft.AspNetCore.Http;

    public interface ISongsServcie
    {
        public Task UploadAsync(IFormFile input, string username);
    }
}