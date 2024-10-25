using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace DAM.Database.Migrations
{
    /// <inheritdoc />
    public partial class AddScreenPostViews : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.Sql("CREATE OR ALTER VIEW DefScreens AS\r\nWITH cte_type AS\r\n(\r\nSELECT \r\nsp.Id\r\n,MAX(sp.Description) as Description\r\n,MAX(sp.Target) as Target\r\n,MAX(sp.CreatedByMemberId) as CreatedByMemberId\r\n,MAX(sp.CreatedOn) as CreatedOn\r\n,SUM(msp.CharacterCount) AS Nombre_Defenseur\r\n,MAX(sp.EnemyCount) AS Nombre_Attaquant\r\n,MAX(sp.DiscordChannelId) as DiscordChannelId\r\n,MAX(sp.DiscordMessageId) as DiscordMessageId\r\n,MAX(sv.ProcessingState) as StatutTraitementValidation\r\n,MAX(sv.ResultState) as StatutResultatValidation\r\nFROM ScreenPosts sp\r\nLEFT JOIN Member_ScreenPosts msp\r\nON msp.ScreenPostId = sp.Id\r\nLEFT JOIN Members m\r\nON msp.AllianceMemberId = m.Id\r\nLeft Join ScreenValidations sv on sv.ScreenPostId = sp.Id\r\nWHERE sp.Type = 1\r\nGROUP BY sp.Id\r\n)\r\nSELECT \r\nT.*\r\nFROM\r\ncte_type t\r\n\r\nGO");
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.Sql(@"DROP VIEW dbo.DefScreens;");
        }
    }
}
