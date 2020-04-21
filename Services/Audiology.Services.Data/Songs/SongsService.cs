namespace Audiology.Services.Data.Songs
{
    using System;
    using System.Collections.Generic;
    using System.IO;
    using System.Linq;
    using System.Net.Http;
    using System.Text.Json;
    using System.Text.RegularExpressions;
    using System.Threading.Tasks;

    using Audiology.Data;
    using Audiology.Data.Common.Repositories;
    using Audiology.Data.Models;
    using Audiology.Data.Models.Enumerations;
    using Audiology.Services.Mapping;
    using Audiology.Web.ViewModels.Songs;
    using HtmlAgilityPack;
    using Microsoft.AspNetCore.Hosting;
    using Microsoft.AspNetCore.Http;
    using Microsoft.AspNetCore.Identity;
    using Microsoft.EntityFrameworkCore;
    using Microsoft.Extensions.Configuration;
    using NAudio.Wave;

    public class SongsService : ISongsServcie
    {
        private static readonly char Slash = Path.DirectorySeparatorChar;

        private readonly IHostingEnvironment env;
        private readonly IDeletableEntityRepository<PlaylistsSongs> playlistsSongsRepo;
        private readonly IDeletableEntityRepository<Favourites> favouritesRepo;
        private readonly IDeletableEntityRepository<Lyrics> lyricsRepository;
        private readonly IRepository<ApplicationUser> userRepo;
        private readonly IRepository<Song> songRepository;
        private readonly IConfiguration configuration;

        public SongsService(
            IHostingEnvironment env,
            IDeletableEntityRepository<PlaylistsSongs> playlistsSongsRepo,
            IDeletableEntityRepository<Favourites> favouritesRepo,
            IDeletableEntityRepository<Lyrics> lyricsRepository,
            IDeletableEntityRepository<Song> songRepository,
            IRepository<ApplicationUser> userRepo,
            IConfiguration configuration)
        {
            this.env = env;
            this.playlistsSongsRepo = playlistsSongsRepo;
            this.lyricsRepository = lyricsRepository;
            this.songRepository = songRepository;
            this.favouritesRepo = favouritesRepo;
            this.configuration = configuration;
            this.userRepo = userRepo;
        }

        public async Task DeleteSong(int songId)
        {
            var song = await this.songRepository.All().Where(s => s.Id == songId).FirstOrDefaultAsync();
            var playlistSong = await this.playlistsSongsRepo.All().Where(ps => ps.SongId == song.Id).FirstOrDefaultAsync();
            var favouritedSong = await this.favouritesRepo.All().Where(f => f.SongId == songId).FirstOrDefaultAsync();
            var lyricsSong = await this.lyricsRepository.All().Where(l => l.SongId == songId).FirstOrDefaultAsync();
            var userName = await this.userRepo.All().Where(u => u.Id == song.UserId).Select(x => x.UserName).FirstOrDefaultAsync();

            if (song != null)
            {
                if (playlistSong != null)
                {
                    this.playlistsSongsRepo.Delete(playlistSong);
                    await this.playlistsSongsRepo.SaveChangesAsync();
                }

                if (favouritedSong != null)
                {
                    this.favouritesRepo.Delete(favouritedSong);
                    await this.favouritesRepo.SaveChangesAsync();
                }

                if (lyricsSong != null)
                {
                    this.lyricsRepository.Delete(lyricsSong);
                    await this.lyricsRepository.SaveChangesAsync();
                }

                string webRootPath = Path.Combine(this.env.WebRootPath, "Songs");

                string songName = song.Name;
                string username = userName;

                string fullPath = Path.Combine(webRootPath, username, songName);

                this.songRepository.Delete(song);
                await this.songRepository.SaveChangesAsync();

                if (File.Exists(fullPath))
                {
                    File.Delete(fullPath);
                }
            }
        }

        public async Task<int> UploadAsync(IFormFile input, string username, string songName, string description, string producer, int? albumId, Enum genre, int year, string userId, string songArt, string featuring, string writtenBy, string youtubeUrl, string soundcloudUrl, string instagramPostUrl)
        {
            var songExists = await this.songRepository.All().Where(s => s.UserId == userId && s.Name == songName).FirstOrDefaultAsync();

            if (songExists != null)
            {
                throw new ArgumentException("Song alredy exist.");
            }

            var dotIndex = input.FileName.LastIndexOf('.');
            var fileExtension = input.FileName.Substring(dotIndex);

            string webRootPath = this.env.WebRootPath + Slash + "Songs" + Slash;

            if (!Directory.Exists(webRootPath + username))
            {
                Directory.CreateDirectory(webRootPath + username);
            }

            string name = songName + fileExtension;

            var filePath = Path.Combine(webRootPath, username, name);

            using (var fileStream = new FileStream(filePath, FileMode.Create))
            {
                await input.CopyToAsync(fileStream);
            }

            var song = new Song
            {
                UserId = userId,
                SongArtUrl = songArt,
                Name = name,
                Description = description,
                Year = year,
                Producer = producer,
                AlbumId = albumId,
                Genre = (Genre)genre,
                Featuring = featuring,
                WrittenBy = writtenBy,
                YoutubeUrl = youtubeUrl,
                SoundcloudUrl = soundcloudUrl,
                InstagramPostUrl = instagramPostUrl,
            };

            await this.songRepository.AddAsync(song);
            await this.songRepository.SaveChangesAsync();

            bool isFound = await this.GetLyricsForSong(song, username);

            return song.Id;
        }

        public async Task<int> EditSongAsync(int id, string username, string name, string description, int? albumId, string producer, string songArtUrl, Enum genre, int year, string featuring, string writtenBy, string youtubeUrl, string soundcloudUrl, string instagramPostUrl)
        {
            var song = this.songRepository.All().Where(s => s.Id == id).Include(s => s.User).FirstOrDefault();

            int dotIndex = song.Name.LastIndexOf(".");
            string fileExtension = song.Name.Substring(dotIndex);
            if (dotIndex == -1)
            {
                throw new IndexOutOfRangeException("file extension is missing");
            }

            if (song.Name != name)
            {
                string newName = name + fileExtension;

                string webRootPath = this.env.WebRootPath + Slash + "Songs" + Slash;

                var newPath = Path.Combine(webRootPath, username, newName);

                string oldPath = webRootPath + username + Slash + song.Name;

                var file = new FileInfo(oldPath);

                file.MoveTo(newPath);

                bool isFound = await this.GetLyricsForSong(song);

                song.Name = newName;
            }

            song.Year = year;
            song.AlbumId = albumId;
            song.Producer = producer;
            song.Genre = (Genre)genre;
            song.Featuring = featuring;
            song.WrittenBy = writtenBy;
            song.YoutubeUrl = youtubeUrl;
            song.SongArtUrl = songArtUrl;
            song.Description = description;
            song.SoundcloudUrl = soundcloudUrl;
            song.InstagramPostUrl = instagramPostUrl;

            this.songRepository.Update(song);
            await this.songRepository.SaveChangesAsync();

            return song.Id;
        }

        public async Task<T> GetSong<T>(int songId)
        {
            var song = await this.songRepository.All().Where(s => s.Id == songId).To<T>().FirstOrDefaultAsync();
            return song;
        }

        public IEnumerable<T> GetAll<T>(int? count = null)
        {
            IQueryable<Song> query =
                this.songRepository.All().OrderByDescending(x => x.CreatedOn);
            if (count.HasValue)
            {
                query = query.Take(count.Value);
            }

            return query.To<T>().ToList();
        }

        public async Task<IEnumerable<T>> GetAllSongsForUserAsync<T>(string userId)
        {
            var songs = await this.songRepository.All().Where(s => s.UserId == userId).OrderByDescending(s => s.CreatedOn).To<T>().ToListAsync();

            return songs;
        }

        public async Task<IEnumerable<T>> GetTopSongsForUserAsync<T>(string userId, int count)
        {
            var songs = await this.songRepository.All().Where(s => s.UserId == userId).OrderByDescending(s => s.FavouritesCount).Take(count).To<T>().ToListAsync();

            return songs;
        }

        public IEnumerable<T> GetNewestSongs<T>()
        {
            var songs = this.songRepository.All().OrderByDescending(x => x.CreatedOn).To<T>().ToList();
            return songs;
        }

        public async Task<IEnumerable<T>> GetRandomSongsFromGenre<T>(Genre genre)
        {
            var songs = await this.songRepository.All().Where(s => s.Genre == genre).OrderBy(s => s.Genre).Take(3).To<T>().ToListAsync();

            return songs;
        }

        public async Task<IEnumerable<T>> GetSongsByAlbumAsync<T>(int? albumId)
        {
            var songs = await this.songRepository.All().Where(s => s.AlbumId == albumId).To<T>().ToListAsync();
            return songs;
        }

        public string GetMediaDuration(string songName, string username)
        {
            var dotIndex = songName.LastIndexOf('.');
            var fileExtension = songName.Substring(dotIndex);

            string name = songName;

            string webRootPath = this.env.WebRootPath + "\\Songs\\";

            var filePath = Path.Combine(webRootPath, username, name);

            Mp3FileReader reader = new Mp3FileReader(filePath);
            TimeSpan duration = reader.TotalTime;
            /*            double duration = 0.0;
                        using (FileStream fs = File.OpenRead(mediaFilename))
                        {
                            Mp3Frame frame = Mp3Frame.LoadFromStream(fs);
                            if (frame != null)
                            {
                               // _sampleFrequency = (uint)frame.SampleRate;
                            }
                            while (frame != null)
                            {
                                if (frame.ChannelMode == ChannelMode.Mono)
                                {
                                    duration += (double)frame.SampleCount * 2.0 / (double)frame.SampleRate;
                                }
                                else
                                {
                                    duration += (double)frame.SampleCount * 4.0 / (double)frame.SampleRate;
                                }
                                frame = Mp3Frame.LoadFromStream(fs);
                            }
                        }*/
            return duration.ToString(@"hh\:mm\:ss");
        }

        /// <summary>
        /// Gets the top favourited Songs.
        /// </summary>
        /// <typeparam name="T">View model.</typeparam>
        /// <param name="howMuch">How many songs to get.</param>
        /// <returns></returns>
        public async Task<IEnumerable<T>> GetTopFavouritedSongs<T>(int howMuch)
        {
            var songs = await this.songRepository.All().OrderByDescending(s => s.FavouritesCount).Take(howMuch).To<T>().ToListAsync();

            return songs;
        }

        public async Task<IEnumerable<T>> GetTopUsers<T>(int count)
        {
            var users = await this.songRepository.All().OrderByDescending(s => s.FavouritesCount).Take(count).Select(s => s.User).To<T>().ToListAsync();

            return users;
        }

        public async Task<IEnumerable<T>> GetTopSongsByGenre<T>(Genre genre)
        {
            var songs = await this.songRepository.All().Where(s => s.Genre == genre).OrderByDescending(s => s.FavouritesCount).To<T>().ToListAsync();

            return songs;
        }

        public async Task BackgroundLyricsGathering()
        {
            var songs = await this.GetSongsWithoutLyrics();

            foreach (var song in songs)
            {
                int dotIndex = song.Name.LastIndexOf(".");
                song.Name.Remove(dotIndex);
                await this.GetLyricsForSong(song);
            }
        }

        public async Task<IEnumerable<T>> Search<T>(string searchTerm)
        {
            var result = await this.songRepository.AllAsNoTracking().Where(
                s => s.Name.Contains(searchTerm) ||
                s.Album.Name.Contains(searchTerm) ||
                s.Album.User.UserName.Contains(searchTerm) ||
                s.User.UserName.Contains(searchTerm)).To<T>().ToListAsync();

            return result;
        }

        public async Task<IEnumerable<T>> GetSongsByGenre<T>(string genre)
        {
            Genre genre1;
            if (!Enum.TryParse<Genre>(genre, out genre1))
            {
                return null;
            }

            var songs = await this.songRepository.All().Where(s => s.Genre == genre1).OrderByDescending(s => s.CreatedOn).To<T>().ToListAsync();

            return songs;
        }

        public string EmbedYoutube(string url)
        {
            if (url == null)
            {
                return null;
            }

            var videoId = url.Split("v=");
            var ampersandPosition = videoId[1].IndexOf('&');
            if (ampersandPosition != -1)
            {
                videoId[1] = videoId[1].Substring(0, ampersandPosition);
            }

            var embed = $"https://www.youtube.com/embed/{videoId[1]}?controls=1";

            return embed;
        }

        private async Task<bool> GetLyricsForSong(Song song)
        {
            var ovhLyrics = await this.GetOvhLyrics(song.User.UserName, song.Name, song.Id);
            if (ovhLyrics == null || ovhLyrics == string.Empty)
            {
                var apiSeedsLyrics = await this.GetApiSeedLyrics(song.User.UserName, song.Name, this.configuration["ApiSeedsLyrics:AppKey"], song.Id);
                if (apiSeedsLyrics == null || apiSeedsLyrics == string.Empty)
                {
                    var geniusLyrics = await this.GeniusLyricsScraper(song.User.UserName, song.Name, song.Id);
                    if (geniusLyrics == null || geniusLyrics == string.Empty)
                    {
                        return false;
                    }
                }
            }

            return true;
        }

        private async Task<bool> GetLyricsForSong(Song song, string username)
        {
            var ovhLyrics = await this.GetOvhLyrics(username, song.Name, song.Id);
            if (ovhLyrics == null || ovhLyrics == string.Empty)
            {
                var apiSeedsLyrics = await this.GetApiSeedLyrics(username, song.Name, this.configuration["ApiSeedsLyrics:AppKey"], song.Id);
                if (apiSeedsLyrics == null || apiSeedsLyrics == string.Empty)
                {
                    var geniusLyrics = await this.GeniusLyricsScraper(username, song.Name, song.Id);
                    if (geniusLyrics == null || geniusLyrics == string.Empty)
                    {
                        return false;
                    }
                }
            }

            return true;
        }

        private async Task<IEnumerable<Song>> GetSongsWithoutLyrics()
        {
            var lyrics = await this.songRepository.All().Select(s => s.Lyrics.SongId).ToListAsync();

            var missingSongsLyrics = await this.songRepository.All().Where(s => lyrics.All(l => l != s.Id)).Include(s => s.User).ToListAsync();
            ;
            return missingSongsLyrics;
        }

        private async Task<string> GetApiSeedLyrics(string artist, string song, string appKey, int songId)
        {
            // Add featuring
            var apiSeedsClient = new HttpClient();

            var dotIndex = song.LastIndexOf(".");
            string baseUrl = "https://orion.apiseeds.com/api/music/lyric/";
            string artistName = artist;
            string songName = $"/{song.Remove(dotIndex)}";
            string apiKey = appKey;
            string search = baseUrl + artistName + songName + apiKey;

            var jsonResultString = await apiSeedsClient.GetAsync(search);
            string lyrics = string.Empty;

            if (!jsonResultString.IsSuccessStatusCode)
            {
                // throw new ArgumentException("Song lyrics can't be found right now");
                return null;
            }
            else
            {
                var jsonStr = await jsonResultString.Content.ReadAsStringAsync();

                var json = JsonDocument.Parse(jsonStr);

                lyrics = json.RootElement.GetProperty("result").GetProperty("track").GetProperty("text").ToString();

                string modifiedLyrics = Regex.Replace(lyrics, @"(\r\n)|\n|\r", "<br>");

                var lyricsEntity = new Lyrics
                {
                    Text = modifiedLyrics,
                    SongId = songId,
                };
                await this.lyricsRepository.AddAsync(lyricsEntity);
                await this.lyricsRepository.SaveChangesAsync();
            }

            return lyrics;
        }

        private async Task<string> GetOvhLyrics(string artist, string song, int songId)
        {
            // Add featuring
            var ovhClient = new HttpClient();
            string lyrics = string.Empty;

            var dotIndex = song.LastIndexOf(".");
            var artistName = artist;
            var songName = song.Remove(dotIndex);

            var response = await ovhClient.GetAsync("https://api.lyrics.ovh/v1/" + artistName + "/" + songName);

            if (!response.IsSuccessStatusCode)
            {
                // throw new ArgumentException("Song lyrics can't be found right now");
                return null;
            }
            else
            {
                var jsonString = await response.Content.ReadAsStringAsync();

                var json = JsonDocument.Parse(jsonString);

                lyrics = json.RootElement.GetProperty("lyrics").ToString();

                if (lyrics.Length > 5)
                {
                    string modifiedLyrics = Regex.Replace(lyrics, @"(\r\n)|\n|\r", "<br>");

                    var lyricsEntity = new Lyrics
                    {
                        Text = modifiedLyrics,
                        SongId = songId,
                    };
                    await this.lyricsRepository.AddAsync(lyricsEntity);
                    await this.lyricsRepository.SaveChangesAsync();
                }
            }

            return lyrics;
        }

        private async Task<string> GeniusLyricsScraper(string artist, string song, int songId)
        {
            var geniusClient = new HttpClient();
            var lyrics = string.Empty;

            var baseUrl = "https://genius.com";
            var artistName = artist;
            var songName = song;

            var query = $"/{artistName}-{songName}-lyrics";
            var search = Regex.Replace(query, "( )+", "-");

            var content = await geniusClient.GetAsync(baseUrl + search);

            if (!content.IsSuccessStatusCode)
            {
                throw new ArgumentException("Song lyrics can't be found right now");
            }
            else
            {
                var htmlDoc = new HtmlDocument();
                var body = await content.Content.ReadAsStringAsync();
                htmlDoc.LoadHtml(body);

                lyrics = htmlDoc.DocumentNode.SelectSingleNode("//*[@class = 'lyrics']/p").InnerText.Trim();
                string modifiedLyrics = Regex.Replace(lyrics, @"(\r\n)|\n|\r", "<br/>");

                var lyricsEntity = new Lyrics
                {
                    Text = modifiedLyrics,
                    SongId = songId,
                };
                await this.lyricsRepository.AddAsync(lyricsEntity);
                await this.lyricsRepository.SaveChangesAsync();
            }

            return lyrics;
        }
    }
}
