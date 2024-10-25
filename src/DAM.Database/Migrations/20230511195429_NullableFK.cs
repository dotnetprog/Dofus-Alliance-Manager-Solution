using System;
using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace DAM.Database.Migrations
{
    /// <inheritdoc />
    public partial class NullableFK : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropForeignKey(
                name: "FK_Alliances_AllianceConfigurations_AllianceConfigurationId",
                table: "Alliances");

            migrationBuilder.AlterColumn<Guid>(
                name: "AllianceConfigurationId",
                table: "Alliances",
                type: "uniqueidentifier",
                nullable: true,
                oldClrType: typeof(Guid),
                oldType: "uniqueidentifier");

            migrationBuilder.AddForeignKey(
                name: "FK_Alliances_AllianceConfigurations_AllianceConfigurationId",
                table: "Alliances",
                column: "AllianceConfigurationId",
                principalTable: "AllianceConfigurations",
                principalColumn: "Id");
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropForeignKey(
                name: "FK_Alliances_AllianceConfigurations_AllianceConfigurationId",
                table: "Alliances");

            migrationBuilder.AlterColumn<Guid>(
                name: "AllianceConfigurationId",
                table: "Alliances",
                type: "uniqueidentifier",
                nullable: false,
                defaultValue: new Guid("00000000-0000-0000-0000-000000000000"),
                oldClrType: typeof(Guid),
                oldType: "uniqueidentifier",
                oldNullable: true);

            migrationBuilder.AddForeignKey(
                name: "FK_Alliances_AllianceConfigurations_AllianceConfigurationId",
                table: "Alliances",
                column: "AllianceConfigurationId",
                principalTable: "AllianceConfigurations",
                principalColumn: "Id",
                onDelete: ReferentialAction.Cascade);
        }
    }
}
