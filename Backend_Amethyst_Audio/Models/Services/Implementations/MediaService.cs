using System.Security;
using Backend_Amethyst_Audio.Models.Enums;
using Backend_Amethyst_Audio.Services.Abstractions;
using Microsoft.AspNetCore.Http.HttpResults;

namespace Backend_Amethyst_Audio.Services.Implementations;

public class MediaService: IMediaService
{
    private readonly ILogger<MediaService> _logger;
    public string RootPath { get; } = Path.Combine(Environment.GetFolderPath(Environment.SpecialFolder.MyDocuments), "AmethystAudio");

    public MediaService(ILogger<MediaService> logger)
    {
        _logger = logger;
    }

    public async Task<string> SaveFileAsync(IFormFile file, FileTypes typeName)
    {
        _logger.LogInformation("[Info] Saving file. FileName={FileName}, FileType={FileType}, FileSize={FileSize} bytes", 
            file.FileName, typeName, file.Length);
        
        try
        {
            string folderName = typeName switch
            {
                FileTypes.Tracks => "Tracks",
                FileTypes.Covers => "Covers",
                FileTypes.Avatars => "Avatars",
                FileTypes.Headers => "Headers",
                _ => throw new ArgumentException($"[Warn] Unknown file type: {typeName}")
            };
            
            string extension = Path.GetExtension(file.FileName);
            
            // Validate file extension for security
            if (string.IsNullOrWhiteSpace(extension) || IsExtensionDangerous(extension))
            {
                _logger.LogWarning("[Warn] Dangerous file extension rejected. Extension={Extension}, OriginalName={FileName}", 
                    extension, file.FileName);
                throw new SecurityException("Invalid file type");
            }
            
            var fileName = $"{Guid.NewGuid()}{extension}";
            var path = Path.Combine(RootPath, folderName, fileName);

            Directory.CreateDirectory(Path.GetDirectoryName(path));
            _logger.LogDebug("[Debug] Created directory if not exists. Path={Directory}", Path.GetDirectoryName(path));

            await using FileStream stream = new FileStream(path, FileMode.Create);
            await file.CopyToAsync(stream);

            _logger.LogInformation("[Info] File saved successfully. FileType={FileType}, NewFileName={FileName}, Size={Size} bytes", 
                typeName, fileName, file.Length);
            
            return fileName;
        }
        catch (SecurityException e)
        {
            _logger.LogError(e, "[Error] Security validation failed for file upload. FileName={FileName}", file.FileName);
            throw;
        }
        catch (Exception e)
        {
            _logger.LogError(e, "[Error] Failed to save file. FileName={FileName}, FileType={FileType}", 
                file.FileName, typeName);
            throw;
        }
    }
    
    public async Task DeleteFileAsync(string filePath)
    {
        _logger.LogInformation("[Info] Deleting file. FilePath={FilePath}", filePath);
        
        try
        {
            if (string.IsNullOrWhiteSpace(filePath) || !Path.IsPathRooted(filePath))
            {
                var fullPath = Path.Combine(RootPath, filePath);
                
                if (!File.Exists(fullPath))
                {
                    _logger.LogWarning("[Warn] File not found for deletion. FilePath={FilePath}", filePath);
                    throw new FileNotFoundException("File not found");
                }
                
                File.Delete(fullPath);
                _logger.LogInformation("[Info] File deleted successfully. FilePath={FilePath}", filePath);
            }
            else
            {
                // Prevent path traversal attacks
                _logger.LogWarning("[Warn] Absolute path deletion attempt blocked. FilePath={FilePath}", filePath);
                throw new SecurityException("Absolute paths are not allowed for deletion");
            }
        }
        catch (Exception e)
        {
            _logger.LogError(e, "[Error] Failed to delete file. FilePath={FilePath}", filePath);
            throw;
        }
    }

    public async Task<string> GetTrackFilePathAsync(int id)
    {
        _logger.LogDebug("[Debug] Getting track file path. TrackId={TrackId}", id);
        
        // TODO: Replace with actual DB lookup
        var fileName = await GetFileNameFromDbAsync(id, FileTypes.Tracks);
        
        if (string.IsNullOrEmpty(fileName))
        {
            _logger.LogWarning("[Warn] Track file record not found in database. TrackId={TrackId}", id);
            throw new FileNotFoundException("Track not found");
        }
        
        var fullPath = Path.Combine(RootPath, "Tracks", fileName);
        
        if (!File.Exists(fullPath))
        {
            _logger.LogWarning("[Warn] Track file not found on disk. TrackId={TrackId}, ExpectedPath={Path}", id, fullPath);
            throw new FileNotFoundException("Track file missing");
        }
        
        _logger.LogDebug("[Debug] Track file path resolved. TrackId={TrackId}, Path={Path}", id, fullPath);
        return fullPath;
    }

