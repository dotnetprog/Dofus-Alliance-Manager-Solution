using System;
using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace DAM.Database.Migrations
{
    /// <inheritdoc />
    public partial class AddAvaEntities : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.AddColumn<decimal>(
                name: "Ava_DiscordForumChannelId",
                table: "AllianceConfigurations",
                type: "decimal(20,0)",
                nullable: true);

            migrationBuilder.CreateTable(
                name: "AvA",
                columns: table => new
                {
                    Id = table.Column<Guid>(type: "uniqueidentifier", nullable: false),
                    Titre = table.Column<string>(type: "nvarchar(max)", nullable: true),
                    ResultState = table.Column<int>(type: "int", nullable: true),
                    Zone = table.Column<int>(type: "int", nullable: false),
                    ZoneAutres = table.Column<string>(type: "nvarchar(max)", nullable: true),
                    MontantPepitesTotal = table.Column<int>(type: "int", nullable: true),
                    MontantPepitesObtenu = table.Column<int>(type: "int", nullable: true),
                    PourcentagePaye = table.Column<decimal>(type: "decimal(5,2)", precision: 5, scale: 2, nullable: true),
                    MontantPayeFixe = table.Column<int>(type: "int", nullable: true),
                    ImageUrl = table.Column<string>(type: "nvarchar(max)", nullable: true),
                    AllianceId = table.Column<Guid>(type: "uniqueidentifier", nullable: false),
                    CreatedOn = table.Column<DateTime>(type: "datetime2", nullable: false),
                    CreatedById = table.Column<Guid>(type: "uniqueidentifier", nullable: false),
                    ClosedOn = table.Column<DateTime>(type: "datetime2", nullable: true),
                    ClosedById = table.Column<Guid>(type: "uniqueidentifier", nullable: true),
                    DiscordForumChannelId = table.Column<decimal>(type: "decimal(20,0)", nullable: false),
                    DiscordThreadChannelId = table.Column<decimal>(type: "decimal(20,0)", nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_AvA", x => x.Id);
                    table.ForeignKey(
                        name: "FK_AvA_Alliances_AllianceId",
                        column: x => x.AllianceId,
                        principalTable: "Alliances",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Cascade);
                    table.ForeignKey(
                        name: "FK_AvA_Members_ClosedById",
                        column: x => x.ClosedById,
                        principalTable: "Members",
                        principalColumn: "Id");
                    table.ForeignKey(
                        name: "FK_AvA_Members_CreatedById",
                        column: x => x.CreatedById,
                        principalTable: "Members",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Cascade);
                });

            migrationBuilder.CreateTable(
                name: "AvaMembers",
                columns: table => new
                {
                    Id = table.Column<Guid>(type: "uniqueidentifier", nullable: false),
                    AvaId = table.Column<Guid>(type: "uniqueidentifier", nullable: false),
                    MemberId = table.Column<Guid>(type: "uniqueidentifier", nullable: true),
                    CreatedOn = table.Column<DateTime>(type: "datetime2", nullable: false),
                    ImageUrl = table.Column<string>(type: "nvarchar(max)", nullable: false),
                    ValidationState = table.Column<int>(type: "int", nullable: true),
                    ValidatedById = table.Column<Guid>(type: "uniqueidentifier", nullable: true),
                    MontantPepites = table.Column<int>(type: "int", nullable: true),
                    DiscordMessageId = table.Column<decimal>(type: "decimal(20,0)", nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_AvaMembers", x => x.Id);
                    table.ForeignKey(
                        name: "FK_AvaMembers_AvA_AvaId",
                        column: x => x.AvaId,
                        principalTable: "AvA",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Cascade);
                    table.ForeignKey(
                        name: "FK_AvaMembers_Members_MemberId",
                        column: x => x.MemberId,
                        principalTable: "Members",
                        principalColumn: "Id");
                    table.ForeignKey(
                        name: "FK_AvaMembers_Members_ValidatedById",
                        column: x => x.ValidatedById,
                        principalTable: "Members",
                        principalColumn: "Id");
                });

            migrationBuilder.CreateIndex(
                name: "IX_AvA_AllianceId",
                table: "AvA",
                column: "AllianceId");

            migrationBuilder.CreateIndex(
                name: "IX_AvA_ClosedById",
                table: "AvA",
                column: "ClosedById");

            migrationBuilder.CreateIndex(
                name: "IX_AvA_CreatedById",
                table: "AvA",
                column: "CreatedById");

            migrationBuilder.CreateIndex(
                name: "IX_AvaMembers_AvaId",
                table: "AvaMembers",
                column: "AvaId");

            migrationBuilder.CreateIndex(
                name: "IX_AvaMembers_MemberId",
                table: "AvaMembers",
                column: "MemberId");

            migrationBuilder.CreateIndex(
                name: "IX_AvaMembers_ValidatedById",
                table: "AvaMembers",
                column: "ValidatedById");
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropTable(
                name: "AvaMembers");

            migrationBuilder.DropTable(
                name: "AvA");

            migrationBuilder.DropColumn(
                name: "Ava_DiscordForumChannelId",
                table: "AllianceConfigurations");
        }
    }
}
