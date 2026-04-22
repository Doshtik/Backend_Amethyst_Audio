using AutoMapper;
using Backend_Amethyst_Audio.DTO;
using Backend_Amethyst_Audio.Models.Entities;

namespace Backend_Amethyst_Audio.Profiles;

public class PlaylistMappingProfile : Profile
{
    private readonly string _baseUrl = Environment.GetEnvironmentVariable("BASE_URL");

    public PlaylistMappingProfile()
    {
        CreateMap<CreatePlaylistDto, Playlist>()
            .ForMember(dest => dest.CoverFileName, opt => opt.Ignore())
            .ForMember(dest => dest.IdUser, opt => opt.Ignore()) // Проставляется в сервисе из Claims
            .ForMember(dest => dest.CreatedAt, opt => opt.Ignore())
            .ForMember(dest => dest.UpdatedAt, opt => opt.Ignore()); 

        CreateMap<Playlist, PlaylistInfoDto>()
            .ForMember(dest => dest.OwnerId, opt => opt.MapFrom(src => src.IdUser))
            .ForMember(dest => dest.CoverUrl, opt => opt.MapFrom(src =>
                string.IsNullOrEmpty(src.CoverFileName) 
                    ? null 
                    : $"{_baseUrl}/playlists/covers/{src.Id}"))
            .ForMember(dest => dest.TrackList, opt => opt.MapFrom(src =>
                src.PlaylistsTracks.Select(pt => pt.IdTrackNavigation)));

        CreateMap<ChangePlaylistInfoDto, Playlist>()
            .ForMember(dest => dest.CoverFileName, opt => opt.Ignore())
            .ForMember(dest => dest.IdUser, opt => opt.Ignore())
            .ForMember(dest => dest.CreatedAt, opt => opt.Ignore())
            .ForMember(dest => dest.UpdatedAt, opt => opt.Ignore())
            .ForAllMembers(opts => opts.Condition((src, dest, srcMember) => srcMember != null));
    }
}