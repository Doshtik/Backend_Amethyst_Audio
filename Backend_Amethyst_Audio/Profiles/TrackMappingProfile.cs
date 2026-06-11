using AutoMapper;
using Backend_Amethyst_Audio.DTO;
using Backend_Amethyst_Audio.Models.Entities;

namespace Backend_Amethyst_Audio.Profiles;

public class TrackMappingProfile : Profile
{
    private readonly  string _baseUrl = Environment.GetEnvironmentVariable("BASE_URL");
    public TrackMappingProfile()
    {
        CreateMap<CreateTrackDto, Track>()
            .ForMember(dest => dest.CoverFileName, opt => opt.Ignore())
            .ForMember(dest => dest.TrackFileName, opt => opt.Ignore())
            .ForMember(dest => dest.TracksAuthors, opt => opt.Ignore())
            .ForMember(dest => dest.IdPace, opt => opt.Ignore())
            .ForMember(dest => dest.IdMood, opt => opt.Ignore());
        
        CreateMap<Genre, GenreInfoDto>();
        
        CreateMap<Track, TrackInfoDto>()
            .ForMember(dest => dest.UserList,
                opt => opt.MapFrom(src => 
                    src.TracksAuthors.Select(ta => ta.IdAuthorNavigation)))
            .ForMember(dest => dest.GenreList, opt => opt.MapFrom(src => 
                src.TracksGenres != null 
                    ? src.TracksGenres.Select(tg => tg.IdGenreNavigation).ToList() // Извлекаем Genre из промежуточной таблицы
                    : new List<Genre>()
            ))
            .ForMember(dest => dest.PaceName, 
                opt => opt.MapFrom(src => src.IdPaceNavigation != null ? src.IdPaceNavigation.PaceName : null))
            .ForMember(dest => dest.MoodName, 
                opt => opt.MapFrom(src => src.IdMoodNavigation != null ? src.IdMoodNavigation.MoodName : null))
            .ForMember(dest => dest.IsTextless, 
                opt => opt.MapFrom(src => src.IsTextless)) 
            .ForMember(dest => dest.CoverUrl, 
                opt => opt.MapFrom(src => 
                    string.IsNullOrEmpty(src.CoverFileName) 
                        ? null 
                        : $"{_baseUrl}/api/media/tracks/covers/{src.Id}"))
            .ForMember(dest => dest.TrackUrl, 
                opt => opt.MapFrom(src => 
                    string.IsNullOrEmpty(src.TrackFileName) 
                        ? null 
                        : $"{_baseUrl}/api/media/tracks/audio/{src.Id}"));
        
        CreateMap<ChangeTrackInfoDto, Track>()
            .ForMember(dest => dest.CoverFileName, opt => opt.Ignore())
            .ForMember(dest => dest.TrackFileName, opt => opt.Ignore())
            .ForMember(dest => dest.IdPace, opt => opt.Ignore())
            .ForMember(dest => dest.IdMood, opt => opt.Ignore())
            .ForAllMembers(opts => 
                opts.Condition((src, dest, srcMember) => srcMember != null));
    }
}