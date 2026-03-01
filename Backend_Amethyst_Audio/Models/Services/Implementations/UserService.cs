using AutoMapper;
using Backend_Amethyst_Audio.DTO;
using Backend_Amethyst_Audio.Models.Data;
using Backend_Amethyst_Audio.Models.Entities;
using Backend_Amethyst_Audio.Services.Abstractions;

namespace Backend_Amethyst_Audio.Services.Implementations;

public class UserService(AppDbContext db, IMapper mapper) : IUserService
{
    public async Task<UserInfoDto> GetByIdAsync(long id)
    {
        User user = await db.Users.FindAsync(id);
        if (user == null)
            throw new Exception("Пользователь не найден");
        return mapper.Map<UserInfoDto>(user);
    }

    public async Task<List<UserInfoDto>> GetAllAsync()
    {
        List<UserInfoDto> userDto = new List<UserInfoDto>();
        List<User> users = db.Users.ToList();
        foreach (var user in users)
            userDto.Add(mapper.Map<UserInfoDto>(user));
        return userDto;
    }

    public async Task<UserInfoDto> CreateAsync(CreateUserDto dto)
    {
        User user = mapper.Map<User>(dto);
        user.PasswordHash = dto.Password; // Хэширование
        
        db.Users.Add(user);
        await db.SaveChangesAsync();
        
        return mapper.Map<UserInfoDto>(user);
    }

    public async Task UpdateAsync(long id, ChangeUserInfoDto dto)
    {
        User user = mapper.Map<User>(dto);
        await db.SaveChangesAsync();
    }

    public async Task ChangePasswordAsync(long userId, ChangeUserPasswordDto dto)
    {
        throw new NotImplementedException();
    }

    public async Task DeleteAsync(long id)
    {
        User user = await db.Users.FindAsync(id);
        if (user == null)
            throw new Exception("Пользователь не найден");
        db.Users.Remove(user);
        await db.SaveChangesAsync();
    }

    public async Task<UserInfoDto> GetByNicknameAsync(string nickname)
    {
        User user = db.Users.Where(u => u.Nickname == nickname).FirstOrDefault();
        if (user == null)
            throw new Exception("Пользователь не найден");
        return mapper.Map<UserInfoDto>(user);
    }

    public async Task<UserInfoDto> GetBySearchAsync(string search)
    {
        throw new NotImplementedException();
    }

    public async Task<int> GetListenersAmountAsync(long userId)
    {
        throw new NotImplementedException();
    }

    public async Task FollowAsync(FollowUserDto dto)
    {
        throw new NotImplementedException();
    }

    public async Task UnfollowAsync(FollowUserDto dto)
    {
        throw new NotImplementedException();
    }
}