using Backend_Amethyst_Audio.Models.Entities;

namespace Backend_Amethyst_Audio.Services.Abstractions;

public interface IPlaylistService
{
    Task<Playlist> GetByIdAsync(long id);
    Task<long> GetLikedPlaylistId(long userId);
    
    Task CreateAsync(Album album);
    Task UpdateAsync(Album album);
    
    Task<List<Playlist>> GetListByUserIdAsync(long userId);
    Task<List<Playlist>> GetListBySearchAsync(string search);
    Task<List<Playlist>> GetListSavedAsync(long userId);
    
    Task SavePlaylistAsync(long idUser, long idPlaylist);
    Task UnsavePlaylistAsync(long idUser, long idPlaylist);
}