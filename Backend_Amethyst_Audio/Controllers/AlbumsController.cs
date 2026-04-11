using Backend_Amethyst_Audio.DTO;
using Backend_Amethyst_Audio.Services.Abstractions;
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
    public async Task<IActionResult> GetById(long albumId)
    {
        try
        {
            AlbumInfoDto album = await _albumService.GetByIdAsync(albumId);
            return Ok(album);
        }
        catch (Exception ex)
        {
            return StatusCode(500, ex);
        }
    }
    
    [HttpGet("")]
    public async Task<IActionResult> GetAll()
    {
        throw new NotImplementedException();
    }
    
    [HttpPost("")]
    public async Task<IActionResult> Create()
    {
        throw new NotImplementedException();
    }
    
    [HttpPut("")]
    public async Task<IActionResult> Update()
    {
        throw new NotImplementedException();
    }
    
    [HttpDelete("")]
    public async Task<IActionResult> Delete()
    {
        throw new NotImplementedException();
    }
}