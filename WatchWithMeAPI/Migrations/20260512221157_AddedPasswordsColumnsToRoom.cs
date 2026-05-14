using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace WatchWithMeAPI.Migrations
{
    /// <inheritdoc />
    public partial class AddedPasswordsColumnsToRoom : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.AddColumn<string>(
                name: "HostPassword",
                table: "Rooms",
                type: "nvarchar(9)",
                maxLength: 9,
                nullable: false,
                defaultValue: "");

            migrationBuilder.AddColumn<string>(
                name: "ViewerPassword",
                table: "Rooms",
                type: "nvarchar(9)",
                maxLength: 9,
                nullable: false,
                defaultValue: "");
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropColumn(
                name: "HostPassword",
                table: "Rooms");

            migrationBuilder.DropColumn(
                name: "ViewerPassword",
                table: "Rooms");
        }
    }
}
