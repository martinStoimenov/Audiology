namespace Audiology.Data.Migrations
{
    using Microsoft.EntityFrameworkCore.Migrations;

    public partial class IsPrivatePlaylistAndSongsAdditionalInfo : Migration
    {
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.AlterColumn<string>(
                name: "Name",
                table: "Songs",
                maxLength: 100,
                nullable: false,
                oldClrType: typeof(string),
                oldType: "nvarchar(50)",
                oldMaxLength: 50);

            migrationBuilder.AlterColumn<string>(
                name: "Description",
                table: "Songs",
                maxLength: 1000,
                nullable: true,
                oldClrType: typeof(string),
                oldType: "nvarchar(100)",
                oldMaxLength: 100,
                oldNullable: true);

            migrationBuilder.AddColumn<string>(
                name: "Featuring",
                table: "Songs",
                maxLength: 100,
                nullable: true);

            migrationBuilder.AddColumn<string>(
                name: "SoundcloudUrl",
                table: "Songs",
                maxLength: 500,
                nullable: true);

            migrationBuilder.AddColumn<string>(
                name: "WrittenBy",
                table: "Songs",
                maxLength: 100,
                nullable: true);

            migrationBuilder.AddColumn<string>(
                name: "YoutubeUrl",
                table: "Songs",
                maxLength: 500,
                nullable: true);

            migrationBuilder.AlterColumn<int>(
                name: "SongId",
                table: "Playlists",
                nullable: true,
                oldClrType: typeof(int),
                oldType: "int");

            migrationBuilder.AddColumn<bool>(
                name: "IsPrivate",
                table: "Playlists",
                nullable: false,
                defaultValue: false);
        }

        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropColumn(
                name: "Featuring",
                table: "Songs");

            migrationBuilder.DropColumn(
                name: "SoundcloudUrl",
                table: "Songs");

            migrationBuilder.DropColumn(
                name: "WrittenBy",
                table: "Songs");

            migrationBuilder.DropColumn(
                name: "YoutubeUrl",
                table: "Songs");

            migrationBuilder.DropColumn(
                name: "IsPrivate",
                table: "Playlists");

            migrationBuilder.AlterColumn<string>(
                name: "Name",
                table: "Songs",
                type: "nvarchar(50)",
                maxLength: 50,
                nullable: false,
                oldClrType: typeof(string),
                oldMaxLength: 100);

            migrationBuilder.AlterColumn<string>(
                name: "Description",
                table: "Songs",
                type: "nvarchar(100)",
                maxLength: 100,
                nullable: true,
                oldClrType: typeof(string),
                oldMaxLength: 1000,
                oldNullable: true);

            migrationBuilder.AlterColumn<int>(
                name: "SongId",
                table: "Playlists",
                type: "int",
                nullable: false,
                oldClrType: typeof(int),
                oldNullable: true);
        }
    }
}
