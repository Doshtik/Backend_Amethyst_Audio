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
    public async Task<IActionResult> GetById(long albumId)
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
    public async Task<IActionResult> GetAll()
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

    [HttpPost]
    [Authorize]
    public async Task<IActionResult> Create([FromBody] CreateAlbumDto dto)
    {
        _logger.LogDebug("[Debug] Request to create a new album. DTO: {@Dto}", dto);
        try
        {
            var album = await _albumService.CreateAsync(dto);
            _logger.LogInformation("[Info] Successfully created album {AlbumId}", album.Id);
            return CreatedAtAction(nameof(GetById), new { albumId = album.Id }, album);
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "[Error] Failed to create album. DTO: {@Dto}", dto);
            return Problem(statusCode: 500, title: "Internal Server Error", detail: "An error occurred while processing your request.");
        }
    }

    [HttpPut]
    [Authorize]
    public async Task<IActionResult> Update([FromBody] ChangeAlbumInfoDto dto)
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
    public async Task<IActionResult> Delete(long id)
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

    [HttpPost("save/{idUser}/{idAlbum}")]
    [Authorize]
    public async Task<IActionResult> SaveAlbum(long idUser, long idAlbum)
    {
        _logger.LogDebug("[Debug] Request to save album {AlbumId} for user {UserId}", idAlbum, idUser);
        try
        {
            await _albumService.SaveAlbumAsync(idUser, idAlbum);
            _logger.LogInformation("[Info] Successfully saved album {AlbumId} for user {UserId}", idAlbum, idUser);
            return Ok();
        }
        catch (InvalidOperationException ex)
        {
            _logger.LogWarning("[Warn] Failed to save album {AlbumId} for user {UserId}: {Message}", idAlbum, idUser, ex.Message);
            return Conflict(new { message = ex.Message });
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "[Error] Unexpected error occurred while saving album {AlbumId} for user {UserId}", idAlbum, idUser);
            return Problem(statusCode: 500, title: "Internal Server Error", detail: "An error occurred while processing your request.");
        }
    }

    [HttpDelete("save/{idUser}/{idAlbum}")]
    [Authorize]
    public async Task<IActionResult> UnsaveAlbum(long idUser, long idAlbum)
    {
        _logger.LogDebug("[Debug] Request to unsave album {AlbumId} for user {UserId}", idAlbum, idUser);
        try
        {
            await _albumService.UnsaveAlbumAsync(idUser, idAlbum);
            _logger.LogInformation("[Info] Successfully unsaved album {AlbumId} for user {UserId}", idAlbum, idUser);
            return NoContent();
        }
        catch (InvalidOperationException ex)
        {
            _logger.LogWarning("[Warn] Failed to unsave album {AlbumId} for user {UserId}: {Message}", idAlbum, idUser, ex.Message);
            return NotFound(new { message = ex.Message });
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "[Error] Unexpected error occurred while unsaving album {AlbumId} for user {UserId}", idAlbum, idUser);
            return Problem(statusCode: 500, title: "Internal Server Error", detail: "An error occurred while processing your request.");
        }
    }
}