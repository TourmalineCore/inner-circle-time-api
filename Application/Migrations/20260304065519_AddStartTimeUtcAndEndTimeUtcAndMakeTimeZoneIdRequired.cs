using System;
using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace Application.Migrations
{
    /// <inheritdoc />
    public partial class AddStartTimeUtcAndEndTimeUtcAndMakeTimeZoneIdRequired : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.AlterColumn<string>(
                name: "time_zone_id",
                table: "tracked_entries",
                type: "text",
                nullable: false,
                defaultValue: "",
                oldClrType: typeof(string),
                oldType: "text",
                oldNullable: true);

            migrationBuilder.AddColumn<DateTime>(
                name: "end_time_utc",
                table: "tracked_entries",
                type: "timestamptz",
                nullable: false,
                computedColumnSql: "end_time AT TIME ZONE time_zone_id",
                stored: true);

            migrationBuilder.AddColumn<DateTime>(
                name: "start_time_utc",
                table: "tracked_entries",
                type: "timestamptz",
                nullable: false,
                computedColumnSql: "start_time AT TIME ZONE time_zone_id",
                stored: true);
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropColumn(
                name: "end_time_utc",
                table: "tracked_entries");

            migrationBuilder.DropColumn(
                name: "start_time_utc",
                table: "tracked_entries");

            migrationBuilder.AlterColumn<string>(
                name: "time_zone_id",
                table: "tracked_entries",
                type: "text",
                nullable: true,
                oldClrType: typeof(string),
                oldType: "text");
        }
    }
}
