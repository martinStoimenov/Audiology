namespace Audiology.Data.Migrations
{
    using Microsoft.EntityFrameworkCore.Migrations;

    public partial class ErrorSolvingMigration : Migration
    {
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.AddColumn<int>(
                name: "AlbumId",
                table: "AspNetUsers",
                nullable: true);

            migrationBuilder.AddColumn<int>(
                name: "SongId",
                table: "AspNetUsers",
                nullable: true);

            migrationBuilder.CreateIndex(
                name: "IX_AspNetUsers_AlbumId",
                table: "AspNetUsers",
                column: "AlbumId");

            migrationBuilder.CreateIndex(
                name: "IX_AspNetUsers_SongId",
                table: "AspNetUsers",
                column: "SongId");

            migrationBuilder.AddForeignKey(
                name: "FK_AspNetUsers_Albums_AlbumId",
                table: "AspNetUsers",
                column: "AlbumId",
                principalTable: "Albums",
                principalColumn: "Id",
                onDelete: ReferentialAction.Restrict);

            migrationBuilder.AddForeignKey(
                name: "FK_AspNetUsers_Songs_SongId",
                table: "AspNetUsers",
                column: "SongId",
                principalTable: "Songs",
                principalColumn: "Id",
                onDelete: ReferentialAction.Restrict);
        }

        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropForeignKey(
                name: "FK_AspNetUsers_Albums_AlbumId",
                table: "AspNetUsers");

            migrationBuilder.DropForeignKey(
                name: "FK_AspNetUsers_Songs_SongId",
                table: "AspNetUsers");

            migrationBuilder.DropIndex(
                name: "IX_AspNetUsers_AlbumId",
                table: "AspNetUsers");

            migrationBuilder.DropIndex(
                name: "IX_AspNetUsers_SongId",
                table: "AspNetUsers");

            migrationBuilder.DropColumn(
                name: "AlbumId",
                table: "AspNetUsers");

            migrationBuilder.DropColumn(
                name: "SongId",
                table: "AspNetUsers");
        }
    }
}
