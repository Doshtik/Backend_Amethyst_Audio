using AutoMapper;
using Backend_Amethyst_Audio.Data;
using Backend_Amethyst_Audio.DTO.Reports;
using Backend_Amethyst_Audio.DTO.Reports.ReportAnswer;
using Backend_Amethyst_Audio.Entities;
using Backend_Amethyst_Audio.Services.Abstractions;
using Microsoft.EntityFrameworkCore;

namespace Backend_Amethyst_Audio.Services.Implementations;

public class ReportService : IReportService
{
    private readonly AppDbContext _db;
    private readonly IMapper _mapper;
    private readonly ILogger<ReportService> _logger;
    
    public ReportService(
        AppDbContext db, 
        IMapper mapper, 
        ILogger<ReportService> logger)
    {
        _db = db;
        _mapper = mapper;
        _logger = logger;
    }
    
    public async Task<ReportInfoDto> GetByIdAsync(long id)
    {
        _logger.LogInformation("[Info] Getting report for id {id}", id);
        Report? report = await _db.Reports.FirstOrDefaultAsync(x => x.Id == id);

        if (report is null)
        {
            _logger.LogError("[Error] Report for id {id} was not found", id);
            throw new KeyNotFoundException($"Report with id {id} not found");
        }
        
        return _mapper.Map<ReportInfoDto>(report);
    }

    public async Task<List<ReportInfoDto>> GetAllAsync()
    {
        _logger.LogInformation("[Info] Getting all reports");
        List<Report>? reports = await _db.Reports.ToListAsync();
        
        return _mapper.Map<List<ReportInfoDto>>(reports);
    }

    public async Task<ReportInfoDto> CreateAsync(CreateReportDto dto)
    {
        _logger.LogInformation("[Info] Creating report");
        Report report = _mapper.Map<Report>(dto);
        report.CreatedAt = DateTime.Now;
        
        await _db.Reports.AddAsync(report);
        await _db.SaveChangesAsync();
        
        return _mapper.Map<ReportInfoDto>(report);
    }

    public async Task DeleteAsync(long id)
    {
        _logger.LogInformation("[Info] Deleting report");
        Report? report = await _db.Reports.FirstOrDefaultAsync(x => x.Id == id);
        
        if (report is null)
        {
            _logger.LogError("[Error] Report for id {id} was not found", id);
            throw new KeyNotFoundException($"Report with id {id} not found");
        }
        
        _db.Reports.Remove(report);
        await _db.SaveChangesAsync();
    }

    public async Task<ReportAnswerInfoDto> GetAnswerByIdAsync(long id)
    {
        _logger.LogInformation("[Info] Getting report answer for id {id}", id);
        ReportAnswer? answer = await _db.ReportAnswers.FirstOrDefaultAsync(x => x.Id == id);

        if (answer is null)
        {
            _logger.LogError("[Error] Answer for id {id} was not found", id);
            throw new KeyNotFoundException($"Answer with id {id} not found");
        }
        
        return _mapper.Map<ReportAnswerInfoDto>(answer);
    }

    public async Task<List<ReportAnswerInfoDto>> GetAnswerAllAsync()
    {
        _logger.LogInformation("[Info] Getting all answers");
        List<ReportAnswer>? answers = await _db.ReportAnswers.ToListAsync();
        
        return _mapper.Map<List<ReportAnswerInfoDto>>(answers);
    }

    public async Task<ReportAnswerInfoDto> CreateAnswerAsync(CreateReportAnswerDto report)
    {
        _logger.LogInformation("[Info] Creating report answer");
        ReportAnswer answer = _mapper.Map<ReportAnswer>(report);
        answer.CreatedAt = DateTime.Now;
        
        await _db.ReportAnswers.AddAsync(answer);
        await _db.SaveChangesAsync();
        
        return _mapper.Map<ReportAnswerInfoDto>(answer);
    }

    public async Task DeleteAnswerAsync(long id)
    {
        _logger.LogInformation("[Info] Deleting report");
        ReportAnswer? answer = await _db.ReportAnswers.FirstOrDefaultAsync(x => x.Id == id);

        if (answer is null)
        {
            _logger.LogError("[Error] Answer for id {id} was not found", id);
            throw new KeyNotFoundException($"Answer with id {id} not found");
        }
        
        _db.ReportAnswers.Remove(answer);
        await _db.SaveChangesAsync();
    }
}