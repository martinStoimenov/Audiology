namespace Audiology.Services.Data.Tests
{
    using System;
    using System.Linq;
    using System.Reflection;
    using System.Threading.Tasks;

    using Audiology.Data;
    using Audiology.Data.Common.Repositories;
    using Audiology.Data.Models;
    using Audiology.Data.Models.Enumerations;
    using Audiology.Data.Repositories;
    using Audiology.Services.Data.Administration;
    using Audiology.Services.Mapping;
    using Audiology.Web.ViewModels.Administration.Dashboard;
    using Audiology.Web.ViewModels.Profile;
    using Microsoft.EntityFrameworkCore;
    using Xunit;

    public class AdminServiceTests
    {
        public AdminServiceTests()
        {
            AutoMapperConfig.RegisterMappings(
                typeof(AdminService).GetTypeInfo().Assembly,
                typeof(ArtistProfileViewModel).GetTypeInfo().Assembly,
                typeof(SongDropDownViewModel).GetTypeInfo().Assembly);
        }

        [Fact]
        public async Task EditLyricsAsyncShouldEditCorrectly()
        {
            var options = new DbContextOptionsBuilder<ApplicationDbContext>().UseInMemoryDatabase(Guid.NewGuid().ToString());
            var userRepo = new EfRepository<ApplicationUser>(new ApplicationDbContext(options.Options));
            var lyricsRepo = new EfDeletableEntityRepository<Lyrics>(new ApplicationDbContext(options.Options));
            var songRepo = new EfDeletableEntityRepository<Song>(new ApplicationDbContext(options.Options));
            var service = new AdminService(userRepo, lyricsRepo, songRepo);

            var song = new Song
            {
                Name = "Test Song",
                Producer = "asddsaasd",
                SongArtUrl = "skjahjdsakkashd.com",
                CreatedOn = DateTime.Now,
                IsDeleted = false,
                UserId = "dhsakja",
                Description = "kjldsaklsajdkjasklsad",
                Genre = Genre.Rap,
                Year = 2000,
            };
            await songRepo.AddAsync(song);
            await songRepo.SaveChangesAsync();

            var song1 = await lyricsRepo.All().Where(s => s.Id == song.Id).FirstOrDefaultAsync();

            song1 = new Lyrics
            {
                Text = "asd",
                SongId = song.Id,
            };
            await lyricsRepo.AddAsync(song1);
            await lyricsRepo.SaveChangesAsync();

            await service.EditLyricsAsync("opaaa song lyrics by \r\nAdmin \r\n\r\nhere \r\n\r\n\r\nbruh\r\nand again", song1.Id);

            var song2 = await lyricsRepo.All().Where(s => s.Id == song1.Id).FirstOrDefaultAsync();

            Assert.Equal("opaaa song lyrics by <br>Admin <br><br>here <br><br><br>bruh<br>and again", song2.Text);
        }

        [Fact]
        public async Task DeleteLyricsShouldBeSuccessfull()
        {
            var options = new DbContextOptionsBuilder<ApplicationDbContext>().UseInMemoryDatabase(Guid.NewGuid().ToString());
            var userRepo = new EfRepository<ApplicationUser>(new ApplicationDbContext(options.Options));
            var lyricsRepo = new EfDeletableEntityRepository<Lyrics>(new ApplicationDbContext(options.Options));
            var songRepo = new EfDeletableEntityRepository<Song>(new ApplicationDbContext(options.Options));
            var service = new AdminService(userRepo, lyricsRepo, songRepo);

            var song = new Song
            {
                Name = "Test Song",
                Producer = "asddsaasd",
                SongArtUrl = "skjahjdsakkashd.com",
                CreatedOn = DateTime.Now,
                IsDeleted = false,
                UserId = "dhsakja",
                Description = "kjldsaklsajdkjasklsad",
                Genre = Genre.Rap,
                Year = 2000,
            };
            await songRepo.AddAsync(song);
            await songRepo.SaveChangesAsync();

            await service.DeleteLyricsAsync(song.Id);

            var songDB = await songRepo.All().Where(s => s.Id == song.Id).FirstOrDefaultAsync();

            Assert.Null(songDB.Lyrics);
        }

        [Fact]
        public async Task GetUserAsyncShouldBeSuccesfull()
        {
            var options = new DbContextOptionsBuilder<ApplicationDbContext>().UseInMemoryDatabase(Guid.NewGuid().ToString());
            var userRepo = new EfRepository<ApplicationUser>(new ApplicationDbContext(options.Options));
            var lyricsRepo = new EfDeletableEntityRepository<Lyrics>(new ApplicationDbContext(options.Options));
            var songRepo = new EfDeletableEntityRepository<Song>(new ApplicationDbContext(options.Options));
            var service = new AdminService(userRepo, lyricsRepo, songRepo);

            var user = new ApplicationUser
            {
                Id = Guid.NewGuid().ToString(),
                UserName = "Gosho",
                FirstName = "gosh",
                LastName = "peshov",
                EmailConfirmed = true,
                Gender = Gender.Female,
                Birthday = DateTime.UtcNow,
                Email = "asdas@gmail.com",
            };

            await userRepo.AddAsync(user);
            await userRepo.SaveChangesAsync();

            var dbUser = await service.GetUserAsync<ArtistProfileViewModel>(user.Id);

            Assert.NotNull(dbUser);
        }

        [Fact]
        public async Task AllSongsShoudReturnAll()
        {
            var options = new DbContextOptionsBuilder<ApplicationDbContext>().UseInMemoryDatabase(Guid.NewGuid().ToString());
            var userRepo = new EfRepository<ApplicationUser>(new ApplicationDbContext(options.Options));
            var lyricsRepo = new EfDeletableEntityRepository<Lyrics>(new ApplicationDbContext(options.Options));
            var songRepo = new EfDeletableEntityRepository<Song>(new ApplicationDbContext(options.Options));
            var service = new AdminService(userRepo, lyricsRepo, songRepo);

            var song = new Song
            {
                Name = "Test Song",
                Producer = "asddsaasd",
                SongArtUrl = "skjahjdsakkashd.com",
                CreatedOn = DateTime.Now,
                IsDeleted = false,
                UserId = "dhsakja",
                Description = "kjldsaklsajdkjasklsad",
                Genre = Genre.Rap,
                Year = 2000,
            };

            var song2 = new Song
            {
                Name = "Test2 Song2",
                Producer = "asddsaasd",
                SongArtUrl = "skjahjdsakkashd.com",
                CreatedOn = DateTime.Now,
                IsDeleted = false,
                UserId = "dhsakja",
                Description = "kjldsaklsajdkjasklsad",
                Genre = Genre.Rap,
                Year = 2000,
            };
            await songRepo.AddAsync(song);
            await songRepo.AddAsync(song2);
            await songRepo.SaveChangesAsync();

            var allSongs = await service.AllSongs<SongDropDownViewModel>();

            Assert.Equal(2, allSongs.Count());
        }

        [Fact]
        public async Task GetLyricsForSongShouldBeSuccessfull()
        {
            var options = new DbContextOptionsBuilder<ApplicationDbContext>().UseInMemoryDatabase(Guid.NewGuid().ToString());
            var userRepo = new EfRepository<ApplicationUser>(new ApplicationDbContext(options.Options));
            var lyricsRepo = new EfDeletableEntityRepository<Lyrics>(new ApplicationDbContext(options.Options));
            var songRepo = new EfDeletableEntityRepository<Song>(new ApplicationDbContext(options.Options));
            var service = new AdminService(userRepo, lyricsRepo, songRepo);

            var song = new Song
            {
                Name = "Test Song",
                Producer = "asddsaasd",
                SongArtUrl = "skjahjdsakkashd.com",
                CreatedOn = DateTime.Now,
                IsDeleted = false,
                UserId = "dhsakja",
                Description = "kjldsaklsajdkjasklsad",
                Genre = Genre.Rap,
                Year = 2000,
            };

            await songRepo.AddAsync(song);
            await songRepo.SaveChangesAsync();

            var song1 = await songRepo.All().Where(s => s.Id == song.Id).FirstOrDefaultAsync();

            song1.Lyrics = new Lyrics
            { Text = "opaaa song lyrics by \r\nAdmin \r\n\r\nhere \r\n\r\n\r\nbruh\r\nand again",
            SongId = song1.Id, };
            songRepo.Update(song1);
            await songRepo.SaveChangesAsync();

            var lyrics = await service.GetLyricsForSong(song1.Id);

            Assert.Equal("opaaa song lyrics by \r\nAdmin \r\n\r\nhere \r\n\r\n\r\nbruh\r\nand again", lyrics);
        }

        [Fact]
        public async Task EditUserShouldBeSuccessfull()
        {
            var options = new DbContextOptionsBuilder<ApplicationDbContext>().UseInMemoryDatabase(Guid.NewGuid().ToString());
            var userRepo = new EfRepository<ApplicationUser>(new ApplicationDbContext(options.Options));
            var lyricsRepo = new EfDeletableEntityRepository<Lyrics>(new ApplicationDbContext(options.Options));
            var songRepo = new EfDeletableEntityRepository<Song>(new ApplicationDbContext(options.Options));
            var service = new AdminService(userRepo, lyricsRepo, songRepo);

            var user = new ApplicationUser
            {
                UserName = "Gosho",
                FirstName = "gosh",
                LastName = "peshov",
                EmailConfirmed = true,
                Gender = Gender.Female,
                Birthday = DateTime.UtcNow,
                Email = "asdas@gmail.com",
                ProfilePicUrl = "asdas.com",
                FacebookUrl = "www.facebook.com",
            };

            await userRepo.AddAsync(user);
            await userRepo.SaveChangesAsync();

            await service.EditUserAsync(user.Id, "asddsaa.png", "Peshkata", "Goshobratmu", "adaasd", "emai@gmail.com", DateTime.Now.AddDays(2), "instagramurl", null, null, null, null, Gender.Male);
            var user1 = await userRepo.All().Where(x => x.Id == user.Id).FirstOrDefaultAsync();

            Assert.NotEqual("Gosho", user1.FirstName);
            Assert.NotEqual("gosh", user1.LastName);
            Assert.NotEqual(Gender.Female, user1.Gender);
            Assert.NotEqual("asdas@gmail.com", user1.Email);
            Assert.NotEqual("www.facebook.com", user1.FacebookUrl);
        }
    }
}
