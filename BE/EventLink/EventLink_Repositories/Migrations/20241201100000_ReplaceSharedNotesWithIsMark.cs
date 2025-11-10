using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace EventLink_Repositories.Migrations
{
    /// <inheritdoc />
    public partial class ReplaceSharedNotesWithIsMark : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            // ? Step 1: Drop old SharedNotes column
            migrationBuilder.DropColumn(
                name: "SharedNotes",
                table: "Partnerships");

            // ? Step 2: Add new IsMark column (bit/boolean)
            migrationBuilder.AddColumn<bool>(
                name: "IsMark",
                table: "Partnerships",
                type: "bit",
                nullable: true,
                defaultValue: false);
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            // ? Rollback: Drop IsMark column
            migrationBuilder.DropColumn(
                name: "IsMark",
                table: "Partnerships");

            // ? Rollback: Add back SharedNotes column
            migrationBuilder.AddColumn<string>(
                name: "SharedNotes",
                table: "Partnerships",
                type: "nvarchar(max)",
                nullable: true);
        }
    }
}
