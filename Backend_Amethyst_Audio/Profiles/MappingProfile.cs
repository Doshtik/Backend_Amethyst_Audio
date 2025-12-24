using AutoMapper;
using Backend_Amethyst_Audio.DTO;
using Backend_Amethyst_Audio.Models;

namespace Backend_Amethyst_Audio.Profiles;

public class MappingProfile : Profile
{
    public MappingProfile()
    {
        CreateMap<UserCreateDTO, User>();
        CreateMap<User, UserReadDTO>();
    }
}