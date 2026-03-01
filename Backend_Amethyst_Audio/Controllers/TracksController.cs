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
    public IActionResult CreateTrack([FromBody] CreateTrackDto dto)
    {
        throw new NotImplementedException();
    }
    
    [HttpGet("{trackId}")]
    public IActionResult GetTrack(long trackId)
    {
        TrackInfoDto dto;
        throw new NotImplementedException();
    }

    [HttpPut("{trackId}")]
    public IActionResult UpdateTrack(long trackId, [FromBody] ChangeTrackInfoDto dto)
    {
        TrackInfoDto newTrackInfoDto;
        throw new NotImplementedException();
    }

    [HttpDelete]
    public IActionResult DeleteTrack(long trackId)
    {
        throw new NotImplementedException();
    }
    
    
}