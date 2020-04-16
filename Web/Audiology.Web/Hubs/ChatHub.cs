namespace Audiology.Web.Hubs
{
    using System.Threading.Tasks;

    using Audiology.Data;
    using Audiology.Data.Models;
    using Audiology.Web.ViewModels;
    using Microsoft.AspNetCore.Authorization;
    using Microsoft.AspNetCore.Identity;
    using Microsoft.AspNetCore.SignalR;

    [Authorize]
    public class ChatHub : Hub
    {
        private readonly ApplicationDbContext context;
        private readonly UserManager<ApplicationUser> userManager;

        public ChatHub(ApplicationDbContext context, UserManager<ApplicationUser> userManager)
        {
            this.context = context;
            this.userManager = userManager;
        }

        public async Task Send(string message, string artistId)
        {
            await this.Clients.User(artistId).SendAsync(
                "NewMessage",
                new MessageModel { User = this.Context.User.Identity.Name, Text = message, });

            if (message.Length < 1000)
            {
                var userId = this.userManager.GetUserId(this.Context.User);
                var dbMessage = new Messages { ArtistId = artistId, UserId = userId, Text = message };
                await this.context.Messages.AddAsync(dbMessage);
                await this.context.SaveChangesAsync();
            }
        }
    }
}
