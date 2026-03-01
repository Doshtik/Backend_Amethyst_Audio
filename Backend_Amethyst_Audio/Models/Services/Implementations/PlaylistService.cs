using Backend_Amethyst_Audio.Models.Data;
using Backend_Amethyst_Audio.Models.Entities;
using Backend_Amethyst_Audio.Services.Abstractions;

namespace Backend_Amethyst_Audio.Services.Implementations;

public class PlaylistService(AppDbContext db) : IPlaylistService
{
    public Task<Playlist> GetByIdAsync(long id)
    {
        throw new NotImplementedException();
    }

    public Task<long> GetLikedPlaylistId(long userId)
    {
        long playlistId = db.Playlists
            .Where(x => x.Name == "Liked" && x.IdUser == userId)
            .Select(x => x.Id)
            .First();
        return Task.FromResult(playlistId);
    }

    public Task CreateAsync(Album album)
    {
        throw new NotImplementedException();
    }

    public Task UpdateAsync(Album album)
    {
        throw new NotImplementedException();
    }

    public Task<List<Playlist>> GetListByUserIdAsync(long userId)
    {
        throw new NotImplementedException();
    }

    public Task<List<Playlist>> GetListBySearchAsync(string search)
    {
        throw new NotImplementedException();
    }

    public Task<List<Playlist>> GetListSavedAsync(long userId)
    {
        throw new NotImplementedException();
    }

    public Task SavePlaylistAsync(long idUser, long idPlaylist)
    {
        throw new NotImplementedException();
    }

    public Task UnsavePlaylistAsync(long idUser, long idPlaylist)
    {
        throw new NotImplementedException();
    }
}