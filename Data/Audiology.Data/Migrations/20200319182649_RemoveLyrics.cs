using System;
using Microsoft.EntityFrameworkCore.Migrations;

namespace Audiology.Data.Migrations
{
    public partial class RemoveLyrics : Migration
    {
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropForeignKey(
                name: "FK_Songs_Lyrics_LyricsId",
                table: "Songs");

            migrationBuilder.DropTable(
                name: "Lyrics");

            migrationBuilder.DropIndex(
                name: "IX_Songs_LyricsId",
                table: "Songs");

            migrationBuilder.DropColumn(
                name: "LyricsId",
                table: "Songs");

            migrationBuilder.AddColumn<string>(
                name: "Producer",
                table: "Songs",
                maxLength: 100,
                nullable: true);
        }

        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropColumn(
                name: "Producer",
                table: "Songs");

            migrationBuilder.AddColumn<int>(
                name: "LyricsId",
                table: "Songs",
                type: "int",
                nullable: true);

            migrationBuilder.CreateTable(
                name: "Lyrics",
                columns: table => new
                {
                    Id = table.Column<int>(type: "int", nullable: false)
                        .Annotation("SqlServer:Identity", "1, 1"),
                    CreatedOn = table.Column<DateTime>(type: "datetime2", nullable: false),
                    DeletedOn = table.Column<DateTime>(type: "datetime2", nullable: true),
                    IsDeleted = table.Column<bool>(type: "bit", nullable: false),
                    ModifiedOn = table.Column<DateTime>(type: "datetime2", nullable: true),
                    SongId = table.Column<string>(type: "nvarchar(max)", nullable: false),
                    Text = table.Column<string>(type: "nvarchar(max)", maxLength: 10000, nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_Lyrics", x => x.Id);
                });

            migrationBuilder.CreateIndex(
                name: "IX_Songs_LyricsId",
                table: "Songs",
                column: "LyricsId",
                unique: true,
                filter: "[LyricsId] IS NOT NULL");

            migrationBuilder.CreateIndex(
                name: "IX_Lyrics_IsDeleted",
                table: "Lyrics",
                column: "IsDeleted");

            migrationBuilder.AddForeignKey(
                name: "FK_Songs_Lyrics_LyricsId",
                table: "Songs",
                column: "LyricsId",
                principalTable: "Lyrics",
                principalColumn: "Id",
                onDelete: ReferentialAction.Restrict);
        }
    }
}
