using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace DominosDriverHustleComp.Server.Migrations
{
    /// <inheritdoc />
    public partial class AddedSettingsEnableCompetition : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.AddColumn<bool>(
                name: "EnableCompetition",
                table: "Settings",
                type: "INTEGER",
                nullable: false,
                defaultValue: false);
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropColumn(
                name: "EnableCompetition",
                table: "Settings");
        }
    }
}
