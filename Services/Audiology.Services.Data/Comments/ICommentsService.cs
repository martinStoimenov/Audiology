namespace Audiology.Services.Data.Comments
{
    using System.Collections.Generic;
    using System.Threading.Tasks;

    using Audiology.Web.ViewModels.Comments;

    public interface ICommentsService
    {
        Task AddComment(string userId, int songId, string content);

        Task<IEnumerable<CommentViewModel>> AllComments(int songId);

        Task Delete(string userId, int songId, int commentId);
    }
}
