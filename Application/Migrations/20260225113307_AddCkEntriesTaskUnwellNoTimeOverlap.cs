using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace Application.Migrations
{
    /// <inheritdoc />
    public partial class AddCkEntriesTaskUnwellNoTimeOverlap : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            // Enable the btree_gist PostgreSQL extension. 
            // This extension makes it possible for our complex constraint to work with several fields.
            // In our case they are: tenant_id, employee_id, and tsrange
            migrationBuilder.Sql(@"
            CREATE EXTENSION IF NOT EXISTS btree_gist;
        ");
            // ADR about validation can be read here https://github.com/TourmalineCore/inner-circle-documentation/blob/master/time-tracker/adrs/003-time-api-overlap-validation.md
            migrationBuilder.Sql(@"
            ALTER TABLE tracked_entries
            ADD CONSTRAINT ck_entries_task_unwell_no_time_overlap
            EXCLUDE USING GIST (
                tenant_id WITH =,
                employee_id WITH =,
                tsrange(start_time, end_time, '[)') WITH &&
            )
            WHERE (type IN (1, 2) AND deleted_at_utc IS NULL); 
        ");
        }

        // Reverts the migration and removes the constraint and exstention if it exists.
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.Sql(@"
            ALTER TABLE tracked_entries 
            DROP CONSTRAINT IF EXISTS ck_entries_task_unwell_no_time_overlap;
        ");

            migrationBuilder.Sql(@"
            DROP EXTENSION IF EXISTS btree_gist;
        ");
        }
    }
}
