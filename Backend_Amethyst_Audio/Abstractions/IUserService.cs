using Backend_Amethyst_Audio.Models;

namespace Backend_Amethyst_Audio.Abstractions;

public interface IUserService
{
    Task<User> GetByIdAsync(long id);
    Task<List<User>> GetAllAsync();
    
    Task CreateAsync(User user);
    Task UpdateAsync(User user);
    Task DeleteAsync(long id);
    
    Task<User> GetByEmailAsync(string email, string password);
    Task<User> GetByNicknameAsync(string nickname, string password);
    Task<User> GetBySearchAsync(string search);
    
    Task<int> GetListenersAmountAsync(long userId);
    
    Task SubscribeAsync(long userId, long subscriberId);
    Task UnsubscribeAsync(long userId, long subscriberId);
}