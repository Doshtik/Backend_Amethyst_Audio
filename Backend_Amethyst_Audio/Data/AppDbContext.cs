using System;
using System.Collections.Generic;
using Backend_Amethyst_Audio.Models;
using Microsoft.EntityFrameworkCore;

namespace Backend_Amethyst_Audio.Data
{
    public partial class AppDbContext : DbContext
    {
        public AppDbContext() { }

        public AppDbContext(DbContextOptions<AppDbContext> options) : base(options) { }

        public virtual DbSet<Album> Albums { get; set; }

        public virtual DbSet<AlbumsAuthor> AlbumsAuthors { get; set; }

        public virtual DbSet<AlbumsTrack> AlbumsTracks { get; set; }

        public virtual DbSet<Employee> Employees { get; set; }

        public virtual DbSet<Genre> Genres { get; set; }

        public virtual DbSet<Notification> Notifications { get; set; }

        public virtual DbSet<OauthEmployee> OauthEmployees { get; set; }

        public virtual DbSet<OauthProvider> OauthProviders { get; set; }

        public virtual DbSet<OauthUser> OauthUsers { get; set; }

        public virtual DbSet<Playlist> Playlists { get; set; }

        public virtual DbSet<PlaylistsTrack> PlaylistsTracks { get; set; }

        public virtual DbSet<Reason> Reasons { get; set; }

        public virtual DbSet<Report> Reports { get; set; }

        public virtual DbSet<ReportAnswer> ReportAnswers { get; set; }

        public virtual DbSet<Role> Roles { get; set; }

        public virtual DbSet<SavedAlbum> SavedAlbums { get; set; }

        public virtual DbSet<SavedPlaylist> SavedPlaylists { get; set; }

        public virtual DbSet<Track> Tracks { get; set; }

        public virtual DbSet<TracksAuthor> TracksAuthors { get; set; }

        public virtual DbSet<TracksGenre> TracksGenres { get; set; }

        public virtual DbSet<TypesAccess> TypesAccesses { get; set; }

        public virtual DbSet<TypesReport> TypesReports { get; set; }

        public virtual DbSet<User> Users { get; set; }

        public virtual DbSet<UsersHistory> UsersHistories { get; set; }

        public virtual DbSet<UsersSub> UsersSubs { get; set; }

