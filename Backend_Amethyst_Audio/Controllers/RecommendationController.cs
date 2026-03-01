using Backend_Amethyst_Audio.DTO;
using Backend_Amethyst_Audio.DTO.Pages;
using Backend_Amethyst_Audio.Services.Abstractions;
using Microsoft.AspNetCore.Mvc;

namespace Backend_Amethyst_Audio.Controllers;

[ApiController]
[Route("api/pages/[controller]")]
public class RecommendationController : ControllerBase
{
    private ITrackService _trackService;
    
    public RecommendationController(ITrackService trackService) => _trackService = trackService;
    
    [HttpGet("")]
    public async Task<IActionResult> GetPageInfo()
    {
        PageMyRecordDto dto = await _trackService.PageMyRecordAsync();
        return Ok(dto);
    }
    
    
    [HttpGet("recommend")]
    public async Task<IActionResult> GetRecommendations([FromBody] PageMyRecordPersonalizedDto dto)
    {
        List<TrackInfoDto> trackDto = await _trackService.GetListOfPersonalizedAsync(dto);
        return Ok(trackDto);
    }
}