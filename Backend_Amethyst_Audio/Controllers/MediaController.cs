using Backend_Amethyst_Audio.Services.Abstractions;
using Microsoft.AspNetCore.Mvc;

namespace Backend_Amethyst_Audio.Controllers;

[ApiController]
[Route("api/{controller}")]
public class MediaController : ControllerBase
{
    private IMediaSevice _service;
    public MediaController(IMediaSevice service)
    {
        _service = service;
    }
    
    [HttpGet("/tracks/{id}")]
    public async Task<IActionResult> GetTrackFileAsync(int id)
    {

        string filePath = await _service.GetTrackFilePathAsync(id); 
        return PhysicalFile(filePath, "audio/mpeg", enableRangeProcessing: true);
    }

    [HttpGet("/tracks/covers/{id}")]
    public async Task<IActionResult> GetTrackCoverAsync(int id)
    {
        string filePath = await _service.GetTrackCoverPathAsync(id);
        return PhysicalFile(filePath, "image/jpeg");
    }

    [HttpGet("/playlists/covers/{id}")]
    public async Task<IActionResult> GetPlaylistCoverAsync(int id)
    {
        string filePath = await _service.GetPlaylistCoverPathAsync(id);
        return PhysicalFile(filePath, "image/jpeg");
    }

    [HttpGet("/albums/covers/{id}")]
    public async Task<IActionResult> GetAlbumCoverAsync(int id)
    {
        string filePath = await _service.GetAlbumCoverPathAsync(id);
        return PhysicalFile(filePath, "image/jpeg");
    }

    [HttpGet("/users/avatars/{id}")]
    public async Task<IActionResult> GetUserAvatarAsync(int id)
    {
        string filePath = await _service.GetUserAvatarPathAsync(id);
        return PhysicalFile(filePath, "image/jpeg");
    }

    [HttpGet("/users/headers/{id}")]
    public async Task<IActionResult> GetUserHeaderAsync(int id)
    {
        string filePath = await _service.GetUserHeaderPathAsync(id);
        return PhysicalFile(filePath, "image/jpeg");
    }
    
}