using AutoMapper;
using Backend_Amethyst_Audio.DTO;
using Backend_Amethyst_Audio.Models.Data;
using Backend_Amethyst_Audio.Models.Entities;
using Backend_Amethyst_Audio.Services.Abstractions;
using Microsoft.AspNetCore.Identity;
using Microsoft.EntityFrameworkCore;

namespace Backend_Amethyst_Audio.Services.Implementations;

public class UserService : IUserService
{
    private readonly AppDbContext _db;
    private readonly ITokenService _tokenService;
    private readonly IMapper _mapper;
    private readonly ILogger<UserService> _logger;
    private IUserService _userServiceImplementation;

    public UserService(
        AppDbContext db, 
        ITokenService tokenService, 
        IMapper mapper, 
        ILogger<UserService> logger)
    {
        _db = db;
        _tokenService = tokenService;
        _mapper = mapper;
        _logger = logger;
    }

    public async Task<UserInfoDto> GetByIdAsync(long id)
    {
        _logger.LogDebug("[Debug] Get user by Id. UserId={UserId}", id);
        
        User user = await _db.Users.FindAsync(id);
        if (user == null)
        {
            _logger.LogWarning("[Warn] User not found. UserId={UserId}", id);
            throw new KeyNotFoundException("User not found");
        }
        
        _logger.LogDebug("[Debug] User retrieved successfully. UserId={UserId}", id);
        return _mapper.Map<UserInfoDto>(user);
    }

    public async Task<List<UserInfoDto>> GetAllAsync()
    {
        _logger.LogDebug("[Debug] Get all users request");
        
        var users = await _db.Users.AsNoTracking().ToListAsync();
        var result = _mapper.Map<List<UserInfoDto>>(users);
        
        _logger.LogInformation("[Info] Retrieved {Count} users", result.Count);
        return result;
    }

    public async Task<UserInfoDto> CreateAsync(CreateUserDto dto)
    {
        _logger.LogInformation("[Info] Creating new user. Email={Email}", dto.Email);
        
        if (await _db.Users.AnyAsync(u => u.Email == dto.Email))
        {
            _logger.LogWarning("[Warn] Duplicate email detected. Email={Email}", dto.Email);
            throw new BadHttpRequestException("User with this email already exists");
        }

        // Username validation
        if (!string.IsNullOrWhiteSpace(dto.Nickname) && await _db.Users.AnyAsync(u => u.Nickname == dto.Nickname))
        {
            _logger.LogWarning("[Warn] Duplicate nickname detected. Nickname={Nickname}", dto.Nickname);
            throw new BadHttpRequestException("Nickname is already taken");
        }

        User user = _mapper.Map<User>(dto);
        var hasher = new PasswordHasher<User>();
        user.PasswordHash = hasher.HashPassword(user, dto.Password);

        _db.Users.Add(user);
        await _db.SaveChangesAsync();
        _db.Libraries.Add(new Library { IdUser = user.Id });
        await _db.SaveChangesAsync();
        
        _logger.LogInformation("[Info] User created. UserId={UserId}, Email={Email}", user.Id, user.Email);

        UserInfoDto userDto = _mapper.Map<UserInfoDto>(user);
        userDto.Token = _tokenService.GenerateJwtToken(user);
        
        return userDto;
    }

    public async Task<UserInfoDto> LoginAsync(LoginDto dto)
    {
        _logger.LogDebug("[Debug] Authentication attempt. Email={Email}", dto.Email);
        
        var user = await _db.Users.FirstOrDefaultAsync(x => x.Email == dto.Email);

        if (user == null)
        {
            _logger.LogWarning("[Warn] User not found. Email={Email}", dto.Email);
            throw new KeyNotFoundException("User not found");
        }

        var hasher = new PasswordHasher<User>();
        var verificationResult = hasher.VerifyHashedPassword(user, user.PasswordHash, dto.Password);

        if (verificationResult == PasswordVerificationResult.Failed)
        {
            _logger.LogWarning("[Warn] Invalid password. Email={Email}", dto.Email);
            throw new UnauthorizedAccessException("Invalid password");
        }

        _logger.LogInformation("[Info] Authentication successful. UserId={UserId}, Email={Email}", 
            user.Id, dto.Email);
        
        UserInfoDto userDto = _mapper.Map<UserInfoDto>(user);
        userDto.Token = _tokenService.GenerateJwtToken(user);
        return userDto;
    }

