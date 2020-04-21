namespace Audiology.Services.Data.Tests
{
    using System;
    using System.Linq;
    using System.Reflection;
    using System.Threading.Tasks;

    using Audiology.Data;
    using Audiology.Data.Models;
    using Audiology.Data.Repositories;
    using Audiology.Services.Data.Comments;
    using Audiology.Services.Mapping;
    using Audiology.Web.ViewModels.Comments;
    using AutoMapper;
    using Microsoft.EntityFrameworkCore;
    using Moq;
    using Xunit;

    public class CommentsServiceTests
    {
        public CommentsServiceTests()
        {
            AutoMapperConfig.RegisterMappings(
                typeof(CommentsService).GetTypeInfo().Assembly,
                typeof(CommentViewModel).GetTypeInfo().Assembly);
        }

        [Fact]
        public async Task AddCommentShouldAddCorrectly()
        {
            var options = new DbContextOptionsBuilder<ApplicationDbContext>().UseInMemoryDatabase(Guid.NewGuid().ToString());
            var commentRepository = new EfDeletableEntityRepository<Comment>(new ApplicationDbContext(options.Options));
            var service = new CommentsService(commentRepository);

            await service.AddComment("gudId", 1, "content of the comment.");

            var comment = await commentRepository.All().Where(c => c.SongId == 1).FirstOrDefaultAsync();

            Assert.Equal("content of the comment.", comment.Content);
        }

        [Fact]
        public async Task AllCommentsShouldReturnCorrectCount()
        {
            var options = new DbContextOptionsBuilder<ApplicationDbContext>().UseInMemoryDatabase(Guid.NewGuid().ToString());
            var commentRepository = new EfDeletableEntityRepository<Comment>(new ApplicationDbContext(options.Options));
            var service = new CommentsService(commentRepository);

            var c = new Comment
            {
                UserId = "gudId",
                SongId = 1,
                Content = "content of the comment.",
            };
            var c1 = new Comment
            {
                UserId = "gudId1",
                SongId = 1,
                Content = "content of the comment1.",
            };
            var c2 = new Comment
            {
                UserId = "gudId2",
                SongId = 1,
                Content = "content of the comment2.",
            };

            await commentRepository.AddAsync(c);
            await commentRepository.AddAsync(c1);
            await commentRepository.AddAsync(c2);
            await commentRepository.SaveChangesAsync();

            var comments = await service.AllComments(1);

            Assert.Equal(3, comments.Count());
        }

        [Fact]
        public async Task DeleteCommentShouldDeleteCorrectly()
        {
            var options = new DbContextOptionsBuilder<ApplicationDbContext>().UseInMemoryDatabase(Guid.NewGuid().ToString());
            var commentRepository = new EfDeletableEntityRepository<Comment>(new ApplicationDbContext(options.Options));
            var service = new CommentsService(commentRepository);

            var c = new Comment
            {
                UserId = "gudId",
                SongId = 1,
                Content = "content of the comment.",
            };
            var c1 = new Comment
            {
                UserId = "gudId1",
                SongId = 1,
                Content = "content of the comment1.",
            };
            var c2 = new Comment
            {
                UserId = "gudId2",
                SongId = 1,
                Content = "content of the comment2.",
            };

            await commentRepository.AddAsync(c);
            await commentRepository.AddAsync(c1);
            await commentRepository.AddAsync(c2);
            await commentRepository.SaveChangesAsync();

            await service.Delete("gudId1", 1, c1.Id);
            var comment = await commentRepository.All().Where(c => c.Id == c1.Id).FirstOrDefaultAsync();

            Assert.Null(comment);
        }

        [Fact]
        public async Task DeleteCommentShouldNotDeleteIfNoComment()
        {
            var options = new DbContextOptionsBuilder<ApplicationDbContext>().UseInMemoryDatabase(Guid.NewGuid().ToString());
            var commentRepository = new EfDeletableEntityRepository<Comment>(new ApplicationDbContext(options.Options));
            var service = new CommentsService(commentRepository);

            var c = new Comment
            {
                UserId = "gudId",
                SongId = 1,
                Content = "content of the comment.",
            };
            var c1 = new Comment
            {
                UserId = "gudId1",
                SongId = 1,
                Content = "content of the comment1.",
            };
            var c2 = new Comment
            {
                UserId = "gudId2",
                SongId = 1,
                Content = "content of the comment2.",
            };

            await commentRepository.AddAsync(c);
            await commentRepository.AddAsync(c1);
            await commentRepository.AddAsync(c2);
            await commentRepository.SaveChangesAsync();

            await service.Delete("sadaasdda4", 1, c1.Id);
            var comment = await commentRepository.All().Where(c => c.Id == c1.Id).FirstOrDefaultAsync();

            Assert.NotNull(comment);
        }
    }
}
