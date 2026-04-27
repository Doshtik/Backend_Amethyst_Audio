using AutoMapper;
using Backend_Amethyst_Audio.DTO;
using Backend_Amethyst_Audio.Models.Entities;

namespace Backend_Amethyst_Audio.Profiles;

public class UserMappingProfile : Profile
{
    private readonly string _baseUrl = Environment.GetEnvironmentVariable("BASE_URL");
    public UserMappingProfile()
    {
        CreateMap<CreateUserDto, User>();
        
        CreateMap<User, UserInfoDto>()
            .ForMember(dest => dest.AvatarUrl, 
                opt => opt.MapFrom(src => 
                    string.IsNullOrEmpty(src.AvatarFileName) 
                        ? null 
                        : $"{_baseUrl}/users/avatars/{src.Id}"))
            .ForMember(dest => dest.HeaderUrl, 
                opt => opt.MapFrom(src => 
                    string.IsNullOrEmpty(src.HeaderFileName) 
                        ? null 
                        : $"{_baseUrl}/users/headers/{src.Id}"));

        CreateMap<ChangeUserInfoDto, User>()
            .ForMember(dest => dest.AvatarFileName, opt => opt.Ignore())
            .ForMember(dest => dest.HeaderFileName, opt => opt.Ignore())
            .ForAllMembers(opts => 
                opts.Condition((src, dest, srcMember) => srcMember != null));

        CreateMap<UsersHistory, UserHistoryDto>()
            .ForMember(dest => dest.User, opt => opt.MapFrom(src => src.IdUserNavigation))
            .ForMember(dest => dest.Track, opt => opt.MapFrom(src => src.IdTrackNavigation));
    }
}