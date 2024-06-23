using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace DominosDriverHustleComp.Server.Migrations
{
    /// <inheritdoc />
    public partial class TrackedDeliveriesNowInt : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.AlterColumn<int>(
                name: "MinTrackedPercentage",
                table: "Settings",
                type: "INTEGER",
                nullable: false,
                comment: "0-100",
                oldClrType: typeof(float),
                oldType: "REAL");

            migrationBuilder.AlterColumn<int>(
                name: "TrackedPercentage",
                table: "DeliverySummaries",
                type: "INTEGER",
                nullable: false,
                comment: "0-100",
                oldClrType: typeof(float),
                oldType: "REAL");
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.AlterColumn<float>(
                name: "MinTrackedPercentage",
                table: "Settings",
                type: "REAL",
                nullable: false,
                oldClrType: typeof(int),
                oldType: "INTEGER",
                oldComment: "0-100");

            migrationBuilder.AlterColumn<float>(
                name: "TrackedPercentage",
                table: "DeliverySummaries",
                type: "REAL",
                nullable: false,
                oldClrType: typeof(int),
                oldType: "INTEGER",
                oldComment: "0-100");
        }
    }
}
