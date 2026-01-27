using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

// We use check of range times in migration because it is impossible to check it in AppDBContext.
// PostgreSQL range types are used here because EF Core doesn't natively support 
// complex constraints like exclusion constraints.

// https://www.postgresql.org/docs/current/rangetypes.html

namespace Application.Migrations
{
    /// <inheritdoc />
    public partial class AddNoTimeOverlapConstraintToWorkEntries : Migration
    {
        // Apply the migration
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            // Enable the btree_gist PostgreSQL extension. 
            // This extension makes it possible for our complex constraint to work with several operators.
            // In our case they are: tenant_id, employee_id, and tsrange
            migrationBuilder.Sql(@"
            CREATE EXTENSION IF NOT EXISTS btree_gist;
        ");

            // Add complex exclusion constraint to prevent time overlaps.
            // This ensures employees cannot have overlapping work time entries in the same tenant.
            migrationBuilder.Sql(@"
            ALTER TABLE work_entries
            ADD CONSTRAINT ck_work_entries_no_time_overlap
            EXCLUDE USING GIST (
                tenant_id WITH =,
                employee_id WITH =,
                tsrange(start_time, end_time, '[)') WITH &&
            ) WHERE (is_deleted = false);
        ");
        }

        // Reverts the migration and removes the constraint if it exists.
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.Sql(@"
            ALTER TABLE work_entries 
            DROP CONSTRAINT IF EXISTS ck_work_entries_no_time_overlap;
        ");
        }
    }
}
