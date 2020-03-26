namespace Audiology.Services.Data.Albums
{
    using System;
    using System.Collections.Generic;
    using System.Linq;
    using System.Threading.Tasks;

    using Audiology.Web.ViewModels.Albums;

    public interface IAlbumsService
    {
        IEnumerable<T> GetAllForUser<T>(string userId);

        AlbumViewModel GetCurrentAlbumById(int albumId);

        Task<int> AddAsync(string name, string coverUrl, string description, string producer, string userId, DateTime? releaseDate);
    }
}
