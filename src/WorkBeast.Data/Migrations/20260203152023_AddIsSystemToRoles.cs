using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace WorkBeast.Data.Migrations
{
    /// <inheritdoc />
    public partial class AddIsSystemToRoles : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.AddColumn<bool>(
                name: "IsSystem",
                table: "AspNetUsers",
                type: "INTEGER",
                nullable: false,
                defaultValue: false);

            migrationBuilder.AddColumn<bool>(
                name: "IsSystem",
                table: "AspNetRoles",
                type: "INTEGER",
                nullable: false,
                defaultValue: false);
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropColumn(
                name: "IsSystem",
                table: "AspNetUsers");

            migrationBuilder.DropColumn(
                name: "IsSystem",
                table: "AspNetRoles");
        }
    }
}
