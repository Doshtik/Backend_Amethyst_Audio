using AutoMapper;
using Backend_Amethyst_Audio.DTO;
using Backend_Amethyst_Audio.Models.Entities;

namespace Backend_Amethyst_Audio.Profiles;

public class UserMappingProfile : Profile
{
    private readonly string _baseUrl;
    public UserMappingProfile()
    {
        // Создание: из DTO в сущность
        CreateMap<CreateUserDto, User>();

        // Чтение: из сущности в DTO
        CreateMap<User, UserInfoDto>()
            .ForMember(dest => dest.AvatarUrl, 
                opt => opt.MapFrom(src => $"{_baseUrl}/users/avatars/{src.Id}"))
            .ForMember(dest => dest.HeaderUrl, 
                opt => opt.MapFrom(src => $"{_baseUrl}/users/headers/{src.Id}"));

        // Обновление: мапим данные из DTO в уже существующий объект User
        CreateMap<ChangeUserInfoDto, User>()
            .ForAllMembers(opts =>
                opts.Condition((src, dest, srcMember) => srcMember != null)); // мапить только не пустые поля
    }
}