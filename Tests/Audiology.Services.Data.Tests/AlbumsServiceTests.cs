namespace Audiology.Services.Data.Tests
{
    using System;
    using System.Collections.Generic;
    using System.Linq;
    using System.Reflection;
    using System.Threading.Tasks;

    using Audiology.Data;
    using Audiology.Data.Common.Repositories;
    using Audiology.Data.Models;
    using Audiology.Data.Models.Enumerations;
    using Audiology.Data.Repositories;
    using Audiology.Services.Data.Albums;
    using Audiology.Services.Data.Songs;
    using Audiology.Services.Mapping;
    using Audiology.Web.ViewModels.Albums;
    using AutoMapper;
    using Microsoft.AspNetCore.Identity;
    using Microsoft.EntityFrameworkCore;

    using Moq;

    using Xunit;

    public class AlbumsServiceTests
    {
        public AlbumsServiceTests()
        {
            AutoMapperConfig.RegisterMappings(
                typeof(AlbumsService).GetTypeInfo().Assembly,
                typeof(AlbumViewModel).GetTypeInfo().Assembly);
        }

        [Fact]
        public async Task UploadingAlbumIsSuccessfull()
        {
            var options = new DbContextOptionsBuilder<ApplicationDbContext>().UseInMemoryDatabase(Guid.NewGuid().ToString());
            var albumRepository = new EfRepository<Album>(new ApplicationDbContext(options.Options));
            var songRepository = new EfRepository<Song>(new ApplicationDbContext(options.Options));
            var mockSongsService = new Mock<ISongsServcie>().Object;
            var service = new AlbumsService(albumRepository, songRepository, mockSongsService);

            int albumId = await service.AddAsync("Makavelli", "https://blabla.png", "description abou the album", "Death Row Records", Guid.NewGuid().ToString(), DateTime.UtcNow, Genre.Rap);
            var album = service.GetCurrentAlbumById<AlbumViewModel>(albumId);

            Assert.NotNull(album);
        }

        [Fact]
        public async Task UploadingAlbumWithTheSameNameShouldReturn0()
        {
            var options = new DbContextOptionsBuilder<ApplicationDbContext>().UseInMemoryDatabase(Guid.NewGuid().ToString());
            var albumRepository = new EfRepository<Album>(new ApplicationDbContext(options.Options));
            var songRepository = new EfRepository<Song>(new ApplicationDbContext(options.Options));
            var mockSongsService = new Mock<ISongsServcie>().Object;
            var service = new AlbumsService(albumRepository, songRepository, mockSongsService);

            int albumId = await service.AddAsync("Makavelli", "https://blabla.png", "description abou the album", "Death Row Records", Guid.NewGuid().ToString(), DateTime.UtcNow, Genre.Rap);
            int albumId2 = await service.AddAsync("Makavelli", "https://blabla.jpg", "description about the album here", "Death Row Records", Guid.NewGuid().ToString(), DateTime.UtcNow, Genre.Rap);

            Assert.Equal(0, albumId2);
        }

        [Fact]
        public async Task EditingAlbumShouldReturnAlbumId()
        {
            var options = new DbContextOptionsBuilder<ApplicationDbContext>().UseInMemoryDatabase(Guid.NewGuid().ToString());
            var albumRepository = new EfRepository<Album>(new ApplicationDbContext(options.Options));
            var songRepository = new EfRepository<Song>(new ApplicationDbContext(options.Options));
            var mockSongsService = new Mock<ISongsServcie>().Object;
            var service = new AlbumsService(albumRepository, songRepository, mockSongsService);

            int albumId = await service.AddAsync("Makavelli", "https://blabla.png", "description abou the album", "Death Row Records", Guid.NewGuid().ToString(), DateTime.UtcNow, Genre.Rap);
            int albumId2 = await service.EditAlbumAsync(albumId, "All Eyez on me", "description about the album here", "Death Row Records", "https://blabla.jpg", Genre.Rap, DateTime.UtcNow);

            Assert.Equal(albumId, albumId2);
        }

        [Fact]
        public async Task EditingAlbumShouldSaveDataCorrectly()
        {
            var options = new DbContextOptionsBuilder<ApplicationDbContext>().UseInMemoryDatabase(Guid.NewGuid().ToString());
            var albumRepository = new EfRepository<Album>(new ApplicationDbContext(options.Options));
            var songRepository = new EfRepository<Song>(new ApplicationDbContext(options.Options));
            var mockSongsService = new Mock<ISongsServcie>().Object;
            var service = new AlbumsService(albumRepository, songRepository, mockSongsService);

            int albumId = await service.AddAsync("Makavelli", "https://blabla.png", "description abou the album", "Death Row Records", Guid.NewGuid().ToString(), DateTime.UtcNow, Genre.Rap);
            int albumId2 = await service.EditAlbumAsync(albumId, "All Eyez on me", "long description about the album here", "Death Row Records", "https://blabla.jpg", Genre.Classical, DateTime.UtcNow.AddDays(1));
            var album = service.GetCurrentAlbumById<AlbumViewModel>(albumId2);

            Assert.Equal("All Eyez on me", album.Name);
            Assert.Equal(Genre.Classical, album.Genre);
            Assert.Equal("long description about the album here", album.Description);
        }

        [Fact]
        public async Task DeletingAlbumShouldBeSuccesfull()
        {
            var options = new DbContextOptionsBuilder<ApplicationDbContext>().UseInMemoryDatabase(Guid.NewGuid().ToString());
            var albumRepository = new EfRepository<Album>(new ApplicationDbContext(options.Options));
            var songRepository = new EfRepository<Song>(new ApplicationDbContext(options.Options));
            var mockSongsService = new Mock<ISongsServcie>().Object;
            var service = new AlbumsService(albumRepository, songRepository, mockSongsService);

            int albumId = await service.AddAsync("Makavelli", "https://blabla.png", "description abou the album", "Death Row Records", Guid.NewGuid().ToString(), DateTime.UtcNow, Genre.Rap);
            await service.DeleteAlbum(albumId);
            var album = service.GetCurrentAlbumById<AlbumViewModel>(albumId);

            Assert.Null(album);
        }

        [Fact]
        public async Task NewestAlbumsShouldReturnExactly12Items()
        {
            var options = new DbContextOptionsBuilder<ApplicationDbContext>().UseInMemoryDatabase(Guid.NewGuid().ToString());
            var albumRepository = new EfRepository<Album>(new ApplicationDbContext(options.Options));
            var songRepository = new EfRepository<Song>(new ApplicationDbContext(options.Options));
            var mockSongsService = new Mock<ISongsServcie>().Object;
            var service = new AlbumsService(albumRepository, songRepository, mockSongsService);

            await service.AddAsync("Makavelli", "https://blabla.png", "description abou the album", "Death Row Records", Guid.NewGuid().ToString(), DateTime.UtcNow, Genre.Rap);
            await service.AddAsync("asdf", "https://blabla.png", "description abou the album", "Death Row Records", Guid.NewGuid().ToString(), DateTime.UtcNow, Genre.Rap);
            await service.AddAsync("Makavgbselli", "https://blabla.png", "description abou the album", "Death Row Records", Guid.NewGuid().ToString(), DateTime.UtcNow, Genre.Rap);
            await service.AddAsync("sdf", "https://blabla.png", "description abou the album", "Death Row Records", Guid.NewGuid().ToString(), DateTime.UtcNow, Genre.Rap);
            await service.AddAsync("Makavgfdsgvelli", "https://blabla.png", "description abou the album", "Death Row Records", Guid.NewGuid().ToString(), DateTime.UtcNow, Genre.Rap);
            await service.AddAsync("grsfdv", "https://blabla.png", "description abou the album", "Death Row Records", Guid.NewGuid().ToString(), DateTime.UtcNow, Genre.Rap);
            await service.AddAsync("Makabdfrservelli", "https://blabla.png", "description abou the album", "Death Row Records", Guid.NewGuid().ToString(), DateTime.UtcNow, Genre.Rap);
            await service.AddAsync("aergvgera", "https://blabla.png", "description abou the album", "Death Row Records", Guid.NewGuid().ToString(), DateTime.UtcNow, Genre.Rap);
            await service.AddAsync("egveaearg", "https://blabla.png", "description abou the album", "Death Row Records", Guid.NewGuid().ToString(), DateTime.UtcNow, Genre.Rap);
            await service.AddAsync("Makavvgedergaelli", "https://blabla.png", "description abou the album", "Death Row Records", Guid.NewGuid().ToString(), DateTime.UtcNow, Genre.Rap);
            await service.AddAsync("eargverag", "https://blabla.png", "description abou the album", "Death Row Records", Guid.NewGuid().ToString(), DateTime.UtcNow, Genre.Rap);
            await service.AddAsync("geraerga", "https://blabla.png", "description abou the album", "Death Row Records", Guid.NewGuid().ToString(), DateTime.UtcNow, Genre.Rap);
            await service.AddAsync("gaer", "https://blabla.png", "description abou the album", "Death Row Records", Guid.NewGuid().ToString(), DateTime.UtcNow, Genre.Rap);

            var albums = await service.NewestAlbumsAsync<AlbumViewModel>();

            Assert.Equal(12, albums.Count());
        }

        [Fact]
        public async Task TopAlbumsForUserShouldReturnExactly3albums()
        {
            var options = new DbContextOptionsBuilder<ApplicationDbContext>().UseInMemoryDatabase(Guid.NewGuid().ToString());
            var albumRepository = new EfRepository<Album>(new ApplicationDbContext(options.Options));
            var songRepository = new EfRepository<Song>(new ApplicationDbContext(options.Options));
            var mockSongsService = new Mock<ISongsServcie>().Object;
            var service = new AlbumsService(albumRepository, songRepository, mockSongsService);

            var user = new ApplicationUser
            {
                Id = Guid.NewGuid().ToString(),
                UserName = "Gosho",
                FirstName = "gosh",
                LastName = "peshov",
                EmailConfirmed = true,
            };

            var usermanager = MockUserManager<ApplicationUser>();
            await usermanager.Object.CreateAsync(user, "123456");

            var album1 = await service.AddAsync("Makavelli", "https://blabla.png", "description abou the album", "Death Row Records", user.Id, DateTime.UtcNow, Genre.Rap);
            await service.AddAsync("sdfsdf", "https://blabla.png", "description abou the album", "Death Row Records", user.Id, DateTime.UtcNow, Genre.Rap);
            await service.AddAsync("dssdfds", "https://blabla.png", "description abou the album", "Death Row Records", user.Id, DateTime.UtcNow, Genre.Rap);
            await service.AddAsync("sdffds", "https://blabla.png", "description abou the album", "Death Row Records", user.Id, DateTime.UtcNow, Genre.Rap);
            await service.AddAsync("sdffdssdf", "https://blabla.png", "description abou the album", "Death Row Records", user.Id, DateTime.UtcNow, Genre.Rap);
            var topAlbum = service.GetCurrentAlbumById<AlbumViewModel>(album1);

            var topAlbums = await service.TopAlbumsForUser<AlbumViewModel>(user.Id);

            Assert.Equal(3, topAlbums.Count());
        }

        public static Mock<UserManager<TUser>> MockUserManager<TUser>()
    where TUser : class
        {
            var store = new Mock<IUserStore<TUser>>();

            var mgr = new Mock<UserManager<TUser>>(store.Object, null, null, null, null, null, null, null, null);

            mgr.Object.UserValidators.Add(new UserValidator<TUser>());

            mgr.Object.PasswordValidators.Add(new PasswordValidator<TUser>());

            return mgr;
        }
    }
}
