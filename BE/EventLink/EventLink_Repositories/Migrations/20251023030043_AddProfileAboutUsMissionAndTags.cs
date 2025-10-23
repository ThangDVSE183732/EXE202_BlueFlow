using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace EventLink_Repositories.Migrations
{
    /// <inheritdoc />
    public partial class AddProfileAboutUsMissionAndTags : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.AddColumn<string>(
                name: "AboutUs",
                table: "UserProfiles",
                type: "nvarchar(2000)",
                maxLength: 2000,
                nullable: false,
                defaultValue: "");

            migrationBuilder.AddColumn<string>(
                name: "Mission",
                table: "UserProfiles",
                type: "nvarchar(2000)",
                maxLength: 2000,
                nullable: false,
                defaultValue: "");

            migrationBuilder.AddColumn<string>(
                name: "Tags",
                table: "UserProfiles",
                type: "nvarchar(1000)",
                maxLength: 1000,
                nullable: false,
                defaultValue: "");
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropColumn(
                name: "AboutUs",
                table: "UserProfiles");

            migrationBuilder.DropColumn(
                name: "Mission",
                table: "UserProfiles");

            migrationBuilder.DropColumn(
                name: "Tags",
                table: "UserProfiles");
        }
    }
}
