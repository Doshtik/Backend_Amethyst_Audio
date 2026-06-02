using Backend_Amethyst_Audio.DTO;
using Backend_Amethyst_Audio.DTO.Pages;
using Backend_Amethyst_Audio.Models;
using Microsoft.AspNetCore.Mvc;

namespace Backend_Amethyst_Audio.Services.Abstractions;

public interface ITrackService
{
    // Track CRUD
    Task<TrackInfoDto> GetByIdAsync(long id);
    Task<List<TrackInfoDto>> GetAllAsync();
    Task<TrackInfoDto> CreateAsync(CreateTrackDto dto, long userId);
    Task<TrackInfoDto> UpdateAsync(ChangeTrackInfoDto dto);
    Task DeleteAsync(long id);

    /*
    Task GetUserHistory();
    Task CreateUserHistory();
    Task UpdateUserHistory();
     */
    
    // Search & Filters
    Task<List<GenreInfoDto>> GetListGenresAsync();
    Task<List<TrackInfoDto>> GetListByGenreAsync(string genre);
    Task<List<TrackInfoDto>> GetListByTrackNameAsync(string trackName);
    Task<List<TrackInfoDto>> GetUserLibraryAsync(long userId);
    Task AddTrackToLibraryAsync(long trackId, long userId);
    Task RemoveTrackToLibraryAsync(long trackId, long userId);
    Task<List<TrackInfoDto>> GetListTrackByUserIdAsync(long userId);
    Task<List<TrackInfoDto>> GetListOfNewestAsync(int limit = 50);
    
    // Recommendations
    Task<ResonanceConfigDto> GetRecommendationConfigAsync();
    Task<List<TrackInfoDto>> GetPersonalizedRecommendationsAsync(
        PageResonanceDto recommendationsDto, 
        string userId = null);
}