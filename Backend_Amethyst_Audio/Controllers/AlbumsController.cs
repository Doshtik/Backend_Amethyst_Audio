using Backend_Amethyst_Audio.DTO;
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
        try
        {
            AlbumInfoDto album = await _albumService.GetByIdAsync(albumId);
            return Ok(album);
        }
        catch (Exception ex)
        {
            return StatusCode(500, ex.Message);
        }
    }
    
    [HttpGet("")]
    [Authorize]
    public async Task<IActionResult> GetAll()
    {
        try
        {
            List<AlbumInfoDto> albums = await _albumService.GetAllAsync();
            return Ok(albums);
        }
        catch (Exception ex)
        {
            return StatusCode(500, ex.Message);
        }
    }
    
    [HttpPost("")]
    [Authorize]
    public async Task<IActionResult> Create([FromBody] CreateAlbumDto dto)
    {
        try
        {
            AlbumInfoDto album = await _albumService.CreateAsync(dto);
            return Ok(album);
        }
        catch (Exception ex)
        {
            return StatusCode(500, ex.Message);
        }
    }
    
    [HttpPut("")]
    [Authorize]
    public async Task<IActionResult> Update([FromBody] ChangeAlbumInfoDto dto)
    {
        try
        {
            AlbumInfoDto album = await _albumService.UpdateAsync(dto);
            return Ok(album);
        }
        catch (Exception ex)
        {
            return StatusCode(500, ex.Message);
        }
    }
    
    [HttpDelete("{id}")]
    [Authorize]
    public async Task<IActionResult> Delete(long id)
    {
        try
        {
            await _albumService.DeleteAsync(id);
            return NotFound();
        }
        catch (Exception ex)
        {
            return StatusCode(500, ex.Message);
        }
    }
}