using System.Security.Claims;
using Backend_Amethyst_Audio.DTO;
using Backend_Amethyst_Audio.Entities;
using Backend_Amethyst_Audio.Services.Abstractions;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;

namespace Backend_Amethyst_Audio.Controllers;

[ApiController]
[Route("api/[controller]")]
public class AlbumsController : ControllerBase
{
    private readonly IAlbumService _albumService;
    private readonly ILogger<AlbumsController> _logger;

    public AlbumsController(IAlbumService albumService, ILogger<AlbumsController> logger)
    {
        _albumService = albumService;
        _logger = logger;
    }

    [HttpGet("{albumId}")]
    [Authorize]
    public async Task<IActionResult> GetByIdAsync(long albumId)
    {
        _logger.LogDebug("[Debug] Request to get album by ID: {AlbumId}", albumId);
        try
        {
            var album = await _albumService.GetByIdAsync(albumId);
            _logger.LogInformation("[Info] Successfully retrieved album {AlbumId}", albumId);
            return Ok(album);
        }
        catch (KeyNotFoundException)
        {
            _logger.LogWarning("[Warn] Album {AlbumId} not found", albumId);
            return NotFound(new { message = "Album not found" });
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "[Error] Unexpected error occurred while retrieving album {AlbumId}", albumId);
            return Problem(statusCode: 500, title: "Internal Server Error", detail: "An error occurred while processing your request.");
        }
    }

    [HttpGet]
    [Authorize]
    public async Task<IActionResult> GetAllAsync()
    {
        _logger.LogDebug("[Debug] Request to get all albums");
        try
        {
            var albums = await _albumService.GetAllAsync();
            _logger.LogInformation("[Info] Successfully retrieved {Count} albums", albums.Count);
            return Ok(albums);
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "[Error] Unexpected error occurred while retrieving all albums");
            return Problem(statusCode: 500, title: "Internal Server Error", detail: "An error occurred while processing your request.");
        }
    }

    [HttpGet("user/{userId}")]
    [Authorize]
    public async Task<IActionResult> GetListByUserIdAsync(long userId)
    {
        var currentUserId = long.Parse(User.FindFirstValue(System.Security.Claims.ClaimTypes.NameIdentifier));
        
        try
        {
            _logger.LogDebug("[Debug] Get list of albums by user id. TargetUserId={UserId}, RequestedBy={CurrentUserId}", 
                userId, currentUserId);
            
            List<AlbumInfoDto> albums = await _albumService.GetListByUserIdAsync(userId);
            
            _logger.LogInformation("[Info] Retrieved {Count} albums for user. UserId={UserId}", 
                albums.Count, userId);
            return Ok(albums);
        }
        catch (Exception e)
        {
            _logger.LogError(e, "[Error] Failed to get user albums. UserId={UserId}", userId);
            return StatusCode(500, new { error = "Internal server error" });
        }
    }

    [HttpPost]
    [Authorize]
    public async Task<IActionResult> CreateAsync([FromBody] CreateAlbumDto dto)
    {
        _logger.LogDebug("[Debug] Request to create a new album. DTO: {@Dto}", dto);
        try
        {
            var album = await _albumService.CreateAsync(dto);
            _logger.LogInformation("[Info] Successfully created album {AlbumId}", album.Id);
            return CreatedAtAction(nameof(CreateAsync), new { albumId = album.Id }, album);
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "[Error] Failed to create album. DTO: {@Dto}", dto);
            return Problem(statusCode: 500, title: "Internal Server Error", detail: "An error occurred while processing your request.");
        }
    }

    [HttpPut("{id}")]
    [Authorize]
    public async Task<IActionResult> UpdateAsync(long id, [FromBody] ChangeAlbumInfoDto dto)
    {
        _logger.LogDebug("[Debug] Request to update album. DTO: {@Dto}", dto);
        try
        {
            var album = await _albumService.UpdateAsync(dto);
            _logger.LogInformation("[Info] Successfully updated album {AlbumId}", album.Id);
            return Ok(album);
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "[Error] Failed to update album. DTO: {@Dto}", dto);
            return Problem(statusCode: 500, title: "Internal Server Error", detail: "An error occurred while processing your request.");
        }
    }

    [HttpDelete("{id}")]
    [Authorize]
    public async Task<IActionResult> DeleteAsync(long id)
    {
        _logger.LogDebug("[Debug] Request to delete album {AlbumId}", id);
        try
        {
            await _albumService.DeleteAsync(id);
            _logger.LogInformation("[Info] Successfully deleted album {AlbumId}", id);
            return NoContent();
        }
        catch (KeyNotFoundException)
        {
            _logger.LogWarning("[Warn] Attempted to delete non-existent album {AlbumId}", id);
            return NotFound(new { message = "Album not found" });
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "[Error] Unexpected error occurred while deleting album {AlbumId}", id);
            return Problem(statusCode: 500, title: "Internal Server Error", detail: "An error occurred while processing your request.");
        }
    }

    [HttpPost("{id}/save")]
    [Authorize]
    public async Task<IActionResult> SaveAlbumAsync(long id)
    {
        var userId = int.Parse(User.FindFirstValue(System.Security.Claims.ClaimTypes.NameIdentifier));
        _logger.LogDebug("[Debug] Request to save album {AlbumId} for user {UserId}", id, userId);
        try
        {
            await _albumService.SaveAlbumAsync(userId, id);
            _logger.LogInformation("[Info] Successfully saved album {AlbumId} for user {UserId}", id, userId);
            return Ok();
        }
        catch (InvalidOperationException ex)
        {
            _logger.LogWarning("[Warn] Failed to save album {AlbumId} for user {UserId}: {Message}", id, userId, ex.Message);
            return Conflict(new { message = ex.Message });
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "[Error] Unexpected error occurred while saving album {AlbumId} for user {UserId}", id, userId);
            return Problem(statusCode: 500, title: "Internal Server Error", detail: "An error occurred while processing your request.");
        }
    }

    [HttpDelete("{id}/save")]
    [Authorize]
    public async Task<IActionResult> UnsaveAlbumAsync(long id)
    {
        var userId = int.Parse(User.FindFirstValue(System.Security.Claims.ClaimTypes.NameIdentifier));
        _logger.LogDebug("[Debug] Request to unsave album {AlbumId} for user {UserId}", id, userId);
        try
        {
            await _albumService.UnsaveAlbumAsync(userId, id);
            _logger.LogInformation("[Info] Successfully unsaved album {AlbumId} for user {UserId}", id, userId);
            return NoContent();
        }
        catch (InvalidOperationException ex)
        {
            _logger.LogWarning("[Warn] Failed to unsave album {AlbumId} for user {UserId}: {Message}", id, userId, ex.Message);
            return NotFound(new { message = ex.Message });
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "[Error] Unexpected error occurred while unsaving album {AlbumId} for user {UserId}", id, userId);
            return Problem(statusCode: 500, title: "Internal Server Error", detail: "An error occurred while processing your request.");
        }
    }
}