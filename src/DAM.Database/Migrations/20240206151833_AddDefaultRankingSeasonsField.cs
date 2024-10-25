using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace DAM.Database.Migrations
{
    /// <inheritdoc />
    public partial class AddDefaultRankingSeasonsField : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.AddColumn<decimal>(
                name: "SeasonRankingChannelId",
                table: "Saisons",
                type: "decimal(20,0)",
                nullable: true);

            migrationBuilder.AddColumn<decimal>(
                name: "DefaultSeasonRankingChannelId",
                table: "AllianceConfigurations",
                type: "decimal(20,0)",
                nullable: true);
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropColumn(
                name: "SeasonRankingChannelId",
                table: "Saisons");

            migrationBuilder.DropColumn(
                name: "DefaultSeasonRankingChannelId",
                table: "AllianceConfigurations");
        }
    }
}
