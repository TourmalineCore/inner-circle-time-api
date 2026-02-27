using System;
using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace Application.Migrations
{
    /// <inheritdoc />
    public partial class ChangeIsDeletedToDeletedAtUtcAndAddDeletionReason : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropColumn(
                name: "is_deleted",
                table: "tracked_entries");

            migrationBuilder.AddColumn<DateTime>(
                name: "deleted_at_utc",
                table: "tracked_entries",
                type: "timestamp with time zone",
                nullable: true);

            migrationBuilder.AddColumn<string>(
                name: "deletion_reason",
                table: "tracked_entries",
                type: "text",
                nullable: true);
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropColumn(
                name: "deleted_at_utc",
                table: "tracked_entries");

            migrationBuilder.DropColumn(
                name: "deletion_reason",
                table: "tracked_entries");

            migrationBuilder.AddColumn<bool>(
                name: "is_deleted",
                table: "tracked_entries",
                type: "boolean",
                nullable: false,
                defaultValue: false);
        }
    }
}
