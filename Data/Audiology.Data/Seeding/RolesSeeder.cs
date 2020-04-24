namespace AspNetCoreTemplate.Data.Seeding
{
    using System;
    using System.Linq;
    using System.Threading.Tasks;

    using Audiology.Common;
    using Audiology.Data;
    using Audiology.Data.Models;
    using Audiology.Data.Models.Enumerations;
    using Audiology.Data.Seeding;
    using Microsoft.AspNetCore.Identity;
    using Microsoft.EntityFrameworkCore;
    using Microsoft.Extensions.DependencyInjection;

    internal class RolesSeeder : ISeeder
    {
        public async Task SeedAsync(ApplicationDbContext dbContext, IServiceProvider serviceProvider)
        {
            var roleManager = serviceProvider.GetRequiredService<RoleManager<ApplicationRole>>();
            var userManager = serviceProvider.GetRequiredService<UserManager<ApplicationUser>>();

            await SeedRoleAsync(roleManager, GlobalConstants.AdministratorRoleName);

            await SeedRoleAsync(roleManager, GlobalConstants.UserRoleName);

            await SeedRoleAsync(roleManager, GlobalConstants.ArtistRoleName);

            if (!await userManager.Users.AnyAsync())
            {
                var user = new ApplicationUser
                {
                    UserName = "admin",
                    Email = "admin@gmail.com",
                    EmailConfirmed = true,
                    FirstName = "Admin",
                    LastName = "Admin",
                    Gender = Gender.Male,
                };

                var password = "askjdhAAd#3";

                var result = await userManager.CreateAsync(user, password);

                if (result.Succeeded)
                {
                    await userManager.AddToRoleAsync(user, GlobalConstants.AdministratorRoleName);
                }
            }
        }

        private static async Task SeedRoleAsync(RoleManager<ApplicationRole> roleManager, string roleName)
        {
            var role = await roleManager.FindByNameAsync(roleName);
            if (role == null)
            {
                var result = await roleManager.CreateAsync(new ApplicationRole(roleName));
                if (!result.Succeeded)
                {
                    throw new Exception(string.Join(Environment.NewLine, result.Errors.Select(e => e.Description)));
                }
            }


        }
    }
}