    public async Task<string> GetTrackCoverPathAsync(int id)
    {
        _logger.LogDebug("[Debug] Getting track cover path. TrackId={TrackId}", id);
        
        var fileName = await GetFileNameFromDbAsync(id, FileTypes.Covers);
        
        if (string.IsNullOrEmpty(fileName))
        {
            _logger.LogWarning("[Warn] Track cover not found. TrackId={TrackId}", id);
            throw new FileNotFoundException("Cover not found");
        }
        
        var fullPath = Path.Combine(RootPath, "Covers", fileName);
        
        if (!File.Exists(fullPath))
        {
            _logger.LogWarning("[Warn] Track cover file missing on disk. TrackId={TrackId}", id);
            throw new FileNotFoundException("Cover file missing");
        }
        
        _logger.LogDebug("[Debug] Track cover path resolved. TrackId={TrackId}", id);
        return fullPath;
    }

    public async Task<string> GetPlaylistCoverPathAsync(int id)
    {
        _logger.LogDebug("[Debug] Getting playlist cover path. PlaylistId={PlaylistId}", id);
        
        var fileName = await GetFileNameFromDbAsync(id, FileTypes.Covers);
        
        if (string.IsNullOrEmpty(fileName))
        {
            _logger.LogWarning("[Warn] Playlist cover not found. PlaylistId={PlaylistId}", id);
            throw new FileNotFoundException("Cover not found");
        }
        
        var fullPath = Path.Combine(RootPath, "Covers", fileName);
        
        if (!File.Exists(fullPath))
        {
            _logger.LogWarning("[Warn] Playlist cover file missing on disk. PlaylistId={PlaylistId}", id);
            throw new FileNotFoundException("Cover file missing");
        }
        
        _logger.LogDebug("[Debug] Playlist cover path resolved. PlaylistId={PlaylistId}", id);
        return fullPath;
    }

    public async Task<string> GetAlbumCoverPathAsync(int id)
    {
        _logger.LogDebug("[Debug] Getting album cover path. AlbumId={AlbumId}", id);
        
        var fileName = await GetFileNameFromDbAsync(id, FileTypes.Covers);
        
        if (string.IsNullOrEmpty(fileName))
        {
            _logger.LogWarning("[Warn] Album cover not found. AlbumId={AlbumId}", id);
            throw new FileNotFoundException("Cover not found");
        }
        
        var fullPath = Path.Combine(RootPath, "Covers", fileName);
        
        if (!File.Exists(fullPath))
        {
            _logger.LogWarning("[Warn] Album cover file missing on disk. AlbumId={AlbumId}", id);
            throw new FileNotFoundException("Cover file missing");
        }
        
        _logger.LogDebug("[Debug] Album cover path resolved. AlbumId={AlbumId}", id);
        return fullPath;
    }

    public async Task<string> GetUserAvatarPathAsync(int id)
    {
        _logger.LogDebug("[Debug] Getting user avatar path. UserId={UserId}", id);
        
        var fileName = await GetFileNameFromDbAsync(id, FileTypes.Avatars);
        
        if (string.IsNullOrEmpty(fileName))
        {
            _logger.LogWarning("[Warn] User avatar not found. UserId={UserId}", id);
            throw new FileNotFoundException("Avatar not found");
        }
        
        var fullPath = Path.Combine(RootPath, "Avatars", fileName);
        
        if (!File.Exists(fullPath))
        {
            _logger.LogWarning("[Warn] User avatar file missing on disk. UserId={UserId}", id);
            throw new FileNotFoundException("Avatar file missing");
        }
        
        _logger.LogDebug("[Debug] User avatar path resolved. UserId={UserId}", id);
        return fullPath;
    }

    public async Task<string> GetUserHeaderPathAsync(int id)
    {
        _logger.LogDebug("[Debug] Getting user header path. UserId={UserId}", id);
        
        var fileName = await GetFileNameFromDbAsync(id, FileTypes.Headers);
        
        if (string.IsNullOrEmpty(fileName))
        {
            _logger.LogWarning("[Warn] User header not found. UserId={UserId}", id);
            throw new FileNotFoundException("Header not found");
        }
        
        var fullPath = Path.Combine(RootPath, "Headers", fileName);
        
        if (!File.Exists(fullPath))
        {
            _logger.LogWarning("[Warn] User header file missing on disk. UserId={UserId}", id);
            throw new FileNotFoundException("Header file missing");
        }
        
        _logger.LogDebug("[Debug] User header path resolved. UserId={UserId}", id);
        return fullPath;
    }
    
    // Helper method for DB lookup (placeholder)
    private async Task<string> GetFileNameFromDbAsync(int id, FileTypes type)
    {
        // TODO: Replace with actual database query
        // Example: return await _db.MediaFiles.Where(x => x.EntityId == id && x.Type == type).Select(x => x.FileName).FirstOrDefaultAsync();
        await Task.Yield();
        return null; // Placeholder
    }
    
    // Security: block dangerous extensions
    private bool IsExtensionDangerous(string extension)
    {
        var dangerousExtensions = new[] 
        { 
            ".exe", ".dll", ".bat", ".cmd", ".ps1", ".js", ".vbs", ".scr", ".msi", ".php", ".asp", ".aspx" 
        };
        return dangerousExtensions.Contains(extension.ToLowerInvariant());
    }
}