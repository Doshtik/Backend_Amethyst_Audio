using Backend_Amethyst_Audio.Models.Entities;

namespace Backend_Amethyst_Audio.Services.Abstractions;

public interface IReportService
{
    Task<Report> GetByIdAsync(long id);
    Task<List<Report>> GetAllAsync();
    Task<List<Report>> GetAllTrackReportsAsync();
    Task<List<Report>> GetAllUserReportsAsync();
    
    Task CreateAsync(Report report);
    Task UpdateAsync(Report report);
    Task DeleteAsync(long id);

    Task ConfirmAsync(ReportAnswer answer);
    Task DeclineAsync(ReportAnswer answer);
}