using Microsoft.EntityFrameworkCore.Migrations;

namespace FaceRecognition.Back.Api.Migrations
{
    public partial class FilesUpdated : Migration
    {
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.AddColumn<int>(
                name: "MimeType",
                table: "Files",
                nullable: false,
                defaultValue: 0);

            migrationBuilder.AddColumn<int>(
                name: "Size",
                table: "Files",
                nullable: false,
                defaultValue: 0);
        }

        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropColumn(
                name: "MimeType",
                table: "Files");

            migrationBuilder.DropColumn(
                name: "Size",
                table: "Files");
        }
    }
}
