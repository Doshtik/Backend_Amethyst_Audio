using System.Data;
using System.Security.Claims;
using Backend_Amethyst_Audio.DTO;
using Backend_Amethyst_Audio.Services.Abstractions;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;

namespace Backend_Amethyst_Audio.Controllers;

[ApiController]
[Route("api/[controller]")]
public class PlaylistsController : ControllerBase
{
    private readonly IPlaylistService _playlistService;
    private readonly ILogger<PlaylistsController> _logger;

    public PlaylistsController(IPlaylistService playlistService, ILogger<PlaylistsController> logger)
    {
        _playlistService = playlistService;
        _logger = logger;
    }

    [HttpGet("{id}")]
    [Authorize]
    public async Task<IActionResult> GetByIdAsync(long id)
    {
        long userId = long.Parse(User.FindFirstValue(System.Security.Claims.ClaimTypes.NameIdentifier));
        
        try
        {
            _logger.LogDebug("[Debug] Get playlist by Id. PlaylistId={PlaylistId}, RequestedBy={UserId}", id, userId);
            
            PlaylistInfoDto playlist = await _playlistService.GetByIdAsync(id, userId.ToString());
            
            _logger.LogInformation("[Info] Playlist retrieved. PlaylistId={PlaylistId}, OwnerId={OwnerId}", 
                id, playlist.OwnerId);
            return Ok(playlist);
        }
        catch (KeyNotFoundException e)
        {
            _logger.LogWarning("[Warn] Playlist not found. PlaylistId={PlaylistId}, RequestedBy={UserId}", id, userId);
            return NotFound(new { error = e.Message });
        }
        catch (UnauthorizedAccessException e)
        {
            _logger.LogWarning("[Warn] Access denied to playlist. PlaylistId={PlaylistId}, RequestedBy={UserId}", id, userId);
            return Forbid();
        }
        catch (Exception e)
        {
            _logger.LogError(e, "[Error] Failed to get playlist. PlaylistId={PlaylistId}", id);
            return StatusCode(500, new { error = "Internal server error" });
        }
    }

    [HttpGet]
    [Authorize]
    public async Task<IActionResult> GetAllAsync()
    {
        var userId = User.FindFirstValue(System.Security.Claims.ClaimTypes.NameIdentifier);
        
        try
        {
            _logger.LogDebug("[Debug] Get all playlists request. RequestedBy={UserId}", userId);
            
            List<PlaylistInfoDto> playlists = await _playlistService.GetAllAsync();
            
            _logger.LogInformation("[Info] Retrieved {Count} playlists", playlists.Count);
            return Ok(playlists);
        }
        catch (Exception e)
        {
            _logger.LogError(e, "[Error] Failed to retrieve playlists list");
            return StatusCode(500, new { error = "Internal server error" });
        }
    }
    
    [HttpGet("user/{userId}")]
    [Authorize]
    public async Task<IActionResult> GetListByUserIdAsync(long userId)
    {
        var currentUserId = long.Parse(User.FindFirstValue(System.Security.Claims.ClaimTypes.NameIdentifier));
        
        try
        {
            _logger.LogDebug("[Debug] Get list of playlists by user id. TargetUserId={UserId}, RequestedBy={CurrentUserId}", 
                userId, currentUserId);
            
            List<PlaylistInfoDto> playlists = await _playlistService.GetListByUserIdAsync(userId);
            
            _logger.LogInformation("[Info] Retrieved {Count} playlists for user. UserId={UserId}", 
                playlists.Count, userId);
            return Ok(playlists);
        }
        catch (Exception e)
        {
            _logger.LogError(e, "[Error] Failed to get user playlists. UserId={UserId}", userId);
            return StatusCode(500, new { error = "Internal server error" });
        }
    }

    [HttpPost]
    [Authorize]
    public async Task<IActionResult> CreateAsync([FromForm] CreatePlaylistDto dto)
    {
        var userId = User.FindFirstValue(System.Security.Claims.ClaimTypes.NameIdentifier);
        
        if (string.IsNullOrEmpty(userId))
        {
            _logger.LogWarning("[Warn] Playlist creation attempt without valid user claim");
            return Unauthorized(new { error = "Authentication required" });
        }
        
        try
        {
            _logger.LogInformation("[Info] Create playlist request. UserId={UserId}, Title={Title}", 
                userId, dto.Name);
            
            PlaylistInfoDto playlist = await _playlistService.CreateAsync(dto, long.Parse(userId));
            
            _logger.LogInformation("[Info] Playlist created successfully. PlaylistId={PlaylistId}, Title={Title}", 
                playlist.Id, dto.Name);
            return Ok(playlist);
        }
        catch (ArgumentException e)
        {
            _logger.LogWarning("[Warn] Validation failed for playlist creation. , Reason={Reason}", 
                userId, e.Message);
            return BadRequest(new { error = e.Message });
        }
        catch (NoNullAllowedException e)
        {
            _logger.LogWarning("[Warn] Playlist must have at least one track. UserId={UserId}, Reason={Reason}",
                userId, e.Message);
            return BadRequest(new { error = e.Message });
        }
        catch (Exception e)
        {
            _logger.LogError(e, "[Error] Failed to create playlist. UserId={UserId}", userId);
            return StatusCode(500, new { error = "Internal server error", detail = e.Message });
        }
    }

