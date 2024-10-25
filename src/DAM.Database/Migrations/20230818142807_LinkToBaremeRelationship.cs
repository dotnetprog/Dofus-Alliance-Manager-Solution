using System;
using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace DAM.Database.Migrations
{
    /// <inheritdoc />
    public partial class LinkToBaremeRelationship : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.AddColumn<Guid>(
                name: "BaremeAttaqueId",
                table: "Enemies",
                type: "uniqueidentifier",
                nullable: true);

            migrationBuilder.AddColumn<Guid>(
                name: "BaremeDefenseId",
                table: "Enemies",
                type: "uniqueidentifier",
                nullable: true);

            migrationBuilder.CreateIndex(
                name: "IX_Enemies_BaremeAttaqueId",
                table: "Enemies",
                column: "BaremeAttaqueId");

            migrationBuilder.CreateIndex(
                name: "IX_Enemies_BaremeDefenseId",
                table: "Enemies",
                column: "BaremeDefenseId");

            migrationBuilder.AddForeignKey(
                name: "FK_Enemies_Baremes_BaremeAttaqueId",
                table: "Enemies",
                column: "BaremeAttaqueId",
                principalTable: "Baremes",
                principalColumn: "Id");

            migrationBuilder.AddForeignKey(
                name: "FK_Enemies_Baremes_BaremeDefenseId",
                table: "Enemies",
                column: "BaremeDefenseId",
                principalTable: "Baremes",
                principalColumn: "Id");
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropForeignKey(
                name: "FK_Enemies_Baremes_BaremeAttaqueId",
                table: "Enemies");

            migrationBuilder.DropForeignKey(
                name: "FK_Enemies_Baremes_BaremeDefenseId",
                table: "Enemies");

            migrationBuilder.DropIndex(
                name: "IX_Enemies_BaremeAttaqueId",
                table: "Enemies");

            migrationBuilder.DropIndex(
                name: "IX_Enemies_BaremeDefenseId",
                table: "Enemies");

            migrationBuilder.DropColumn(
                name: "BaremeAttaqueId",
                table: "Enemies");

            migrationBuilder.DropColumn(
                name: "BaremeDefenseId",
                table: "Enemies");
        }
    }
}
