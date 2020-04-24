namespace Audiology.Services.Data.Favourites
{
    using System.Collections.Generic;
    using System.Threading.Tasks;

    public interface IFavouritesService
    {
        Task<int> GetCount(int? songId, int? albumId);

        Task FavouritedAsync(int? songId, int? albumId, string userId);

        Task<IEnumerable<T>> GetAllAsync<T>(string userId);

        Task<int> TotalFavsForArtist(string userId);
    }
}