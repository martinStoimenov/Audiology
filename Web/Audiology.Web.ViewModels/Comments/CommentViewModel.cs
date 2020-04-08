namespace Audiology.Web.ViewModels.Comments
{
    using System.ComponentModel.DataAnnotations;

    using Audiology.Data.Models;
    using Audiology.Services.Mapping;

    public class CommentViewModel : IMapFrom<Comment>, IMapTo<Comment>
    {
        public int Id { get; set; }

        public int SongId { get; set; }

        public string UserId { get; set; }

        public string UserUserName { get; set; }

        public string UserProfilePicUrl { get; set; }

        [Required]
        [MaxLength(1000)]
        public string Content { get; set; }
    }
}
