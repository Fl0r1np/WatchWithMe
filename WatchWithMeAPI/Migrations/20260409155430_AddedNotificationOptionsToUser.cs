using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace WatchWithMeAPI.Migrations
{
    /// <inheritdoc />
    public partial class AddedNotificationOptionsToUser : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.AddColumn<bool>(
                name: "NotifyBasic",
                table: "AspNetUsers",
                type: "bit",
                nullable: false,
                defaultValue: false);

            migrationBuilder.AddColumn<bool>(
                name: "NotifyInvitations",
                table: "AspNetUsers",
                type: "bit",
                nullable: false,
                defaultValue: false);
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropColumn(
                name: "NotifyBasic",
                table: "AspNetUsers");

            migrationBuilder.DropColumn(
                name: "NotifyInvitations",
                table: "AspNetUsers");
        }
    }
}
