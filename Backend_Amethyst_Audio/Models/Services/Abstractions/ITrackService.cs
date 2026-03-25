using Backend_Amethyst_Audio.DTO;
using Backend_Amethyst_Audio.DTO.Pages;
using Backend_Amethyst_Audio.Models;
using Microsoft.AspNetCore.Mvc;

namespace Backend_Amethyst_Audio.Services.Abstractions;

public interface ITrackService
{
    Task<TrackInfoDto> GetByIdAsync(long id);
    Task<List<TrackInfoDto>> GetAllAsync();
    
    Task<TrackInfoDto> CreateAsync(CreateTrackDto dto);
    Task<TrackInfoDto> UpdateAsync(ChangeTrackInfoDto dto);
    Task DeleteAsync(long id);
    
    Task<List<TrackInfoDto>> GetListByGenreAsync(string genre);
    Task<List<TrackInfoDto>> GetListByTrackNameAsync(string trackName);
    Task<List<TrackInfoDto>> GetListByUserIdAsync(long userId);
    
    Task<List<TrackInfoDto>> GetListOfNewestAsync();
    Task<List<TrackInfoDto>> GetListOfLikedAsync(long likedPlaylistId);
    Task<List<TrackInfoDto>> GetListOfPersonalizedAsync(PageMyRecordPersonalizedDto dto);
    Task<PageMyRecordDto> PageMyRecordAsync();
    
    Task<bool> IsLikedAsync(long idUser,  long idTrack);
    
    Task AddInPlaylistAsync(long idUser, long idPlaylist);
    Task RemoveFromPlaylistAsync(long idUser, long idPlaylist);
}