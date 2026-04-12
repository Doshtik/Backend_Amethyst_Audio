using AutoMapper;
using Backend_Amethyst_Audio.DTO;
using Backend_Amethyst_Audio.Models.Entities;

namespace Backend_Amethyst_Audio.Profiles;

public class TrackMappingProfile : Profile
{
    private readonly  string _baseUrl = Environment.GetEnvironmentVariable("BASE_URL");
    public TrackMappingProfile()
    {
        CreateMap<CreateTrackDto, Track>();
        
        CreateMap<Track, TrackInfoDto>()
            .ForMember(dest => dest.UserList,
                opt => opt.MapFrom(src => src.TracksAuthors.Select(ta => ta.IdAuthorNavigation)))
            .ForMember(dest => dest.TrackUrl, 
                opt => opt.MapFrom(src => $"{_baseUrl}/Audio/{src.Id}"))
            .ForMember(dest => dest.CoverUrl, 
                opt => opt.MapFrom(src => $"{_baseUrl}/Cover/{src.Id}"));
    }
}