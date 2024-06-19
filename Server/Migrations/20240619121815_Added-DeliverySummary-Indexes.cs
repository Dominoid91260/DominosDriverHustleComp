using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace DominosDriverHustleComp.Server.Migrations
{
    /// <inheritdoc />
    public partial class AddedDeliverySummaryIndexes : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.CreateIndex(
                name: "IX_DeliverySummaries_WeekEnding",
                table: "DeliverySummaries",
                column: "WeekEnding");
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropIndex(
                name: "IX_DeliverySummaries_WeekEnding",
                table: "DeliverySummaries");
        }
    }
}
