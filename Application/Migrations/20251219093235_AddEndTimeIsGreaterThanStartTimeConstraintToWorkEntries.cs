using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace Application.Migrations
{
    /// <inheritdoc />
    public partial class AddEndTimeIsGreaterThanStartTimeConstraintToWorkEntries : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.AddCheckConstraint(
                name: "ck_work_entries_end_time_is_greater_than_start_time",
                table: "work_entries",
                sql: "\"end_time\" > \"start_time\"");
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropCheckConstraint(
                name: "ck_work_entries_end_time_is_greater_than_start_time",
                table: "work_entries");
        }
    }
}
