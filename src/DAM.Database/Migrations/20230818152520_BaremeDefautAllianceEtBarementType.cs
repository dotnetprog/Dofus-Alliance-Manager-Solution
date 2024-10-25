using System;
using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace DAM.Database.Migrations
{
    /// <inheritdoc />
    public partial class BaremeDefautAllianceEtBarementType : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.AddColumn<int>(
                name: "Type",
                table: "Baremes",
                type: "int",
                nullable: true);

            migrationBuilder.AddColumn<Guid>(
                name: "DefaultBaremeAttaqueId",
                table: "AllianceConfigurations",
                type: "uniqueidentifier",
                nullable: true);

            migrationBuilder.AddColumn<Guid>(
                name: "DefaultBaremeDefenseId",
                table: "AllianceConfigurations",
                type: "uniqueidentifier",
                nullable: true);

            migrationBuilder.CreateIndex(
                name: "IX_AllianceConfigurations_DefaultBaremeAttaqueId",
                table: "AllianceConfigurations",
                column: "DefaultBaremeAttaqueId");

            migrationBuilder.CreateIndex(
                name: "IX_AllianceConfigurations_DefaultBaremeDefenseId",
                table: "AllianceConfigurations",
                column: "DefaultBaremeDefenseId");

            migrationBuilder.AddForeignKey(
                name: "FK_AllianceConfigurations_Baremes_DefaultBaremeAttaqueId",
                table: "AllianceConfigurations",
                column: "DefaultBaremeAttaqueId",
                principalTable: "Baremes",
                principalColumn: "Id");

            migrationBuilder.AddForeignKey(
                name: "FK_AllianceConfigurations_Baremes_DefaultBaremeDefenseId",
                table: "AllianceConfigurations",
                column: "DefaultBaremeDefenseId",
                principalTable: "Baremes",
                principalColumn: "Id");
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropForeignKey(
                name: "FK_AllianceConfigurations_Baremes_DefaultBaremeAttaqueId",
                table: "AllianceConfigurations");

            migrationBuilder.DropForeignKey(
                name: "FK_AllianceConfigurations_Baremes_DefaultBaremeDefenseId",
                table: "AllianceConfigurations");

            migrationBuilder.DropIndex(
                name: "IX_AllianceConfigurations_DefaultBaremeAttaqueId",
                table: "AllianceConfigurations");

            migrationBuilder.DropIndex(
                name: "IX_AllianceConfigurations_DefaultBaremeDefenseId",
                table: "AllianceConfigurations");

            migrationBuilder.DropColumn(
                name: "Type",
                table: "Baremes");

            migrationBuilder.DropColumn(
                name: "DefaultBaremeAttaqueId",
                table: "AllianceConfigurations");

            migrationBuilder.DropColumn(
                name: "DefaultBaremeDefenseId",
                table: "AllianceConfigurations");
        }
    }
}
