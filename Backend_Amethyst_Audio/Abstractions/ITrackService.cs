using Backend_Amethyst_Audio.Models;
using Microsoft.AspNetCore.Mvc;

namespace Backend_Amethyst_Audio.Abstractions;

public interface ITrackService
{
    Task<Track> GetByIdAsync(long id);
    Task<List<Track>> GetAllAsync();
    
    Task CreateAsync(Track track);
    Task UpdateAsync(Track track);
    Task DeleteAsync(long id);
    
    Task<List<Track>> GetListByGenreAsync(string genre);
    Task<List<Track>> GetListBySearchAsync(string search);
    Task<List<Track>> GetListByUserIdAsync(long userId);
    
    Task<List<Track>> GetListOfNewestAsync();
    Task<List<Track>> GetListOfLiked(long userId);
    Task<List<Track>> GetListOfPersonalizedAsync(string trackPace, string trackMood, bool is_textless, string? country);
    
    Task<bool> IsLikedAsync(long idUser);
    
    Task AddInPlaylistAsync(long idUser, long idPlaylist);
    Task RemoveFromPlaylistAsync(long idUser, long idPlaylist);
    IActionResult GetImageByName(string name);
}