        protected override void OnModelCreating(ModelBuilder modelBuilder)
        {
            modelBuilder.Entity<Album>(entity =>
            {
                entity.HasKey(e => e.Id).HasName("pk_albums_id");

                entity.ToTable("albums", "content");

                entity.Property(e => e.Id).HasColumnName("id");
                entity.Property(e => e.CoverName).HasColumnName("cover_name");
                entity.Property(e => e.CreatedAt)
                    .HasDefaultValueSql("CURRENT_TIMESTAMP")
                    .HasColumnName("created_at");
                entity.Property(e => e.Name).HasColumnName("name");
                entity.Property(e => e.UpdatedAt)
                    .HasDefaultValueSql("CURRENT_TIMESTAMP")
                    .HasColumnName("updated_at");
            });

            modelBuilder.Entity<AlbumsAuthor>(entity =>
            {
                entity.HasKey(e => e.Id).HasName("pk_album_authors_id");

                entity.ToTable("albums_authors", "content");

                entity.HasIndex(e => new { e.IdAlbum, e.IdAuthor }, "uq_albums_authors").IsUnique();

                entity.Property(e => e.Id).HasColumnName("id");
                entity.Property(e => e.IdAlbum).HasColumnName("id_album");
                entity.Property(e => e.IdAuthor).HasColumnName("id_author");

                entity.HasOne(d => d.IdAlbumNavigation).WithMany(p => p.AlbumsAuthors)
                    .HasForeignKey(d => d.IdAlbum)
                    .HasConstraintName("fk_albums_authors_id_album");

                entity.HasOne(d => d.IdAuthorNavigation).WithMany(p => p.AlbumsAuthors)
                    .HasForeignKey(d => d.IdAuthor)
                    .HasConstraintName("fk_albums_authors_id_author");
            });

            modelBuilder.Entity<AlbumsTrack>(entity =>
            {
                entity.HasKey(e => e.Id).HasName("pk_albums_tracks_id");

                entity.ToTable("albums_tracks", "content");

                entity.HasIndex(e => new { e.IdAlbum, e.IdTrack }, "uq_albums_tracks").IsUnique();

                entity.Property(e => e.Id).HasColumnName("id");
                entity.Property(e => e.IdAlbum).HasColumnName("id_album");
                entity.Property(e => e.IdTrack).HasColumnName("id_track");

                entity.HasOne(d => d.IdAlbumNavigation).WithMany(p => p.AlbumsTracks)
                    .HasForeignKey(d => d.IdAlbum)
                    .HasConstraintName("fk_albums_tracks_id_album");

                entity.HasOne(d => d.IdTrackNavigation).WithMany(p => p.AlbumsTracks)
                    .HasForeignKey(d => d.IdTrack)
                    .HasConstraintName("fk_albums_tracks_id_track");
            });

            modelBuilder.Entity<Employee>(entity =>
            {
                entity.HasKey(e => e.Id).HasName("pk_employees_id");

                entity.ToTable("employees", "admin");

                entity.HasIndex(e => e.Email, "uq_employees_email").IsUnique();

                entity.Property(e => e.Id).HasColumnName("id");
                entity.Property(e => e.CreatedAt)
                    .HasDefaultValueSql("CURRENT_TIMESTAMP")
                    .HasColumnName("created_at");
                entity.Property(e => e.Email).HasColumnName("email");
                entity.Property(e => e.Firstname).HasColumnName("firstname");
                entity.Property(e => e.IdRole).HasColumnName("id_role");
                entity.Property(e => e.IsActive)
                    .HasDefaultValue(false)
                    .HasColumnName("is_active");
                entity.Property(e => e.LastVisit)
                    .HasDefaultValueSql("CURRENT_TIMESTAMP")
                    .HasColumnName("last_visit");
                entity.Property(e => e.Lastname).HasColumnName("lastname");
                entity.Property(e => e.PasswordHash).HasColumnName("password_hash");
                entity.Property(e => e.UpdatedAt)
                    .HasDefaultValueSql("CURRENT_TIMESTAMP")
                    .HasColumnName("updated_at");

                entity.HasOne(d => d.IdRoleNavigation).WithMany(p => p.Employees)
                    .HasForeignKey(d => d.IdRole)
                    .HasConstraintName("fk_employees_id_role");
            });

            modelBuilder.Entity<Genre>(entity =>
            {
                entity.HasKey(e => e.Id).HasName("pk_genres_id");

                entity.ToTable("genres", "content");

                entity.Property(e => e.Id).HasColumnName("id");
                entity.Property(e => e.GenreName).HasColumnName("genre_name");
            });

            modelBuilder.Entity<Notification>(entity =>
            {
                entity.HasKey(e => e.Id).HasName("pk_notifications_id");

                entity.ToTable("notifications", "users");

                entity.Property(e => e.Id).HasColumnName("id");
                entity.Property(e => e.Body).HasColumnName("body");
                entity.Property(e => e.CreatedAt)
                    .HasDefaultValueSql("CURRENT_TIMESTAMP")
                    .HasColumnName("created_at");
                entity.Property(e => e.IdUser).HasColumnName("id_user");
                entity.Property(e => e.Subject).HasColumnName("subject");

                entity.HasOne(d => d.IdUserNavigation).WithMany(p => p.Notifications)
                    .HasForeignKey(d => d.IdUser)
                    .HasConstraintName("fk_notifications_id_user");
            });

            modelBuilder.Entity<OauthEmployee>(entity =>
            {
                entity.HasKey(e => e.Id).HasName("pk_oauth_employees_id");

                entity.ToTable("oauth_employees", "auth");

                entity.HasIndex(e => new { e.IdEmployee, e.IdProvider }, "uq_oauth_employees_employee_provider").IsUnique();

                entity.Property(e => e.Id).HasColumnName("id");
                entity.Property(e => e.AccessToken).HasColumnName("access_token");
                entity.Property(e => e.CreatedAt)
                    .HasDefaultValueSql("CURRENT_TIMESTAMP")
                    .HasColumnName("created_at");
                entity.Property(e => e.Email).HasColumnName("email");
                entity.Property(e => e.IdEmployee).HasColumnName("id_employee");
                entity.Property(e => e.IdProvider).HasColumnName("id_provider");
                entity.Property(e => e.RefreshToken).HasColumnName("refresh_token");
                entity.Property(e => e.TokenExpiresAt).HasColumnName("token_expires_at");
                entity.Property(e => e.UpdatedAt)
                    .HasDefaultValueSql("CURRENT_TIMESTAMP")
                    .HasColumnName("updated_at");

                entity.HasOne(d => d.IdEmployeeNavigation).WithMany(p => p.OauthEmployees)
                    .HasForeignKey(d => d.IdEmployee)
                    .HasConstraintName("fk_oauth_employees_id_employee");

                entity.HasOne(d => d.IdProviderNavigation).WithMany(p => p.OauthEmployees)
                    .HasForeignKey(d => d.IdProvider)
                    .HasConstraintName("fk_oauth_users_id_provider");
            });

            modelBuilder.Entity<OauthProvider>(entity =>
            {
                entity.HasKey(e => e.Id).HasName("pk_oauth_providers_id");

                entity.ToTable("oauth_providers", "auth");

                entity.Property(e => e.Id).HasColumnName("id");
                entity.Property(e => e.ProviderName).HasColumnName("provider_name");
            });

            modelBuilder.Entity<OauthUser>(entity =>
            {
                entity.HasKey(e => e.Id).HasName("pk_oauth_users_id");

                entity.ToTable("oauth_users", "auth");

                entity.HasIndex(e => new { e.IdUser, e.IdProvider }, "uq_oauth_users_id_user_id_provider").IsUnique();

                entity.Property(e => e.Id).HasColumnName("id");
                entity.Property(e => e.AccessToken).HasColumnName("access_token");
                entity.Property(e => e.CreatedAt)
                    .HasDefaultValueSql("CURRENT_TIMESTAMP")
                    .HasColumnName("created_at");
                entity.Property(e => e.Email).HasColumnName("email");
                entity.Property(e => e.IdProvider).HasColumnName("id_provider");
                entity.Property(e => e.IdUser).HasColumnName("id_user");
                entity.Property(e => e.RefreshToken).HasColumnName("refresh_token");
                entity.Property(e => e.TokenExpiresAt).HasColumnName("token_expires_at");
                entity.Property(e => e.UpdatedAt)
                    .HasDefaultValueSql("CURRENT_TIMESTAMP")
                    .HasColumnName("updated_at");

                entity.HasOne(d => d.IdProviderNavigation).WithMany(p => p.OauthUsers)
                    .HasForeignKey(d => d.IdProvider)
                    .HasConstraintName("fk_oauth_users_id_provider");

                entity.HasOne(d => d.IdUserNavigation).WithMany(p => p.OauthUsers)
                    .HasForeignKey(d => d.IdUser)
                    .HasConstraintName("fk_oauth_users_id_user");
            });

            modelBuilder.Entity<Playlist>(entity =>
            {
                entity.HasKey(e => e.Id).HasName("pk_playlists_id");

                entity.ToTable("playlists", "collections");

                entity.Property(e => e.Id).HasColumnName("id");
                entity.Property(e => e.CoverName)
                    .HasDefaultValueSql("'empty_cover.png'::text")
                    .HasColumnName("cover_name");
                entity.Property(e => e.CreatedAt)
                    .HasDefaultValueSql("CURRENT_TIMESTAMP")
                    .HasColumnName("created_at");
                entity.Property(e => e.Discription).HasColumnName("discription");
                entity.Property(e => e.IdAccessType).HasColumnName("id_access_type");
                entity.Property(e => e.IdUser).HasColumnName("id_user");
                entity.Property(e => e.Name).HasColumnName("name");
                entity.Property(e => e.UpdatedAt)
                    .HasDefaultValueSql("CURRENT_TIMESTAMP")
                    .HasColumnName("updated_at");

                entity.HasOne(d => d.IdAccessTypeNavigation).WithMany(p => p.Playlists)
                    .HasForeignKey(d => d.IdAccessType)
                    .HasConstraintName("fk_playlists_id_access_type");

                entity.HasOne(d => d.IdUserNavigation).WithMany(p => p.Playlists)
                    .HasForeignKey(d => d.IdUser)
                    .HasConstraintName("fk_playlists_id_user");
            });

            modelBuilder.Entity<PlaylistsTrack>(entity =>
            {
                entity.HasKey(e => e.Id).HasName("pk_playlists_tracks_id");

                entity.ToTable("playlists_tracks", "collections");

                entity.HasIndex(e => new { e.IdPlaylist, e.IdTrack }, "uq_playlists_tracks").IsUnique();

                entity.Property(e => e.Id).HasColumnName("id");
                entity.Property(e => e.IdPlaylist).HasColumnName("id_playlist");
                entity.Property(e => e.IdTrack).HasColumnName("id_track");

                entity.HasOne(d => d.IdPlaylistNavigation).WithMany(p => p.PlaylistsTracks)
                    .HasForeignKey(d => d.IdPlaylist)
                    .HasConstraintName("fk_playlists_tracks_id_playlist");

                entity.HasOne(d => d.IdTrackNavigation).WithMany(p => p.PlaylistsTracks)
                    .HasForeignKey(d => d.IdTrack)
                    .HasConstraintName("fk_playlists_tracks_id_track");
            });

            modelBuilder.Entity<Reason>(entity =>
            {
                entity.HasKey(e => e.Id).HasName("pk_types_reason_id");

                entity.ToTable("reasons", "moderation");

                entity.Property(e => e.Id).HasColumnName("id");
                entity.Property(e => e.TypeName).HasColumnName("type_name");
            });

            modelBuilder.Entity<Report>(entity =>
            {
                entity.HasKey(e => e.Id).HasName("pk_reports_id");

                entity.ToTable("reports", "moderation");

                entity.Property(e => e.Id).HasColumnName("id");
                entity.Property(e => e.CreatedAt)
                    .HasDefaultValueSql("CURRENT_TIMESTAMP")
                    .HasColumnName("created_at");
                entity.Property(e => e.Discription).HasColumnName("discription");
                entity.Property(e => e.IdReason).HasColumnName("id_reason");
                entity.Property(e => e.IdType).HasColumnName("id_type");
                entity.Property(e => e.IdUser).HasColumnName("id_user");
                entity.Property(e => e.IsActive)
                    .HasDefaultValue(true)
                    .HasColumnName("is_active");

                entity.HasOne(d => d.IdReasonNavigation).WithMany(p => p.Reports)
                    .HasForeignKey(d => d.IdReason)
                    .HasConstraintName("fk_reports_id_reason");

                entity.HasOne(d => d.IdTypeNavigation).WithMany(p => p.Reports)
                    .HasForeignKey(d => d.IdType)
                    .HasConstraintName("fk_reports_id_type");

                entity.HasOne(d => d.IdUserNavigation).WithMany(p => p.Reports)
                    .HasForeignKey(d => d.IdUser)
                    .HasConstraintName("fk_reports_id_user");
            });

            modelBuilder.Entity<ReportAnswer>(entity =>
            {
                entity.HasKey(e => e.Id).HasName("pk_report_answers_id");

                entity.ToTable("report_answers", "moderation");

                entity.Property(e => e.Id).HasColumnName("id");
                entity.Property(e => e.CreatedAt)
                    .HasDefaultValueSql("CURRENT_TIMESTAMP")
                    .HasColumnName("created_at");
                entity.Property(e => e.IdEmployee).HasColumnName("id_employee");
                entity.Property(e => e.IdReport).HasColumnName("id_report");
                entity.Property(e => e.Message).HasColumnName("message");

                entity.HasOne(d => d.IdEmployeeNavigation).WithMany(p => p.ReportAnswers)
                    .HasForeignKey(d => d.IdEmployee)
                    .HasConstraintName("fk_report_answers_id_employee");

                entity.HasOne(d => d.IdReportNavigation).WithMany(p => p.ReportAnswers)
                    .HasForeignKey(d => d.IdReport)
                    .HasConstraintName("fk_report_answers_id_report");
            });

            modelBuilder.Entity<Role>(entity =>
            {
                entity.HasKey(e => e.Id).HasName("pk_roles_id");

                entity.ToTable("roles", "admin");

                entity.Property(e => e.Id).HasColumnName("id");
                entity.Property(e => e.RoleName).HasColumnName("role_name");
            });

            modelBuilder.Entity<SavedAlbum>(entity =>
            {
                entity.HasKey(e => e.Id).HasName("pk_album_users_id");

                entity.ToTable("saved_albums", "content");

                entity.HasIndex(e => new { e.IdAlbum, e.IdUser }, "uq_album_users").IsUnique();

                entity.Property(e => e.Id).HasColumnName("id");
                entity.Property(e => e.IdAlbum).HasColumnName("id_album");
                entity.Property(e => e.IdUser).HasColumnName("id_user");

                entity.HasOne(d => d.IdAlbumNavigation).WithMany(p => p.SavedAlbums)
                    .HasForeignKey(d => d.IdAlbum)
                    .HasConstraintName("fk_album_users_id_album");

                entity.HasOne(d => d.IdUserNavigation).WithMany(p => p.SavedAlbums)
                    .HasForeignKey(d => d.IdUser)
                    .HasConstraintName("fk_albums_users_id_user");
            });

            modelBuilder.Entity<SavedPlaylist>(entity =>
            {
                entity.HasKey(e => e.Id).HasName("pk_playlists_users_id");

                entity.ToTable("saved_playlists", "collections");

                entity.HasIndex(e => new { e.IdPlaylist, e.IdUser }, "uq_playlists_users").IsUnique();

                entity.Property(e => e.Id).HasColumnName("id");
                entity.Property(e => e.IdPlaylist).HasColumnName("id_playlist");
                entity.Property(e => e.IdUser).HasColumnName("id_user");

                entity.HasOne(d => d.IdPlaylistNavigation).WithMany(p => p.SavedPlaylists)
                    .HasForeignKey(d => d.IdPlaylist)
                    .HasConstraintName("fk_playlists_users_id_playlist");

                entity.HasOne(d => d.IdUserNavigation).WithMany(p => p.SavedPlaylists)
                    .HasForeignKey(d => d.IdUser)
                    .HasConstraintName("fk_playlists_users_id_user");
            });

            modelBuilder.Entity<Track>(entity =>
            {
                entity.HasKey(e => e.Id).HasName("pk_tracks_id");

                entity.ToTable("tracks", "content");

                entity.HasIndex(e => e.Name, "uq_tracks_name").IsUnique();

                entity.Property(e => e.Id).HasColumnName("id");
                entity.Property(e => e.Country).HasColumnName("country");
                entity.Property(e => e.CoverName).HasColumnName("cover_name");
                entity.Property(e => e.CreatedAt)
                    .HasDefaultValueSql("CURRENT_TIMESTAMP")
                    .HasColumnName("created_at");
                entity.Property(e => e.DurationSec).HasColumnName("duration_sec");
                entity.Property(e => e.ExplicitContent).HasColumnName("explicit_content");
                entity.Property(e => e.Name).HasColumnName("name");
                entity.Property(e => e.Textless).HasColumnName("textless");
                entity.Property(e => e.TrackMimeType).HasColumnName("track_mime_type");
                entity.Property(e => e.TrackMood).HasColumnName("track_mood");
                entity.Property(e => e.TrackName).HasColumnName("track_name");
                entity.Property(e => e.TrackPace).HasColumnName("track_pace");
                entity.Property(e => e.TrackWeight).HasColumnName("track_weight");
                entity.Property(e => e.UpdatedAt)
                    .HasDefaultValueSql("CURRENT_TIMESTAMP")
                    .HasColumnName("updated_at");
            });

            modelBuilder.Entity<TracksAuthor>(entity =>
            {
                entity.HasKey(e => e.Id).HasName("pk_tracks_authors_id");

                entity.ToTable("tracks_authors", "content");

                entity.Property(e => e.Id).HasColumnName("id");
                entity.Property(e => e.IdAuthor).HasColumnName("id_author");
                entity.Property(e => e.IdTrack).HasColumnName("id_track");

                entity.HasOne(d => d.IdAuthorNavigation).WithMany(p => p.TracksAuthors)
                    .HasForeignKey(d => d.IdAuthor)
                    .HasConstraintName("fk_tracks_authors_id_author");

                entity.HasOne(d => d.IdTrackNavigation).WithMany(p => p.TracksAuthors)
                    .HasForeignKey(d => d.IdTrack)
                    .HasConstraintName("fk_tracks_authors_id_track");
            });

            modelBuilder.Entity<TracksGenre>(entity =>
            {
                entity.HasKey(e => e.Id).HasName("pk_track_genres_id");

                entity.ToTable("tracks_genres", "content");

                entity.HasIndex(e => new { e.IdTrack, e.IdGenre }, "uq_track_genres").IsUnique();

                entity.Property(e => e.Id).HasColumnName("id");
                entity.Property(e => e.IdGenre).HasColumnName("id_genre");
                entity.Property(e => e.IdTrack).HasColumnName("id_track");

                entity.HasOne(d => d.IdGenreNavigation).WithMany(p => p.TracksGenres)
                    .HasForeignKey(d => d.IdGenre)
                    .HasConstraintName("fk_track_genres_id_genre");

                entity.HasOne(d => d.IdTrackNavigation).WithMany(p => p.TracksGenres)
                    .HasForeignKey(d => d.IdTrack)
                    .HasConstraintName("fk_track_genres_id_track");
            });

            modelBuilder.Entity<TypesAccess>(entity =>
            {
                entity.HasKey(e => e.Id).HasName("pk_types_access_id");

                entity.ToTable("types_access", "collections");

                entity.Property(e => e.Id).HasColumnName("id");
                entity.Property(e => e.TypeName).HasColumnName("type_name");
            });

            modelBuilder.Entity<TypesReport>(entity =>
            {
                entity.HasKey(e => e.Id).HasName("pk_types_report_id");

                entity.ToTable("types_report", "moderation");

                entity.Property(e => e.Id).HasColumnName("id");
                entity.Property(e => e.TypeName).HasColumnName("type_name");
            });

            modelBuilder.Entity<User>(entity =>
            {
                entity.HasKey(e => e.Id).HasName("pk_users_id");

                entity.ToTable("users", "users");

                entity.HasIndex(e => e.Email, "uq_users_email").IsUnique();

                entity.HasIndex(e => e.Nickname, "uq_users_nickname").IsUnique();

                entity.Property(e => e.Id).HasColumnName("id");
                entity.Property(e => e.AvatarName).HasColumnName("avatar_name");
                entity.Property(e => e.Country).HasColumnName("country");
                entity.Property(e => e.CreatedAt)
                    .HasDefaultValueSql("CURRENT_TIMESTAMP")
                    .HasColumnName("created_at");
                entity.Property(e => e.Email).HasColumnName("email");
                entity.Property(e => e.Firstname).HasColumnName("firstname");
                entity.Property(e => e.Gender).HasColumnName("gender");
                entity.Property(e => e.HeaderName).HasColumnName("header_name");
                entity.Property(e => e.IsVerified)
                    .HasDefaultValue(false)
                    .HasColumnName("is_verified");
                entity.Property(e => e.IsEmailVerified)
                    .HasDefaultValue(false)
                    .HasColumnName("is_email_verified");
                entity.Property(e => e.IsActive)
                    .HasDefaultValue(false)
                    .HasColumnName("is_active");
                entity.Property(e => e.LastVisit)
                    .HasDefaultValueSql("CURRENT_TIMESTAMP")
                    .HasColumnName("last_visit");
                entity.Property(e => e.Lastname).HasColumnName("lastname");
                entity.Property(e => e.Nickname).HasColumnName("nickname");
                entity.Property(e => e.PasswordHash).HasColumnName("password_hash");
                entity.Property(e => e.UpdatedAt)
                    .HasDefaultValueSql("CURRENT_TIMESTAMP")
                    .HasColumnName("updated_at");
            });

            modelBuilder.Entity<UsersHistory>(entity =>
            {
                entity.HasKey(e => e.Id).HasName("pk_tracks_users_id");

                entity.ToTable("users_history", "users");

                entity.HasIndex(e => new { e.IdUser, e.IdTrack }, "uk_users_history").IsUnique();

                entity.Property(e => e.Id).HasColumnName("id");
                entity.Property(e => e.IdTrack).HasColumnName("id_track");
                entity.Property(e => e.IdUser).HasColumnName("id_user");
                entity.Property(e => e.ListeningAt)
                    .HasDefaultValueSql("CURRENT_TIMESTAMP")
                    .HasColumnName("listening_at");
                entity.Property(e => e.TotalListeningSec).HasColumnName("total_listening_sec");

                entity.HasOne(d => d.IdTrackNavigation).WithMany(p => p.UsersHistories)
                    .HasForeignKey(d => d.IdTrack)
                    .HasConstraintName("fk_tracks_users_id_track");

                entity.HasOne(d => d.IdUserNavigation).WithMany(p => p.UsersHistories)
                    .HasForeignKey(d => d.IdUser)
                    .HasConstraintName("fk_tracks_users_id_user");
            });

            modelBuilder.Entity<UsersSub>(entity =>
            {
                entity.HasKey(e => e.Id).HasName("pk_users_subs_id");

                entity.ToTable("users_subs", "users");

                entity.HasIndex(e => new { e.IdUser, e.IdSubscriber }, "uq_users_subs_id_user_id_subscriber").IsUnique();

                entity.Property(e => e.Id).HasColumnName("id");
                entity.Property(e => e.IdSubscriber).HasColumnName("id_subscriber");
                entity.Property(e => e.IdUser).HasColumnName("id_user");
                entity.Property(e => e.SubscribedAt)
                    .HasDefaultValueSql("CURRENT_TIMESTAMP")
                    .HasColumnName("subscribed_at");

                entity.HasOne(d => d.IdSubscriberNavigation).WithMany(p => p.UsersSubIdSubscriberNavigations)
                    .HasForeignKey(d => d.IdSubscriber)
                    .HasConstraintName("fk_users_subs_id_subscriber");

                entity.HasOne(d => d.IdUserNavigation).WithMany(p => p.UsersSubIdUserNavigations)
                    .HasForeignKey(d => d.IdUser)
                    .HasConstraintName("fk_users_subs_id_user");
            });

            OnModelCreatingPartial(modelBuilder);
        }

        partial void OnModelCreatingPartial(ModelBuilder modelBuilder);
    }
}