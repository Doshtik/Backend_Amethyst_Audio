using Backend_Amethyst_Audio.DTO;
using Backend_Amethyst_Audio.DTO.Reports;
using Backend_Amethyst_Audio.DTO.Reports.ReportAnswer;
using Backend_Amethyst_Audio.Services.Abstractions;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;

namespace Backend_Amethyst_Audio.Controllers;

[ApiController]
[Route("api/[controller]")]
public class ReportsController : ControllerBase
{
    private readonly IReportService _reportService;

    public ReportsController(IReportService reportService)
    {
        _reportService = reportService;
    }
    
    [HttpGet("{reportId}")]
    public async Task<IActionResult> GetById(long reportId)
    {
        try
        {
            ReportInfoDto dto = await _reportService.GetByIdAsync(reportId);
            return Ok(dto);
        }
        catch (KeyNotFoundException e)
        {
            return StatusCode(500, new { error = "Internal server error" } );
        }
    }

    [HttpGet]
    public async Task<IActionResult> GetAll(string reportType)
    {
        try
        {
            List<ReportInfoDto> dto = await _reportService.GetAllAsync();
            return Ok(dto);
        }
        catch (KeyNotFoundException e)
        {
            return StatusCode(500, new { error = "Internal server error" } );
        }
    }
    
    [HttpPost]
    public async Task<IActionResult> CreateAsync([FromBody] CreateReportDto dto)
    {
        try
        {
            ReportInfoDto report = await _reportService.CreateAsync(dto);
            return Ok(report);
        }
        catch (KeyNotFoundException e)
        {
            return StatusCode(500, new { error = "Internal server error" } );
        }
    }

    [HttpDelete("{reoprtId}")]
    public async Task<IActionResult> DeleteAsync(long reportId)
    {
        await _reportService.DeleteAsync(reportId);
        return NoContent();
    }
    
    
    [HttpGet("answer/{reportAnswerId}")]
    public async Task<IActionResult> GetAnswerById(long reportAnswerId)
    {
        try
        {
            ReportAnswerInfoDto answer = await _reportService.GetAnswerByIdAsync(reportAnswerId);
            return Ok(answer);
        }
        catch (KeyNotFoundException e)
        {
            return StatusCode(500, new { error = "Internal server error" } );
        }
    }

    [HttpGet("answer")]
    public async Task<IActionResult> GetAnswerAll()
    {
        try
        {
            List<ReportAnswerInfoDto> answer = await _reportService.GetAnswerAllAsync();
            return Ok(answer);
        }
        catch (KeyNotFoundException e)
        {
            return StatusCode(500, new { error = "Internal server error" } );
        }
    }

    [HttpPost("answer")]
    public async Task<IActionResult> CreateAnswerAsync([FromBody] CreateReportAnswerDto dto)
    {
        try
        {
            ReportAnswerInfoDto answer = await _reportService.CreateAnswerAsync(dto);
            return Ok(answer);
        }
        catch (KeyNotFoundException e)
        {
            return StatusCode(500, new { error = "Internal server error" } );
        }
    }

    [HttpDelete("{reportAnswerId}")]
    public async Task<IActionResult> DeleteAnswerAsync(long reportAnswerId)
    {
        try
        {
            await _reportService.DeleteAnswerAsync(reportAnswerId);
            return NoContent();
        }
        catch (KeyNotFoundException e)
        {
            return StatusCode(500, new { error = "Internal server error" } );
        }
    }
}