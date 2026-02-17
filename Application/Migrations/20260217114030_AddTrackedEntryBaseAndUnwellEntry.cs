using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace Application.Migrations
{
    /// <inheritdoc />
    public partial class AddTrackedEntryBaseAndUnwellEntry : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropPrimaryKey(
                name: "pk_work_entries",
                table: "work_entries");

            migrationBuilder.RenameTable(
                name: "work_entries",
                newName: "tracked_entries");

            migrationBuilder.AlterColumn<string>(
                name: "title",
                table: "tracked_entries",
                type: "text",
                nullable: true,
                oldClrType: typeof(string),
                oldType: "text");

            migrationBuilder.AlterColumn<string>(
                name: "task_id",
                table: "tracked_entries",
                type: "text",
                nullable: true,
                oldClrType: typeof(string),
                oldType: "text");

            migrationBuilder.AlterColumn<long>(
                name: "project_id",
                table: "tracked_entries",
                type: "bigint",
                nullable: true,
                oldClrType: typeof(long),
                oldType: "bigint");

            migrationBuilder.AlterColumn<string>(
                name: "description",
                table: "tracked_entries",
                type: "text",
                nullable: true,
                oldClrType: typeof(string),
                oldType: "text");

            // Add custom default value to preserve old records from prod
            // There were only task entries on prod at the time when this migration was created
            migrationBuilder.AddColumn<string>(
                name: "discriminator",
                table: "tracked_entries",
                type: "character varying(21)",
                maxLength: 21,
                nullable: false,
                defaultValue: "TaskEntry");

            migrationBuilder.AddPrimaryKey(
                name: "pk_tracked_entries",
                table: "tracked_entries",
                column: "id");
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropPrimaryKey(
                name: "pk_tracked_entries",
                table: "tracked_entries");

            migrationBuilder.DropColumn(
                name: "discriminator",
                table: "tracked_entries");

            migrationBuilder.RenameTable(
                name: "tracked_entries",
                newName: "work_entries");

            migrationBuilder.AlterColumn<string>(
                name: "title",
                table: "work_entries",
                type: "text",
                nullable: false,
                defaultValue: "",
                oldClrType: typeof(string),
                oldType: "text",
                oldNullable: true);

            migrationBuilder.AlterColumn<string>(
                name: "task_id",
                table: "work_entries",
                type: "text",
                nullable: false,
                defaultValue: "",
                oldClrType: typeof(string),
                oldType: "text",
                oldNullable: true);

            migrationBuilder.AlterColumn<long>(
                name: "project_id",
                table: "work_entries",
                type: "bigint",
                nullable: false,
                defaultValue: 0L,
                oldClrType: typeof(long),
                oldType: "bigint",
                oldNullable: true);

            migrationBuilder.AlterColumn<string>(
                name: "description",
                table: "work_entries",
                type: "text",
                nullable: false,
                defaultValue: "",
                oldClrType: typeof(string),
                oldType: "text",
                oldNullable: true);

            migrationBuilder.AddPrimaryKey(
                name: "pk_work_entries",
                table: "work_entries",
                column: "id");
        }
    }
}
