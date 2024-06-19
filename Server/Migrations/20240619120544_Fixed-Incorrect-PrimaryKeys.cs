using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace DominosDriverHustleComp.Server.Migrations
{
    /// <inheritdoc />
    public partial class FixedIncorrectPrimaryKeys : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropPrimaryKey(
                name: "PK_DeliverySummaries",
                table: "DeliverySummaries");

            migrationBuilder.DropColumn(
                name: "Id",
                table: "WeeklySummaries");

            migrationBuilder.AlterColumn<int>(
                name: "Id",
                table: "DeliverySummaries",
                type: "INTEGER",
                nullable: false,
                oldClrType: typeof(int),
                oldType: "INTEGER")
                .Annotation("Sqlite:Autoincrement", true);

            migrationBuilder.AddPrimaryKey(
                name: "PK_DeliverySummaries",
                table: "DeliverySummaries",
                column: "Id");
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropPrimaryKey(
                name: "PK_DeliverySummaries",
                table: "DeliverySummaries");

            migrationBuilder.AddColumn<int>(
                name: "Id",
                table: "WeeklySummaries",
                type: "INTEGER",
                nullable: false,
                defaultValue: 0);

            migrationBuilder.AlterColumn<int>(
                name: "Id",
                table: "DeliverySummaries",
                type: "INTEGER",
                nullable: false,
                oldClrType: typeof(int),
                oldType: "INTEGER")
                .OldAnnotation("Sqlite:Autoincrement", true);

            migrationBuilder.AddPrimaryKey(
                name: "PK_DeliverySummaries",
                table: "DeliverySummaries",
                column: "WeekEnding");
        }
    }
}
