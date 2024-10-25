using System;
using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace DAM.Database.Migrations
{
    /// <inheritdoc />
    public partial class AddSaisonEntities : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.CreateTable(
                name: "Saisons",
                columns: table => new
                {
                    Id = table.Column<Guid>(type: "uniqueidentifier", nullable: false),
                    Name = table.Column<string>(type: "nvarchar(max)", nullable: false),
                    Description = table.Column<string>(type: "nvarchar(max)", nullable: false),
                    StartDate = table.Column<DateTime>(type: "datetime2", nullable: false),
                    EndDate = table.Column<DateTime>(type: "datetime2", nullable: false),
                    AllianceId = table.Column<Guid>(type: "uniqueidentifier", nullable: false),
                    CreatedOn = table.Column<DateTime>(type: "datetime2", nullable: false),
                    CreatedById = table.Column<Guid>(type: "uniqueidentifier", nullable: true),
                    ModifiedOn = table.Column<DateTime>(type: "datetime2", nullable: false),
                    ModifiedById = table.Column<Guid>(type: "uniqueidentifier", nullable: true),
                    State = table.Column<int>(type: "int", nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_Saisons", x => x.Id);
                    table.ForeignKey(
                        name: "FK_Saisons_Alliances_AllianceId",
                        column: x => x.AllianceId,
                        principalTable: "Alliances",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Cascade);
                    table.ForeignKey(
                        name: "FK_Saisons_Members_CreatedById",
                        column: x => x.CreatedById,
                        principalTable: "Members",
                        principalColumn: "Id");
                    table.ForeignKey(
                        name: "FK_Saisons_Members_ModifiedById",
                        column: x => x.ModifiedById,
                        principalTable: "Members",
                        principalColumn: "Id");
                });

            migrationBuilder.CreateTable(
                name: "SaisonRankings",
                columns: table => new
                {
                    Id = table.Column<Guid>(type: "uniqueidentifier", nullable: false),
                    SaisonId = table.Column<Guid>(type: "uniqueidentifier", nullable: false),
                    MemberId = table.Column<Guid>(type: "uniqueidentifier", nullable: false),
                    Nombre_attaques = table.Column<int>(type: "int", nullable: false),
                    Nombre_defense = table.Column<int>(type: "int", nullable: false),
                    NombreParticipationAvA = table.Column<int>(type: "int", nullable: false),
                    MontantDefPepites = table.Column<int>(type: "int", nullable: false),
                    MontantAtkPepites = table.Column<int>(type: "int", nullable: false),
                    MontantAvAPepites = table.Column<int>(type: "int", nullable: false),
                    MontantTotalPepite = table.Column<int>(type: "int", nullable: false),
                    CreatedOn = table.Column<DateTime>(type: "datetime2", nullable: false),
                    CreatedById = table.Column<Guid>(type: "uniqueidentifier", nullable: true),
                    ModifiedOn = table.Column<DateTime>(type: "datetime2", nullable: false),
                    ModifiedById = table.Column<Guid>(type: "uniqueidentifier", nullable: true)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_SaisonRankings", x => x.Id);
                    table.ForeignKey(
                        name: "FK_SaisonRankings_Members_CreatedById",
                        column: x => x.CreatedById,
                        principalTable: "Members",
                        principalColumn: "Id");
                    table.ForeignKey(
                        name: "FK_SaisonRankings_Members_MemberId",
                        column: x => x.MemberId,
                        principalTable: "Members",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Cascade);
                    table.ForeignKey(
                        name: "FK_SaisonRankings_Members_ModifiedById",
                        column: x => x.ModifiedById,
                        principalTable: "Members",
                        principalColumn: "Id");
                    table.ForeignKey(
                        name: "FK_SaisonRankings_Saisons_SaisonId",
                        column: x => x.SaisonId,
                        principalTable: "Saisons",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Cascade);
                });

            migrationBuilder.CreateIndex(
                name: "IX_SaisonRankings_CreatedById",
                table: "SaisonRankings",
                column: "CreatedById");

            migrationBuilder.CreateIndex(
                name: "IX_SaisonRankings_MemberId",
                table: "SaisonRankings",
                column: "MemberId");

            migrationBuilder.CreateIndex(
                name: "IX_SaisonRankings_ModifiedById",
                table: "SaisonRankings",
                column: "ModifiedById");

            migrationBuilder.CreateIndex(
                name: "IX_SaisonRankings_SaisonId",
                table: "SaisonRankings",
                column: "SaisonId");

            migrationBuilder.CreateIndex(
                name: "IX_Saisons_AllianceId",
                table: "Saisons",
                column: "AllianceId");

            migrationBuilder.CreateIndex(
                name: "IX_Saisons_CreatedById",
                table: "Saisons",
                column: "CreatedById");

            migrationBuilder.CreateIndex(
                name: "IX_Saisons_ModifiedById",
                table: "Saisons",
                column: "ModifiedById");
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropTable(
                name: "SaisonRankings");

            migrationBuilder.DropTable(
                name: "Saisons");
        }
    }
}
