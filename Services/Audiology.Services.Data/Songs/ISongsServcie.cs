namespace Audiology.Services.Data.Songs
{
    using System;
    using System.Collections.Generic;
    using System.Threading.Tasks;

    using Audiology.Web.ViewModels.Songs;
    using Microsoft.AspNetCore.Http;

    public interface ISongsServcie
    {
        Task<int> UploadAsync(IFormFile input, string username, string songName, string description, string producer, int? albumId, Enum genre, int year, string userId, string songArt, string featuring, string writtenBy, string youtubeUrl, string soundcloudUrl, string instagramPostUrl);

        Task<int> EditSongAsync(int id,string username, string name, string description, int? albumId, string producer, string songArtUrl, Enum genre, int year, string featuring, string writtenBy, string youtubeUrl, string soundcloudUrl, string instagramPostUrl);

        Task DeleteSong(int songId);

        Task<IEnumerable<T>> GetTopFavouritedSongs<T>(int howMuch);

        Task<IEnumerable<T>> GetSongsByAlbumAsync<T>(int? albumId);

        IEnumerable<T> GetAllSongsForUserAsync<T>(string userId);

        IEnumerable<T> GetNewestSongs<T>();

        Task<T> GetSong<T>(int songId);

        IEnumerable<T> GetAll<T>(int? count = null);

        string GetMediaDuration(string songName, string username);
    }
}