    [HttpPut("{id}")]
    [Authorize]
    public async Task<IActionResult> UpdateAsync(long id, [FromForm] ChangePlaylistInfoDto dto) 
    {
        long? userId = long.Parse(User.FindFirstValue(ClaimTypes.NameIdentifier));
        
        if (userId is null)
        {
            _logger.LogWarning("[Warn] Playlist creation attempt without valid user claim");
            return Unauthorized(new { error = "Authentication required" });
        }

        try
        {
            _logger.LogInformation("[Info] Update playlist request. PlaylistId={PlaylistId}, UserId={UserId}",
                id, userId);

            PlaylistInfoDto playlist = await _playlistService.UpdateAsync(id, dto, userId.ToString());

            _logger.LogInformation("[Info] Playlist updated successfully. PlaylistId={PlaylistId}", id);
            return Ok(playlist);
        }
        catch (KeyNotFoundException e)
        {
            _logger.LogWarning("[Warn] Playlist not found for update. PlaylistId={PlaylistId}", id);
            return NotFound(new { error = e.Message });
        }
        catch (UnauthorizedAccessException e)
        {
            _logger.LogWarning("[Warn] Unauthorized update attempt. PlaylistId={PlaylistId}, UserId={UserId}", id,
                userId);
            return Forbid();
        }
        catch (ArgumentException e)
        {
            _logger.LogWarning("[Warn] Validation failed for playlist update. PlaylistId={PlaylistId}, Reason={Reason}",
                id, e.Message);
            return BadRequest(new { error = e.Message });
        }
        catch (Exception e)
        {
            _logger.LogError(e, "[Error] Failed to update playlist. PlaylistId={PlaylistId}", id);
            return StatusCode(500, new { error = "Internal server error", detail = e.Message });
        }
    }

    [HttpDelete("{id}")]
    [Authorize]
    public async Task<IActionResult> DeleteAsync(long id)
    {
        var userId = User.FindFirstValue(ClaimTypes.NameIdentifier);
        
        try
        {
            _logger.LogInformation("[Info] Delete playlist request. PlaylistId={PlaylistId}, UserId={UserId}", 
                id, userId);
            
            await _playlistService.DeleteAsync(id, userId);
            
            _logger.LogInformation("[Info] Playlist deleted successfully. PlaylistId={PlaylistId}", id);
            return NoContent();
        }
        catch (KeyNotFoundException e)
        {
            _logger.LogWarning("[Warn] Playlist not found for deletion. PlaylistId={PlaylistId}", id);
            return NotFound(new { error = e.Message });
        }
        catch (UnauthorizedAccessException e)
        {
            _logger.LogWarning("[Warn] Unauthorized delete attempt. PlaylistId={PlaylistId}, UserId={UserId}", id, userId);
            return Forbid();
        }
        catch (Exception e)
        {
            _logger.LogError(e, "[Error] Failed to delete playlist. PlaylistId={PlaylistId}", id);
            return StatusCode(500, new { error = "Internal server error" });
        }
    }
    
    //TODO: сделать методы для UserHistory

    [HttpPost("{id}/save")]
    [Authorize]
    public async Task<IActionResult> SavePlaylistAsync(long id)
    {
        var userId = User.FindFirstValue(ClaimTypes.NameIdentifier);
        
        if (string.IsNullOrEmpty(userId))
            return Unauthorized(new { error = "Authentication required" });
        
        try
        {
            _logger.LogInformation("[Info] Save playlist request. PlaylistId={PlaylistId}, UserId={UserId}", 
                id, userId);
            
            await _playlistService.SavePlaylistAsync(long.Parse(userId), id);
            
            _logger.LogInformation("[Info] Playlist saved successfully. PlaylistId={PlaylistId}, UserId={UserId}", 
                id, userId);
            return Ok(new { message = "Playlist saved" });
        }
        catch (InvalidOperationException e)
        {
            _logger.LogWarning("[Warn] Playlist already saved. PlaylistId={PlaylistId}, UserId={UserId}", id, userId);
            return Conflict(new { error = e.Message });
        }
        catch (KeyNotFoundException e)
        {
            _logger.LogWarning("[Warn] Playlist not found for save. PlaylistId={PlaylistId}", id);
            return NotFound(new { error = e.Message });
        }
        catch (Exception e)
        {
            _logger.LogError(e, "[Error] Failed to save playlist. PlaylistId={PlaylistId}", id);
            return StatusCode(500, new { error = "Internal server error" });
        }
    }

    [HttpDelete("{id}/save")]
    [Authorize]
    public async Task<IActionResult> UnsavePlaylistAsync(long id)
    {
        var userId = User.FindFirstValue(System.Security.Claims.ClaimTypes.NameIdentifier);
        
        if (string.IsNullOrEmpty(userId))
            return Unauthorized(new { error = "Authentication required" });
        
        try
        {
            _logger.LogInformation("[Info] Unsave playlist request. PlaylistId={PlaylistId}, UserId={UserId}", 
                id, userId);
            
            await _playlistService.UnsavePlaylistAsync(long.Parse(userId), id);
            
            _logger.LogInformation("[Info] Playlist unsaved successfully. PlaylistId={PlaylistId}, UserId={UserId}", 
                id, userId);
            return Ok(new { message = "Playlist removed from saved" });
        }
        catch (InvalidOperationException e)
        {
            _logger.LogWarning("[Warn] Playlist not in saved list. PlaylistId={PlaylistId}, UserId={UserId}", id, userId);
            return NotFound(new { error = e.Message });
        }
        catch (Exception e)
        {
            _logger.LogError(e, "[Error] Failed to unsave playlist. PlaylistId={PlaylistId}", id);
            return StatusCode(500, new { error = "Internal server error" });
        }
    }
}