namespace Audiology.Web.ViewComponents
{
    using System.Linq;
    using System.Threading.Tasks;

    using Audiology.Data;
    using Audiology.Web.ViewModels.Messages;
    using Microsoft.AspNetCore.Mvc;
    using Microsoft.EntityFrameworkCore;

    public class ChatViewComponent : ViewComponent
    {
        private readonly ApplicationDbContext context;

        public ChatViewComponent(ApplicationDbContext context)
        {
            this.context = context;
        }

        public async Task<IViewComponentResult> InvokeAsync(string artistId)
        {
            // remove the current artist from the list and retrieve the messages stored in DB
            var convos = await this.context.Messages
                .Where(m => m.ArtistId == artistId)
                .Select(m => new MessagesDropDownViewModel { UserId = m.UserId, Name = m.User.UserName })
                .Distinct()
                .ToListAsync();

            this.ViewData["ArtistId"] = artistId;

            return this.View(convos);
        }
    }
}
