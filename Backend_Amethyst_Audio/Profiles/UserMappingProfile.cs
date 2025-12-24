using AutoMapper;
using Backend_Amethyst_Audio.DTO;
using Backend_Amethyst_Audio.Models;

namespace Backend_Amethyst_Audio.Profiles;

public class UserMappingProfile : Profile
{
    public UserMappingProfile()
    {
        CreateMap<UserCreateDTO, User>();
        CreateMap<User, UserReadDTO>();
    }
}