using System;
using Microsoft.EntityFrameworkCore.Migrations;
using Npgsql.EntityFrameworkCore.PostgreSQL.Metadata;

#nullable disable

namespace Backend_Amethyst_Audio.Migrations
{
    /// <inheritdoc />
    public partial class RemoveTokensAndAddExternalId : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.EnsureSchema(
                name: "collections");

            migrationBuilder.EnsureSchema(
                name: "auth");

            migrationBuilder.EnsureSchema(
                name: "admin");

            migrationBuilder.EnsureSchema(
                name: "content");

            migrationBuilder.EnsureSchema(
                name: "users");

            migrationBuilder.EnsureSchema(
                name: "moderation");

            migrationBuilder.CreateTable(
                name: "albums",
                schema: "collections",
                columns: table => new
                {
                    id = table.Column<long>(type: "bigint", nullable: false)
                        .Annotation("Npgsql:ValueGenerationStrategy", NpgsqlValueGenerationStrategy.IdentityByDefaultColumn),
                    name = table.Column<string>(type: "text", nullable: false),
                    cover_file_name = table.Column<string>(type: "text", nullable: false),
                    created_at = table.Column<DateTime>(type: "timestamp with time zone", nullable: false, defaultValueSql: "CURRENT_TIMESTAMP"),
                    updated_at = table.Column<DateTime>(type: "timestamp with time zone", nullable: false, defaultValueSql: "CURRENT_TIMESTAMP")
                },
                constraints: table =>
                {
                    table.PrimaryKey("pk_albums_id", x => x.id);
                });

