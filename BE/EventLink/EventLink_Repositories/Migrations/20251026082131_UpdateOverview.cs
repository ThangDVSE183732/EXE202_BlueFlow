using System;
using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace EventLink_Repositories.Migrations
{
    /// <inheritdoc />
    public partial class UpdateOverview : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropForeignKey(
                name: "FK_Partnerships_Events",
                table: "Partnerships");

            migrationBuilder.AddColumn<decimal>(
                name: "AverageRating",
                table: "Events",
                type: "decimal(18,2)",
                nullable: true);

            migrationBuilder.AddColumn<string>(
                name: "EventHighlights",
                table: "Events",
                type: "nvarchar(max)",
                nullable: true);

            migrationBuilder.AddColumn<int>(
                name: "ReviewCount",
                table: "Events",
                type: "int",
                nullable: true);

            migrationBuilder.AddColumn<string>(
                name: "Tags",
                table: "Events",
                type: "nvarchar(max)",
                nullable: true);

            migrationBuilder.AddColumn<string>(
                name: "TargetAudienceList",
                table: "Events",
                type: "nvarchar(max)",
                nullable: true);

            migrationBuilder.AddColumn<Guid>(
                name: "EventId1",
                table: "EventActivities",
                type: "uniqueidentifier",
                nullable: true);

            migrationBuilder.CreateIndex(
                name: "IX_EventActivities_EventId1",
                table: "EventActivities",
                column: "EventId1");

            migrationBuilder.AddForeignKey(
                name: "FK_EventActivities_Events_EventId1",
                table: "EventActivities",
                column: "EventId1",
                principalTable: "Events",
                principalColumn: "Id");

            migrationBuilder.AddForeignKey(
                name: "FK_Partnerships_Events_EventId",
                table: "Partnerships",
                column: "EventId",
                principalTable: "Events",
                principalColumn: "Id",
                onDelete: ReferentialAction.Cascade);
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropForeignKey(
                name: "FK_EventActivities_Events_EventId1",
                table: "EventActivities");

            migrationBuilder.DropForeignKey(
                name: "FK_Partnerships_Events_EventId",
                table: "Partnerships");

            migrationBuilder.DropIndex(
                name: "IX_EventActivities_EventId1",
                table: "EventActivities");

            migrationBuilder.DropColumn(
                name: "AverageRating",
                table: "Events");

            migrationBuilder.DropColumn(
                name: "EventHighlights",
                table: "Events");

            migrationBuilder.DropColumn(
                name: "ReviewCount",
                table: "Events");

            migrationBuilder.DropColumn(
                name: "Tags",
                table: "Events");

            migrationBuilder.DropColumn(
                name: "TargetAudienceList",
                table: "Events");

            migrationBuilder.DropColumn(
                name: "EventId1",
                table: "EventActivities");

            migrationBuilder.AddForeignKey(
                name: "FK_Partnerships_Events",
                table: "Partnerships",
                column: "EventId",
                principalTable: "Events",
                principalColumn: "Id",
                onDelete: ReferentialAction.Cascade);
        }
    }
}
