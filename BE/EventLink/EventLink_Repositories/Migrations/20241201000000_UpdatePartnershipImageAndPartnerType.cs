using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace EventLink_Repositories.Migrations
{
    /// <inheritdoc />
    public partial class UpdatePartnershipImageAndPartnerType : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            // ? Rename column PartnerNotes to PartnershipImage
            migrationBuilder.RenameColumn(
                name: "PartnerNotes",
                table: "Partnerships",
                newName: "PartnershipImage");

            // ?? Note: PartnerType is a string column, so adding "Organizer" value
            // doesn't require schema change - just update the application code
            // The PartnerType enum already allows any string value
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            // ? Revert: Rename column back to PartnerNotes
            migrationBuilder.RenameColumn(
                name: "PartnershipImage",
                table: "Partnerships",
                newName: "PartnerNotes");
        }
    }
}
