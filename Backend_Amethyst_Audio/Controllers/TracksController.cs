using System.Security.Claims;
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
    private readonly ITrackService _trackService;
    private readonly ILogger<TracksController> _logger;

    public TracksController(ITrackService trackService, ILogger<TracksController> logger)
    {
        _trackService = trackService;
        _logger = logger;
    }

    [HttpGet("{trackId}")]
    [Authorize]
    public async Task<IActionResult> GetById(long trackId)
    {
        _logger.LogDebug("[Debug] Request to get track by ID: {TrackId}", trackId);
        try
        {
            var dto = await _trackService.GetByIdAsync(trackId);
            _logger.LogInformation("[Info] Successfully retrieved track {TrackId}", trackId);
            return Ok(dto);
        }
        catch (KeyNotFoundException)
        {
            _logger.LogWarning("[Warn] Track {TrackId} not found", trackId);
            return NotFound(new { message = "Track not found" });
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "[Error] Unexpected error retrieving track {TrackId}", trackId);
            return Problem(statusCode: 500, title: "Internal Server Error", detail: "An error occurred while processing your request.");
        }
    }

    [HttpGet]
    [Authorize]
    public async Task<IActionResult> GetAll()
    {
        _logger.LogDebug("[Debug] Request to get all tracks");
        try
        {
            var listDto = await _trackService.GetAllAsync();
            _logger.LogInformation("[Info] Successfully retrieved {Count} tracks", listDto.Count);
            return Ok(listDto);
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "[Error] Unexpected error retrieving all tracks");
            return Problem(statusCode: 500, title: "Internal Server Error", detail: "An error occurred while processing your request.");
        }
    }

    [HttpGet("user/{userId}")]
    [Authorize]
    public async Task<IActionResult> GetListByUserIdAsync(long userId)
    {
        _logger.LogDebug("[Debug] Request to get list of tracks by user Id: {Id}", userId);
        try
        {
            List<TrackInfoDto> dto = await _trackService.GetListTrackByUserIdAsync(userId);
            _logger.LogInformation("[Info] Successfully retrieved {Count} tracks", dto.Count);
            return Ok(dto);
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "[Error] Unexpected error retrieving all user tracks");
            return Problem(statusCode: 500, title: "Internal Server Error", detail: "An error occurred while processing your request.");
        }
    }

    [HttpPost]
    [Authorize]
    public async Task<IActionResult> CreateTrack([FromForm] CreateTrackDto dto)
    {
        _logger.LogDebug("[Debug] Request to create track. Title={Title}", dto.Name);
        try
        {
            long userId = long.Parse(User.FindFirstValue(ClaimTypes.NameIdentifier));
            var result = await _trackService.CreateAsync(dto, userId);
            _logger.LogInformation("[Info] Successfully created track {TrackId}", result.Id);
            return CreatedAtAction(nameof(GetById), new { trackId = result.Id }, result);
        }
        catch (ArgumentException ex)
        {
            _logger.LogWarning("[Warn] Validation failed for track creation: {Message}", ex.Message);
            return BadRequest(new { message = ex.Message });
        }
        catch (InvalidOperationException ex)
        {
            _logger.LogWarning("[Warn] Operation failed during track creation: {Message}", ex.Message);
            return Conflict(new { message = ex.Message });
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "[Error] Unexpected error creating track '{Title}'", dto.Name);
            return Problem(statusCode: 500, title: "Internal Server Error", detail: "An error occurred while processing your request.");
        }
    }

    [HttpPut("{trackId}")]
    [Authorize]
    public async Task<IActionResult> UpdateTrack(long trackId, [FromForm] ChangeTrackInfoDto dto)
    {
        dto.Id = trackId;
        
        _logger.LogDebug("[Debug] Request to update track. TrackId={TrackId}", trackId);
        try
        {
            var result = await _trackService.UpdateAsync(dto);
            _logger.LogInformation("[Info] Successfully updated track {TrackId}", trackId);
            return Ok(result);
        }
        catch (KeyNotFoundException)
        {
            _logger.LogWarning("[Warn] Track {TrackId} not found for update", trackId);
            return NotFound(new { message = "Track not found" });
        }
        catch (InvalidOperationException ex)
        {
            _logger.LogWarning("[Warn] Operation failed during track update: {Message}", ex.Message);
            return Conflict(new { message = ex.Message });
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "[Error] Unexpected error updating track {TrackId}", trackId);
            return Problem(statusCode: 500, title: "Internal Server Error", detail: "An error occurred while processing your request.");
        }
    }

    [HttpDelete("{trackId}")]
    [Authorize]
    public async Task<IActionResult> DeleteTrack(long trackId)
    {
        _logger.LogDebug("[Debug] Request to delete track. TrackId={TrackId}", trackId);
        try
        {
            await _trackService.DeleteAsync(trackId);
            _logger.LogInformation("[Info] Successfully deleted track {TrackId}", trackId);
            return NoContent();
        }
        catch (KeyNotFoundException)
        {
            _logger.LogWarning("[Warn] Track {TrackId} not found for deletion", trackId);
            return NotFound(new { message = "Track not found" });
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "[Error] Unexpected error deleting track {TrackId}", trackId);
            return Problem(statusCode: 500, title: "Internal Server Error", detail: "An error occurred while processing your request.");
        }
    }
}