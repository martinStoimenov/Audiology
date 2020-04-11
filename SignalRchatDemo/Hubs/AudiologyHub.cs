namespace SignalRchatDemo.Hubs
{
    using Microsoft.AspNetCore.Authorization;
    using Microsoft.AspNetCore.SignalR;
    using SignalRchatDemo.Models.Chat;
    using System.Threading.Tasks;

    [Authorize]
    public class AudiologyHub : Hub
    {
        public async Task Send(string message)
        {
            await this.Clients.All.SendAsync(
                "NewMessage",
                new Message { User = this.Context.User.Identity.Name, Text = message, });
        }
    }
}
