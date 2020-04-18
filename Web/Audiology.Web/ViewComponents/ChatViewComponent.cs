namespace Audiology.Web.ViewComponents
{
    using System.Linq;
    using System.Threading.Tasks;

    using Audiology.Data;
    using Audiology.Data.Models;
    using Audiology.Web.ViewModels.Messages;
    using Microsoft.AspNetCore.Identity;
    using Microsoft.AspNetCore.Mvc;
    using Microsoft.EntityFrameworkCore;

    public class ChatViewComponent : ViewComponent
    {
        private readonly ApplicationDbContext context;
        private readonly UserManager<ApplicationUser> userManager;

        public ChatViewComponent(ApplicationDbContext context, UserManager<ApplicationUser> userManager)
        {
            this.context = context;
            this.userManager = userManager;
        }

        public async Task<IViewComponentResult> InvokeAsync(string artistId)
        {
            this.ViewData["ArtistId"] = artistId; // try storing it in viewbag

            // remove the current artist from the list and retrieve the messages stored in DB
            var convos = await this.context.Messages
                .Where(m => m.ArtistId == artistId)
                .Select(m => new MessagesDropDownViewModel { UserId = m.UserId, Name = m.User.UserName })
                .Distinct()
                .ToListAsync();

            var messages = await this.context.Messages.Where(m => m.ArtistId == artistId).Select(m => new MessagesListViewModel { CreatedOn = m.CreatedOn.ToShortTimeString(), Text = m.Text, UserName = m.User.UserName }).ToListAsync();

            this.ViewData["Messages"] = messages;

            return this.View(convos);
        }
    }
}
