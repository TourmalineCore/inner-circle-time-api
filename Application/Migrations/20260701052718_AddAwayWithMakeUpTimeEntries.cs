using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace Application.Migrations
{
    /// <inheritdoc />
    public partial class AddAwayWithMakeUpTimeEntries : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.AlterColumn<string>(
                name: "discriminator",
                table: "tracked_entries",
                type: "character varying(34)",
                maxLength: 34,
                nullable: false,
                oldClrType: typeof(string),
                oldType: "character varying(21)",
                oldMaxLength: 21);

            migrationBuilder.AddColumn<long>(
                name: "related_entry_id",
                table: "tracked_entries",
                type: "bigint",
                nullable: true);

            migrationBuilder.AddColumn<int>(
                name: "related_entry_type",
                table: "tracked_entries",
                type: "integer",
                nullable: true);

            migrationBuilder.CreateIndex(
                name: "ix_tracked_entries_related_entry_id",
                table: "tracked_entries",
                column: "related_entry_id");

            migrationBuilder.AddForeignKey(
                name: "fk_tracked_entries_tracked_entries_related_entry_id",
                table: "tracked_entries",
                column: "related_entry_id",
                principalTable: "tracked_entries",
                principalColumn: "id",
                onDelete: ReferentialAction.Cascade);
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropForeignKey(
                name: "fk_tracked_entries_tracked_entries_related_entry_id",
                table: "tracked_entries");

            migrationBuilder.DropIndex(
                name: "ix_tracked_entries_related_entry_id",
                table: "tracked_entries");

            migrationBuilder.DropColumn(
                name: "related_entry_id",
                table: "tracked_entries");

            migrationBuilder.DropColumn(
                name: "related_entry_type",
                table: "tracked_entries");

            migrationBuilder.AlterColumn<string>(
                name: "discriminator",
                table: "tracked_entries",
                type: "character varying(21)",
                maxLength: 21,
                nullable: false,
                oldClrType: typeof(string),
                oldType: "character varying(34)",
                oldMaxLength: 34);
        }
    }
}
