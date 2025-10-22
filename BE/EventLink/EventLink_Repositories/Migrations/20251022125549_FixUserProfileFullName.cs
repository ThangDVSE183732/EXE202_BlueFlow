using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace EventLink_Repositories.Migrations
{
    /// <inheritdoc />
    public partial class FixUserProfileFullName : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.RenameColumn(
                name: "ContactFullName",
                table: "UserProfiles",
                newName: "FullName");
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.RenameColumn(
                name: "FullName",
                table: "UserProfiles",
                newName: "ContactFullName");
        }
    }
}
