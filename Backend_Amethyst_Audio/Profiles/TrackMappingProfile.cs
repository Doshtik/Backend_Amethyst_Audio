using AutoMapper;
using Backend_Amethyst_Audio.DTO;
using Backend_Amethyst_Audio.Models.Entities;

namespace Backend_Amethyst_Audio.Profiles;

public class TrackMappingProfile : Profile
{
    private readonly  string _baseUrl;
    public TrackMappingProfile()
    {
        CreateMap<CreateTrackDto, Track>();
        
        CreateMap<Track, TrackInfoDto>()
            .ForMember(dest => dest.TrackUrl, 
                opt => opt.MapFrom(src => $"{_baseUrl}/audio/{src.Id}"))
            .ForMember(dest => dest.CoverUrl, 
                opt => opt.MapFrom(src => $"{_baseUrl}/cover/{src.Id}"));
    }
}