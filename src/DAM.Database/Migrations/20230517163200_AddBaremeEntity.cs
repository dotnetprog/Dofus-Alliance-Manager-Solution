using System;
using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace DAM.Database.Migrations
{
    /// <inheritdoc />
    public partial class AddBaremeEntity : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.CreateTable(
                name: "Baremes",
                columns: table => new
                {
                    Id = table.Column<Guid>(type: "uniqueidentifier", nullable: false),
                    Name = table.Column<string>(type: "nvarchar(max)", nullable: false),
                    AllianceId = table.Column<Guid>(type: "uniqueidentifier", nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_Baremes", x => x.Id);
                    table.ForeignKey(
                        name: "FK_Baremes_Alliances_AllianceId",
                        column: x => x.AllianceId,
                        principalTable: "Alliances",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Cascade);
                });

            migrationBuilder.CreateTable(
                name: "BaremeDetails",
                columns: table => new
                {
                    Id = table.Column<Guid>(type: "uniqueidentifier", nullable: false),
                    BaremeId = table.Column<Guid>(type: "uniqueidentifier", nullable: false),
                    NbEnemie = table.Column<int>(type: "int", nullable: false),
                    NbAllie = table.Column<int>(type: "int", nullable: false),
                    NbPepite = table.Column<int>(type: "int", nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_BaremeDetails", x => x.Id);
                    table.ForeignKey(
                        name: "FK_BaremeDetails_Baremes_BaremeId",
                        column: x => x.BaremeId,
                        principalTable: "Baremes",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Cascade);
                });

            migrationBuilder.CreateIndex(
                name: "IX_BaremeDetails_BaremeId",
                table: "BaremeDetails",
                column: "BaremeId");

            migrationBuilder.CreateIndex(
                name: "IX_Baremes_AllianceId",
                table: "Baremes",
                column: "AllianceId");
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropTable(
                name: "BaremeDetails");

            migrationBuilder.DropTable(
                name: "Baremes");
        }
    }
}
