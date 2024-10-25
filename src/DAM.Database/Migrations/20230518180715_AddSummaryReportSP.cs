using System;
using DAM.Database.Extensions;
using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace DAM.Database.Migrations
{
    /// <inheritdoc />
    public partial class AddSummaryReportSP : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.CreateTable(
                name: "SummaryReportRows",
                columns: table => new
                {
                    MemberId = table.Column<Guid>(type: "uniqueidentifier", nullable: false),
                    DiscordUserId = table.Column<string>(type: "nvarchar(max)", nullable: false),
                    Username = table.Column<string>(type: "nvarchar(max)", nullable: false),
                    Totaldefenses = table.Column<int>(type: "int", nullable: false),
                    Totalattacks = table.Column<int>(type: "int", nullable: false),
                    AmountpepitesAtk = table.Column<int>(type: "int", nullable: false),
                    AmountpepitesDef = table.Column<int>(type: "int", nullable: false)
                },
                constraints: table =>
                {
                });
            //migrationBuilder.CreateProcedure("spSummaryReport", "dbo", @"\Sql\spSummaryReport.sql", true);
            
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropTable(
                name: "SummaryReportRows");
           // migrationBuilder.DropProcedure("spSummaryReport", "dbo");
        }
    }
}
