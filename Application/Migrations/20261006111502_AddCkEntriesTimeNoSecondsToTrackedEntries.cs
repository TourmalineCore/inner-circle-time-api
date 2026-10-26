using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace Application.Migrations
{
    /// <inheritdoc />
    public partial class AddCkEntriesTimeNoSecondsToTrackedEntries : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.AddCheckConstraint(
                name: "ck_entries_time_no_seconds",
                table: "tracked_entries",
                sql: "date_trunc('minute', \"start_time\") = \"start_time\" AND date_trunc('minute', \"end_time\") = \"end_time\"");
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropCheckConstraint(
                name: "ck_entries_time_no_seconds",
                table: "tracked_entries");
        }
    }
}
