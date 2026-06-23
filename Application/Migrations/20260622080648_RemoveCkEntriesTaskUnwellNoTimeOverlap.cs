using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace Application.Migrations
{
    /// <inheritdoc />
    public partial class RemoveCkEntriesTaskUnwellNoTimeOverlap : Migration
    {
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            // Drop constraint
            migrationBuilder.Sql(@"
                ALTER TABLE tracked_entries 
                DROP CONSTRAINT IF EXISTS ck_entries_task_unwell_no_time_overlap;
            ");
            // Drop btree_gist extension
            migrationBuilder.Sql(@"
                DROP EXTENSION IF EXISTS btree_gist;
            ");
        }


        // Reverts the migration.
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.Sql(@"
                CREATE EXTENSION IF NOT EXISTS btree_gist;
            ");

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
    }
}
