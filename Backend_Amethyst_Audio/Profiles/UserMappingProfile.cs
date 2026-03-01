using AutoMapper;
using Backend_Amethyst_Audio.DTO;
using Backend_Amethyst_Audio.Models.Entities;

namespace Backend_Amethyst_Audio.Profiles;

public class UserMappingProfile : Profile
{
    public UserMappingProfile()
    {
        // Создание: из DTO в сущность
        CreateMap<CreateUserDto, User>();

        // Чтение: из сущности в DTO (для отображения в UI)
        CreateMap<User, UserInfoDto>();

        // Обновление: мапим данные из DTO в уже существующий объект User
        CreateMap<ChangeUserInfoDto, User>()
            .ForAllMembers(opts =>
                opts.Condition((src, dest, srcMember) => srcMember != null)); // мапить только не пустые поля
    }
}