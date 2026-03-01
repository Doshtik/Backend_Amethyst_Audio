using Backend_Amethyst_Audio.DTO;
using Backend_Amethyst_Audio.DTO.Pages;
using Backend_Amethyst_Audio.Models;
using Microsoft.AspNetCore.Mvc;

namespace Backend_Amethyst_Audio.Services.Abstractions;

public interface ITrackService
{
    Task<TrackInfoDto> GetByIdAsync(long id);
    Task<List<TrackInfoDto>> GetAllAsync();
    
    Task CreateAsync(CreateTrackDto dto);
    Task UpdateAsync(ChangeTrackInfoDto dto);
    Task DeleteAsync(long id);
    
    Task<List<TrackInfoDto>> GetListByGenreAsync(string genre);
    Task<List<TrackInfoDto>> GetListBySearchAsync(string search);
    Task<List<TrackInfoDto>> GetListByUserIdAsync(long userId);
    
    Task<List<TrackInfoDto>> GetListOfNewestAsync();
    Task<List<TrackInfoDto>> GetListOfLikedAsync(long likedPlaylistId);
    Task<List<TrackInfoDto>> GetListOfPersonalizedAsync(PageMyRecordPersonalizedDto dto);
    Task<PageMyRecordDto> PageMyRecordAsync();
    
    Task<bool> IsLikedAsync(long idUser);
    
    Task AddInPlaylistAsync(long idUser, long idPlaylist);
    Task RemoveFromPlaylistAsync(long idUser, long idPlaylist);
    IActionResult GetImageByName(string name);
}