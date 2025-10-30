using System;
using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace EventLink_Repositories.Migrations
{
    /// <inheritdoc />
    public partial class AddEventProposal : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.AlterColumn<string>(
                name: "CompanyLogoUrl",
                table: "UserProfiles",
                type: "nvarchar(500)",
                maxLength: 500,
                nullable: true,
                oldClrType: typeof(string),
                oldType: "nvarchar(500)",
                oldMaxLength: 500);

            migrationBuilder.AddColumn<DateTime>(
                name: "ApprovedAt",
                table: "EventProposals",
                type: "datetime",
                nullable: true);

            migrationBuilder.AddColumn<Guid>(
                name: "ApprovedBy",
                table: "EventProposals",
                type: "uniqueidentifier",
                nullable: true);

            migrationBuilder.AddColumn<string>(
                name: "Benefits",
                table: "EventProposals",
                type: "nvarchar(4000)",
                maxLength: 4000,
                nullable: true);

            migrationBuilder.AddColumn<string>(
                name: "FundingBreakdown",
                table: "EventProposals",
                type: "nvarchar(4000)",
                maxLength: 4000,
                nullable: true);

            migrationBuilder.AddColumn<Guid>(
                name: "ProposedBy",
                table: "EventProposals",
                type: "uniqueidentifier",
                nullable: true);

            migrationBuilder.AddColumn<string>(
                name: "RejectionReason",
                table: "EventProposals",
                type: "nvarchar(1000)",
                maxLength: 1000,
                nullable: true);

            migrationBuilder.AddColumn<string>(
                name: "SponsorTier",
                table: "EventProposals",
                type: "nvarchar(50)",
                maxLength: 50,
                nullable: true);

            migrationBuilder.AddColumn<string>(
                name: "Status",
                table: "EventProposals",
                type: "nvarchar(50)",
                maxLength: 50,
                nullable: true,
                defaultValue: "Pending");

            migrationBuilder.CreateTable(
                name: "EventActivities",
                columns: table => new
                {
                    Id = table.Column<Guid>(type: "uniqueidentifier", nullable: false, defaultValueSql: "(newid())"),
                    EventId = table.Column<Guid>(type: "uniqueidentifier", nullable: false),
                    ActivityName = table.Column<string>(type: "nvarchar(255)", maxLength: 255, nullable: false),
                    ActivityDescription = table.Column<string>(type: "nvarchar(2000)", maxLength: 2000, nullable: true),
                    StartTime = table.Column<TimeSpan>(type: "time", nullable: false),
                    EndTime = table.Column<TimeSpan>(type: "time", nullable: false),
                    Location = table.Column<string>(type: "nvarchar(500)", maxLength: 500, nullable: true),
                    Speakers = table.Column<string>(type: "nvarchar(1000)", maxLength: 1000, nullable: true),
                    ActivityType = table.Column<string>(type: "nvarchar(100)", maxLength: 100, nullable: true),
                    MaxParticipants = table.Column<int>(type: "int", nullable: true, defaultValue: 0),
                    CurrentParticipants = table.Column<int>(type: "int", nullable: true, defaultValue: 0),
                    IsPublic = table.Column<bool>(type: "bit", nullable: true, defaultValue: true),
                    DisplayOrder = table.Column<int>(type: "int", nullable: true, defaultValue: 0),
                    CreatedAt = table.Column<DateTime>(type: "datetime2", nullable: true, defaultValueSql: "(getdate())"),
                    UpdatedAt = table.Column<DateTime>(type: "datetime2", nullable: true, defaultValueSql: "(getdate())")
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK__EventAct__3214EC07", x => x.Id);
                    table.ForeignKey(
                        name: "FK_EventActivities_Events",
                        column: x => x.EventId,
                        principalTable: "Events",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Cascade);
                });

            migrationBuilder.CreateIndex(
                name: "IX_EventProposals_ApprovedBy",
                table: "EventProposals",
                column: "ApprovedBy");

            migrationBuilder.CreateIndex(
                name: "IX_EventProposals_ProposedBy",
                table: "EventProposals",
                column: "ProposedBy");

            migrationBuilder.CreateIndex(
                name: "IX_EventActivities_ActivityType",
                table: "EventActivities",
                column: "ActivityType");

            migrationBuilder.CreateIndex(
                name: "IX_EventActivities_EventId",
                table: "EventActivities",
                column: "EventId");

            migrationBuilder.CreateIndex(
                name: "IX_EventActivities_EventId_StartTime",
                table: "EventActivities",
                columns: new[] { "EventId", "StartTime" });

            migrationBuilder.AddForeignKey(
                name: "FK_EventProposals_Users_ApprovedBy",
                table: "EventProposals",
                column: "ApprovedBy",
                principalTable: "Users",
                principalColumn: "Id");

            migrationBuilder.AddForeignKey(
                name: "FK_EventProposals_Users_ProposedBy",
                table: "EventProposals",
                column: "ProposedBy",
                principalTable: "Users",
                principalColumn: "Id");
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropForeignKey(
                name: "FK_EventProposals_Users_ApprovedBy",
                table: "EventProposals");

            migrationBuilder.DropForeignKey(
                name: "FK_EventProposals_Users_ProposedBy",
                table: "EventProposals");

            migrationBuilder.DropTable(
                name: "EventActivities");

            migrationBuilder.DropIndex(
                name: "IX_EventProposals_ApprovedBy",
                table: "EventProposals");

            migrationBuilder.DropIndex(
                name: "IX_EventProposals_ProposedBy",
                table: "EventProposals");

            migrationBuilder.DropColumn(
                name: "ApprovedAt",
                table: "EventProposals");

            migrationBuilder.DropColumn(
                name: "ApprovedBy",
                table: "EventProposals");

            migrationBuilder.DropColumn(
                name: "Benefits",
                table: "EventProposals");

            migrationBuilder.DropColumn(
                name: "FundingBreakdown",
                table: "EventProposals");

            migrationBuilder.DropColumn(
                name: "ProposedBy",
                table: "EventProposals");

            migrationBuilder.DropColumn(
                name: "RejectionReason",
                table: "EventProposals");

            migrationBuilder.DropColumn(
                name: "SponsorTier",
                table: "EventProposals");

            migrationBuilder.DropColumn(
                name: "Status",
                table: "EventProposals");

            migrationBuilder.AlterColumn<string>(
                name: "CompanyLogoUrl",
                table: "UserProfiles",
                type: "nvarchar(500)",
                maxLength: 500,
                nullable: false,
                defaultValue: "",
                oldClrType: typeof(string),
                oldType: "nvarchar(500)",
                oldMaxLength: 500,
                oldNullable: true);
        }
    }
}
