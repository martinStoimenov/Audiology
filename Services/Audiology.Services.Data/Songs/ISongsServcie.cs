namespace Audiology.Services.Data.Songs
{
    using System;
    using System.Collections.Generic;
    using System.Threading.Tasks;

    using Audiology.Web.ViewModels.Songs;
    using Microsoft.AspNetCore.Http;

    public interface ISongsServcie
    {
        Task<int> UploadAsync(IFormFile input, string username, string songName, string description, int? albumId, Enum genre, int year, string userId, string songArt);

        IEnumerable<T> GetAllSongsForUserAsync<T>(string userId);

        IEnumerable<T> GetNewestSongs<T>();

        SongViewModel GetSong(int songId);

        IEnumerable<T> GetAll<T>(int? count = null);

        string GetMediaDuration(string songName, string username);
    }
}