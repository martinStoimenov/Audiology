namespace Audiology.Services.Data.Comments
{
    using System.Collections.Generic;
    using System.Linq;
    using System.Threading.Tasks;

    using Audiology.Data.Common.Repositories;
    using Audiology.Data.Models;
    using Audiology.Services.Mapping;
    using Audiology.Web.ViewModels.Comments;
    using Microsoft.EntityFrameworkCore;

    public class CommentsService : ICommentsService
    {
        private readonly IDeletableEntityRepository<Comment> commentsRepository;

        public CommentsService(IDeletableEntityRepository<Comment> commentsRepository)
        {
            this.commentsRepository = commentsRepository;
        }

        public async Task<CommentViewModel> AddComment(string userId, int songId, string content)
        {
            var comment = new Comment
            {
                UserId = userId,
                SongId = songId,
                Content = content,
            };

            await this.commentsRepository.AddAsync(comment);
            await this.commentsRepository.SaveChangesAsync();

            return null;
        }

        public async Task<IEnumerable<CommentViewModel>> AllComments(int songId)
        {
            var comments = await this.commentsRepository.All().Where(c => c.SongId == songId).To<CommentViewModel>().ToListAsync();

            return comments;
        }

        public async Task Delete(string userId, int songId, int commentId)
        {
            var comment = await this.commentsRepository.All().Where(c => c.Id == commentId && c.SongId == songId && c.UserId == userId).FirstOrDefaultAsync();

            if (comment != null)
            {
                this.commentsRepository.Delete(comment);
            }

            await this.commentsRepository.SaveChangesAsync();
        }

        public async Task<int> Edit(string userId, int songId, int commentId, string content)
        {
            var comment = await this.commentsRepository.All().Where(c => c.Id == commentId && c.SongId == songId && c.UserId == userId).FirstOrDefaultAsync();

            if (content != null)
            {
                if (content.Length > 1000)
                {
                    comment.Content = content;
                    await this.commentsRepository.AddAsync(comment);
                }
            }
            await this.commentsRepository.SaveChangesAsync();

            return comment.Id;
        }
    }
}
