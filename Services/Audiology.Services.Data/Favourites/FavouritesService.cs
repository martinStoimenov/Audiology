namespace Audiology.Services.Data.Favourites
{
    using System.Linq;
    using System.Threading.Tasks;

    using Audiology.Data.Common.Repositories;
    using Audiology.Data.Models;
    using Microsoft.EntityFrameworkCore;

    public class FavouritesService : IFavouritesService
    {
        private readonly IDeletableEntityRepository<Favourites> repository;

        public FavouritesService(IDeletableEntityRepository<Favourites> repository)
        {
            this.repository = repository;
        }

        public int GetCount(int? songId, int? albumId)
        {
            var favourites = this.repository.All().Where(f => f.SongId == songId || f.AlbumId == albumId).Select(f => f.Id);

            return favourites.Count();
        }

        public async Task FavouritedAsync(int? songId, int? albumId, string userId)
        {
            var favourite = await this.repository.All().Where(f => f.SongId == songId || f.AlbumId == albumId || f.UserId == userId).FirstOrDefaultAsync();

            if (favourite != null)
            {
                this.repository.HardDelete(favourite);
            }
            else
            {
                var liked = new Favourites
                {
                    SongId = songId,
                    AlbumId = albumId,
                    UserId = userId,
                };
                await this.repository.AddAsync(liked);
            }

            await this.repository.SaveChangesAsync();
        }
    }
}
