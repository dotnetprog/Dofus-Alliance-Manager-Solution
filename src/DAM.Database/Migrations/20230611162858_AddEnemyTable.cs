using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace DAM.Database.Migrations
{
    /// <inheritdoc />
    public partial class AddEnemyTable : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropForeignKey(
                name: "FK_AllianceEnemy_Alliances_AllianceId",
                table: "AllianceEnemy");

            migrationBuilder.DropForeignKey(
                name: "FK_ScreenPosts_AllianceEnemy_AllianceEnemyId",
                table: "ScreenPosts");

            migrationBuilder.DropPrimaryKey(
                name: "PK_AllianceEnemy",
                table: "AllianceEnemy");

            migrationBuilder.RenameTable(
                name: "AllianceEnemy",
                newName: "Enemies");

            migrationBuilder.RenameIndex(
                name: "IX_AllianceEnemy_AllianceId",
                table: "Enemies",
                newName: "IX_Enemies_AllianceId");

            migrationBuilder.AddPrimaryKey(
                name: "PK_Enemies",
                table: "Enemies",
                column: "Id");

            migrationBuilder.AddForeignKey(
                name: "FK_Enemies_Alliances_AllianceId",
                table: "Enemies",
                column: "AllianceId",
                principalTable: "Alliances",
                principalColumn: "Id",
                onDelete: ReferentialAction.Cascade);

            migrationBuilder.AddForeignKey(
                name: "FK_ScreenPosts_Enemies_AllianceEnemyId",
                table: "ScreenPosts",
                column: "AllianceEnemyId",
                principalTable: "Enemies",
                principalColumn: "Id");
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropForeignKey(
                name: "FK_Enemies_Alliances_AllianceId",
                table: "Enemies");

            migrationBuilder.DropForeignKey(
                name: "FK_ScreenPosts_Enemies_AllianceEnemyId",
                table: "ScreenPosts");

            migrationBuilder.DropPrimaryKey(
                name: "PK_Enemies",
                table: "Enemies");

            migrationBuilder.RenameTable(
                name: "Enemies",
                newName: "AllianceEnemy");

            migrationBuilder.RenameIndex(
                name: "IX_Enemies_AllianceId",
                table: "AllianceEnemy",
                newName: "IX_AllianceEnemy_AllianceId");

            migrationBuilder.AddPrimaryKey(
                name: "PK_AllianceEnemy",
                table: "AllianceEnemy",
                column: "Id");

            migrationBuilder.AddForeignKey(
                name: "FK_AllianceEnemy_Alliances_AllianceId",
                table: "AllianceEnemy",
                column: "AllianceId",
                principalTable: "Alliances",
                principalColumn: "Id",
                onDelete: ReferentialAction.Cascade);

            migrationBuilder.AddForeignKey(
                name: "FK_ScreenPosts_AllianceEnemy_AllianceEnemyId",
                table: "ScreenPosts",
                column: "AllianceEnemyId",
                principalTable: "AllianceEnemy",
                principalColumn: "Id");
        }
    }
}
