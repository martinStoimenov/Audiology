namespace Audiology.Services.Data.Tests
{
    using System;
    using System.Linq;
    using System.Reflection;
    using System.Threading.Tasks;

    using Audiology.Data;
    using Audiology.Data.Models;
    using Audiology.Data.Repositories;
    using Audiology.Services.Data.Playlists;
    using Audiology.Services.Mapping;
    using Audiology.Web.ViewModels.Playlists;
    using Audiology.Web.ViewModels.Songs;
    using Microsoft.EntityFrameworkCore;
    using Xunit;

    public class PlaylistsTests
    {
        public PlaylistsTests()
        {
            AutoMapperConfig.RegisterMappings(
                typeof(PlaylistsService).GetTypeInfo().Assembly,
                typeof(SongListViewModel).GetTypeInfo().Assembly,
                typeof(PlaylistViewModel).GetTypeInfo().Assembly);
        }

        [Fact]
        public async Task AddAsyncShouldAddCorrectly()
        {
            var options = new DbContextOptionsBuilder<ApplicationDbContext>().UseInMemoryDatabase(Guid.NewGuid().ToString());
            var playlistRepository = new EfDeletableEntityRepository<Playlist>(new ApplicationDbContext(options.Options));
            var playlistSongRepository = new EfDeletableEntityRepository<PlaylistsSongs>(new ApplicationDbContext(options.Options));
            var service = new PlaylistsService(playlistRepository, playlistSongRepository);

            var playlist = new Playlist
            {
                Name = "test playlist",
                UserId = "user",
            };
            await playlistRepository.AddAsync(playlist);
            await playlistRepository.SaveChangesAsync();

            var playlistsong = new PlaylistsSongs
            {
                PlaylistId = playlist.Id,
                SongId = 1,
            };
            await playlistSongRepository.AddAsync(playlistsong);
            await playlistSongRepository.SaveChangesAsync();

            await service.AddAsync(playlist.UserId, playlist.Id, 1);
            var song = await playlistSongRepository.All().Where(s => s.SongId == 1 && s.PlaylistId == playlist.Id).FirstOrDefaultAsync();

            Assert.NotNull(song);
        }

        [Fact]
        public async Task GetAllPlaylistsShouldWorkCorrectly()
        {
            var options = new DbContextOptionsBuilder<ApplicationDbContext>().UseInMemoryDatabase(Guid.NewGuid().ToString());
            var playlistRepository = new EfDeletableEntityRepository<Playlist>(new ApplicationDbContext(options.Options));
            var playlistSongRepository = new EfDeletableEntityRepository<PlaylistsSongs>(new ApplicationDbContext(options.Options));
            var service = new PlaylistsService(playlistRepository, playlistSongRepository);

            var playlist = new Playlist
            {
                Name = "test playlist",
                UserId = "user",
            };

            var playlist2 = new Playlist
            {
                Name = "test playlist2",
                UserId = "user",
            };

            var playlist3 = new Playlist
            {
                Name = "test playlist3",
                UserId = "user",
            };
            await playlistRepository.AddAsync(playlist);
            await playlistRepository.AddAsync(playlist2);
            await playlistRepository.AddAsync(playlist3);
            await playlistRepository.SaveChangesAsync();

            var playlists = await service.GetAllPlaylistsAsync<PlaylistViewModel>("user");

            Assert.Equal(3, playlists.Count());
        }

        [Fact]
        public async Task CreatePlaylistsShouldWorkCorrectly()
        {
            var options = new DbContextOptionsBuilder<ApplicationDbContext>().UseInMemoryDatabase(Guid.NewGuid().ToString());
            var playlistRepository = new EfDeletableEntityRepository<Playlist>(new ApplicationDbContext(options.Options));
            var playlistSongRepository = new EfDeletableEntityRepository<PlaylistsSongs>(new ApplicationDbContext(options.Options));
            var service = new PlaylistsService(playlistRepository, playlistSongRepository);

            await service.CreateAsync("name", "userId", 1, false);

            var playlist = await playlistRepository.All().Where(s => s.UserId == "userId").FirstOrDefaultAsync();

            Assert.NotNull(playlist);
        }

        [Fact]
        public async Task GetPlaylistArtShouldWorkCorrectly()
        {
            var options = new DbContextOptionsBuilder<ApplicationDbContext>().UseInMemoryDatabase(Guid.NewGuid().ToString());
            var playlistRepository = new EfDeletableEntityRepository<Playlist>(new ApplicationDbContext(options.Options));
            var playlistSongRepository = new EfDeletableEntityRepository<PlaylistsSongs>(new ApplicationDbContext(options.Options));
            var songRepository = new EfRepository<Song>(new ApplicationDbContext(options.Options));
            var service = new PlaylistsService(playlistRepository, playlistSongRepository);

            var playlist = new Playlist
            {
                Name = "test playlist",
                UserId = "user",
            };
            await playlistRepository.AddAsync(playlist);
            await playlistRepository.SaveChangesAsync();

            var song = new Song
            {
                UserId = "UserId",
                FavouritesCount = 0,
                Name = "Smile",
                Description = "asdsaasd",
                Year = 1999,
                SongArtUrl = "asd.jpeg",
            };
            await songRepository.AddAsync(song);
            await songRepository.SaveChangesAsync();

            var playlistsong = new PlaylistsSongs
            {
                PlaylistId = playlist.Id,
                SongId = song.Id,
            };
            await playlistSongRepository.AddAsync(playlistsong);
            await playlistSongRepository.SaveChangesAsync();

            string art = await service.GetPlaylistArt(playlist.Id);

            Assert.Equal("asd.jpeg", art);
        }

        [Fact]
        public async Task GetAllSongsInPlaylistShouldWorkCorrectly()
        {
            var options = new DbContextOptionsBuilder<ApplicationDbContext>().UseInMemoryDatabase(Guid.NewGuid().ToString());
            var playlistRepository = new EfDeletableEntityRepository<Playlist>(new ApplicationDbContext(options.Options));
            var playlistSongRepository = new EfDeletableEntityRepository<PlaylistsSongs>(new ApplicationDbContext(options.Options));
            var songRepository = new EfRepository<Song>(new ApplicationDbContext(options.Options));
            var service = new PlaylistsService(playlistRepository, playlistSongRepository);

            var playlist = new Playlist
            {
                Name = "test playlist",
                UserId = "user",
            };
            await playlistRepository.AddAsync(playlist);
            await playlistRepository.SaveChangesAsync();

            var song = new Song
            {
                UserId = "UserId",
                FavouritesCount = 0,
                Name = "Smile",
                Description = "asdsaasd",
                Year = 1999,
                SongArtUrl = "asd.jpeg",
            };
            await songRepository.AddAsync(song);
            await songRepository.SaveChangesAsync();

            var song2 = new Song
            {
                UserId = "UserId",
                FavouritesCount = 0,
                Name = "Hard",
                Description = "asdsaasd",
                Year = 1999,
                SongArtUrl = "asd.jpeg",
            };
            await songRepository.AddAsync(song2);
            await songRepository.SaveChangesAsync();

            var playlistsong = new PlaylistsSongs
            {
                PlaylistId = playlist.Id,
                SongId = song.Id,
            };
            await playlistSongRepository.AddAsync(playlistsong);
            await playlistSongRepository.SaveChangesAsync();

            var playlistsong2 = new PlaylistsSongs
            {
                PlaylistId = playlist.Id,
                SongId = song2.Id,
            };
            await playlistSongRepository.AddAsync(playlistsong2);
            await playlistSongRepository.SaveChangesAsync();

            var songsForPlaylist = await playlistSongRepository.All().Where(s => s.PlaylistId == playlist.Id).ToListAsync();
            var playlistWithSongs = await playlistRepository.All().Where(s => s.UserId == playlist.UserId && s.Id == playlist.Id).Select(x => x.PlaylistsSongs.Select(x=>x.Song)).FirstOrDefaultAsync();
            var songsInPlaylist = await service.GetAllSongsInPlaylistAsync<SongListViewModel>(playlist.UserId, playlist.Id);
            ;
            Assert.Equal(2, songsInPlaylist.Count());
        }

        [Fact]
        public async Task RemoveAsyncShouldRemoveCorrectly()
        {
            var options = new DbContextOptionsBuilder<ApplicationDbContext>().UseInMemoryDatabase(Guid.NewGuid().ToString());
            var playlistRepository = new EfDeletableEntityRepository<Playlist>(new ApplicationDbContext(options.Options));
            var playlistSongRepository = new EfDeletableEntityRepository<PlaylistsSongs>(new ApplicationDbContext(options.Options));
            var songRepository = new EfRepository<Song>(new ApplicationDbContext(options.Options));
            var service = new PlaylistsService(playlistRepository, playlistSongRepository);

            var playlist = new Playlist
            {
                Name = "test playlist",
                UserId = "user",
            };
            await playlistRepository.AddAsync(playlist);
            await playlistRepository.SaveChangesAsync();

            var song = new Song
            {
                UserId = "UserId",
                FavouritesCount = 0,
                Name = "Smile",
                Description = "asdsaasd",
                Year = 1999,
                SongArtUrl = "asd.jpeg",
            };
            await songRepository.AddAsync(song);
            await songRepository.SaveChangesAsync();

            var song2 = new Song
            {
                UserId = "UserId",
                FavouritesCount = 0,
                Name = "Hard",
                Description = "asdsaasd",
                Year = 1999,
                SongArtUrl = "asd.jpeg",
            };
            await songRepository.AddAsync(song2);
            await songRepository.SaveChangesAsync();

            var playlistsong = new PlaylistsSongs
            {
                PlaylistId = playlist.Id,
                SongId = song.Id,
            };
            await playlistSongRepository.AddAsync(playlistsong);
            await playlistSongRepository.SaveChangesAsync();

            var playlistsong2 = new PlaylistsSongs
            {
                PlaylistId = playlist.Id,
                SongId = song2.Id,
            };
            await playlistSongRepository.AddAsync(playlistsong2);
            await playlistSongRepository.SaveChangesAsync();

            await service.RemoveAsync(playlist.UserId, playlist.Id, song2.Id);
            var colllectionPlaylist = await playlistSongRepository.All().Where(s => s.PlaylistId == playlist.Id).ToListAsync();

            Assert.Single(colllectionPlaylist);
        }
    }
}
