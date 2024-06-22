using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace DominosDriverHustleComp.Server.Migrations
{
    /// <inheritdoc />
    public partial class AddedSettingsMinTrackedPercentage : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.AddColumn<float>(
                name: "MinTrackedPercentage",
                table: "Settings",
                type: "REAL",
                nullable: false,
                defaultValue: 0.95f);
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropColumn(
                name: "MinTrackedPercentage",
                table: "Settings");
        }
    }
}
