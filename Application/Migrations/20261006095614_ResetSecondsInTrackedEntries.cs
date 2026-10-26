using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace Application.Migrations
{
    /// <inheritdoc />
    public partial class ResetSecondsInTrackedEntries : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            // about date_trunc https://www.postgresql.org/docs/current/functions-datetime.html#FUNCTIONS-DATETIME-TRUNC
            migrationBuilder.Sql(@"
                UPDATE tracked_entries
                SET start_time = date_trunc('minute', start_time)
                WHERE EXTRACT(SECOND FROM start_time) <> 0;

                UPDATE tracked_entries
                SET end_time = date_trunc('minute', end_time)
                WHERE EXTRACT(SECOND FROM end_time) <> 0;
            ");
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            // rollback is not possible
        }
    }
}
