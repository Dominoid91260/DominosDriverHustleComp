using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace DominosDriverHustleComp.Server.Migrations
{
    /// <inheritdoc />
    public partial class RenamedDriverHustleFields : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.RenameColumn(
                name: "AvgHustleOut",
                table: "Deliveries",
                newName: "HustleOut");

            migrationBuilder.RenameColumn(
                name: "AvgHustleIn",
                table: "Deliveries",
                newName: "HustleIn");
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.RenameColumn(
                name: "HustleOut",
                table: "Deliveries",
                newName: "AvgHustleOut");

            migrationBuilder.RenameColumn(
                name: "HustleIn",
                table: "Deliveries",
                newName: "AvgHustleIn");
        }
    }
}
