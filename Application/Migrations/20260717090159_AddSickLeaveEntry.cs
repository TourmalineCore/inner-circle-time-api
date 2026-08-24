using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace Application.Migrations
{
    // The migration is empty because SickLeaveEntry has not added any new fields, all the necessary fields are already in TrackedEntryBase, from which it inherits
    /// <inheritdoc />
    public partial class AddSickLeaveEntry : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {

        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {

        }
    }
}
