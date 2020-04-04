namespace Audiology.Services.Data.Playlists
{
    using System;
    using System.Collections.Generic;
    using System.Linq;
    using System.Threading.Tasks;

    using Audiology.Data.Common.Repositories;
    using Audiology.Data.Models;
    using Audiology.Services.Mapping;
    using Audiology.Web.ViewModels.Playlists;
    using Microsoft.EntityFrameworkCore;

    public class PlaylistsService : IPlaylistsService
    {
        private readonly IDeletableEntityRepository<Playlist> playlistRepository;
        private readonly IDeletableEntityRepository<PlaylistsSongs> playlistsSongsRepository;

        public PlaylistsService(
            IDeletableEntityRepository<Playlist> playlistRepository,
            IDeletableEntityRepository<PlaylistsSongs> playlistsSongsRepository)
        {
            this.playlistRepository = playlistRepository;
            this.playlistsSongsRepository = playlistsSongsRepository;
        }

        public async Task AddAsync(string userId, int playlistId, int songId)
        {
            var song = await this.playlistsSongsRepository.All().Where(ps => ps.SongId == songId && ps.PlaylistId == playlistId).FirstOrDefaultAsync();
            var isPlaylistToUser = await this.playlistRepository.All().Where(p => p.Id == playlistId && p.UserId == userId).FirstOrDefaultAsync();

            if (isPlaylistToUser != null)
            {
                if (song == null)
                {
                    var playlistSong = new PlaylistsSongs
                    {
                        PlaylistId = playlistId,
                        SongId = songId,
                    };

                    await this.playlistsSongsRepository.AddAsync(playlistSong);
                }
            }

            await this.playlistsSongsRepository.SaveChangesAsync();
        }

        public async Task CreateAsync(string name, string userId, int songId, bool isPrivate)
        {
            var playlist = await this.playlistRepository.All().Where(p => p.Name == name && p.UserId == userId).FirstOrDefaultAsync();
            if (playlist == null)
            {
                if (name.Length >= 30 || name == null)
                {
                    throw new ArgumentOutOfRangeException("Playlist name shoud be less than 30 characters.");
                }

                var newPlaylist = new Playlist
                {
                    Name = name,
                    UserId = userId,
                    IsPrivate = isPrivate,
                };
                await this.playlistRepository.AddAsync(newPlaylist);
                await this.playlistRepository.SaveChangesAsync();

                var playlistSong = new PlaylistsSongs
                {
                    PlaylistId = newPlaylist.Id,
                    SongId = songId,
                };
                await this.playlistsSongsRepository.AddAsync(playlistSong);
                await this.playlistsSongsRepository.SaveChangesAsync();
            }
            else
            {
                throw new Exception("Playlist name alredy exists.");
            }
        }

        public async Task<IEnumerable<T>> GetAllPlaylistsAsync<T>(string userId)
        {
            var playlist = await this.playlistRepository.All().Where(p => p.UserId == userId).To<T>().ToListAsync();
            return playlist;
        }

        public async Task<IEnumerable<T>> GetAllSongsInPlaylistAsync<T>(string userId, int playlistId)
        {
            var playlist = await this.playlistRepository.All().Where(p => p.UserId == userId && p.Id == playlistId).To<PlaylistViewModel>().FirstOrDefaultAsync();

            var playlistSongs = await this.playlistsSongsRepository.All().Where(ps => ps.PlaylistId == playlistId).Select(x => x.Song).To<T>().ToListAsync();

            return playlistSongs;
        }
    }
}
