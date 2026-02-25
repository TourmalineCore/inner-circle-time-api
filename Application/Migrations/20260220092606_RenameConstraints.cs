using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace Application.Migrations
{
    /// <inheritdoc />
    public partial class RenameConstraints : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropCheckConstraint(
                name: "ck_work_entries_end_time_is_greater_than_start_time",
                table: "tracked_entries");

            migrationBuilder.DropCheckConstraint(
                name: "ck_work_entries_type_not_zero",
                table: "tracked_entries");

            migrationBuilder.AddCheckConstraint(
                name: "ck_entries_end_time_is_greater_than_start_time",
                table: "tracked_entries",
                sql: "\"end_time\" > \"start_time\"");

            migrationBuilder.AddCheckConstraint(
                name: "ck_entries_type_not_zero",
                table: "tracked_entries",
                sql: "\"type\" <> 0");
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropCheckConstraint(
                name: "ck_entries_end_time_is_greater_than_start_time",
                table: "tracked_entries");

            migrationBuilder.DropCheckConstraint(
                name: "ck_entries_type_not_zero",
                table: "tracked_entries");

            migrationBuilder.AddCheckConstraint(
                name: "ck_work_entries_end_time_is_greater_than_start_time",
                table: "tracked_entries",
                sql: "\"end_time\" > \"start_time\"");

            migrationBuilder.AddCheckConstraint(
                name: "ck_work_entries_type_not_zero",
                table: "tracked_entries",
                sql: "\"type\" <> 0");
        }
    }
}
