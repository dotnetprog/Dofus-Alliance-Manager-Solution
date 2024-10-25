using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace DAM.Database.Migrations
{
    /// <inheritdoc />
    public partial class AddConfigROleId : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.AddColumn<decimal>(
                name: "DiscordChannelId",
                table: "ScreenPosts",
                type: "decimal(20,0)",
                nullable: true);

            migrationBuilder.AddColumn<decimal>(
                name: "DiscordMessageId",
                table: "ScreenPosts",
                type: "decimal(20,0)",
                nullable: true);

            migrationBuilder.AddColumn<decimal>(
                name: "ScreenApproverRoleId",
                table: "AllianceConfigurations",
                type: "decimal(20,0)",
                nullable: true);
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropColumn(
                name: "DiscordChannelId",
                table: "ScreenPosts");

            migrationBuilder.DropColumn(
                name: "DiscordMessageId",
                table: "ScreenPosts");

            migrationBuilder.DropColumn(
                name: "ScreenApproverRoleId",
                table: "AllianceConfigurations");
        }
    }
}
