using System.Security.Claims;
using Backend_Amethyst_Audio.DTO;
using Backend_Amethyst_Audio.DTO.Pages;
using Backend_Amethyst_Audio.Services.Abstractions;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;

namespace Backend_Amethyst_Audio.Controllers;

[ApiController]
[Route("api/[controller]")]
public class RecommendationController : ControllerBase
{
    private readonly ITrackService _trackService;
    private readonly ILogger<RecommendationController> _logger;
    
    public RecommendationController(ITrackService trackService, ILogger<RecommendationController> logger)
    {
        _trackService = trackService;
        _logger = logger;
    }
    
    /// <summary>
    /// Get available filters for personalized recommendations (moods, paces, countries)
    /// GET /api/Recommendation/config
    /// </summary>
    [HttpGet("config")]
    public async Task<IActionResult> GetRecommendationConfig()
    {
        try
        {
            _logger.LogDebug("[Debug] Requesting recommendation config. UserId={UserId}", 
                User.FindFirstValue(System.Security.Claims.ClaimTypes.NameIdentifier) ?? "anonymous");
            
            PageMyRecordDto config = await _trackService.GetRecommendationConfigAsync();
            
            _logger.LogInformation("[Info] Recommendation config retrieved. Moods={MoodCount}, Paces={PaceCount}", 
                config.AvailableMoods?.Count ?? 0, config.AvailablePaces?.Count ?? 0);
            return Ok(config);
        }
        catch (Exception e)
        {
            _logger.LogError(e, "[Error] Failed to retrieve recommendation config");
            return StatusCode(500, new { error = "Internal server error" });
        }
    }
    
    /// <summary>
    /// Get personalized track recommendations based on user preferences
    /// GET /api/Recommendation/query
    /// </summary>
    [HttpGet("query")]
    [Authorize]
    public async Task<IActionResult> GetPersonalizedRecommendations([FromBody] PageMyRecordPersonalizedDto dto)
    {
        var userId = User.FindFirstValue(System.Security.Claims.ClaimTypes.NameIdentifier);
        
        try
        {
            _logger.LogInformation("[Info] Personalized recommendations request. UserId={UserId}, Mood={Mood}, Pace={Pace}, Country={Country}", 
                userId, dto.MoodName ?? "any", dto.PaceName ?? "any", dto.Country ?? "any");
            
            List<TrackInfoDto> tracks = await _trackService.GetPersonalizedRecommendationsAsync(dto, userId);
            
            _logger.LogInformation("[Info] Recommendations generated. UserId={UserId}, Count={Count}", 
                userId, tracks.Count);
            return Ok(tracks);
        }
        catch (ArgumentException e)
        {
            _logger.LogWarning("[Warn] Invalid recommendation parameters. UserId={UserId}, Reason={Reason}", 
                userId, e.Message);
            return BadRequest(new { error = e.Message });
        }
        catch (Exception e)
        {
            _logger.LogError(e, "[Error] Failed to generate recommendations. UserId={UserId}", userId);
            return StatusCode(500, new { error = "Internal server error" });
        }
    }
}