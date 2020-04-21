namespace Audiology.Web.ViewComponents
{
    using System.Linq;

    using Audiology.Data.Common.Repositories;
    using Audiology.Data.Models;
    using Microsoft.AspNetCore.Mvc;

    public class UserDetailsViewComponent : ViewComponent
    {
        private readonly IRepository<ApplicationUser> repository;

        public UserDetailsViewComponent(IRepository<ApplicationUser> repository)
        {
            this.repository = repository;
        }

        public IViewComponentResult Invoke(string userId)
        {
            var user = this.repository.All().Where(u => u.Id == userId).FirstOrDefault();
            user.Birthday?.ToString("dd.MM.yyyy");

            return this.View(user);
        }
    }
}
