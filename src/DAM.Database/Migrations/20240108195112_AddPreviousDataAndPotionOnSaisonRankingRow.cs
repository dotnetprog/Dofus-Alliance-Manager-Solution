using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace DAM.Database.Migrations
{
    /// <inheritdoc />
    public partial class AddPreviousDataAndPotionOnSaisonRankingRow : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.AddColumn<int>(
                name: "BonusPepite",
                table: "SaisonRankings",
                type: "int",
                nullable: true);

            migrationBuilder.AddColumn<int>(
                name: "PositionRank",
                table: "SaisonRankings",
                type: "int",
                nullable: false,
                defaultValue: 0);

            migrationBuilder.AddColumn<int>(
                name: "Previous_BonusPepite",
                table: "SaisonRankings",
                type: "int",
                nullable: true);

            migrationBuilder.AddColumn<int>(
                name: "Previous_MontantAtkPepites",
                table: "SaisonRankings",
                type: "int",
                nullable: true);

            migrationBuilder.AddColumn<int>(
                name: "Previous_MontantAvAPepites",
                table: "SaisonRankings",
                type: "int",
                nullable: true);

            migrationBuilder.AddColumn<int>(
                name: "Previous_MontantDefPepites",
                table: "SaisonRankings",
                type: "int",
                nullable: true);

            migrationBuilder.AddColumn<int>(
                name: "Previous_MontantTotalPepite",
                table: "SaisonRankings",
                type: "int",
                nullable: true);

            migrationBuilder.AddColumn<int>(
                name: "Previous_NombreParticipationAvA",
                table: "SaisonRankings",
                type: "int",
                nullable: true);

            migrationBuilder.AddColumn<int>(
                name: "Previous_Nombre_attaques",
                table: "SaisonRankings",
                type: "int",
                nullable: true);

            migrationBuilder.AddColumn<int>(
                name: "Previous_Nombre_defense",
                table: "SaisonRankings",
                type: "int",
                nullable: true);

            migrationBuilder.AddColumn<int>(
                name: "Previous_PositionRank",
                table: "SaisonRankings",
                type: "int",
                nullable: true);

            migrationBuilder.AddColumn<string>(
                name: "Previous_bonusReason",
                table: "SaisonRankings",
                type: "nvarchar(max)",
                nullable: true);

            migrationBuilder.AddColumn<string>(
                name: "bonusReason",
                table: "SaisonRankings",
                type: "nvarchar(max)",
                nullable: true);
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropColumn(
                name: "BonusPepite",
                table: "SaisonRankings");

            migrationBuilder.DropColumn(
                name: "PositionRank",
                table: "SaisonRankings");

            migrationBuilder.DropColumn(
                name: "Previous_BonusPepite",
                table: "SaisonRankings");

            migrationBuilder.DropColumn(
                name: "Previous_MontantAtkPepites",
                table: "SaisonRankings");

            migrationBuilder.DropColumn(
                name: "Previous_MontantAvAPepites",
                table: "SaisonRankings");

            migrationBuilder.DropColumn(
                name: "Previous_MontantDefPepites",
                table: "SaisonRankings");

            migrationBuilder.DropColumn(
                name: "Previous_MontantTotalPepite",
                table: "SaisonRankings");

            migrationBuilder.DropColumn(
                name: "Previous_NombreParticipationAvA",
                table: "SaisonRankings");

            migrationBuilder.DropColumn(
                name: "Previous_Nombre_attaques",
                table: "SaisonRankings");

            migrationBuilder.DropColumn(
                name: "Previous_Nombre_defense",
                table: "SaisonRankings");

            migrationBuilder.DropColumn(
                name: "Previous_PositionRank",
                table: "SaisonRankings");

            migrationBuilder.DropColumn(
                name: "Previous_bonusReason",
                table: "SaisonRankings");

            migrationBuilder.DropColumn(
                name: "bonusReason",
                table: "SaisonRankings");
        }
    }
}
