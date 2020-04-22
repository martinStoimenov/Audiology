namespace Audiology.Services.Data.Tests
{
    using System;
    using System.Linq;
    using System.Reflection;
    using System.Threading.Tasks;

    using Audiology.Data;
    using Audiology.Data.Models;
    using Audiology.Data.Repositories;
    using Audiology.Services.Data.Favourites;
    using Audiology.Services.Mapping;
    using Audiology.Web.ViewModels.Favourites;
    using Microsoft.EntityFrameworkCore;
    using Xunit;

    public class FavouritesServiceTests
    {
        public FavouritesServiceTests()
        {
            AutoMapperConfig.RegisterMappings(
                typeof(FavouritesViewModel).GetTypeInfo().Assembly,
                typeof(FavouritesService).GetTypeInfo().Assembly);
        }

        [Fact]
        public async Task GetCountShouldReturnCorrectCount()
        {
            var options = new DbContextOptionsBuilder<ApplicationDbContext>().UseInMemoryDatabase(Guid.NewGuid().ToString());
            var albumRepository = new EfDeletableEntityRepository<Album>(new ApplicationDbContext(options.Options));
            var songRepository = new EfDeletableEntityRepository<Song>(new ApplicationDbContext(options.Options));
            var favouritesRepository = new EfDeletableEntityRepository<Favourites>(new ApplicationDbContext(options.Options));
            var service = new FavouritesService(favouritesRepository, songRepository, albumRepository);

            var favourite = new Favourites
            {
                AlbumId = null,
                SongId = 1,
                UserId = "userGuid",
            };
            var favourite1 = new Favourites
            {
                AlbumId = null,
                SongId = 1,
                UserId = "userGuid1",
            };
            var favourite2 = new Favourites
            {
                AlbumId = null,
                SongId = 1,
                UserId = "userGuid2",
            };
            await favouritesRepository.AddAsync(favourite);
            await favouritesRepository.AddAsync(favourite1);
            await favouritesRepository.AddAsync(favourite2);
            await favouritesRepository.SaveChangesAsync();

            int count = await service.GetCount(1, null);

            Assert.Equal(3, count);
        }

        [Fact]
        public async Task FavouritedShouldWorkCorrectly()
        {
            var options = new DbContextOptionsBuilder<ApplicationDbContext>().UseInMemoryDatabase(Guid.NewGuid().ToString());
            var albumRepository = new EfDeletableEntityRepository<Album>(new ApplicationDbContext(options.Options));
            var songRepository = new EfDeletableEntityRepository<Song>(new ApplicationDbContext(options.Options));
            var favouritesRepository = new EfDeletableEntityRepository<Favourites>(new ApplicationDbContext(options.Options));
            var service = new FavouritesService(favouritesRepository, songRepository, albumRepository);

            var song = new Song
            {
                UserId = "UserId",
                FavouritesCount = 0,
                Name = "Smile",
                Description = "asdsaasd",
                Year = 1999,
                SongArtUrl = "asddsaasd.com",
            };
            await songRepository.AddAsync(song);
            await songRepository.SaveChangesAsync();

            var favourite = new Favourites
            {
                CreatedOn = DateTime.Now,
                AlbumId = null,
                SongId = 1,
                UserId = "userGuid",
            };
            var favourite1 = new Favourites
            {
                CreatedOn = DateTime.Now,
                AlbumId = null,
                SongId = 1,
                UserId = "userGuid1",
            };
            var favourite2 = new Favourites
            {
                CreatedOn = DateTime.Now,
                AlbumId = null,
                SongId = 1,
                UserId = "userGuid2",
            };
            await favouritesRepository.AddAsync(favourite);
            await favouritesRepository.AddAsync(favourite1);
            await favouritesRepository.AddAsync(favourite2);
            await favouritesRepository.SaveChangesAsync();

            await service.FavouritedAsync(favourite.SongId, favourite.AlbumId, favourite.UserId);
            await service.FavouritedAsync(favourite.SongId, favourite.AlbumId, "4th-user");
            var count = await favouritesRepository.All().Where(f => f.SongId == 1).ToListAsync();

            Assert.Equal(3, count.Count());
        }

        [Fact]
        public async Task GetAllShouldWorkCorrect()
        {
            var options = new DbContextOptionsBuilder<ApplicationDbContext>().UseInMemoryDatabase(Guid.NewGuid().ToString());
            var albumRepository = new EfDeletableEntityRepository<Album>(new ApplicationDbContext(options.Options));
            var songRepository = new EfDeletableEntityRepository<Song>(new ApplicationDbContext(options.Options));
            var favouritesRepository = new EfDeletableEntityRepository<Favourites>(new ApplicationDbContext(options.Options));
            var service = new FavouritesService(favouritesRepository, songRepository, albumRepository);

            var favourite = new Favourites
            {
                AlbumId = null,
                SongId = 1,
                UserId = "userGuid",
            };
            var favourite1 = new Favourites
            {
                AlbumId = null,
                SongId = 2,
                UserId = "userGuid",
            };
            var favourite2 = new Favourites
            {
                AlbumId = null,
                SongId = 1,
                UserId = "userGuid2",
            };
            await favouritesRepository.AddAsync(favourite);
            await favouritesRepository.AddAsync(favourite1);
            await favouritesRepository.AddAsync(favourite2);
            await favouritesRepository.SaveChangesAsync();

            var favourites = await service.GetAllAsync<FavouritesViewModel>(favourite.UserId);

            Assert.Equal(2, favourites.Count());
        }
    }
}
