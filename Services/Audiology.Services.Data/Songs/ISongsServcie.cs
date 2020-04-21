namespace Audiology.Services.Data.Songs
{
    using System;
    using System.Collections.Generic;
    using System.Threading.Tasks;

    using Audiology.Data.Models.Enumerations;
    using Audiology.Web.ViewModels.Songs;
    using Microsoft.AspNetCore.Http;

    public interface ISongsServcie
    {
        Task<int> UploadAsync(IFormFile input, string username, string songName, string description, string producer, int? albumId, Enum genre, int year, string userId, string songArt, string featuring, string writtenBy, string youtubeUrl, string soundcloudUrl, string instagramPostUrl);

        Task<int> EditSongAsync(int id,string username, string name, string description, int? albumId, string producer, string songArtUrl, Enum genre, int year, string featuring, string writtenBy, string youtubeUrl, string soundcloudUrl, string instagramPostUrl);

        Task DeleteSong(int songId);

        Task<IEnumerable<T>> GetTopFavouritedSongs<T>(int howMuch);

        Task<IEnumerable<T>> GetSongsByAlbumAsync<T>(int? albumId);

        Task<IEnumerable<T>> GetTopSongsByGenre<T>(Genre genre);

        Task<IEnumerable<T>> GetTopSongsForUserAsync<T>(string userId, int count);

        Task<T> GetSong<T>(int songId);

        Task BackgroundLyricsGathering();

        Task<IEnumerable<T>> GetAllSongsForUserAsync<T>(string userId);

        Task<IEnumerable<T>> GetRandomSongsFromGenre<T>(Genre genre);

        Task<IEnumerable<T>> Search<T>(string searchTerm);

        Task<IEnumerable<T>> GetSongsByGenre<T>(string genre);

        IEnumerable<T> GetNewestSongs<T>();

        IEnumerable<T> GetAll<T>(int? count = null);

        string EmbedYoutube(string url);

        string GetMediaDuration(string songName, string username);
    }
}