            migrationBuilder.CreateTable(
                name: "auth_providers",
                schema: "auth",
                columns: table => new
                {
                    id = table.Column<short>(type: "smallint", nullable: false)
                        .Annotation("Npgsql:ValueGenerationStrategy", NpgsqlValueGenerationStrategy.IdentityByDefaultColumn),
                    provider_name = table.Column<string>(type: "text", nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("pk_auth_providers_id", x => x.id);
                });

            migrationBuilder.CreateTable(
                name: "genres",
                schema: "content",
                columns: table => new
                {
                    id = table.Column<short>(type: "smallint", nullable: false)
                        .Annotation("Npgsql:ValueGenerationStrategy", NpgsqlValueGenerationStrategy.IdentityByDefaultColumn),
                    genre_name = table.Column<string>(type: "text", nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("pk_genres_id", x => x.id);
                });

            migrationBuilder.CreateTable(
                name: "moods",
                schema: "content",
                columns: table => new
                {
                    id = table.Column<short>(type: "smallint", nullable: false)
                        .Annotation("Npgsql:ValueGenerationStrategy", NpgsqlValueGenerationStrategy.IdentityByDefaultColumn),
                    mood_name = table.Column<string>(type: "text", nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("pk_moods_id", x => x.id);
                });

            migrationBuilder.CreateTable(
                name: "paces",
                schema: "content",
                columns: table => new
                {
                    id = table.Column<short>(type: "smallint", nullable: false)
                        .Annotation("Npgsql:ValueGenerationStrategy", NpgsqlValueGenerationStrategy.IdentityByDefaultColumn),
                    pace_name = table.Column<string>(type: "text", nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("pk_paces_id", x => x.id);
                });

            migrationBuilder.CreateTable(
                name: "reasons",
                schema: "moderation",
                columns: table => new
                {
                    id = table.Column<short>(type: "smallint", nullable: false)
                        .Annotation("Npgsql:ValueGenerationStrategy", NpgsqlValueGenerationStrategy.IdentityByDefaultColumn),
                    reason_name = table.Column<string>(type: "text", nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("pk_types_reason_id", x => x.id);
                });

            migrationBuilder.CreateTable(
                name: "roles",
                schema: "admin",
                columns: table => new
                {
                    id = table.Column<short>(type: "smallint", nullable: false)
                        .Annotation("Npgsql:ValueGenerationStrategy", NpgsqlValueGenerationStrategy.IdentityByDefaultColumn),
                    role_name = table.Column<string>(type: "text", nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("pk_roles_id", x => x.id);
                });

            migrationBuilder.CreateTable(
                name: "types_access",
                schema: "collections",
                columns: table => new
                {
                    id = table.Column<short>(type: "smallint", nullable: false)
                        .Annotation("Npgsql:ValueGenerationStrategy", NpgsqlValueGenerationStrategy.IdentityByDefaultColumn),
                    type_name = table.Column<string>(type: "text", nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("pk_types_access_id", x => x.id);
                });

            migrationBuilder.CreateTable(
                name: "types_notifications",
                schema: "users",
                columns: table => new
                {
                    id = table.Column<short>(type: "smallint", nullable: false)
                        .Annotation("Npgsql:ValueGenerationStrategy", NpgsqlValueGenerationStrategy.IdentityByDefaultColumn),
                    type_name = table.Column<string>(type: "text", nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("pk_types_notifications_id", x => x.id);
                });

            migrationBuilder.CreateTable(
                name: "types_report",
                schema: "moderation",
                columns: table => new
                {
                    id = table.Column<short>(type: "smallint", nullable: false)
                        .Annotation("Npgsql:ValueGenerationStrategy", NpgsqlValueGenerationStrategy.IdentityByDefaultColumn),
                    type_name = table.Column<string>(type: "text", nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("pk_types_report_id", x => x.id);
                });

            migrationBuilder.CreateTable(
                name: "users",
                schema: "users",
                columns: table => new
                {
                    id = table.Column<long>(type: "bigint", nullable: false)
                        .Annotation("Npgsql:ValueGenerationStrategy", NpgsqlValueGenerationStrategy.IdentityByDefaultColumn),
                    lastname = table.Column<string>(type: "text", nullable: true),
                    firstname = table.Column<string>(type: "text", nullable: true),
                    nickname = table.Column<string>(type: "text", nullable: false),
                    email = table.Column<string>(type: "text", nullable: false),
                    password_hash = table.Column<string>(type: "text", nullable: false),
                    gender = table.Column<string>(type: "text", nullable: true),
                    country = table.Column<string>(type: "text", nullable: true),
                    avatar_file_name = table.Column<string>(type: "text", nullable: true),
                    header_file_name = table.Column<string>(type: "text", nullable: true),
                    is_verified = table.Column<bool>(type: "boolean", nullable: false, defaultValue: false),
                    is_email_verified = table.Column<bool>(type: "boolean", nullable: false, defaultValue: false),
                    is_active = table.Column<bool>(type: "boolean", nullable: false, defaultValue: false),
                    last_visit = table.Column<DateTime>(type: "timestamp with time zone", nullable: false, defaultValueSql: "CURRENT_TIMESTAMP"),
                    created_at = table.Column<DateTime>(type: "timestamp with time zone", nullable: false, defaultValueSql: "CURRENT_TIMESTAMP"),
                    updated_at = table.Column<DateTime>(type: "timestamp with time zone", nullable: false, defaultValueSql: "CURRENT_TIMESTAMP")
                },
                constraints: table =>
                {
                    table.PrimaryKey("pk_users_id", x => x.id);
                });

            migrationBuilder.CreateTable(
                name: "tracks",
                schema: "content",
                columns: table => new
                {
                    id = table.Column<long>(type: "bigint", nullable: false)
                        .Annotation("Npgsql:ValueGenerationStrategy", NpgsqlValueGenerationStrategy.IdentityByDefaultColumn),
                    id_pace = table.Column<short>(type: "smallint", nullable: true),
                    id_mood = table.Column<short>(type: "smallint", nullable: true),
                    name = table.Column<string>(type: "text", nullable: false),
                    country = table.Column<string>(type: "text", nullable: true),
                    is_textless = table.Column<bool>(type: "boolean", nullable: true),
                    is_explicit = table.Column<bool>(type: "boolean", nullable: false),
                    cover_file_name = table.Column<string>(type: "text", nullable: false),
                    track_file_name = table.Column<string>(type: "text", nullable: false),
                    duration_sec = table.Column<int>(type: "integer", nullable: false),
                    created_at = table.Column<DateTime>(type: "timestamp with time zone", nullable: false, defaultValueSql: "CURRENT_TIMESTAMP"),
                    updated_at = table.Column<DateTime>(type: "timestamp with time zone", nullable: false, defaultValueSql: "CURRENT_TIMESTAMP")
                },
                constraints: table =>
                {
                    table.PrimaryKey("pk_tracks_id", x => x.id);
                    table.ForeignKey(
                        name: "fk_tracks_id_mood",
                        column: x => x.id_mood,
                        principalSchema: "content",
                        principalTable: "moods",
                        principalColumn: "id",
                        onDelete: ReferentialAction.Cascade);
                    table.ForeignKey(
                        name: "fk_tracks_id_pace",
                        column: x => x.id_pace,
                        principalSchema: "content",
                        principalTable: "paces",
                        principalColumn: "id",
                        onDelete: ReferentialAction.Cascade);
                });

            migrationBuilder.CreateTable(
                name: "employees",
                schema: "admin",
                columns: table => new
                {
                    id = table.Column<int>(type: "integer", nullable: false)
                        .Annotation("Npgsql:ValueGenerationStrategy", NpgsqlValueGenerationStrategy.IdentityByDefaultColumn),
                    id_role = table.Column<short>(type: "smallint", nullable: false),
                    lastname = table.Column<string>(type: "text", nullable: true),
                    firstname = table.Column<string>(type: "text", nullable: true),
                    email = table.Column<string>(type: "text", nullable: false),
                    password_hash = table.Column<string>(type: "text", nullable: false),
                    is_active = table.Column<bool>(type: "boolean", nullable: false, defaultValue: false),
                    last_visit = table.Column<DateTime>(type: "timestamp with time zone", nullable: false, defaultValueSql: "CURRENT_TIMESTAMP"),
                    created_at = table.Column<DateTime>(type: "timestamp with time zone", nullable: false, defaultValueSql: "CURRENT_TIMESTAMP"),
                    updated_at = table.Column<DateTime>(type: "timestamp with time zone", nullable: false, defaultValueSql: "CURRENT_TIMESTAMP")
                },
                constraints: table =>
                {
                    table.PrimaryKey("pk_employees_id", x => x.id);
                    table.ForeignKey(
                        name: "fk_employees_id_role",
                        column: x => x.id_role,
                        principalSchema: "admin",
                        principalTable: "roles",
                        principalColumn: "id",
                        onDelete: ReferentialAction.Cascade);
                });

            migrationBuilder.CreateTable(
                name: "albums_authors",
                schema: "collections",
                columns: table => new
                {
                    id = table.Column<long>(type: "bigint", nullable: false)
                        .Annotation("Npgsql:ValueGenerationStrategy", NpgsqlValueGenerationStrategy.IdentityByDefaultColumn),
                    id_album = table.Column<long>(type: "bigint", nullable: false),
                    id_author = table.Column<long>(type: "bigint", nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("pk_album_authors_id", x => x.id);
                    table.ForeignKey(
                        name: "fk_albums_authors_id_album",
                        column: x => x.id_album,
                        principalSchema: "collections",
                        principalTable: "albums",
                        principalColumn: "id",
                        onDelete: ReferentialAction.Cascade);
                    table.ForeignKey(
                        name: "fk_albums_authors_id_author",
                        column: x => x.id_author,
                        principalSchema: "users",
                        principalTable: "users",
                        principalColumn: "id",
                        onDelete: ReferentialAction.Cascade);
                });

            migrationBuilder.CreateTable(
                name: "auth_users",
                schema: "auth",
                columns: table => new
                {
                    id = table.Column<long>(type: "bigint", nullable: false)
                        .Annotation("Npgsql:ValueGenerationStrategy", NpgsqlValueGenerationStrategy.IdentityByDefaultColumn),
                    id_user = table.Column<long>(type: "bigint", nullable: false),
                    id_provider = table.Column<short>(type: "smallint", nullable: false),
                    external_id = table.Column<long>(type: "bigint", nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("pk_auth_users_id", x => x.id);
                    table.ForeignKey(
                        name: "fk_oauth_users_id_provider",
                        column: x => x.id_provider,
                        principalSchema: "auth",
                        principalTable: "auth_providers",
                        principalColumn: "id",
                        onDelete: ReferentialAction.Cascade);
                    table.ForeignKey(
                        name: "fk_oauth_users_id_user",
                        column: x => x.id_user,
                        principalSchema: "users",
                        principalTable: "users",
                        principalColumn: "id",
                        onDelete: ReferentialAction.Cascade);
                });

            migrationBuilder.CreateTable(
                name: "libraries",
                schema: "collections",
                columns: table => new
                {
                    id = table.Column<long>(type: "bigint", nullable: false)
                        .Annotation("Npgsql:ValueGenerationStrategy", NpgsqlValueGenerationStrategy.IdentityByDefaultColumn),
                    id_user = table.Column<long>(type: "bigint", nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("pk_libraries_id", x => x.id);
                    table.ForeignKey(
                        name: "fk_libraries_id_user",
                        column: x => x.id_user,
                        principalSchema: "users",
                        principalTable: "users",
                        principalColumn: "id",
                        onDelete: ReferentialAction.Cascade);
                });

            migrationBuilder.CreateTable(
                name: "notifications",
                schema: "users",
                columns: table => new
                {
                    id = table.Column<long>(type: "bigint", nullable: false)
                        .Annotation("Npgsql:ValueGenerationStrategy", NpgsqlValueGenerationStrategy.IdentityByDefaultColumn),
                    id_type = table.Column<short>(type: "smallint", nullable: false),
                    id_user = table.Column<long>(type: "bigint", nullable: false),
                    title = table.Column<string>(type: "text", nullable: false),
                    body = table.Column<string>(type: "text", nullable: false),
                    is_read = table.Column<bool>(type: "boolean", nullable: false, defaultValue: false),
                    created_at = table.Column<DateTime>(type: "timestamp with time zone", nullable: false, defaultValueSql: "CURRENT_TIMESTAMP")
                },
                constraints: table =>
                {
                    table.PrimaryKey("pk_notifications_id", x => x.id);
                    table.ForeignKey(
                        name: "fk_notifications_id_type",
                        column: x => x.id_type,
                        principalSchema: "users",
                        principalTable: "types_notifications",
                        principalColumn: "id",
                        onDelete: ReferentialAction.Cascade);
                    table.ForeignKey(
                        name: "fk_notifications_id_user",
                        column: x => x.id_user,
                        principalSchema: "users",
                        principalTable: "users",
                        principalColumn: "id",
                        onDelete: ReferentialAction.Cascade);
                });

            migrationBuilder.CreateTable(
                name: "playlists",
                schema: "collections",
                columns: table => new
                {
                    id = table.Column<long>(type: "bigint", nullable: false)
                        .Annotation("Npgsql:ValueGenerationStrategy", NpgsqlValueGenerationStrategy.IdentityByDefaultColumn),
                    id_user = table.Column<long>(type: "bigint", nullable: false),
                    id_access_type = table.Column<short>(type: "smallint", nullable: false),
                    name = table.Column<string>(type: "text", nullable: false),
                    discription = table.Column<string>(type: "text", nullable: true),
                    cover_file_name = table.Column<string>(type: "text", nullable: true),
                    created_at = table.Column<DateTime>(type: "timestamp with time zone", nullable: false, defaultValueSql: "CURRENT_TIMESTAMP"),
                    updated_at = table.Column<DateTime>(type: "timestamp with time zone", nullable: false, defaultValueSql: "CURRENT_TIMESTAMP")
                },
                constraints: table =>
                {
                    table.PrimaryKey("pk_playlists_id", x => x.id);
                    table.ForeignKey(
                        name: "fk_playlists_id_access_type",
                        column: x => x.id_access_type,
                        principalSchema: "collections",
                        principalTable: "types_access",
                        principalColumn: "id",
                        onDelete: ReferentialAction.Cascade);
                    table.ForeignKey(
                        name: "fk_playlists_id_user",
                        column: x => x.id_user,
                        principalSchema: "users",
                        principalTable: "users",
                        principalColumn: "id",
                        onDelete: ReferentialAction.Cascade);
                });

            migrationBuilder.CreateTable(
                name: "reports",
                schema: "moderation",
                columns: table => new
                {
                    id = table.Column<long>(type: "bigint", nullable: false)
                        .Annotation("Npgsql:ValueGenerationStrategy", NpgsqlValueGenerationStrategy.IdentityByDefaultColumn),
                    id_user = table.Column<long>(type: "bigint", nullable: false),
                    id_type = table.Column<short>(type: "smallint", nullable: false),
                    id_reason = table.Column<short>(type: "smallint", nullable: false),
                    discription = table.Column<string>(type: "text", nullable: true),
                    is_active = table.Column<bool>(type: "boolean", nullable: false, defaultValue: true),
                    created_at = table.Column<DateTime>(type: "timestamp with time zone", nullable: false, defaultValueSql: "CURRENT_TIMESTAMP")
                },
                constraints: table =>
                {
                    table.PrimaryKey("pk_reports_id", x => x.id);
                    table.ForeignKey(
                        name: "fk_reports_id_reason",
                        column: x => x.id_reason,
                        principalSchema: "moderation",
                        principalTable: "reasons",
                        principalColumn: "id",
                        onDelete: ReferentialAction.Cascade);
                    table.ForeignKey(
                        name: "fk_reports_id_type",
                        column: x => x.id_type,
                        principalSchema: "moderation",
                        principalTable: "types_report",
                        principalColumn: "id",
                        onDelete: ReferentialAction.Cascade);
                    table.ForeignKey(
                        name: "fk_reports_id_user",
                        column: x => x.id_user,
                        principalSchema: "users",
                        principalTable: "users",
                        principalColumn: "id",
                        onDelete: ReferentialAction.Cascade);
                });

            migrationBuilder.CreateTable(
                name: "saved_albums",
                schema: "collections",
                columns: table => new
                {
                    id = table.Column<long>(type: "bigint", nullable: false)
                        .Annotation("Npgsql:ValueGenerationStrategy", NpgsqlValueGenerationStrategy.IdentityByDefaultColumn),
                    id_album = table.Column<long>(type: "bigint", nullable: false),
                    id_user = table.Column<long>(type: "bigint", nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("pk_album_users_id", x => x.id);
                    table.ForeignKey(
                        name: "fk_album_users_id_album",
                        column: x => x.id_album,
                        principalSchema: "collections",
                        principalTable: "albums",
                        principalColumn: "id",
                        onDelete: ReferentialAction.Cascade);
                    table.ForeignKey(
                        name: "fk_albums_users_id_user",
                        column: x => x.id_user,
                        principalSchema: "users",
                        principalTable: "users",
                        principalColumn: "id",
                        onDelete: ReferentialAction.Cascade);
                });

            migrationBuilder.CreateTable(
                name: "users_subs",
                schema: "users",
                columns: table => new
                {
                    id = table.Column<long>(type: "bigint", nullable: false)
                        .Annotation("Npgsql:ValueGenerationStrategy", NpgsqlValueGenerationStrategy.IdentityByDefaultColumn),
                    id_user = table.Column<long>(type: "bigint", nullable: false),
                    id_subscriber = table.Column<long>(type: "bigint", nullable: false),
                    subscribed_at = table.Column<DateTime>(type: "timestamp with time zone", nullable: false, defaultValueSql: "CURRENT_TIMESTAMP")
                },
                constraints: table =>
                {
                    table.PrimaryKey("pk_users_subs_id", x => x.id);
                    table.ForeignKey(
                        name: "fk_users_subs_id_subscriber",
                        column: x => x.id_subscriber,
                        principalSchema: "users",
                        principalTable: "users",
                        principalColumn: "id",
                        onDelete: ReferentialAction.Cascade);
                    table.ForeignKey(
                        name: "fk_users_subs_id_user",
                        column: x => x.id_user,
                        principalSchema: "users",
                        principalTable: "users",
                        principalColumn: "id",
                        onDelete: ReferentialAction.Cascade);
                });

            migrationBuilder.CreateTable(
                name: "albums_tracks",
                schema: "collections",
                columns: table => new
                {
                    id = table.Column<long>(type: "bigint", nullable: false)
                        .Annotation("Npgsql:ValueGenerationStrategy", NpgsqlValueGenerationStrategy.IdentityByDefaultColumn),
                    id_album = table.Column<long>(type: "bigint", nullable: false),
                    id_track = table.Column<long>(type: "bigint", nullable: false),
                    created_at = table.Column<DateTime>(type: "timestamp with time zone", nullable: false, defaultValueSql: "CURRENT_TIMESTAMP")
                },
                constraints: table =>
                {
                    table.PrimaryKey("pk_albums_tracks_id", x => x.id);
                    table.ForeignKey(
                        name: "fk_albums_tracks_id_album",
                        column: x => x.id_album,
                        principalSchema: "collections",
                        principalTable: "albums",
                        principalColumn: "id",
                        onDelete: ReferentialAction.Cascade);
                    table.ForeignKey(
                        name: "fk_albums_tracks_id_track",
                        column: x => x.id_track,
                        principalSchema: "content",
                        principalTable: "tracks",
                        principalColumn: "id",
                        onDelete: ReferentialAction.Cascade);
                });

            migrationBuilder.CreateTable(
                name: "tracks_authors",
                schema: "content",
                columns: table => new
                {
                    id = table.Column<long>(type: "bigint", nullable: false)
                        .Annotation("Npgsql:ValueGenerationStrategy", NpgsqlValueGenerationStrategy.IdentityByDefaultColumn),
                    id_track = table.Column<long>(type: "bigint", nullable: false),
                    id_author = table.Column<long>(type: "bigint", nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("pk_tracks_authors_id", x => x.id);
                    table.ForeignKey(
                        name: "fk_tracks_authors_id_author",
                        column: x => x.id_author,
                        principalSchema: "users",
                        principalTable: "users",
                        principalColumn: "id",
                        onDelete: ReferentialAction.Cascade);
                    table.ForeignKey(
                        name: "fk_tracks_authors_id_track",
                        column: x => x.id_track,
                        principalSchema: "content",
                        principalTable: "tracks",
                        principalColumn: "id",
                        onDelete: ReferentialAction.Cascade);
                });

            migrationBuilder.CreateTable(
                name: "tracks_genres",
                schema: "content",
                columns: table => new
                {
                    id = table.Column<long>(type: "bigint", nullable: false)
                        .Annotation("Npgsql:ValueGenerationStrategy", NpgsqlValueGenerationStrategy.IdentityByDefaultColumn),
                    id_track = table.Column<long>(type: "bigint", nullable: false),
                    id_genre = table.Column<short>(type: "smallint", nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("pk_track_genres_id", x => x.id);
                    table.ForeignKey(
                        name: "fk_track_genres_id_genre",
                        column: x => x.id_genre,
                        principalSchema: "content",
                        principalTable: "genres",
                        principalColumn: "id",
                        onDelete: ReferentialAction.Cascade);
                    table.ForeignKey(
                        name: "fk_track_genres_id_track",
                        column: x => x.id_track,
                        principalSchema: "content",
                        principalTable: "tracks",
                        principalColumn: "id",
                        onDelete: ReferentialAction.Cascade);
                });

            migrationBuilder.CreateTable(
                name: "users_history",
                schema: "users",
                columns: table => new
                {
                    id = table.Column<long>(type: "bigint", nullable: false)
                        .Annotation("Npgsql:ValueGenerationStrategy", NpgsqlValueGenerationStrategy.IdentityByDefaultColumn),
                    id_user = table.Column<long>(type: "bigint", nullable: false),
                    id_track = table.Column<long>(type: "bigint", nullable: false),
                    total_listening_sec = table.Column<int>(type: "integer", nullable: false),
                    listening_at = table.Column<DateTime>(type: "timestamp with time zone", nullable: false, defaultValueSql: "CURRENT_TIMESTAMP")
                },
                constraints: table =>
                {
                    table.PrimaryKey("pk_tracks_users_id", x => x.id);
                    table.ForeignKey(
                        name: "fk_tracks_users_id_track",
                        column: x => x.id_track,
                        principalSchema: "content",
                        principalTable: "tracks",
                        principalColumn: "id",
                        onDelete: ReferentialAction.Cascade);
                    table.ForeignKey(
                        name: "fk_tracks_users_id_user",
                        column: x => x.id_user,
                        principalSchema: "users",
                        principalTable: "users",
                        principalColumn: "id",
                        onDelete: ReferentialAction.Cascade);
                });

            migrationBuilder.CreateTable(
                name: "auth_employees",
                schema: "auth",
                columns: table => new
                {
                    id = table.Column<int>(type: "integer", nullable: false)
                        .Annotation("Npgsql:ValueGenerationStrategy", NpgsqlValueGenerationStrategy.IdentityByDefaultColumn),
                    id_employee = table.Column<int>(type: "integer", nullable: false),
                    id_provider = table.Column<short>(type: "smallint", nullable: false),
                    external_id = table.Column<long>(type: "bigint", nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("pk_auth_employees_id", x => x.id);
                    table.ForeignKey(
                        name: "fk_oauth_employees_id_employee",
                        column: x => x.id_employee,
                        principalSchema: "admin",
                        principalTable: "employees",
                        principalColumn: "id",
                        onDelete: ReferentialAction.Cascade);
                    table.ForeignKey(
                        name: "fk_oauth_users_id_provider",
                        column: x => x.id_provider,
                        principalSchema: "auth",
                        principalTable: "auth_providers",
                        principalColumn: "id",
                        onDelete: ReferentialAction.Cascade);
                });

            migrationBuilder.CreateTable(
                name: "libraries_tracks",
                schema: "collections",
                columns: table => new
                {
                    id = table.Column<long>(type: "bigint", nullable: false)
                        .Annotation("Npgsql:ValueGenerationStrategy", NpgsqlValueGenerationStrategy.IdentityByDefaultColumn),
                    id_library = table.Column<long>(type: "bigint", nullable: false),
                    id_track = table.Column<long>(type: "bigint", nullable: false),
                    created_at = table.Column<DateTime>(type: "timestamp with time zone", nullable: false, defaultValueSql: "CURRENT_TIMESTAMP")
                },
                constraints: table =>
                {
                    table.PrimaryKey("pk_libraries_tracks_id", x => x.id);
                    table.ForeignKey(
                        name: "fk_libraries_tracks_id_library",
                        column: x => x.id_library,
                        principalSchema: "collections",
                        principalTable: "libraries",
                        principalColumn: "id",
                        onDelete: ReferentialAction.Cascade);
                    table.ForeignKey(
                        name: "fk_libraries_tracks_id_track",
                        column: x => x.id_track,
                        principalSchema: "content",
                        principalTable: "tracks",
                        principalColumn: "id",
                        onDelete: ReferentialAction.Cascade);
                });

            migrationBuilder.CreateTable(
                name: "playlists_tracks",
                schema: "collections",
                columns: table => new
                {
                    id = table.Column<long>(type: "bigint", nullable: false)
                        .Annotation("Npgsql:ValueGenerationStrategy", NpgsqlValueGenerationStrategy.IdentityByDefaultColumn),
                    id_playlist = table.Column<long>(type: "bigint", nullable: false),
                    id_track = table.Column<long>(type: "bigint", nullable: false),
                    created_at = table.Column<DateTime>(type: "timestamp with time zone", nullable: false, defaultValueSql: "CURRENT_TIMESTAMP")
                },
                constraints: table =>
                {
                    table.PrimaryKey("pk_playlists_tracks_id", x => x.id);
                    table.ForeignKey(
                        name: "fk_playlists_tracks_id_playlist",
                        column: x => x.id_playlist,
                        principalSchema: "collections",
                        principalTable: "playlists",
                        principalColumn: "id",
                        onDelete: ReferentialAction.Cascade);
                    table.ForeignKey(
                        name: "fk_playlists_tracks_id_track",
                        column: x => x.id_track,
                        principalSchema: "content",
                        principalTable: "tracks",
                        principalColumn: "id",
                        onDelete: ReferentialAction.Cascade);
                });

            migrationBuilder.CreateTable(
                name: "saved_playlists",
                schema: "collections",
                columns: table => new
                {
                    id = table.Column<long>(type: "bigint", nullable: false)
                        .Annotation("Npgsql:ValueGenerationStrategy", NpgsqlValueGenerationStrategy.IdentityByDefaultColumn),
                    id_playlist = table.Column<long>(type: "bigint", nullable: false),
                    id_user = table.Column<long>(type: "bigint", nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("pk_playlists_users_id", x => x.id);
                    table.ForeignKey(
                        name: "fk_playlists_users_id_playlist",
                        column: x => x.id_playlist,
                        principalSchema: "collections",
                        principalTable: "playlists",
                        principalColumn: "id",
                        onDelete: ReferentialAction.Cascade);
                    table.ForeignKey(
                        name: "fk_playlists_users_id_user",
                        column: x => x.id_user,
                        principalSchema: "users",
                        principalTable: "users",
                        principalColumn: "id",
                        onDelete: ReferentialAction.Cascade);
                });

            migrationBuilder.CreateTable(
                name: "report_answers",
                schema: "moderation",
                columns: table => new
                {
                    id = table.Column<long>(type: "bigint", nullable: false)
                        .Annotation("Npgsql:ValueGenerationStrategy", NpgsqlValueGenerationStrategy.IdentityByDefaultColumn),
                    id_employee = table.Column<int>(type: "integer", nullable: false),
                    id_report = table.Column<long>(type: "bigint", nullable: false),
                    message = table.Column<string>(type: "text", nullable: false),
                    created_at = table.Column<DateTime>(type: "timestamp with time zone", nullable: false, defaultValueSql: "CURRENT_TIMESTAMP")
                },
                constraints: table =>
                {
                    table.PrimaryKey("pk_report_answers_id", x => x.id);
                    table.ForeignKey(
                        name: "fk_report_answers_id_employee",
                        column: x => x.id_employee,
                        principalSchema: "admin",
                        principalTable: "employees",
                        principalColumn: "id",
                        onDelete: ReferentialAction.Cascade);
                    table.ForeignKey(
                        name: "fk_report_answers_id_report",
                        column: x => x.id_report,
                        principalSchema: "moderation",
                        principalTable: "reports",
                        principalColumn: "id",
                        onDelete: ReferentialAction.Cascade);
                });

            migrationBuilder.CreateIndex(
                name: "IX_albums_authors_id_author",
                schema: "collections",
                table: "albums_authors",
                column: "id_author");

            migrationBuilder.CreateIndex(
                name: "uq_albums_authors",
                schema: "collections",
                table: "albums_authors",
                columns: new[] { "id_album", "id_author" },
                unique: true);

            migrationBuilder.CreateIndex(
                name: "IX_albums_tracks_id_track",
                schema: "collections",
                table: "albums_tracks",
                column: "id_track");

            migrationBuilder.CreateIndex(
                name: "uq_albums_tracks",
                schema: "collections",
                table: "albums_tracks",
                columns: new[] { "id_album", "id_track" },
                unique: true);

            migrationBuilder.CreateIndex(
                name: "IX_auth_employees_id_provider",
                schema: "auth",
                table: "auth_employees",
                column: "id_provider");

            migrationBuilder.CreateIndex(
                name: "uq_auth_employees_employee_provider",
                schema: "auth",
                table: "auth_employees",
                columns: new[] { "id_employee", "id_provider" },
                unique: true);

            migrationBuilder.CreateIndex(
                name: "IX_auth_users_id_provider",
                schema: "auth",
                table: "auth_users",
                column: "id_provider");

            migrationBuilder.CreateIndex(
                name: "uq_auth_users_id_user_id_provider",
                schema: "auth",
                table: "auth_users",
                columns: new[] { "id_user", "id_provider" },
                unique: true);

            migrationBuilder.CreateIndex(
                name: "IX_employees_id_role",
                schema: "admin",
                table: "employees",
                column: "id_role");

            migrationBuilder.CreateIndex(
                name: "uq_employees_email",
                schema: "admin",
                table: "employees",
                column: "email",
                unique: true);

            migrationBuilder.CreateIndex(
                name: "IX_libraries_id_user",
                schema: "collections",
                table: "libraries",
                column: "id_user");

            migrationBuilder.CreateIndex(
                name: "IX_libraries_tracks_id_track",
                schema: "collections",
                table: "libraries_tracks",
                column: "id_track");

            migrationBuilder.CreateIndex(
                name: "uq_libraries_tracks",
                schema: "collections",
                table: "libraries_tracks",
                columns: new[] { "id_library", "id_track" },
                unique: true);

            migrationBuilder.CreateIndex(
                name: "IX_notifications_id_type",
                schema: "users",
                table: "notifications",
                column: "id_type");

            migrationBuilder.CreateIndex(
                name: "IX_notifications_id_user",
                schema: "users",
                table: "notifications",
                column: "id_user");

            migrationBuilder.CreateIndex(
                name: "IX_playlists_id_access_type",
                schema: "collections",
                table: "playlists",
                column: "id_access_type");

            migrationBuilder.CreateIndex(
                name: "IX_playlists_id_user",
                schema: "collections",
                table: "playlists",
                column: "id_user");

            migrationBuilder.CreateIndex(
                name: "IX_playlists_tracks_id_track",
                schema: "collections",
                table: "playlists_tracks",
                column: "id_track");

            migrationBuilder.CreateIndex(
                name: "uq_playlists_tracks",
                schema: "collections",
                table: "playlists_tracks",
                columns: new[] { "id_playlist", "id_track" },
                unique: true);

            migrationBuilder.CreateIndex(
                name: "IX_report_answers_id_employee",
                schema: "moderation",
                table: "report_answers",
                column: "id_employee");

            migrationBuilder.CreateIndex(
                name: "IX_report_answers_id_report",
                schema: "moderation",
                table: "report_answers",
                column: "id_report");

            migrationBuilder.CreateIndex(
                name: "IX_reports_id_reason",
                schema: "moderation",
                table: "reports",
                column: "id_reason");

            migrationBuilder.CreateIndex(
                name: "IX_reports_id_type",
                schema: "moderation",
                table: "reports",
                column: "id_type");

            migrationBuilder.CreateIndex(
                name: "IX_reports_id_user",
                schema: "moderation",
                table: "reports",
                column: "id_user");

            migrationBuilder.CreateIndex(
                name: "IX_saved_albums_id_user",
                schema: "collections",
                table: "saved_albums",
                column: "id_user");

            migrationBuilder.CreateIndex(
                name: "uq_album_users",
                schema: "collections",
                table: "saved_albums",
                columns: new[] { "id_album", "id_user" },
                unique: true);

            migrationBuilder.CreateIndex(
                name: "IX_saved_playlists_id_user",
                schema: "collections",
                table: "saved_playlists",
                column: "id_user");

            migrationBuilder.CreateIndex(
                name: "uq_playlists_users",
                schema: "collections",
                table: "saved_playlists",
                columns: new[] { "id_playlist", "id_user" },
                unique: true);

            migrationBuilder.CreateIndex(
                name: "IX_tracks_id_mood",
                schema: "content",
                table: "tracks",
                column: "id_mood");

            migrationBuilder.CreateIndex(
                name: "IX_tracks_id_pace",
                schema: "content",
                table: "tracks",
                column: "id_pace");

            migrationBuilder.CreateIndex(
                name: "uq_tracks_name",
                schema: "content",
                table: "tracks",
                column: "name",
                unique: true);

            migrationBuilder.CreateIndex(
                name: "IX_tracks_authors_id_author",
                schema: "content",
                table: "tracks_authors",
                column: "id_author");

            migrationBuilder.CreateIndex(
                name: "IX_tracks_authors_id_track",
                schema: "content",
                table: "tracks_authors",
                column: "id_track");

            migrationBuilder.CreateIndex(
                name: "IX_tracks_genres_id_genre",
                schema: "content",
                table: "tracks_genres",
                column: "id_genre");

            migrationBuilder.CreateIndex(
                name: "uq_track_genres",
                schema: "content",
                table: "tracks_genres",
                columns: new[] { "id_track", "id_genre" },
                unique: true);

            migrationBuilder.CreateIndex(
                name: "uq_users_email",
                schema: "users",
                table: "users",
                column: "email",
                unique: true);

            migrationBuilder.CreateIndex(
                name: "uq_users_nickname",
                schema: "users",
                table: "users",
                column: "nickname",
                unique: true);

            migrationBuilder.CreateIndex(
                name: "IX_users_history_id_track",
                schema: "users",
                table: "users_history",
                column: "id_track");

            migrationBuilder.CreateIndex(
                name: "uk_users_history",
                schema: "users",
                table: "users_history",
                columns: new[] { "id_user", "id_track" },
                unique: true);

            migrationBuilder.CreateIndex(
                name: "IX_users_subs_id_subscriber",
                schema: "users",
                table: "users_subs",
                column: "id_subscriber");

            migrationBuilder.CreateIndex(
                name: "uq_users_subs_id_user_id_subscriber",
                schema: "users",
                table: "users_subs",
                columns: new[] { "id_user", "id_subscriber" },
                unique: true);
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropTable(
                name: "albums_authors",
                schema: "collections");

            migrationBuilder.DropTable(
                name: "albums_tracks",
                schema: "collections");

            migrationBuilder.DropTable(
                name: "auth_employees",
                schema: "auth");

            migrationBuilder.DropTable(
                name: "auth_users",
                schema: "auth");

            migrationBuilder.DropTable(
                name: "libraries_tracks",
                schema: "collections");

            migrationBuilder.DropTable(
                name: "notifications",
                schema: "users");

            migrationBuilder.DropTable(
                name: "playlists_tracks",
                schema: "collections");

            migrationBuilder.DropTable(
                name: "report_answers",
                schema: "moderation");

            migrationBuilder.DropTable(
                name: "saved_albums",
                schema: "collections");

            migrationBuilder.DropTable(
                name: "saved_playlists",
                schema: "collections");

            migrationBuilder.DropTable(
                name: "tracks_authors",
                schema: "content");

            migrationBuilder.DropTable(
                name: "tracks_genres",
                schema: "content");

            migrationBuilder.DropTable(
                name: "users_history",
                schema: "users");

            migrationBuilder.DropTable(
                name: "users_subs",
                schema: "users");

            migrationBuilder.DropTable(
                name: "auth_providers",
                schema: "auth");

            migrationBuilder.DropTable(
                name: "libraries",
                schema: "collections");

            migrationBuilder.DropTable(
                name: "types_notifications",
                schema: "users");

            migrationBuilder.DropTable(
                name: "employees",
                schema: "admin");

            migrationBuilder.DropTable(
                name: "reports",
                schema: "moderation");

            migrationBuilder.DropTable(
                name: "albums",
                schema: "collections");

            migrationBuilder.DropTable(
                name: "playlists",
                schema: "collections");

            migrationBuilder.DropTable(
                name: "genres",
                schema: "content");

            migrationBuilder.DropTable(
                name: "tracks",
                schema: "content");

            migrationBuilder.DropTable(
                name: "roles",
                schema: "admin");

            migrationBuilder.DropTable(
                name: "reasons",
                schema: "moderation");

            migrationBuilder.DropTable(
                name: "types_report",
                schema: "moderation");

            migrationBuilder.DropTable(
                name: "types_access",
                schema: "collections");

            migrationBuilder.DropTable(
                name: "users",
                schema: "users");

            migrationBuilder.DropTable(
                name: "moods",
                schema: "content");

            migrationBuilder.DropTable(
                name: "paces",
                schema: "content");
        }
    }
}
