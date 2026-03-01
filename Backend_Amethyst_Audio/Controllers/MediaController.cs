using Backend_Amethyst_Audio.Services.Abstractions;
using Microsoft.AspNetCore.Mvc;

namespace Backend_Amethyst_Audio.Controllers;

[ApiController]
[Route("api/{controller}")]
public class MediaController : ControllerBase
{
    [HttpGet("/tracks/{filePath}")]
    public async Task<IActionResult> GetTrackFile(string filePath)
    {
        if (!System.IO.File.Exists(filePath)) 
            return NotFound(); 
        return PhysicalFile(filePath, "audio/mpeg", enableRangeProcessing: true);
    }

    [HttpGet("/covers/{filePath}")]
    public async Task<IActionResult> GetContentCover(string filePath)
    {
        if (!System.IO.File.Exists(filePath))
            return NotFound();
        return PhysicalFile(filePath, "image/jpeg");
    }

    [HttpGet("/avatars/{filePath}")]
    public async Task<IActionResult> GetUserAvatar(string filePath)
    {
        if (!System.IO.File.Exists(filePath))
            return NotFound();
        return PhysicalFile(filePath, "image/jpeg");
    }

    [HttpGet("/headers/{filePath}")]
    public async Task<IActionResult> GetUserHeader(string filePath)
    {
        if (!System.IO.File.Exists(filePath))
            return NotFound();
        return PhysicalFile(filePath, "image/jpeg");
    }
    
}