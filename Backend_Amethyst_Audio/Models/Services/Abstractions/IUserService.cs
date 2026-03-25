using Backend_Amethyst_Audio.DTO;
using Backend_Amethyst_Audio.Models;

namespace Backend_Amethyst_Audio.Services.Abstractions;

public interface IUserService
{
    Task<UserInfoDto> GetByIdAsync(long id);
    Task<List<UserInfoDto>> GetAllAsync();
    
    Task<UserInfoDto> CreateAsync(CreateUserDto dto);
    Task UpdateAsync(long id, ChangeUserInfoDto dto);
    Task ChangePasswordAsync(long id, ChangeUserPasswordDto dto);
    Task DeleteAsync(long id);
    
    Task<UserInfoDto> GetByNicknameAsync(string nickname);
    
    Task<int> GetListenersAmountAsync(long id);
    
    Task FollowAsync(FollowUserDto dto);
    Task UnfollowAsync(FollowUserDto dto);
    Task<UserInfoDto> GetLoginAsync(LoginDto dto);
}