    public async Task<UserInfoDto> ExternalLoginAsync(ExternalLoginDto dto)
    {
        _logger.LogInformation("[Info] Processing external login. Provider={Provider}", dto.Provider);

        // Token validation
        var externalData = await _tokenService.ValidateExternalTokenAsync(dto.Provider, dto.Token);

        await using var transaction = await _db.Database.BeginTransactionAsync();
        
        try
        {
            // Get Entity from DB
            var authUser = await _db.OauthUsers
                .Include(au => au.IdUser)
                .Include(au => au.IdProvider)
                .FirstOrDefaultAsync(au => au.IdProviderNavigation.ProviderName == dto.Provider && 
                                           au.ExternalId == externalData.ExternalId);

            User user;
            if (authUser == null)
            {
                // Creating a new User
                user = new User
                {
                    Nickname = externalData.DisplayName ?? $"user_{Guid.NewGuid().ToString()[..8]}",
                    Email = externalData.Email ?? $"ext_{dto.Provider}_{externalData.ExternalId}@local"
                };
                
                var randomPassword = GenerateSecurePassword();
                var hasher = new PasswordHasher<User>();
                user.PasswordHash = hasher.HashPassword(user, randomPassword);
                
                _db.Users.Add(user);
                await _db.SaveChangesAsync();

                // Check provider existing
                var provider = await _db.OauthProviders
                    .FirstOrDefaultAsync(p => p.ProviderName == dto.Provider)
                    ?? throw new InvalidOperationException($"Provider {dto.Provider} is not registered in DB");

                // Creating a new AuthUser
                authUser = new AuthUser
                {
                    IdUser = user.Id,
                    IdProvider = provider.Id,
                    ExternalId = externalData.ExternalId
                };
                
                _db.OauthUsers.Add(authUser);
                await _db.SaveChangesAsync();
            }
            else
            {
                user = authUser.IdUserNavigation;
            }

            await transaction.CommitAsync();

            // 4. Маппинг и генерация внутреннего JWT
            var result = _mapper.Map<UserInfoDto>(user);
            result.Token = _tokenService.GenerateJwtToken(user);
            return result;
        }
        catch
        {
            await transaction.RollbackAsync();
            throw;
        }
    }
    
    private static string GenerateSecurePassword(int length = 24)
    {
        const string chars = "abcdefghijklmnopqrstuvwxyzABCDEFGHIJKLMNOPQRSTUVWXYZ1234567890-_";
        var result = new char[length];
        using var rng = System.Security.Cryptography.RandomNumberGenerator.Create();
        var buffer = new byte[length];
        rng.GetBytes(buffer);
    
        for (int i = 0; i < length; i++)
            result[i] = chars[buffer[i] % chars.Length];
        
        return new string(result);
    }

    public async Task LogoutAsync(long userId)
    {
        return;
    }

    public async Task UpdateAsync(long id, ChangeUserInfoDto dto)
    {
        _logger.LogInformation("[Info] Updating user data. UserId={UserId}", id);
        
        var user = await _db.Users.FindAsync(id);
        if (user is null)
        {
            _logger.LogWarning("[Warn] User not found for update. UserId={UserId}", id);
            throw new KeyNotFoundException("User not found");
        }

        // Nickname validation
        if (!string.IsNullOrWhiteSpace(dto.Nickname) && dto.Nickname != user.Nickname)
        {
            if (await _db.Users.AnyAsync(u => u.Nickname == dto.Nickname && u.Id != id))
            {
                _logger.LogWarning("[Warn] Nickname already taken. Nickname={Nickname}, UserId={UserId}", 
                    dto.Nickname, id);
                throw new ArgumentException("Nickname is already taken");
            }
        }

        // Email validation
        if (!string.IsNullOrWhiteSpace(dto.Email) && dto.Email != user.Email)
        {
            if (await _db.Users.AnyAsync(u => u.Email == dto.Email && u.Id != id))
            {
                _logger.LogWarning("[Warn] Email already registered. Email={Email}, UserId={UserId}", 
                    dto.Email, id);
                throw new ArgumentException("Email is already registered");
            }
        }
        
        _mapper.Map(dto, user);
        await _db.SaveChangesAsync();
        
        _logger.LogInformation("[Info] User data updated. UserId={UserId}", id);
    }

    public async Task ChangePasswordAsync(long userId, ChangeUserPasswordDto dto)
    {
        _logger.LogInformation("[Info] Password change request. UserId={UserId}", userId);
        
        var user = await _db.Users.FindAsync(userId);
        if (user == null)
        {
            _logger.LogWarning("[Warn] User not found for password change. UserId={UserId}", userId);
            throw new KeyNotFoundException("User not found");
        }

        // Password validation
        var hasher = new PasswordHasher<User>();
        var verificationResult = hasher.VerifyHashedPassword(user, user.PasswordHash, dto.OldPassword);

        if (verificationResult == PasswordVerificationResult.Failed)
        {
            _logger.LogWarning("[Warn] Invalid current password. UserId={UserId}", userId);
            throw new UnauthorizedAccessException("Invalid current password");
        }

        // New password validation
        if (string.IsNullOrWhiteSpace(dto.NewPassword) || dto.NewPassword.Length < 8)
        {
            _logger.LogWarning("[Warn] New password too weak. UserId={UserId}", userId);
            throw new ArgumentException("Password must be at least 8 characters");
        }

        user.PasswordHash = hasher.HashPassword(user, dto.NewPassword);
        await _db.SaveChangesAsync();
        
        _logger.LogInformation("[Info] Password changed successfully. UserId={UserId}", userId);
    }

