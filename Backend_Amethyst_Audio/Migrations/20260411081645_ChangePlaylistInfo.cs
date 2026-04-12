using Microsoft.EntityFrameworkCore.Migrations;
using Npgsql.EntityFrameworkCore.PostgreSQL.Metadata;

#nullable disable

namespace Backend_Amethyst_Audio.Migrations
{
    /// <inheritdoc />
    public partial class ChangePlaylistInfo : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropForeignKey(
                name: "fk_playlists_id_access_type",
                schema: "collections",
                table: "playlists");

            migrationBuilder.DropTable(
                name: "types_access",
                schema: "collections");

            migrationBuilder.DropIndex(
                name: "IX_playlists_id_access_type",
                schema: "collections",
                table: "playlists");

            migrationBuilder.DropColumn(
                name: "id_access_type",
                schema: "collections",
                table: "playlists");

            migrationBuilder.RenameColumn(
                name: "discription",
                schema: "collections",
                table: "playlists",
                newName: "description");

            migrationBuilder.AddColumn<bool>(
                name: "is_public",
                schema: "collections",
                table: "playlists",
                type: "boolean",
                nullable: false,
                defaultValue: false);
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropColumn(
                name: "is_public",
                schema: "collections",
                table: "playlists");

            migrationBuilder.RenameColumn(
                name: "description",
                schema: "collections",
                table: "playlists",
                newName: "discription");

            migrationBuilder.AddColumn<short>(
                name: "id_access_type",
                schema: "collections",
                table: "playlists",
                type: "smallint",
                nullable: false,
                defaultValue: (short)0);

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

            migrationBuilder.CreateIndex(
                name: "IX_playlists_id_access_type",
                schema: "collections",
                table: "playlists",
                column: "id_access_type");

            migrationBuilder.AddForeignKey(
                name: "fk_playlists_id_access_type",
                schema: "collections",
                table: "playlists",
                column: "id_access_type",
                principalSchema: "collections",
                principalTable: "types_access",
                principalColumn: "id",
                onDelete: ReferentialAction.Cascade);
        }
    }
}
