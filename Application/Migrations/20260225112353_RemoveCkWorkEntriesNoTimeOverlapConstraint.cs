using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace Application.Migrations
{
    /// <inheritdoc />
    public partial class RemoveCkWorkEntriesNoTimeOverlapConstraint : Migration
    {
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            // Drop constraint
            migrationBuilder.Sql(@"
            ALTER TABLE tracked_entries 
            DROP CONSTRAINT IF EXISTS ck_work_entries_no_time_overlap;
        ");
            // Drop btree_gist extension
            migrationBuilder.Sql(@"
            DROP EXTENSION IF EXISTS btree_gist;
        ");
        }


        // Reverts the migration and removes the constraint and exstention if it exists.
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.Sql(@"
                CREATE EXTENSION IF NOT EXISTS btree_gist;
            ");

            migrationBuilder.Sql(@"
                ALTER TABLE tracked_entries
                ADD CONSTRAINT ck_work_entries_no_time_overlap
                EXCLUDE USING GIST (
                    tenant_id WITH =,
                    employee_id WITH =,
                    tsrange(start_time, end_time, '[)') WITH &&
                ) WHERE (is_deleted = false); 
            ");
        }
    }
}
