using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace DAM.Database.Migrations
{
    /// <inheritdoc />
    public partial class AddBehaviorConfigToAlliance : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.AddColumn<string>(
                name: "BehaviorScreenConfigJSONData",
                table: "AllianceConfigurations",
                type: "nvarchar(max)",
                nullable: true);

            migrationBuilder.AddColumn<int>(
                name: "BotScreenBehaviorType",
                table: "AllianceConfigurations",
                type: "int",
                nullable: true, defaultValue: 1);
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropColumn(
                name: "BehaviorScreenConfigJSONData",
                table: "AllianceConfigurations");

            migrationBuilder.DropColumn(
                name: "BotScreenBehaviorType",
                table: "AllianceConfigurations");
        }
    }
}
