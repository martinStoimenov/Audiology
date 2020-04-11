namespace Audiology.Services.Data.Albums
{
    using System;
    using System.Collections.Generic;
    using System.Linq;
    using System.Threading.Tasks;

    using Audiology.Web.ViewModels.Albums;

    public interface IAlbumsService
    {
        Task<IEnumerable<T>> GetAllForUser<T>(string userId);

        T GetCurrentAlbumById<T>(int albumId);

        Task<IEnumerable<T>> NewestAlbumsAsync<T>();

        Task<IEnumerable<T>> TopAlbumsForUser<T>(string userId);

        Task DeleteAlbum(int albumId);

        Task<int> AddAsync(string name, string coverUrl, string description, string producer, string userId, DateTime? releaseDate);

        Task<int> EditAlbumAsync(int id, string name, string description, string producer, string coverUrl, Enum genre, DateTime? releaseDate);
    }
}
