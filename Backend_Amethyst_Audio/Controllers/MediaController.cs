using Backend_Amethyst_Audio.Services.Abstractions;
using Microsoft.AspNetCore.Mvc;

namespace Backend_Amethyst_Audio.Controllers;

[ApiController]
[Route("api/{controller}")]
public class MediaController : ControllerBase
{
    private readonly IMediaService _service;
    private readonly ILogger<MediaController> _logger;

    public MediaController(IMediaService service, ILogger<MediaController> logger)
    {
        _service = service;
        _logger = logger;
    }
    
    [HttpGet("tracks/audio/{id}")]
    public async Task<IActionResult> GetTrackFileAsync(int id)
    {
        try
        {
            _logger.LogDebug("[Debug] Requesting track file. TrackId={TrackId}", id);
            
            string filePath = await _service.GetTrackFilePathAsync(id); 
            
            _logger.LogInformation("[Info] Serving track file. TrackId={TrackId}, FilePath={FilePath}", id, filePath);
            return PhysicalFile(filePath, "audio/mpeg", enableRangeProcessing: true);
        }
        catch (FileNotFoundException e)
        {
            _logger.LogWarning("[Warn] Track file not found. TrackId={TrackId}", id);
            return NotFound(new { error = "Track not found" });
        }
        catch (Exception e)
        {
            _logger.LogError(e, "[Error] Failed to serve track file. TrackId={TrackId}", id);
            return StatusCode(500, new { error = "Internal server error" });
        }
    }

    [HttpGet("tracks/covers/{id}")]
    public async Task<IActionResult> GetTrackCoverAsync(int id)
    {
        try
        {
            _logger.LogDebug("[Debug] Requesting track cover. TrackId={TrackId}", id);
            
            string filePath = await _service.GetTrackCoverPathAsync(id);
            
            _logger.LogInformation("[Info] Serving track cover. TrackId={TrackId}, FilePath={FilePath}", id, filePath);
            return PhysicalFile(filePath, "image/jpeg");
        }
        catch (FileNotFoundException e)
        {
            _logger.LogWarning("[Warn] Track cover not found. TrackId={TrackId}", id);
            return NotFound(new { error = "Cover not found" });
        }
        catch (Exception e)
        {
            _logger.LogError(e, "[Error] Failed to serve track cover. TrackId={TrackId}", id);
            return StatusCode(500, new { error = "Internal server error" });
        }
    }

    [HttpGet("playlists/covers/{id}")]
    public async Task<IActionResult> GetPlaylistCoverAsync(int id)
    {
        try
        {
            _logger.LogDebug("[Debug] Requesting playlist cover. PlaylistId={PlaylistId}", id);
            
            string filePath = await _service.GetPlaylistCoverPathAsync(id);
            
            _logger.LogInformation("[Info] Serving playlist cover. PlaylistId={PlaylistId}, FilePath={FilePath}", id, filePath);
            return PhysicalFile(filePath, "image/jpeg");
        }
        catch (FileNotFoundException e)
        {
            _logger.LogWarning("[Warn] Playlist cover not found. PlaylistId={PlaylistId}", id);
            return NotFound(new { error = "Cover not found" });
        }
        catch (Exception e)
        {
            _logger.LogError(e, "[Error] Failed to serve playlist cover. PlaylistId={PlaylistId}", id);
            return StatusCode(500, new { error = "Internal server error" });
        }
    }

    [HttpGet("albums/covers/{id}")]
    public async Task<IActionResult> GetAlbumCoverAsync(int id)
    {
        try
        {
            _logger.LogDebug("[Debug] Requesting album cover. AlbumId={AlbumId}", id);
            
            string filePath = await _service.GetAlbumCoverPathAsync(id);
            
            _logger.LogInformation("[Info] Serving album cover. AlbumId={AlbumId}, FilePath={FilePath}", id, filePath);
            return PhysicalFile(filePath, "image/jpeg");
        }
        catch (FileNotFoundException e)
        {
            _logger.LogWarning("[Warn] Album cover not found. AlbumId={AlbumId}", id);
            return NotFound(new { error = "Cover not found" });
        }
        catch (Exception e)
        {
            _logger.LogError(e, "[Error] Failed to serve album cover. AlbumId={AlbumId}", id);
            return StatusCode(500, new { error = "Internal server error" });
        }
    }

    [HttpGet("users/avatars/{id}")]
    public async Task<IActionResult> GetUserAvatarAsync(int id)
    {
        try
        {
            _logger.LogDebug("[Debug] Requesting user avatar. UserId={UserId}", id);
            
            string filePath = await _service.GetUserAvatarPathAsync(id);
            
            _logger.LogInformation("[Info] Serving user avatar. UserId={UserId}, FilePath={FilePath}", id, filePath);
            return PhysicalFile(filePath, "image/jpeg");
        }
        catch (FileNotFoundException e)
        {
            _logger.LogWarning("[Warn] User avatar not found. UserId={UserId}", id);
            return NotFound(new { error = "Avatar not found" });
        }
        catch (Exception e)
        {
            _logger.LogError(e, "[Error] Failed to serve user avatar. UserId={UserId}", id);
            return StatusCode(500, new { error = "Internal server error" });
        }
    }

    [HttpGet("users/headers/{id}")]
    public async Task<IActionResult> GetUserHeaderAsync(int id)
    {
        try
        {
            _logger.LogDebug("[Debug] Requesting user header. UserId={UserId}", id);
            
            string filePath = await _service.GetUserHeaderPathAsync(id);
            
            _logger.LogInformation("[Info] Serving user header. UserId={UserId}, FilePath={FilePath}", id, filePath);
            return PhysicalFile(filePath, "image/jpeg");
        }
        catch (FileNotFoundException e)
        {
            _logger.LogWarning("[Warn] User header not found. UserId={UserId}", id);
            return NotFound(new { error = "Header not found" });
        }
        catch (Exception e)
        {
            _logger.LogError(e, "[Error] Failed to serve user header. UserId={UserId}", id);
            return StatusCode(500, new { error = "Internal server error" });
        }
    }
}