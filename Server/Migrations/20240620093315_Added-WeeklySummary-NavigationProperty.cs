using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace DominosDriverHustleComp.Server.Migrations
{
    /// <inheritdoc />
    public partial class AddedWeeklySummaryNavigationProperty : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.AddForeignKey(
                name: "FK_DeliverySummaries_WeeklySummaries_WeekEnding",
                table: "DeliverySummaries",
                column: "WeekEnding",
                principalTable: "WeeklySummaries",
                principalColumn: "WeekEnding",
                onDelete: ReferentialAction.Cascade);
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropForeignKey(
                name: "FK_DeliverySummaries_WeeklySummaries_WeekEnding",
                table: "DeliverySummaries");
        }
    }
}
