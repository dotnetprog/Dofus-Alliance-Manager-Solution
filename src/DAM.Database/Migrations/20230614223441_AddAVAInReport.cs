using DAM.Database.Extensions;
using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace DAM.Database.Migrations
{
    /// <inheritdoc />
    public partial class AddAVAInReport : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.AddColumn<int>(
                name: "MontantAvAPepites",
                table: "SummaryReportRows",
                type: "int",
                nullable: false,
                defaultValue: 0);

            migrationBuilder.AddColumn<int>(
                name: "NombreParticipationAvA",
                table: "SummaryReportRows",
                type: "int",
                nullable: false,
                defaultValue: 0);

        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropProcedure("spSummaryReport", "dbo");
            migrationBuilder.DropColumn(
                name: "MontantAvAPepites",
                table: "SummaryReportRows");

            migrationBuilder.DropColumn(
                name: "NombreParticipationAvA",
                table: "SummaryReportRows");

        }
    }
}
