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
            throw new Exception("User not found");
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
        user.PasswordHash = HashPassword(dto.Password);

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
            throw new Exception("User not found");
        db.Users.Remove(user);
        await db.SaveChangesAsync();
    }

    public async Task<UserInfoDto> GetByNicknameAsync(string nickname)
    {
        User? user = db.Users.FirstOrDefault(u => u.Nickname == nickname);
        if (user == null)
            throw new Exception("User not found");
        return mapper.Map<UserInfoDto>(user);
    }

    public async Task<int> GetListenersAmountAsync(long userId)
    {
        int count = db.UsersSubs.Count(x => x.IdUser == userId);
        return count;
    }

    public async Task FollowAsync(FollowUserDto dto)
    {
        UsersSub? sub = db.UsersSubs.FirstOrDefault(x => 
            x.IdUser == dto.IdTargetUser && 
            x.IdSubscriber == dto.IdSubscriber);

        if (sub != null)
            throw new Exception("Subscription already exists");
        
        sub = new UsersSub()
        {
            IdUser = dto.IdTargetUser,
            IdSubscriber = dto.IdSubscriber
        };
        await db.UsersSubs.AddAsync(sub);
        await db.SaveChangesAsync();
    }

    public async Task UnfollowAsync(FollowUserDto dto)
    {
        UsersSub? sub = db.UsersSubs.FirstOrDefault(x => 
            x.IdUser == dto.IdTargetUser && 
            x.IdSubscriber == dto.IdSubscriber);

        if (sub == null)
            throw new Exception("Subscription doesn't exists");
        
        db.UsersSubs.Remove(sub);
        await db.SaveChangesAsync();
    }

    public Task<UserInfoDto> GetLoginAsync(LoginDto dto)
    {
        User? user = db.Users
            .FirstOrDefault(x => 
                x.Nickname == dto.Nickname || 
                x.Email == dto.Email && 
                x.PasswordHash == HashPassword(dto.Password));
        if (user == null)
            throw new KeyNotFoundException();
        return Task.FromResult(mapper.Map<UserInfoDto>(user));
    }

    private string HashPassword(string password)
    {
        return password;
    }
}