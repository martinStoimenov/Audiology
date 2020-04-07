namespace Audiology.Services.Data.Songs
{
    using System;
    using System.Collections.Generic;
    using System.IO;
    using System.Linq;
    using System.Net.Http;
    using System.Text.Json;
    using System.Threading.Tasks;

    using Audiology.Data;
    using Audiology.Data.Common.Repositories;
    using Audiology.Data.Models;
    using Audiology.Data.Models.Enumerations;
    using Audiology.Services.Mapping;
    using Audiology.Web.ViewModels.Songs;
    using Microsoft.AspNetCore.Hosting;
    using Microsoft.AspNetCore.Http;
    using Microsoft.AspNetCore.Identity;
    using Microsoft.EntityFrameworkCore;
    using NAudio.Wave;

    public class SongsService : ISongsServcie
    {
        private readonly IHostingEnvironment env;
        private readonly IRepository<Song> songRepository;
        private readonly IDeletableEntityRepository<PlaylistsSongs> playlistsSongsRepo;
        private readonly IDeletableEntityRepository<Favourites> favouritesRepo;
        private readonly IRepository<ApplicationUser> userRepo;

        public SongsService(
            IHostingEnvironment env,
            IRepository<Song> songRepository,
            IDeletableEntityRepository<PlaylistsSongs> playlistsSongsRepo,
            IDeletableEntityRepository<Favourites> favouritesRepo,
            IRepository<ApplicationUser> userRepo)
        {
            this.env = env;
            this.songRepository = songRepository;
            this.playlistsSongsRepo = playlistsSongsRepo;
            this.favouritesRepo = favouritesRepo;
            this.userRepo = userRepo;
        }

        public async Task DeleteSong(int songId)
        {
            var song = await this.songRepository.All().Where(s => s.Id == songId).FirstOrDefaultAsync();
            var playlistSong = await this.playlistsSongsRepo.All().Where(ps => ps.SongId == song.Id).FirstOrDefaultAsync();
            var favouritedSong = await this.favouritesRepo.All().Where(f => f.SongId == songId).FirstOrDefaultAsync();
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

                this.songRepository.Delete(song);
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

        public async Task<int> UploadAsync(IFormFile input, string username, string songName, string description, string producer, int? albumId, Enum genre, int year, string userId, string songArt, string featuring, string writtenBy, string youtubeUrl, string soundcloudUrl, string instagramPostUrl)
        {
            // TODO: Add hangfire task scheduler for api calls
            var dotIndex = input.FileName.LastIndexOf('.');
            var fileExtension = input.FileName.Substring(dotIndex);
            var originalFileName = input.FileName.Substring(0, dotIndex);

            string webRootPath = this.env.WebRootPath + "\\Songs\\";

            if (!Directory.Exists(webRootPath + username))
            {
                Directory.CreateDirectory(webRootPath + username);
            }

            if (songName.Length > 50 || songName == null)
            {
                throw new ArgumentOutOfRangeException("Song name cannot be more than 50 characters!");
            }

            if (description.Length > 100)
            {
                throw new ArgumentOutOfRangeException("Description cannot be more than 100 characters!");
            }

            if (year > 2020)
            {
                throw new ArgumentOutOfRangeException("Year cannot be more than the current one!");
            }

            if (featuring.Length > 100)
            {
                throw new ArgumentOutOfRangeException("Featuring cannot be more than 100 characters!");
            }

            if (writtenBy.Length > 100)
            {
                throw new ArgumentOutOfRangeException("WrittenBy cannot be more than 100 characters!");
            }

            if (youtubeUrl.Length > 500)
            {
                throw new ArgumentOutOfRangeException("You Tube url cannot be more than 500 characters!");
            }

            if (soundcloudUrl == null || soundcloudUrl.Length > 500) // refactor these validations
            {
                throw new ArgumentOutOfRangeException("Soundcloud url cannot be more than 500 characters!");
            }

            if (instagramPostUrl == null || instagramPostUrl.Length > 500) // refactor these validations
            {
                throw new ArgumentOutOfRangeException("Instagram url cannot be more than 500 characters!");
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

            var lyrics = await this.GetApiSeedLyrics(username, name, "?apikey=qnvwwzXwsPeyGI7KUILgQSTjlzoBywKYIp1l7KPe0al9jiwYT4qms0UzDJozxM2i");

            using (StreamWriter stream = File.CreateText(@"C:\Users\haloho\Desktop\lyrics.txt"))
            {
                stream.WriteLine("Created on: ");
                stream.WriteLine(DateTime.Now);
                stream.Write(lyrics);
                stream.WriteLine();
                stream.WriteLine();
                stream.WriteLine();
            }

            return song.Id;
        }

        public async Task<int> EditSongAsync(int id, string username, string name, string description, int? albumId, string producer, string songArtUrl, Enum genre, int year, string featuring, string writtenBy, string youtubeUrl, string soundcloudUrl, string instagramPostUrl)
        {
            var song = this.songRepository.All().Where(s => s.Id == id).FirstOrDefault();

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

                if (featuring.Length <= 100 && featuring != null)
                {
                    song.Featuring = featuring;
                }

                if (writtenBy.Length <= 100 && writtenBy != null)
                {
                    song.WrittenBy = writtenBy;
                }

                if (youtubeUrl.Length <= 500 && youtubeUrl != null)
                {
                    song.YoutubeUrl = youtubeUrl;
                }

                if (soundcloudUrl.Length <= 500 && soundcloudUrl != null)
                {
                    song.SoundcloudUrl = soundcloudUrl;
                }

                if (instagramPostUrl.Length <= 500 && instagramPostUrl != null)
                {
                    song.InstagramPostUrl = instagramPostUrl;
                }
            }

            string newName = name + fileExtension;

            string webRootPath = this.env.WebRootPath + "\\Songs\\";

            var newPath = Path.Combine(webRootPath, username, newName);

            string oldPath = webRootPath + username + "\\" + songName + fileExtension;

            var file = new FileInfo(oldPath);

            file.MoveTo(newPath);

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

        public IEnumerable<T> GetAllSongsForUserAsync<T>(string userId)
        {
            var songs = this.songRepository.All().Where(s => s.UserId == userId).To<T>().ToList();

            return songs;
        }

        public IEnumerable<T> GetNewestSongs<T>()
        {
            var songs = this.songRepository.All().OrderBy(x => x.CreatedOn).To<T>().ToList();
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

        public async Task<string> GetApiSeedLyrics(string artist, string song, string appKey)
        {
            var apiSeedsClient = new HttpClient();

            string baseUrl = "https://orion.apiseeds.com/api/music/lyric/";
            string artistName = artist;
            string songName = $"/{song}";
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
            }

            return lyrics;
        }

        /// <summary>
        /// Gets the top favourited Songs.
        /// </summary>
        /// <typeparam name="T">View model</typeparam>
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
    }
}
