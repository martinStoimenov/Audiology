namespace Audiology.Services.Data.Profile
{
    using System;
    using System.Linq;
    using System.Threading.Tasks;

    using Audiology.Data.Common.Repositories;
    using Audiology.Data.Models;
    using Audiology.Services.Mapping;
    using Microsoft.EntityFrameworkCore;

    public class ProfileService : IProfileService
    {
        private readonly IRepository<ApplicationUser> repository;

        public ProfileService(IRepository<ApplicationUser> repository)
        {
            this.repository = repository;
        }

        public async Task<T> GetUserAsync<T>(string userId)
        {
            var user = await this.repository.All().Where(u => u.Id == userId).To<T>().FirstOrDefaultAsync();

            if (user == null)
            {
                throw new ArgumentNullException("Requested user wasn't found.");
            }

            return user;
        }
    }
}
