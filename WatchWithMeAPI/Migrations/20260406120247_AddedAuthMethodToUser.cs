using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace WatchWithMeAPI.Migrations
{
    /// <inheritdoc />
    public partial class AddedAuthMethodToUser : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.AddColumn<int>(
                name: "AuthenticationMethod",
                table: "AspNetUsers",
                type: "int",
                nullable: false,
                defaultValue: 0);
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropColumn(
                name: "AuthenticationMethod",
                table: "AspNetUsers");
        }
    }
}
