using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace DAM.Database.Migrations
{
    /// <inheritdoc />
    public partial class AddAllianceIdInscreenView : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.Sql(@"CREATE OR ALTER   VIEW [dbo].[DefScreens] AS
                                    WITH cte_type AS
                                    (
                                    SELECT 
                                    sp.Id
                                    ,MAX(sp.Description) as Description
                                    ,MAX(sp.Target) as Target
                                    ,MAX(sp.CreatedByMemberId) as CreatedByMemberId
                                    ,MAX(sp.CreatedOn) as CreatedOn
                                    ,SUM(msp.CharacterCount) AS Nombre_Defenseur
                                    ,MAX(sp.EnemyCount) AS Nombre_Attaquant
                                    ,MAX(sp.DiscordChannelId) as DiscordChannelId
                                    ,MAX(sp.DiscordMessageId) as DiscordMessageId
                                    ,MAX(sv.ProcessingState) as StatutTraitementValidation
                                    ,MAX(sv.ResultState) as StatutResultatValidation
                                    ,MAX(sp.ImageUrl) as ImageUrl
                                    FROM ScreenPosts sp
                                    LEFT JOIN Member_ScreenPosts msp
                                    ON msp.ScreenPostId = sp.Id
                                    LEFT JOIN Members m
                                    ON msp.AllianceMemberId = m.Id
                                    Left Join ScreenValidations sv on sv.ScreenPostId = sp.Id
                                    WHERE sp.Type = 1
                                    GROUP BY sp.Id
                                    )
                                    SELECT 
                                    T.*,m.AllianceId as AllianceId
                                    FROM
                                    cte_type t
                                    LEFT JOIN Members m on m.Id = t.CreatedByMemberId
                                    GO");
            migrationBuilder.Sql(@" CREATE OR ALTER   VIEW [dbo].[AtkScreens] AS
                                        WITH cte_type AS
                                        (
                                        SELECT 
                                        sp.Id
                                        ,MAX(sp.Description) as Description
                                        ,MAX(sp.Target) as Target
                                        ,MAX(sp.CreatedByMemberId) as CreatedByMemberId
                                        ,MAX(sp.CreatedOn) as CreatedOn
                                        ,SUM(msp.CharacterCount) AS Nombre_Allie
                                        ,MAX(sp.EnemyCount) AS Nombre_Enemi
                                        ,MAX(sp.DiscordChannelId) as DiscordChannelId
                                        ,MAX(sp.DiscordMessageId) as DiscordMessageId
                                        ,MAX(sv.ProcessingState) as StatutTraitementValidation
                                        ,MAX(sv.ResultState) as StatutResultatValidation
									    ,MAX(sp.ImageUrl) as ImageUrl
                                        FROM ScreenPosts sp
                                        LEFT JOIN Member_ScreenPosts msp
                                        ON msp.ScreenPostId = sp.Id
                                        LEFT JOIN Members m
                                        ON msp.AllianceMemberId = m.Id
                                        Left Join ScreenValidations sv on sv.ScreenPostId = sp.Id
                                        WHERE sp.Type = 0
                                        GROUP BY sp.Id
                                        )
                                        SELECT 
                                        T.*,m.AllianceId as AllianceId
                                        FROM
                                        cte_type t
                                        LEFT JOIN Members m on m.Id = t.CreatedByMemberId
                                    GO");
        
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.Sql(@"DROP VIEW dbo.DefScreens;");
            migrationBuilder.Sql(@"DROP VIEW dbo.AtkScreens;");
        }
    }
}
