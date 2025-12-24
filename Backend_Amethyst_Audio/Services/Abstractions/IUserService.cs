using Backend_Amethyst_Audio.DTO;
using Backend_Amethyst_Audio.Models;

namespace Backend_Amethyst_Audio.Services.Abstractions;

public interface IUserService
{
    Task<UserReadDTO> GetByIdAsync(long id);
    Task<List<UserReadDTO>> GetAllAsync();
    
    Task<UserReadDTO> CreateAsync(UserCreateDTO user);
    Task UpdateAsync(User user);
    Task DeleteAsync(long id);
    
    Task<UserReadDTO> GetByEmailAsync(string email, string password);
    Task<UserReadDTO> GetByNicknameAsync(string nickname, string password);
    Task<UserReadDTO> GetBySearchAsync(string search);
    
    Task<int> GetListenersAmountAsync(long userId);
    
    Task SubscribeAsync(long userId, long subscriberId);
    Task UnsubscribeAsync(long userId, long subscriberId);
}