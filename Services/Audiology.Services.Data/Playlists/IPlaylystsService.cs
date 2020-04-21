namespace Audiology.Services.Data.Playlists
{
    using System.Collections.Generic;
    using System.Threading.Tasks;

    public interface IPlaylistsService
    {
        Task CreateAsync(string name, string userId, int songId, bool isPrivate);

        Task<IEnumerable<T>> GetAllSongsInPlaylistAsync<T>(string userId, int playlistId);

        Task AddAsync(string userId, int playlistId, int songId);

        Task RemoveAsync(string userId, int playlistId, int songId);

        Task<IEnumerable<T>> GetAllPlaylistsAsync<T>(string userId);

        Task<string> GetPlaylistArt(int playlistId);
    }
}