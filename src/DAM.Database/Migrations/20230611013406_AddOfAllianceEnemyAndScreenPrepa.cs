using System;
using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace DAM.Database.Migrations
{
    /// <inheritdoc />
    public partial class AddOfAllianceEnemyAndScreenPrepa : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.AddColumn<Guid>(
                name: "AllianceEnemyId",
                table: "ScreenPosts",
                type: "uniqueidentifier",
                nullable: true);

            migrationBuilder.AddColumn<string>(
                name: "ImagePrepUrl",
                table: "ScreenPosts",
                type: "nvarchar(max)",
                nullable: true);

            migrationBuilder.CreateTable(
                name: "AllianceEnemy",
                columns: table => new
                {
                    Id = table.Column<Guid>(type: "uniqueidentifier", nullable: false),
                    Name = table.Column<string>(type: "nvarchar(max)", nullable: false),
                    AllianceId = table.Column<Guid>(type: "uniqueidentifier", nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_AllianceEnemy", x => x.Id);
                    table.ForeignKey(
                        name: "FK_AllianceEnemy_Alliances_AllianceId",
                        column: x => x.AllianceId,
                        principalTable: "Alliances",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Cascade);
                });

            migrationBuilder.CreateIndex(
                name: "IX_ScreenPosts_AllianceEnemyId",
                table: "ScreenPosts",
                column: "AllianceEnemyId");

            migrationBuilder.CreateIndex(
                name: "IX_AllianceEnemy_AllianceId",
                table: "AllianceEnemy",
                column: "AllianceId");

            migrationBuilder.AddForeignKey(
                name: "FK_ScreenPosts_AllianceEnemy_AllianceEnemyId",
                table: "ScreenPosts",
                column: "AllianceEnemyId",
                principalTable: "AllianceEnemy",
                principalColumn: "Id");
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropForeignKey(
                name: "FK_ScreenPosts_AllianceEnemy_AllianceEnemyId",
                table: "ScreenPosts");

            migrationBuilder.DropTable(
                name: "AllianceEnemy");

            migrationBuilder.DropIndex(
                name: "IX_ScreenPosts_AllianceEnemyId",
                table: "ScreenPosts");

            migrationBuilder.DropColumn(
                name: "AllianceEnemyId",
                table: "ScreenPosts");

            migrationBuilder.DropColumn(
                name: "ImagePrepUrl",
                table: "ScreenPosts");
        }
    }
}
