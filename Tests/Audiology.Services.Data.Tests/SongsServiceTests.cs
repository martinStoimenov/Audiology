namespace Audiology.Services.Data.Tests
{
    using System;
    using System.Collections.Generic;
    using System.IO;
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
    using Audiology.Web.ViewModels.Songs;
    using AutoMapper;
    using Microsoft.AspNetCore.Hosting;
    using Microsoft.AspNetCore.Http;
    using Microsoft.AspNetCore.Http.Internal;
    using Microsoft.AspNetCore.Identity;
    using Microsoft.EntityFrameworkCore;
    using Microsoft.Extensions.Configuration;
    using Moq;

    using Xunit;

    public class SongsServiceTests
    {
        public SongsServiceTests()
        {
            AutoMapperConfig.RegisterMappings(
                typeof(SongsService).GetTypeInfo().Assembly,
                typeof(SongListViewModel).GetTypeInfo().Assembly,
                typeof(SongViewModel).GetTypeInfo().Assembly);
        }

        [Fact]
        public async Task UploadShouldBeSuccesfull()
        {
            var options = new DbContextOptionsBuilder<ApplicationDbContext>().UseInMemoryDatabase(Guid.NewGuid().ToString());
            var userRepository = new EfRepository<ApplicationUser>(new ApplicationDbContext(options.Options));
            var songRepository = new EfDeletableEntityRepository<Song>(new ApplicationDbContext(options.Options));
            var playlistsSongsRepository = new EfDeletableEntityRepository<PlaylistsSongs>(new ApplicationDbContext(options.Options));
            var favouritesRepository = new EfDeletableEntityRepository<Favourites>(new ApplicationDbContext(options.Options));
            var lyricsRepository = new EfDeletableEntityRepository<Lyrics>(new ApplicationDbContext(options.Options));
            var mockHostingEnv = new Mock<IHostingEnvironment>().Object;
            var mockConfiguration = new Mock<IConfiguration>().Object;
            var service = new SongsService(mockHostingEnv, playlistsSongsRepository, favouritesRepository, lyricsRepository, songRepository, userRepository, mockConfiguration);

            var user = new ApplicationUser
            {
                UserName = "Tupac",
                FirstName = "gosh",
                LastName = "peshov",
                EmailConfirmed = true,
            };

            var usermanager = MockUserManager<ApplicationUser>();
            await usermanager.Object.CreateAsync(user, "123456");

            using var stream = File.OpenRead(@"C:\Users\haloho\Desktop\Stream App Content\Tupac- Smile.mp3");
            var file = new FormFile(stream, 0, stream.Length, null, Path.GetFileName(stream.Name))
            {
                Headers = new HeaderDictionary(),
                ContentType = "audio/mp3",
            };

            var songId = await service.UploadAsync(file, user.UserName, "Smile", "asdsaasd", "Pesho", null, Genre.Rap, 1999, "UserId", "asddsaasd.com", "Michael Jackson", null, "youtube.com", null, null);

            var songDB = await songRepository.All().Where(s => s.Id == songId).FirstOrDefaultAsync();

            Assert.NotNull(songDB);
            Assert.Equal("Pesho", songDB.Producer);
        }

        [Fact]
        public async Task DeleteShouldBeSuccesfull()
        {
            var options = new DbContextOptionsBuilder<ApplicationDbContext>().UseInMemoryDatabase(Guid.NewGuid().ToString());
            var userRepository = new EfRepository<ApplicationUser>(new ApplicationDbContext(options.Options));
            var songRepository = new EfDeletableEntityRepository<Song>(new ApplicationDbContext(options.Options));
            var playlistsSongsRepository = new EfDeletableEntityRepository<PlaylistsSongs>(new ApplicationDbContext(options.Options));
            var favouritesRepository = new EfDeletableEntityRepository<Favourites>(new ApplicationDbContext(options.Options));
            var lyricsRepository = new EfDeletableEntityRepository<Lyrics>(new ApplicationDbContext(options.Options));
            var mockHostingEnv = new Mock<IHostingEnvironment>().Object;
            var mockConfiguration = new Mock<IConfiguration>().Object;
            var service = new SongsService(mockHostingEnv, playlistsSongsRepository, favouritesRepository, lyricsRepository, songRepository, userRepository, mockConfiguration);

            var user = new ApplicationUser
            {
                UserName = "Tupac",
                FirstName = "gosh",
                LastName = "peshov",
                EmailConfirmed = true,
            };

            var usermanager = MockUserManager<ApplicationUser>();
            await usermanager.Object.CreateAsync(user, "123456");

            var song = new Song
            {
                Name = "Smile",
                Description = "asdsaasd",
                Producer = "Pesho",
                Genre = Genre.Rap,
                Year = 1999,
                UserId = user.Id,
                SongArtUrl = "asddsaasd.com",
                Featuring = "Michael Jackson",
                YoutubeUrl = "youtube.com",
            };
            await songRepository.AddAsync(song);
            await songRepository.SaveChangesAsync();

            await service.DeleteSong(song.Id);

            var songDB = await songRepository.All().Where(s => s.Id == song.Id).FirstOrDefaultAsync();

            Assert.Null(songDB);
        }

        [Fact]
        public async Task GetSongShouldWorkCorrect()
        {
            var options = new DbContextOptionsBuilder<ApplicationDbContext>().UseInMemoryDatabase(Guid.NewGuid().ToString());
            var userRepository = new EfRepository<ApplicationUser>(new ApplicationDbContext(options.Options));
            var songRepository = new EfDeletableEntityRepository<Song>(new ApplicationDbContext(options.Options));
            var playlistsSongsRepository = new EfDeletableEntityRepository<PlaylistsSongs>(new ApplicationDbContext(options.Options));
            var favouritesRepository = new EfDeletableEntityRepository<Favourites>(new ApplicationDbContext(options.Options));
            var lyricsRepository = new EfDeletableEntityRepository<Lyrics>(new ApplicationDbContext(options.Options));
            var mockHostingEnv = new Mock<IHostingEnvironment>().Object;
            var mockConfiguration = new Mock<IConfiguration>().Object;
            var service = new SongsService(mockHostingEnv, playlistsSongsRepository, favouritesRepository, lyricsRepository, songRepository, userRepository, mockConfiguration);

            var user = new ApplicationUser
            {
                UserName = "Tupac",
                FirstName = "gosh",
                LastName = "peshov",
                EmailConfirmed = true,
            };

            var usermanager = MockUserManager<ApplicationUser>();
            await usermanager.Object.CreateAsync(user, "123456");

            var song = new Song
            {
                Name = "Smile",
                Description = "asdsaasd",
                Producer = "Pesho",
                Genre = Genre.Rap,
                Year = 1999,
                UserId = user.Id,
                SongArtUrl = "asddsaasd.com",
                Featuring = "Michael Jackson",
                YoutubeUrl = "youtube.com",
            };
            await songRepository.AddAsync(song);
            await songRepository.SaveChangesAsync();

            var songtest = await songRepository.All().Where(s => s.Id == song.Id).FirstOrDefaultAsync();
            var songDB = await service.GetSong<SongViewModel>(song.Id);
            
            Assert.NotNull(songDB);
        }

        [Fact]
        public async Task GetAllShouldReturnCorrectCount()
        {
            var options = new DbContextOptionsBuilder<ApplicationDbContext>().UseInMemoryDatabase(Guid.NewGuid().ToString());
            var userRepository = new EfRepository<ApplicationUser>(new ApplicationDbContext(options.Options));
            var songRepository = new EfDeletableEntityRepository<Song>(new ApplicationDbContext(options.Options));
            var playlistsSongsRepository = new EfDeletableEntityRepository<PlaylistsSongs>(new ApplicationDbContext(options.Options));
            var favouritesRepository = new EfDeletableEntityRepository<Favourites>(new ApplicationDbContext(options.Options));
            var lyricsRepository = new EfDeletableEntityRepository<Lyrics>(new ApplicationDbContext(options.Options));
            var mockHostingEnv = new Mock<IHostingEnvironment>().Object;
            var mockConfiguration = new Mock<IConfiguration>().Object;
            var service = new SongsService(mockHostingEnv, playlistsSongsRepository, favouritesRepository, lyricsRepository, songRepository, userRepository, mockConfiguration);

            var user = new ApplicationUser
            {
                UserName = "Tupac",
                FirstName = "gosh",
                LastName = "peshov",
                EmailConfirmed = true,
            };

            var usermanager = MockUserManager<ApplicationUser>();
            await usermanager.Object.CreateAsync(user, "123456");

            var song = new Song
            {
                Name = "Smile",
                Description = "asdsaasd",
                Producer = "Pesho",
                Genre = Genre.Rap,
                Year = 1999,
                UserId = user.Id,
                SongArtUrl = "asddsaasd.com",
                Featuring = "Michael Jackson",
                YoutubeUrl = "youtube.com",
            };
            var song2 = new Song
            {
                Name = "Smile",
                Description = "asdsaasd",
                Producer = "Pesho",
                Genre = Genre.Rap,
                Year = 1999,
                UserId = user.Id,
                SongArtUrl = "asddsaasd.com",
                Featuring = "Michael Jackson",
                YoutubeUrl = "youtube.com",
            };
            var song3 = new Song
            {
                Name = "Smile",
                Description = "asdsaasd",
                Producer = "Pesho",
                Genre = Genre.Rap,
                Year = 1999,
                UserId = user.Id,
                SongArtUrl = "asddsaasd.com",
                Featuring = "Michael Jackson",
                YoutubeUrl = "youtube.com",
            };
            var song4 = new Song
            {
                Name = "Smile",
                Description = "asdsaasd",
                Producer = "Pesho",
                Genre = Genre.Rap,
                Year = 1999,
                UserId = user.Id,
                SongArtUrl = "asddsaasd.com",
                Featuring = "Michael Jackson",
                YoutubeUrl = "youtube.com",
            };
            await songRepository.AddAsync(song);
            await songRepository.AddAsync(song2);
            await songRepository.AddAsync(song3);
            await songRepository.AddAsync(song4);
            await songRepository.SaveChangesAsync();

            var songtest = await songRepository.All().Where(s => s.Id == song.Id).FirstOrDefaultAsync();    // works without automapper
            var songDB = service.GetAll<SongListViewModel>(3);

            Assert.Equal(3, songDB.Count());
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
