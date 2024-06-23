using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace DominosDriverHustleComp.Server.Migrations
{
    /// <inheritdoc />
    public partial class ChangedBenchmarkAndOutlierTypesToInt : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.AlterColumn<int>(
                name: "OutlierSeconds",
                table: "Settings",
                type: "INTEGER",
                nullable: false,
                oldClrType: typeof(float),
                oldType: "REAL");

            migrationBuilder.AlterColumn<int>(
                name: "HustleBenchmarkSeconds",
                table: "Settings",
                type: "INTEGER",
                nullable: false,
                oldClrType: typeof(float),
                oldType: "REAL");
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.AlterColumn<float>(
                name: "OutlierSeconds",
                table: "Settings",
                type: "REAL",
                nullable: false,
                oldClrType: typeof(int),
                oldType: "INTEGER");

            migrationBuilder.AlterColumn<float>(
                name: "HustleBenchmarkSeconds",
                table: "Settings",
                type: "REAL",
                nullable: false,
                oldClrType: typeof(int),
                oldType: "INTEGER");
        }
    }
}
