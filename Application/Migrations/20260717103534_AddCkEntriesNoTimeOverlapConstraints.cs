using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace Application.Migrations
{
    /// <inheritdoc />
    public partial class AddCkEntriesNoTimeOverlapConstraints : Migration
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
            // Entry types: Task - 1, Unwell - 2, Away With Make-up time - 3, Make-up time - 4 Sick leave - 5
            migrationBuilder.Sql(@"
                ALTER TABLE tracked_entries
                ADD CONSTRAINT ck_entries_1_2_3_no_time_overlap
                EXCLUDE USING GIST (
                    tenant_id WITH =,
                    employee_id WITH =,
                    tsrange(start_time, end_time, '[)') WITH &&
                )
                WHERE (type IN (1, 2, 3) AND deleted_at_utc IS NULL); 
            ");

            migrationBuilder.Sql(@"
                ALTER TABLE tracked_entries
                ADD CONSTRAINT ck_entries_2_3_4_5_no_time_overlap
                EXCLUDE USING GIST (
                    tenant_id WITH =,
                    employee_id WITH =,
                    tsrange(start_time, end_time, '[)') WITH &&
                )
                WHERE (type IN (2, 3, 4, 5) AND deleted_at_utc IS NULL); 
            ");
        }

        // Reverts the migration and removes the constraint and extention if it exists.
        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.Sql(@"
                ALTER TABLE tracked_entries
                DROP CONSTRAINT IF EXISTS ck_entries_1_2_3_no_time_overlap;
            ");

            migrationBuilder.Sql(@"
                ALTER TABLE tracked_entries 
                DROP CONSTRAINT IF EXISTS ck_entries_2_3_4_5_no_time_overlap;
            ");

            migrationBuilder.Sql(@"
                DROP EXTENSION IF EXISTS btree_gist;
            ");
        }
    }
}
