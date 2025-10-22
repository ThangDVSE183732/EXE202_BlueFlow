using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace EventLink_Repositories.Migrations
{
    /// <inheritdoc />
    public partial class FixUserProfileEntity : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropColumn(
                name: "Bio",
                table: "UserProfiles");

            migrationBuilder.RenameColumn(
                name: "Website",
                table: "UserProfiles",
                newName: "StateProvince");

            migrationBuilder.RenameColumn(
                name: "ProfileImageUrl",
                table: "UserProfiles",
                newName: "StreetAddress");

            migrationBuilder.RenameColumn(
                name: "Location",
                table: "UserProfiles",
                newName: "OfficialEmail");

            migrationBuilder.RenameColumn(
                name: "CoverImageUrl",
                table: "UserProfiles",
                newName: "SocialProfile");

            migrationBuilder.AddColumn<string>(
                name: "City",
                table: "UserProfiles",
                type: "nvarchar(255)",
                maxLength: 255,
                nullable: false,
                defaultValue: "");

            migrationBuilder.AddColumn<string>(
                name: "ContactFullName",
                table: "UserProfiles",
                type: "nvarchar(255)",
                maxLength: 255,
                nullable: false,
                defaultValue: "");

            migrationBuilder.AddColumn<string>(
                name: "CountryRegion",
                table: "UserProfiles",
                type: "nvarchar(255)",
                maxLength: 255,
                nullable: false,
                defaultValue: "");

            migrationBuilder.AddColumn<string>(
                name: "DirectEmail",
                table: "UserProfiles",
                type: "nvarchar(255)",
                maxLength: 255,
                nullable: false,
                defaultValue: "");

            migrationBuilder.AddColumn<string>(
                name: "DirectPhone",
                table: "UserProfiles",
                type: "nvarchar(50)",
                maxLength: 50,
                nullable: false,
                defaultValue: "");

            migrationBuilder.AddColumn<string>(
                name: "JobTitle",
                table: "UserProfiles",
                type: "nvarchar(255)",
                maxLength: 255,
                nullable: false,
                defaultValue: "");

            migrationBuilder.AddColumn<string>(
                name: "LinkedInProfile",
                table: "UserProfiles",
                type: "nvarchar(500)",
                maxLength: 500,
                nullable: false,
                defaultValue: "");
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropColumn(
                name: "City",
                table: "UserProfiles");

            migrationBuilder.DropColumn(
                name: "ContactFullName",
                table: "UserProfiles");

            migrationBuilder.DropColumn(
                name: "CountryRegion",
                table: "UserProfiles");

            migrationBuilder.DropColumn(
                name: "DirectEmail",
                table: "UserProfiles");

            migrationBuilder.DropColumn(
                name: "DirectPhone",
                table: "UserProfiles");

            migrationBuilder.DropColumn(
                name: "JobTitle",
                table: "UserProfiles");

            migrationBuilder.DropColumn(
                name: "LinkedInProfile",
                table: "UserProfiles");

            migrationBuilder.RenameColumn(
                name: "StreetAddress",
                table: "UserProfiles",
                newName: "ProfileImageUrl");

            migrationBuilder.RenameColumn(
                name: "StateProvince",
                table: "UserProfiles",
                newName: "Website");

            migrationBuilder.RenameColumn(
                name: "SocialProfile",
                table: "UserProfiles",
                newName: "CoverImageUrl");

            migrationBuilder.RenameColumn(
                name: "OfficialEmail",
                table: "UserProfiles",
                newName: "Location");

            migrationBuilder.AddColumn<string>(
                name: "Bio",
                table: "UserProfiles",
                type: "nvarchar(2000)",
                maxLength: 2000,
                nullable: false,
                defaultValue: "");
        }
    }
}
