using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace DominosDriverHustleComp.Server.Migrations
{
    /// <inheritdoc />
    public partial class AddedDeliveryWasTracked : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.AddColumn<float>(
                name: "TrackedPercentage",
                table: "DeliverySummaries",
                type: "REAL",
                nullable: false,
                defaultValue: 0f);

            migrationBuilder.AddColumn<bool>(
                name: "WasTracked",
                table: "Deliveries",
                type: "INTEGER",
                nullable: false,
                defaultValue: false);
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropColumn(
                name: "TrackedPercentage",
                table: "DeliverySummaries");

            migrationBuilder.DropColumn(
                name: "WasTracked",
                table: "Deliveries");
        }
    }
}
