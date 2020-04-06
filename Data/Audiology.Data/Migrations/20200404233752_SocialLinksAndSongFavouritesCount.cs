namespace Audiology.Data.Migrations
{
    using Microsoft.EntityFrameworkCore.Migrations;

    public partial class SocialLinksAndSongFavouritesCount : Migration
    {
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.AddColumn<int>(
                name: "FavouritesCount",
                table: "Songs",
                nullable: true);

            migrationBuilder.AddColumn<string>(
                name: "InstagramPostUrl",
                table: "Songs",
                maxLength: 500,
                nullable: true);

            migrationBuilder.AddColumn<string>(
                name: "FacebookUrl",
                table: "AspNetUsers",
                maxLength: 600,
                nullable: true);

            migrationBuilder.AddColumn<string>(
                name: "InstagramUrl",
                table: "AspNetUsers",
                maxLength: 600,
                nullable: true);

            migrationBuilder.AddColumn<string>(
                name: "SondcloudUrl",
                table: "AspNetUsers",
                maxLength: 600,
                nullable: true);

            migrationBuilder.AddColumn<string>(
                name: "TwitterUrl",
                table: "AspNetUsers",
                maxLength: 600,
                nullable: true);

            migrationBuilder.AddColumn<string>(
                name: "YouTubeUrl",
                table: "AspNetUsers",
                maxLength: 600,
                nullable: true);
        }

        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropColumn(
                name: "FavouritesCount",
                table: "Songs");

            migrationBuilder.DropColumn(
                name: "InstagramPostUrl",
                table: "Songs");

            migrationBuilder.DropColumn(
                name: "FacebookUrl",
                table: "AspNetUsers");

            migrationBuilder.DropColumn(
                name: "InstagramUrl",
                table: "AspNetUsers");

            migrationBuilder.DropColumn(
                name: "SondcloudUrl",
                table: "AspNetUsers");

            migrationBuilder.DropColumn(
                name: "TwitterUrl",
                table: "AspNetUsers");

            migrationBuilder.DropColumn(
                name: "YouTubeUrl",
                table: "AspNetUsers");
        }
    }
}
