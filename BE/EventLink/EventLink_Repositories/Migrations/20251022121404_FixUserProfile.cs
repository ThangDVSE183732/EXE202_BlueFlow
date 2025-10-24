using System;
using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace EventLink_Repositories.Migrations
{
    /// <inheritdoc />
    public partial class FixUserProfile : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropForeignKey(
                name: "FK_UserProfiles_Users_UserId",
                table: "UserProfiles");

            migrationBuilder.DropTable(
                name: "OrganizerProfiles");

            migrationBuilder.DropTable(
                name: "SponsorProfiles");

            migrationBuilder.DropTable(
                name: "SupplierProfiles");

            migrationBuilder.AddColumn<string>(
                name: "CompanyDescription",
                table: "UserProfiles",
                type: "nvarchar(2000)",
                maxLength: 2000,
                nullable: false,
                defaultValue: "");

            migrationBuilder.AddColumn<string>(
                name: "CompanyLogoUrl",
                table: "UserProfiles",
                type: "nvarchar(500)",
                maxLength: 500,
                nullable: false,
                defaultValue: "");

            migrationBuilder.AddColumn<string>(
                name: "CompanyName",
                table: "UserProfiles",
                type: "nvarchar(255)",
                maxLength: 255,
                nullable: false,
                defaultValue: "");

            migrationBuilder.AddColumn<string>(
                name: "CompanySize",
                table: "UserProfiles",
                type: "nvarchar(100)",
                maxLength: 100,
                nullable: false,
                defaultValue: "");

            migrationBuilder.AddColumn<DateTime>(
                name: "CreatedAt",
                table: "UserProfiles",
                type: "datetime2",
                nullable: true,
                defaultValueSql: "(getdate())");

            migrationBuilder.AddColumn<int>(
                name: "FoundedYear",
                table: "UserProfiles",
                type: "int",
                nullable: true);

            migrationBuilder.AddColumn<string>(
                name: "Industry",
                table: "UserProfiles",
                type: "nvarchar(255)",
                maxLength: 255,
                nullable: false,
                defaultValue: "");

            migrationBuilder.AddColumn<DateTime>(
                name: "UpdatedAt",
                table: "UserProfiles",
                type: "datetime2",
                nullable: true,
                defaultValueSql: "(getdate())");

            migrationBuilder.AddForeignKey(
                name: "FK_UserProfiles_Users",
                table: "UserProfiles",
                column: "UserId",
                principalTable: "Users",
                principalColumn: "Id",
                onDelete: ReferentialAction.Cascade);
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropForeignKey(
                name: "FK_UserProfiles_Users",
                table: "UserProfiles");

            migrationBuilder.DropColumn(
                name: "CompanyDescription",
                table: "UserProfiles");

            migrationBuilder.DropColumn(
                name: "CompanyLogoUrl",
                table: "UserProfiles");

            migrationBuilder.DropColumn(
                name: "CompanyName",
                table: "UserProfiles");

            migrationBuilder.DropColumn(
                name: "CompanySize",
                table: "UserProfiles");

            migrationBuilder.DropColumn(
                name: "CreatedAt",
                table: "UserProfiles");

            migrationBuilder.DropColumn(
                name: "FoundedYear",
                table: "UserProfiles");

            migrationBuilder.DropColumn(
                name: "Industry",
                table: "UserProfiles");

            migrationBuilder.DropColumn(
                name: "UpdatedAt",
                table: "UserProfiles");

            migrationBuilder.CreateTable(
                name: "OrganizerProfiles",
                columns: table => new
                {
                    Id = table.Column<Guid>(type: "uniqueidentifier", nullable: false, defaultValueSql: "(newid())"),
                    City = table.Column<string>(type: "nvarchar(100)", maxLength: 100, nullable: false),
                    CompanyDescription = table.Column<string>(type: "nvarchar(2000)", maxLength: 2000, nullable: false),
                    CompanyLogoUrl = table.Column<string>(type: "nvarchar(500)", maxLength: 500, nullable: false),
                    CompanyName = table.Column<string>(type: "nvarchar(255)", maxLength: 255, nullable: false),
                    CompanySize = table.Column<string>(type: "nvarchar(100)", maxLength: 100, nullable: false),
                    ContactFullName = table.Column<string>(type: "nvarchar(255)", maxLength: 255, nullable: false),
                    CountryRegion = table.Column<string>(type: "nvarchar(100)", maxLength: 100, nullable: false),
                    DirectEmail = table.Column<string>(type: "nvarchar(255)", maxLength: 255, nullable: false),
                    DirectPhone = table.Column<string>(type: "nvarchar(50)", maxLength: 50, nullable: false),
                    FoundedYear = table.Column<int>(type: "int", nullable: true),
                    Industry = table.Column<string>(type: "nvarchar(255)", maxLength: 255, nullable: false),
                    JobTitle = table.Column<string>(type: "nvarchar(255)", maxLength: 255, nullable: false),
                    LinkedInProfile = table.Column<string>(type: "nvarchar(500)", maxLength: 500, nullable: false),
                    OfficialEmail = table.Column<string>(type: "nvarchar(255)", maxLength: 255, nullable: false),
                    SocialProfile = table.Column<string>(type: "nvarchar(500)", maxLength: 500, nullable: false),
                    StateProvince = table.Column<string>(type: "nvarchar(100)", maxLength: 100, nullable: false),
                    StreetAddress = table.Column<string>(type: "nvarchar(500)", maxLength: 500, nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_OrganizerProfiles", x => x.Id);
                    table.ForeignKey(
                        name: "FK_OrganizerProfiles_UserProfiles_Id",
                        column: x => x.Id,
                        principalTable: "UserProfiles",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Cascade);
                });

            migrationBuilder.CreateTable(
                name: "SponsorProfiles",
                columns: table => new
                {
                    Id = table.Column<Guid>(type: "uniqueidentifier", nullable: false, defaultValueSql: "(newid())"),
                    BrandName = table.Column<string>(type: "nvarchar(max)", nullable: false),
                    BudgetRange = table.Column<string>(type: "nvarchar(max)", nullable: false),
                    SponsorshipFocus = table.Column<string>(type: "nvarchar(max)", nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_SponsorProfiles", x => x.Id);
                    table.ForeignKey(
                        name: "FK_SponsorProfiles_UserProfiles_Id",
                        column: x => x.Id,
                        principalTable: "UserProfiles",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Cascade);
                });

            migrationBuilder.CreateTable(
                name: "SupplierProfiles",
                columns: table => new
                {
                    Id = table.Column<Guid>(type: "uniqueidentifier", nullable: false, defaultValueSql: "(newid())"),
                    Certifications = table.Column<string>(type: "nvarchar(max)", nullable: false),
                    Skills = table.Column<string>(type: "nvarchar(max)", nullable: false),
                    YearsOfExperience = table.Column<int>(type: "int", nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_SupplierProfiles", x => x.Id);
                    table.ForeignKey(
                        name: "FK_SupplierProfiles_UserProfiles_Id",
                        column: x => x.Id,
                        principalTable: "UserProfiles",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Cascade);
                });

            migrationBuilder.AddForeignKey(
                name: "FK_UserProfiles_Users_UserId",
                table: "UserProfiles",
                column: "UserId",
                principalTable: "Users",
                principalColumn: "Id",
                onDelete: ReferentialAction.Cascade);
        }
    }
}
