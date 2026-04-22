using Backend_Amethyst_Audio.DTO.Reports;
using Backend_Amethyst_Audio.DTO.Reports.ReportAnswer;
using Backend_Amethyst_Audio.Models.Entities;

namespace Backend_Amethyst_Audio.Services.Abstractions;

public interface IReportService
{
    Task<ReportInfoDto> GetByIdAsync(long id);
    Task<List<ReportInfoDto>> GetAllAsync();
    Task<ReportInfoDto> CreateAsync(CreateReportDto report);
    Task DeleteAsync(long id);
    
    Task<ReportAnswerInfoDto> GetAnswerByIdAsync(long id);
    Task<List<ReportAnswerInfoDto>> GetAnswerAllAsync();
    Task<ReportAnswerInfoDto> CreateAnswerAsync(CreateReportAnswerDto report);
    Task DeleteAnswerAsync(long id);
}