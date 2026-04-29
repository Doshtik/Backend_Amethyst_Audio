using AutoMapper;
using Backend_Amethyst_Audio.DTO;
using Backend_Amethyst_Audio.Models.Entities;

namespace Backend_Amethyst_Audio.Profiles;

public class AlbumMappingProfile : Profile
{
    private readonly string _baseUrl = Environment.GetEnvironmentVariable("BASE_URL");

    public AlbumMappingProfile()
    {
        // 1. Создание альбома
        CreateMap<CreateAlbumDto, Album>()
            .ForMember(dest => dest.Name, opt => opt.MapFrom(src => src.Name))
            .ForMember(dest => dest.CoverFileName, opt => opt.Ignore())
            .ForMember(dest => dest.CreatedAt, opt => opt.Ignore())
            .ForMember(dest => dest.UpdatedAt, opt => opt.Ignore());

        // 2. Получение информации об альбоме
        CreateMap<Album, AlbumInfoDto>()
            .ForMember(dest => dest.CoverUrl, opt => opt.MapFrom(src =>
                string.IsNullOrEmpty(src.CoverFileName) 
                    ? null 
                    : $"{_baseUrl}/albums/covers/{src.Id}"))
            .ForMember(dest => dest.AuthorList, opt => opt.MapFrom(src => 
                src.AlbumsAuthors
                    .Select(a => a.IdAuthorNavigation)
                    .ToList() ?? new()))
            .ForMember(dest => dest.TrackList, opt => opt.MapFrom(src => 
                src.AlbumsTracks
                    .Select(t => t.IdTrackNavigation)
                    .ToList() ?? new()));

        // 3. Изменение информации об альбоме (частичное обновление)
        CreateMap<ChangeAlbumInfoDto, Album>()
            .ForMember(dest => dest.Name, opt => opt.MapFrom(src => src.Name))
            .ForMember(dest => dest.CoverFileName, opt => opt.Ignore())
            .ForAllMembers(opts => opts.Condition((src, dest, srcMember) => srcMember != null));
    }
}