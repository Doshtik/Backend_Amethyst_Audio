using System;
using System.Collections.Generic;

namespace Backend_Amethyst_Audio.Entities;

public partial class User
{
    public long Id { get; set; }

    public string? Lastname { get; set; }

    public string? Firstname { get; set; }

    public string Nickname { get; set; } = null!;

    public string Email { get; set; } = null!;

    public string PasswordHash { get; set; } = null!;

    public string? Gender { get; set; }

    public string? Country { get; set; }

    public string? AvatarFileName { get; set; }

    public string? HeaderFileName { get; set; }

    public bool IsVerified { get; set; }

    public bool IsEmailVerified { get; set; }

    public bool IsActive { get; set; }

    public DateTime LastVisit { get; set; }

    public DateTime CreatedAt { get; set; }

    public DateTime UpdatedAt { get; set; }

    public virtual ICollection<AlbumsAuthor> AlbumsAuthors { get; set; } = new List<AlbumsAuthor>();

    public virtual ICollection<AuthUser> AuthUsers { get; set; } = new List<AuthUser>();

    public virtual ICollection<Library> Libraries { get; set; } = new List<Library>();

    public virtual ICollection<Notification> Notifications { get; set; } = new List<Notification>();

    public virtual ICollection<Playlist> Playlists { get; set; } = new List<Playlist>();

    public virtual ICollection<Report> Reports { get; set; } = new List<Report>();

    public virtual ICollection<SavedAlbum> SavedAlbums { get; set; } = new List<SavedAlbum>();

    public virtual ICollection<SavedPlaylist> SavedPlaylists { get; set; } = new List<SavedPlaylist>();

    public virtual ICollection<TracksAuthor> TracksAuthors { get; set; } = new List<TracksAuthor>();

    public virtual ICollection<UsersHistory> UsersHistories { get; set; } = new List<UsersHistory>();

    public virtual ICollection<UsersSub> UsersSubIdSubscriberNavigations { get; set; } = new List<UsersSub>();

    public virtual ICollection<UsersSub> UsersSubIdUserNavigations { get; set; } = new List<UsersSub>();
}
