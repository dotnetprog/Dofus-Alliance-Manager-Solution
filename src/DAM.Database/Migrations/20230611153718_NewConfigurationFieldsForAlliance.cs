using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace DAM.Database.Migrations
{
    /// <inheritdoc />
    public partial class NewConfigurationFieldsForAlliance : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.AddColumn<bool>(
                name: "IsAllianceEnemyRequired",
                table: "AllianceConfigurations",
                type: "bit",
                nullable: true);

            migrationBuilder.AddColumn<bool>(
                name: "IsScreenPrepaRequired",
                table: "AllianceConfigurations",
                type: "bit",
                nullable: true);
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropColumn(
                name: "IsAllianceEnemyRequired",
                table: "AllianceConfigurations");

            migrationBuilder.DropColumn(
                name: "IsScreenPrepaRequired",
                table: "AllianceConfigurations");
        }
    }
}
