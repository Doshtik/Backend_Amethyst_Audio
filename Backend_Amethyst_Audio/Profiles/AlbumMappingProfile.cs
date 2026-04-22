using AutoMapper;
using Backend_Amethyst_Audio.DTO;
using Backend_Amethyst_Audio.Models.Entities;

namespace Backend_Amethyst_Audio.Profiles;

public class AlbumMappingProfile : Profile
{
    private readonly string _baseUrl = Environment.GetEnvironmentVariable("BASE_URL");

    public AlbumMappingProfile()
    {
        CreateMap<CreateAlbumDto, Album>()
            .ForMember(dest => dest.CoverFileName, opt => opt.Ignore())
            .ForMember(dest => dest.CreatedAt, opt => opt.Ignore())
            .ForMember(dest => dest.UpdatedAt, opt => opt.Ignore());

        CreateMap<Album, AlbumInfoDto>()
            .ForMember(dest => dest.CoverUrl, opt => opt.MapFrom(src =>
                string.IsNullOrEmpty(src.CoverFileName) 
                    ? null 
                    : $"{_baseUrl}/albums/covers/{src.Id}"))
            .ForMember(dest => dest.TrackList, opt => opt.MapFrom(src =>
                src.AlbumsTracks.Select(at => at.IdTrackNavigation)));

        CreateMap<ChangeAlbumInfoDto, Album>()
            .ForMember(dest => dest.CoverFileName, opt => opt.Ignore())
            .ForAllMembers(opts => opts.Condition((src, dest, srcMember) => srcMember != null));
    }
}