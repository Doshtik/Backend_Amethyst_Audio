using Backend_Amethyst_Audio.DTO;
using Backend_Amethyst_Audio.Models.Entities;

namespace Backend_Amethyst_Audio.Services.Abstractions;

public interface IPlaylistService
{
    Task<PlaylistInfoDto> GetByIdAsync(long id, string requestedBy = null);
    Task<List<PlaylistInfoDto>> GetAllAsync();
    Task<PlaylistInfoDto> CreateAsync(CreatePlaylistDto dto, long ownerId);
    Task<PlaylistInfoDto> UpdateAsync(long id, ChangePlaylistInfoDto dto, string requestedBy);
    Task DeleteAsync(long id, string requestedBy);
    
    Task<List<PlaylistInfoDto>> GetListByUserIdAsync(long userId);
    Task<List<PlaylistInfoDto>> GetListByPlaylistNameAsync(long excludeUserId, string query, int limit = 50);
    Task<List<PlaylistInfoDto>> GetListSavedAsync(long userId);
    
    
    // Playlists
    Task AddInPlaylistAsync(long trackId, long playlistId);
    Task RemoveFromPlaylistAsync(long trackId, long playlistId);
    
    Task<bool> IsPlaylistSavedAsync(long userId, long playlistId);
    Task SavePlaylistAsync(long userId, long playlistId);
    Task UnsavePlaylistAsync(long userId, long playlistId);
}