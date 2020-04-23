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
    using Audiology.Services.Data.Profile;
    using Audiology.Services.Data.Songs;
    using Audiology.Services.Mapping;
    using Audiology.Web.ViewModels.Albums;
    using Audiology.Web.ViewModels.Profile;
    using AutoMapper;
    using Microsoft.AspNetCore.Identity;
    using Microsoft.EntityFrameworkCore;

    using Moq;

    using Xunit;

    public class ProfileServiceTests
    {
        public ProfileServiceTests()
        {
            AutoMapperConfig.RegisterMappings(
                typeof(ProfileService).GetTypeInfo().Assembly,
                typeof(ArtistProfileViewModel).GetTypeInfo().Assembly);
        }

        [Fact]
        public async Task GetUserAsyncTemplateShouldWorkCorrect()
        {
            var options = new DbContextOptionsBuilder<ApplicationDbContext>().UseInMemoryDatabase(Guid.NewGuid().ToString());
            var userRepository = new EfRepository<ApplicationUser>(new ApplicationDbContext(options.Options));
            var mockSongsService = new Mock<ISongsServcie>().Object;
            var service = new ProfileService(userRepository);

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

            var userDB = await service.GetUserAsync<ArtistProfileViewModel>(user.Id);

            Assert.Equal(user.Id, userDB.Id);
        }

        [Fact]
        public async Task GetUserAsyncShouldWorkCorrect()
        {
            var options = new DbContextOptionsBuilder<ApplicationDbContext>().UseInMemoryDatabase(Guid.NewGuid().ToString());
            var userRepository = new EfRepository<ApplicationUser>(new ApplicationDbContext(options.Options));
            var mockSongsService = new Mock<ISongsServcie>().Object;
            var service = new ProfileService(userRepository);

            var user = new ApplicationUser
            {
                UserName = "Gosho",
                FirstName = "gosh",
                LastName = "peshov",
                EmailConfirmed = true,
            };

            var usermanager = MockUserManager<ApplicationUser>();
            await usermanager.Object.CreateAsync(user, "123456");

            var userDB = await service.GetUserAsync(user.Id);

            Assert.Equal(user.Id, userDB.Id);
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
