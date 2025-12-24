using AutoMapper;
using Backend_Amethyst_Audio.Data;
using Backend_Amethyst_Audio.DTO;
using Backend_Amethyst_Audio.Models;
using Backend_Amethyst_Audio.Services.Abstractions;

namespace Backend_Amethyst_Audio.Services.Impementations;

public class UserService(AppDbContext db, IMapper mapper) : IUserService
{
    public Task<UserReadDTO> GetByIdAsync(long id)
    {
        throw new NotImplementedException();
    }

    public Task<List<UserReadDTO>> GetAllAsync()
    {
        throw new NotImplementedException();
    }

    public async Task<UserReadDTO> CreateAsync(UserCreateDTO dto)
    {
        var user = mapper.Map<User>(dto);
        user.PasswordHash = dto.Password; // Логика
        
        db.Users.Add(user);
        await db.SaveChangesAsync();
        
        return mapper.Map<UserReadDTO>(user);
    }

    public Task UpdateAsync(User user)
    {
        throw new NotImplementedException();
    }

    public Task DeleteAsync(long id)
    {
        throw new NotImplementedException();
    }

    public Task<UserReadDTO> GetByEmailAsync(string email, string password)
    {
        throw new NotImplementedException();
    }

    public Task<UserReadDTO> GetByNicknameAsync(string nickname, string password)
    {
        throw new NotImplementedException();
    }

    public Task<UserReadDTO> GetBySearchAsync(string search)
    {
        throw new NotImplementedException();
    }

    public Task<int> GetListenersAmountAsync(long userId)
    {
        throw new NotImplementedException();
    }

    public Task SubscribeAsync(long userId, long subscriberId)
    {
        throw new NotImplementedException();
    }

    public Task UnsubscribeAsync(long userId, long subscriberId)
    {
        throw new NotImplementedException();
    }
}