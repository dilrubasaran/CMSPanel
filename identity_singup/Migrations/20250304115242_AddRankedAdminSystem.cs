using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace identity_signup.Migrations
{
    /// <inheritdoc />
    public partial class AddRankedAdminSystem : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.AddColumn<bool>(
                name: "IsRootAdmin",
                table: "AspNetUsers",
                type: "bit",
                nullable: false,
                defaultValue: false);

            migrationBuilder.AddColumn<int>(
                name: "PermissionLevel",
                table: "AspNetRoles",
                type: "int",
                nullable: false,
                defaultValue: 0);
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropColumn(
                name: "IsRootAdmin",
                table: "AspNetUsers");

            migrationBuilder.DropColumn(
                name: "PermissionLevel",
                table: "AspNetRoles");
        }
    }
}