    public async Task DeleteAsync(long id)
    {
        _logger.LogInformation("[Info] Deleting user. UserId={UserId}", id);
        
        User user = await _db.Users.FindAsync(id);
        if (user == null)
        {
            _logger.LogWarning("[Warn] User not found for deletion. UserId={UserId}", id);
            throw new KeyNotFoundException("User not found");
        }
        
        _db.Users.Remove(user);
        await _db.SaveChangesAsync();
        
        _logger.LogInformation("[Info] User deleted. UserId={UserId}", id);
    }

    public async Task<List<UserInfoDto>> GetListByNicknameAsync(string nickname)
    {
        _logger.LogDebug("[Debug] Get user list by nickname. Nickname={Nickname}", nickname);
        
        List<User> users = await _db.Users
            .Where(u => u.Nickname == nickname)
            .ToListAsync();
        
        if (users.Count is 0)
        {
            _logger.LogWarning("[Warn] Users not found by nickname. Nickname={Nickname}", nickname);
            throw new KeyNotFoundException("Users not found");
        }
        
        return _mapper.Map<List<UserInfoDto>>(users);
    }

    public async Task<int> GetListenersAmountAsync(long userId)
    {
        _logger.LogDebug("[Debug] Get followers count. UserId={UserId}", userId);
        
        int count = await _db.UsersSubs.CountAsync(x => x.IdUser == userId);
        _logger.LogDebug("[Debug] Followers count: {Count}. UserId={UserId}", count, userId);
        return count;
    }

    public async Task FollowAsync(FollowUserDto dto)
    {
        _logger.LogInformation("[Info] Follow attempt. SubscriberId={SubscriberId} → TargetUserId={TargetUserId}", 
            dto.IdSubscriber, dto.IdTargetUser);
        
        // Check: User cannot follow by himself
        if (dto.IdSubscriber == dto.IdTargetUser)
        {
            _logger.LogWarning("[Warn] Self-follow attempt blocked. UserId={UserId}", dto.IdSubscriber);
            throw new ArgumentException("Cannot follow yourself");
        }
        
        var sub = await _db.UsersSubs.FirstOrDefaultAsync(x => 
            x.IdUser == dto.IdTargetUser && 
            x.IdSubscriber == dto.IdSubscriber);

        if (sub != null)
        {
            _logger.LogWarning("[Warn] Subscription already exists. SubscriberId={SubscriberId} → TargetUserId={TargetUserId}", 
                dto.IdSubscriber, dto.IdTargetUser);
            throw new InvalidOperationException("Subscription already exists");
        }
        
        sub = new UsersSub()
        {
            IdUser = dto.IdTargetUser,
            IdSubscriber = dto.IdSubscriber
        };
        await _db.UsersSubs.AddAsync(sub);
        await _db.SaveChangesAsync();
        
        _logger.LogInformation("[Info] Follow successful. SubscriberId={SubscriberId} → TargetUserId={TargetUserId}", 
            dto.IdSubscriber, dto.IdTargetUser);
    }

    public async Task UnfollowAsync(FollowUserDto dto)
    {
        _logger.LogInformation("[Info] Unfollow attempt. SubscriberId={SubscriberId} → TargetUserId={TargetUserId}", 
            dto.IdSubscriber, dto.IdTargetUser);
        
        var sub = await _db.UsersSubs.FirstOrDefaultAsync(x => 
            x.IdUser == dto.IdTargetUser && 
            x.IdSubscriber == dto.IdSubscriber);

        if (sub == null)
        {
            _logger.LogWarning("[Warn] Subscription not found for unfollow. SubscriberId={SubscriberId} → TargetUserId={TargetUserId}", 
                dto.IdSubscriber, dto.IdTargetUser);
            throw new InvalidOperationException("Subscription doesn't exist");
        }
        
        _db.UsersSubs.Remove(sub);
        await _db.SaveChangesAsync();
        
        _logger.LogInformation("[Info] Unfollow successful. SubscriberId={SubscriberId} → TargetUserId={TargetUserId}", 
            dto.IdSubscriber, dto.IdTargetUser);
    }
}