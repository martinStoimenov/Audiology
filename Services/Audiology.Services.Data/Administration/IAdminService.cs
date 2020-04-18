namespace Audiology.Services.Data.Administration
{
    using System;
    using System.Collections.Generic;
    using System.Threading.Tasks;

    public interface IAdminService
    {
        Task AddLyricsAsync(string text, int songId);

        Task EditUserAsync(string userId, string profilePicUrl, string firstName, string lastName, string username, string email, DateTime? birthday, string instagram, string facebook, string youtube, string twitter, string soundcloud, Enum gender);

        Task<T> GetUserAsync<T>(string userId);

        Task DeleteLyricsAsync(int songId);

        Task<IEnumerable<T>> AllSongs<T>();

        Task<string> GetLyricsForSong(int songId);
    }
}
