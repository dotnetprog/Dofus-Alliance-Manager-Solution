using System;
using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace DAM.Database.Migrations
{
    /// <inheritdoc />
    public partial class SearchUserManagement : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.CreateTable(
                name: "AnkamaPseudos",
                columns: table => new
                {
                    Id = table.Column<Guid>(type: "uniqueidentifier", nullable: false),
                    Pseudo = table.Column<string>(type: "nvarchar(max)", nullable: true),
                    AnkadexUrl = table.Column<string>(type: "nvarchar(max)", nullable: true),
                    PseudoData = table.Column<string>(type: "nvarchar(max)", nullable: true),
                    AllianceMemberId = table.Column<Guid>(type: "uniqueidentifier", nullable: false),
                    CreatedById = table.Column<Guid>(type: "uniqueidentifier", nullable: true),
                    LastRefreshedOn = table.Column<DateTime>(type: "datetime2", nullable: false),
                    AccountCreatedOn = table.Column<DateTime>(type: "datetime2", nullable: true)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_AnkamaPseudos", x => x.Id);
                    table.ForeignKey(
                        name: "FK_AnkamaPseudos_Members_AllianceMemberId",
                        column: x => x.AllianceMemberId,
                        principalTable: "Members",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Cascade);
                    table.ForeignKey(
                        name: "FK_AnkamaPseudos_Members_CreatedById",
                        column: x => x.CreatedById,
                        principalTable: "Members",
                        principalColumn: "Id");
                });

            migrationBuilder.CreateIndex(
                name: "IX_AnkamaPseudos_AllianceMemberId",
                table: "AnkamaPseudos",
                column: "AllianceMemberId");

            migrationBuilder.CreateIndex(
                name: "IX_AnkamaPseudos_CreatedById",
                table: "AnkamaPseudos",
                column: "CreatedById");
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropTable(
                name: "AnkamaPseudos");
        }
    }
}
