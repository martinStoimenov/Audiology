namespace Audiology.Data.Migrations
{
    using Microsoft.EntityFrameworkCore.Migrations;

    public partial class AddDescAndCoverToAlbum : Migration
    {
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropColumn(
                name: "FileExtension",
                table: "Songs");

            migrationBuilder.AlterColumn<string>(
                name: "ProfilePicUrl",
                table: "AspNetUsers",
                maxLength: 150,
                nullable: true,
                oldClrType: typeof(string),
                oldType: "nvarchar(max)",
                oldNullable: true);

            migrationBuilder.AddColumn<string>(
                name: "CoverUrl",
                table: "Albums",
                maxLength: 150,
                nullable: true);

            migrationBuilder.AddColumn<string>(
                name: "Description",
                table: "Albums",
                maxLength: 100,
                nullable: true);

            migrationBuilder.AddColumn<string>(
                name: "Producer",
                table: "Albums",
                nullable: false,
                defaultValue: "");
        }

        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropColumn(
                name: "CoverUrl",
                table: "Albums");

            migrationBuilder.DropColumn(
                name: "Description",
                table: "Albums");

            migrationBuilder.DropColumn(
                name: "Producer",
                table: "Albums");

            migrationBuilder.AddColumn<int>(
                name: "FileExtension",
                table: "Songs",
                type: "int",
                nullable: false,
                defaultValue: 0);

            migrationBuilder.AlterColumn<string>(
                name: "ProfilePicUrl",
                table: "AspNetUsers",
                type: "nvarchar(max)",
                nullable: true,
                oldClrType: typeof(string),
                oldMaxLength: 150,
                oldNullable: true);
        }
    }
}
