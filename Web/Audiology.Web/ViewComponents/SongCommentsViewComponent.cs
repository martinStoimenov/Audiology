namespace Audiology.Web.ViewComponents
{
    using System;
    using System.Collections.Generic;
    using System.Linq;
    using System.Threading.Tasks;

    using Audiology.Services.Data.Comments;
    using Microsoft.AspNetCore.Mvc;

    public class SongCommentsViewComponent : ViewComponent
    {
        private readonly ICommentsService service;

        public SongCommentsViewComponent(ICommentsService service)
        {
            this.service = service;
        }

        public async Task<IViewComponentResult> InvokeAsync(int songId)
        {
            var comments = await this.service.AllComments(songId);

            return this.View(comments);
        }
    }
}
