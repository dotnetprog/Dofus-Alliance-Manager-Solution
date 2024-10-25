using System;
using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace DAM.Database.Migrations
{
    /// <inheritdoc />
    public partial class SeasonFlexibilityEnhancements : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.AddColumn<Guid>(
                name: "BaremeAttackId",
                table: "Saisons",
                type: "uniqueidentifier",
                nullable: true);

            migrationBuilder.AddColumn<Guid>(
                name: "BaremeDefenseId",
                table: "Saisons",
                type: "uniqueidentifier",
                nullable: true);

            migrationBuilder.AddColumn<bool>(
                name: "AllowSeasonOverlap",
                table: "AllianceConfigurations",
                type: "bit",
                nullable: true);

            migrationBuilder.CreateIndex(
                name: "IX_Saisons_BaremeAttackId",
                table: "Saisons",
                column: "BaremeAttackId");

            migrationBuilder.CreateIndex(
                name: "IX_Saisons_BaremeDefenseId",
                table: "Saisons",
                column: "BaremeDefenseId");

            migrationBuilder.AddForeignKey(
                name: "FK_Saisons_Baremes_BaremeAttackId",
                table: "Saisons",
                column: "BaremeAttackId",
                principalTable: "Baremes",
                principalColumn: "Id");

            migrationBuilder.AddForeignKey(
                name: "FK_Saisons_Baremes_BaremeDefenseId",
                table: "Saisons",
                column: "BaremeDefenseId",
                principalTable: "Baremes",
                principalColumn: "Id");
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropForeignKey(
                name: "FK_Saisons_Baremes_BaremeAttackId",
                table: "Saisons");

            migrationBuilder.DropForeignKey(
                name: "FK_Saisons_Baremes_BaremeDefenseId",
                table: "Saisons");

            migrationBuilder.DropIndex(
                name: "IX_Saisons_BaremeAttackId",
                table: "Saisons");

            migrationBuilder.DropIndex(
                name: "IX_Saisons_BaremeDefenseId",
                table: "Saisons");

            migrationBuilder.DropColumn(
                name: "BaremeAttackId",
                table: "Saisons");

            migrationBuilder.DropColumn(
                name: "BaremeDefenseId",
                table: "Saisons");

            migrationBuilder.DropColumn(
                name: "AllowSeasonOverlap",
                table: "AllianceConfigurations");
        }
    }
}
