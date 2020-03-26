namespace Audiology.Services.Data.Songs
{
    using System;
    using System.Collections.Generic;
    using System.Threading.Tasks;

    using Audiology.Web.ViewModels.Songs;
    using Microsoft.AspNetCore.Http;

    public interface ISongsServcie
    {
        public Task<int> UploadAsync(IFormFile input, string username, string songName, string description, int? albumId, Enum genre, int year, string userId, string songArt);

        public IEnumerable<T> GetAllSongsForUserAsync<T>(string userId);

        public IEnumerable<T> GetAll<T>(int? count = null);

        public string GetMediaDuration(string songName, string username);
    }
}