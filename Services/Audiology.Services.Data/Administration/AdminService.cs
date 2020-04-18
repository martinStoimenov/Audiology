namespace Audiology.Services.Data.Administration
{
    using System;
    using System.Collections.Generic;
    using System.Linq;
    using System.Text.RegularExpressions;
    using System.Threading.Tasks;

    using Audiology.Data.Common.Repositories;
    using Audiology.Data.Models;
    using Audiology.Data.Models.Enumerations;
    using Audiology.Services.Mapping;
    using Microsoft.EntityFrameworkCore;

    public class AdminService : IAdminService
    {
        private readonly IRepository<ApplicationUser> userRepository;
        private readonly IDeletableEntityRepository<Lyrics> lyricsRepository;
        private readonly IDeletableEntityRepository<Song> songsRepository;

        public AdminService(
            IRepository<ApplicationUser> userRepository,
            IDeletableEntityRepository<Lyrics> lyricsRepository,
            IDeletableEntityRepository<Song> songsRepository)
        {
            this.userRepository = userRepository;
            this.lyricsRepository = lyricsRepository;
            this.songsRepository = songsRepository;
        }

        public async Task AddLyricsAsync(string text, int songId)
        {
            var lyrics = await this.lyricsRepository.All().Where(l => l.SongId == songId).FirstOrDefaultAsync();

            if (lyrics != null)
            {
                string html = Regex.Replace(text, @"(\r\n)|\n|\r", "<br>");

                if (text.Length < 5000)
                {
                    lyrics.Text = html;
                }

                this.lyricsRepository.Update(lyrics);
                await this.lyricsRepository.SaveChangesAsync();
            }
        }

        public async Task DeleteLyricsAsync(int songId)
        {
            var lyrics = await this.lyricsRepository.All().Where(l => l.SongId == songId).FirstOrDefaultAsync();

            if (lyrics != null)
            {
                this.lyricsRepository.HardDelete(lyrics);
                await this.lyricsRepository.SaveChangesAsync();
            }
        }

        public async Task EditUserAsync(string userId, string profilePicUrl, string firstName, string lastName, string username, string email, DateTime? birthday, string instagram, string facebook, string youtube, string twitter, string soundcloud, Enum gender)
        {
            var user = await this.userRepository.All().Where(u => u.Id == userId).FirstOrDefaultAsync();

            if (user != null)
            {
                user.ProfilePicUrl = profilePicUrl;
                user.FirstName = firstName;
                user.LastName = lastName;
                user.UserName = username;
                user.Email = email;
                user.Birthday = birthday;
                user.InstagramUrl = instagram;
                user.FacebookUrl = facebook;
                user.YouTubeUrl = youtube;
                user.TwitterUrl = twitter;
                user.SondcloudUrl = soundcloud;
                user.Gender = (Gender)gender;
                this.userRepository.Update(user);
            }

            await this.userRepository.SaveChangesAsync();
        }

        public async Task<T> GetUserAsync<T>(string userId)
        {
            var user = await this.userRepository.All().Where(u => u.Id == userId).To<T>().FirstOrDefaultAsync();

            return user;
        }

        public async Task<IEnumerable<T>> AllSongs<T>()
        {
            var songs = await this.songsRepository.All().To<T>().ToListAsync();

            return songs;
        }

        public async Task<string> GetLyricsForSong(int songId)
        {
            var lyrics = await this.lyricsRepository.All().Where(l => l.SongId == songId).Select(l => l.Text).FirstOrDefaultAsync();

            return lyrics;
        }
    }
}
