using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace DominosDriverHustleComp.Server.Migrations
{
    /// <inheritdoc />
    public partial class AddedSettings : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.CreateTable(
                name: "Settings",
                columns: table => new
                {
                    Id = table.Column<int>(type: "INTEGER", nullable: false)
                        .Annotation("Sqlite:Autoincrement", true),
                    HustleBenchmarkSeconds = table.Column<float>(type: "REAL", nullable: false),
                    OutlierSeconds = table.Column<float>(type: "REAL", nullable: false),
                    MinDels = table.Column<int>(type: "INTEGER", nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_Settings", x => x.Id);
                });

            migrationBuilder.InsertData("Settings",
                    [ "HustleBenchmarkSeconds", "OutlierSeconds", "MinDels" ],
                    [ 90, 10, 10] // 1.5 minute benchmark, 10 seconds outlier, 10 dels minimum
                );
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropTable(
                name: "Settings");
        }
    }
}
