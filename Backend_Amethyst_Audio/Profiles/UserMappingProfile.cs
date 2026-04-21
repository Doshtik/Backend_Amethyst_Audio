using AutoMapper;
using Backend_Amethyst_Audio.DTO;
using Backend_Amethyst_Audio.Models.Entities;

namespace Backend_Amethyst_Audio.Profiles;

public class UserMappingProfile : Profile
{
    private readonly string _baseUrl = Environment.GetEnvironmentVariable("BASE_URL");
    public UserMappingProfile()
    {
        // Create: From DTO to Entity
        CreateMap<CreateUserDto, User>();
        
        // Read: From Entity to DTO
        CreateMap<User, UserInfoDto>()
            .ForMember(dest => dest.AvatarUrl, 
                opt => opt.MapFrom(src => $"{_baseUrl}/users/avatars/{src.Id}"))
            .ForMember(dest => dest.HeaderUrl, 
                opt => opt.MapFrom(src => $"{_baseUrl}/users/headers/{src.Id}"));

        // Update: we are mapping data from DTO in already existing object User
        CreateMap<ChangeUserInfoDto, User>()
            .ForAllMembers(opts =>
                opts.Condition((src, dest, srcMember) => srcMember != null)); // мапить только не пустые поля
    }
}