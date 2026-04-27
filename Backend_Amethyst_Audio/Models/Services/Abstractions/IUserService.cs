using Backend_Amethyst_Audio.DTO;
using Backend_Amethyst_Audio.Models;

namespace Backend_Amethyst_Audio.Services.Abstractions;

public interface IUserService
{
    Task<UserInfoDto> GetByIdAsync(long id);
    Task<List<UserInfoDto>> GetAllAsync();
    Task<UserInfoDto> CreateAsync(CreateUserDto dto);
    Task<UserInfoDto> LoginAsync(LoginDto dto);
    Task<UserInfoDto> ExternalLoginAsync(ExternalLoginDto dto);
    Task LogoutAsync(long userId);
    Task UpdateAsync(long id, ChangeUserInfoDto dto);
    Task ChangePasswordAsync(long id, ChangeUserPasswordDto dto);
    Task DeleteAsync(long id);
    
    Task<List<UserHistoryDto>> GetUserHistoryAsync(long userId);
    Task AddToHistoryAsync(long userId, long trackId);
    Task UpdateListeningTimeAsync(long userId, long trackId, int seconds);
    
    Task<List<UserInfoDto>> GetListByNicknameAsync(string nickname);
    
    Task<int> GetSubscriberAmountAsync(long id);
    
    Task FollowAsync(FollowUserDto dto);
    Task UnfollowAsync(FollowUserDto dto);
}