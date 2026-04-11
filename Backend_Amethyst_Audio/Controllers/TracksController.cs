using Backend_Amethyst_Audio.DTO;
using Backend_Amethyst_Audio.Models;
using Backend_Amethyst_Audio.Services.Abstractions;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;

namespace Backend_Amethyst_Audio.Controllers;

[ApiController]
[Route("api/[controller]")]
public class TracksController : ControllerBase
{
    private ITrackService _trackService;
    
    public TracksController(ITrackService trackService) => _trackService = trackService;
    
    [HttpGet("{trackId}")]
    [Authorize]
    public async Task<IActionResult> GetById(long trackId)
    {
        try
        {
            TrackInfoDto dto = await _trackService.GetByIdAsync(trackId);
            return Ok(dto);
        }
        catch (Exception e)
        {
            return  StatusCode(500, e.Message);
        }
    }
    
    [HttpGet]
    [Authorize]
    public async Task<IActionResult> GetAll()
    {
        try
        {
            List<TrackInfoDto> listDto = await _trackService.GetAllAsync();
            return Ok(listDto);
        }
        catch (Exception e)
        {
            return  StatusCode(500, e.Message);
        }
    }

    [HttpPost]
    [Authorize]
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

    [HttpPut]
    [Authorize]
    public async Task<IActionResult> UpdateTrack([FromBody] ChangeTrackInfoDto dto)
    {
        TrackInfoDto newTrackInfoDto = await _trackService.UpdateAsync(dto);
        throw new NotImplementedException();
    }

    [HttpDelete]
    [Authorize]
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