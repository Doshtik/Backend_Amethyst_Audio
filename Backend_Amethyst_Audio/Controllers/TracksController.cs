using Backend_Amethyst_Audio.DTO;
using Backend_Amethyst_Audio.Models;
using Backend_Amethyst_Audio.Services.Abstractions;
using Microsoft.AspNetCore.Mvc;

namespace Backend_Amethyst_Audio.Controllers;

[ApiController]
[Route("api/[controller]")]
public class TracksController : ControllerBase
{
    private ITrackService _trackService;
    
    public TracksController(ITrackService trackService) => _trackService = trackService;

    [HttpPost("")]
    public async Task<IActionResult> CreateTrack([FromBody] CreateTrackDto dto)
    {
        try
        {
            TrackInfoDto result = await _trackService.CreateAsync(dto);
            return CreatedAtAction(nameof(CreateTrack), new { id = result.Id }, result);
        }
        catch (BadHttpRequestException e)
        {
            return BadRequest(e.Message);
        }
        catch (Exception e)
        {
            return StatusCode(500, e.Message);
        }
    }
    
    [HttpGet("{trackId}")]
    public async Task<IActionResult> GetTrack(long trackId)
    {
        TrackInfoDto dto;
        throw new NotImplementedException();
    }

    [HttpPut("{trackId}")]
    public async Task<IActionResult> UpdateTrack(long trackId, [FromBody] ChangeTrackInfoDto dto)
    {
        TrackInfoDto  newTrackInfoDto = await _trackService.UpdateAsync(dto);
        throw new NotImplementedException();
    }

    [HttpDelete]
    public async Task<IActionResult> DeleteTrack(long trackId)
    {
        try
        {
            await _trackService.DeleteAsync(trackId);
            return NoContent();
        }
        catch
        {
            return StatusCode(500);
        }
    }
    
    
}