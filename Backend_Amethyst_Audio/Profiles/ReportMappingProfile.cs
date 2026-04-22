using AutoMapper;
using Backend_Amethyst_Audio.DTO.Reports;
using Backend_Amethyst_Audio.DTO.Reports.ReportAnswer;
using Backend_Amethyst_Audio.Models.Entities;

namespace Backend_Amethyst_Audio.Profiles;

public class ReportMappingProfile : Profile
{
    public ReportMappingProfile()
    {
        CreateMap<CreateReportDto, Report>();
        CreateMap<CreateReportAnswerDto, ReportAnswer>();

        CreateMap<Report, ReportInfoDto>();
        CreateMap<ReportAnswer, ReportAnswerInfoDto>();
    }
}