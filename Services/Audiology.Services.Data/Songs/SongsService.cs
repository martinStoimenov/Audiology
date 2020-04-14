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
        private static readonly char slash = Path.DirectorySeparatorChar;

        private readonly IHostingEnvironment env;
        private readonly IRepository<Song> songRepository;
        private readonly IDeletableEntityRepository<PlaylistsSongs> playlistsSongsRepo;
        private readonly IDeletableEntityRepository<Favourites> favouritesRepo;
        private readonly IRepository<ApplicationUser> userRepo;
        private readonly IDeletableEntityRepository<Lyrics> lyricsRepository;
        private readonly IConfiguration configuration;

        public SongsService(
            IHostingEnvironment env,
            IRepository<Song> songRepository,
            IDeletableEntityRepository<PlaylistsSongs> playlistsSongsRepo,
            IDeletableEntityRepository<Favourites> favouritesRepo,
            IRepository<ApplicationUser> userRepo,
            IDeletableEntityRepository<Lyrics> lyricsRepository,
            IConfiguration configuration)
        {
            this.env = env;
            this.songRepository = songRepository;
            this.playlistsSongsRepo = playlistsSongsRepo;
            this.favouritesRepo = favouritesRepo;
            this.userRepo = userRepo;
            this.lyricsRepository = lyricsRepository;
            this.configuration = configuration;
        }

        // once in a week if they are smaller than 15 songs and gets the lyrics from many apis.

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

                if (File.Exists(fullPath))
                {
                    File.Delete(fullPath);
                }

                await this.songRepository.SaveChangesAsync();
            }
        }

        public async Task<int> UploadAsync(IFormFile input, string username, string songName, string description, string producer, int? albumId, Enum genre, int year, string userId, string songArt, string featuring, string writtenBy, string youtubeUrl, string soundcloudUrl, string instagramPostUrl)
        {
            var dotIndex = input.FileName.LastIndexOf('.');
            var fileExtension = input.FileName.Substring(dotIndex);

            string webRootPath = this.env.WebRootPath + slash + "Songs" + slash;

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

            song.User.UserName = username;
            bool isFound = await this.GetLyricsForSong(song);

            await this.songRepository.SaveChangesAsync();

            return song.Id;
        }

        public async Task<int> EditSongAsync(int id, string username, string name, string description, int? albumId, string producer, string songArtUrl, Enum genre, int year, string featuring, string writtenBy, string youtubeUrl, string soundcloudUrl, string instagramPostUrl)
        {
            var song = this.songRepository.All().Where(s => s.Id == id).Include(s => s.User).FirstOrDefault();

            int dotIndex = song.Name.LastIndexOf(".");
            if (dotIndex == -1)
            {
                throw new IndexOutOfRangeException("file extension is missing");
            }

            string songName = song.Name.Substring(0, dotIndex);
            string fileExtension = song.Name.Substring(dotIndex);

            if (song != null)
            {
                if (producer.Length <= 100 && producer != null)
                {
                    song.Producer = producer;
                }

                if (name.Length <= 50 && name != null)
                {
                    song.Name = name + fileExtension;
                }

                if (description.Length <= 100 && description != null)
                {
                    song.Description = description;
                }

                if (year > 1 || year < 2020)
                {
                    song.Year = year;
                }

                if (songArtUrl.Length <= 500 && songArtUrl != null)
                {
                    song.SongArtUrl = songArtUrl;
                }

                if (albumId != null)
                {
                    song.AlbumId = albumId;  // album can be null add check if selected one is valid
                }

                if (Enum.IsDefined(typeof(Genre), genre))
                {
                    song.Genre = (Genre)genre;
                }

                if (featuring != null)
                {
                    song.Featuring = featuring;
                }

                if (writtenBy != null)
                {
                    song.WrittenBy = writtenBy;
                }

                if (youtubeUrl != null)
                {
                    song.YoutubeUrl = youtubeUrl;
                }

                if (soundcloudUrl != null)
                {
                    song.SoundcloudUrl = soundcloudUrl;
                }

                if (instagramPostUrl != null)
                {
                    song.InstagramPostUrl = instagramPostUrl;
                }
            }

            string newName = name + fileExtension;

            string webRootPath = this.env.WebRootPath + slash + "Songs" + slash;

            var newPath = Path.Combine(webRootPath, username, newName);

            string oldPath = webRootPath + username + slash + songName + fileExtension;

            var file = new FileInfo(oldPath);

            file.MoveTo(newPath);

            bool isFound = await this.GetLyricsForSong(song);

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

        private async Task<bool> GetLyricsForSong(Song song)
        {
            var ovhLyrics = await this.GetOvhLyrics(song.User.UserName, song.Name, song.Id);
            if (ovhLyrics == null || ovhLyrics == string.Empty)
            {
                var apiSeedsLyrics = await this.GetApiSeedLyrics(song.User.UserName, song.Name, this.configuration["ApiSeedsLyrics:AppKey"], song.Id);
                if (apiSeedsLyrics == null || apiSeedsLyrics == string.Empty)
                {
                    var geniusLyrics = await this.GeniusLyricsCrawler(song.User.UserName, song.Name, song.Id);
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

        private async Task<string> GeniusLyricsCrawler(string artist, string song, int songId)
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

    /* public class SongsWithoutLyricsModel : IMapFrom<Song>
     {
         public int Id { get; set; }

         public string Name { get; set; }

         public string Featuring { get; set; }

         public string UserUserName { get; set; }
     }*/
}
