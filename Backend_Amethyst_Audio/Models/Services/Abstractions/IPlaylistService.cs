using Backend_Amethyst_Audio.DTO;
using Backend_Amethyst_Audio.Models.Entities;

namespace Backend_Amethyst_Audio.Services.Abstractions;

public interface IPlaylistService
{
    Task<PlaylistInfoDto> GetByIdAsync(long id);
    Task<long> GetLikedPlaylistId(long userId);
    
    Task<PlaylistInfoDto> CreateAsync(CreatePlaylistDto dto);
    Task UpdateAsync(ChagnePlaylistInfoDto dto);
    
    Task<List<PlaylistInfoDto>> GetListByUserIdAsync(long userId);
    Task<List<PlaylistInfoDto>> GetListBySearchAsync(string search);
    Task<List<PlaylistInfoDto>> GetListSavedAsync(long userId);
    
    Task SavePlaylistAsync(long idUser, long idPlaylist);
    Task UnsavePlaylistAsync(long idUser, long idPlaylist);
}