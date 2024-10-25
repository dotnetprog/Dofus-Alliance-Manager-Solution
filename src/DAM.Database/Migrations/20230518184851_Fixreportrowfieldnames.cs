using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace DAM.Database.Migrations
{
    /// <inheritdoc />
    public partial class Fixreportrowfieldnames : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropColumn(
                name: "DiscordUserId",
                table: "SummaryReportRows");

            migrationBuilder.RenameColumn(
                name: "Totaldefenses",
                table: "SummaryReportRows",
                newName: "Nombre_defense");

            migrationBuilder.RenameColumn(
                name: "Totalattacks",
                table: "SummaryReportRows",
                newName: "Nombre_attaques");

            migrationBuilder.RenameColumn(
                name: "AmountpepitesDef",
                table: "SummaryReportRows",
                newName: "MontantDefPepites");

            migrationBuilder.RenameColumn(
                name: "AmountpepitesAtk",
                table: "SummaryReportRows",
                newName: "MontantAtkPepites");

            migrationBuilder.AddColumn<decimal>(
                name: "DiscordId",
                table: "SummaryReportRows",
                type: "decimal(20,0)",
                nullable: false,
                defaultValue: 0m);
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropColumn(
                name: "DiscordId",
                table: "SummaryReportRows");

            migrationBuilder.RenameColumn(
                name: "Nombre_defense",
                table: "SummaryReportRows",
                newName: "Totaldefenses");

            migrationBuilder.RenameColumn(
                name: "Nombre_attaques",
                table: "SummaryReportRows",
                newName: "Totalattacks");

            migrationBuilder.RenameColumn(
                name: "MontantDefPepites",
                table: "SummaryReportRows",
                newName: "AmountpepitesDef");

            migrationBuilder.RenameColumn(
                name: "MontantAtkPepites",
                table: "SummaryReportRows",
                newName: "AmountpepitesAtk");

            migrationBuilder.AddColumn<string>(
                name: "DiscordUserId",
                table: "SummaryReportRows",
                type: "nvarchar(max)",
                nullable: false,
                defaultValue: "");
        }
    }
}
