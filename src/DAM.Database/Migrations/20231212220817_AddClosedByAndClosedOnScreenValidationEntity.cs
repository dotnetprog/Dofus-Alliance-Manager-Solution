using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace DAM.Database.Migrations
{
    /// <inheritdoc />
    public partial class AddClosedByAndClosedOnScreenValidationEntity : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.AddColumn<Guid>(
                name: "ClosedByMemberId",
                table: "ScreenValidations",
                type: "uniqueidentifier",
                nullable: true);

            migrationBuilder.AddColumn<DateTime>(
                name: "ClosedOn",
                table: "ScreenValidations",
                type: "datetime2",
                nullable: true);

            migrationBuilder.CreateIndex(
                name: "IX_ScreenValidations_ClosedByMemberId",
                table: "ScreenValidations",
                column: "ClosedByMemberId");

            migrationBuilder.AddForeignKey(
                name: "FK_ScreenValidations_Members_ClosedByMemberId",
                table: "ScreenValidations",
                column: "ClosedByMemberId",
                principalTable: "Members",
                principalColumn: "Id");


            migrationBuilder.Sql(@"CREATE OR ALTER VIEW [dbo].[AtkScreens] AS
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
										,MAX(sp.ImagePrepUrl) as ImagePrepUrl
										,MAX(sp.AllianceEnemyId) as AllianceEnemyId
										,MAX(sv.ClosedByMemberId) as ClosedByMemberId
										,MAX(sv.ClosedOn) as ClosedOn
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
                                        LEFT JOIN Members m on m.Id = t.CreatedByMemberId");
            migrationBuilder.Sql(@"CREATE OR ALTER     VIEW [dbo].[DefScreens] AS
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
									,MAX(sp.AllianceEnemyId) as AllianceEnemyId
									,MAX(sv.ClosedByMemberId) as ClosedByMemberId
								    ,MAX(sv.ClosedOn) as ClosedOn
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
                                    LEFT JOIN Members m on m.Id = t.CreatedByMemberId");

        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropForeignKey(
                name: "FK_ScreenValidations_Members_ClosedByMemberId",
                table: "ScreenValidations");

            migrationBuilder.DropIndex(
                name: "IX_ScreenValidations_ClosedByMemberId",
                table: "ScreenValidations");

            migrationBuilder.DropColumn(
                name: "ClosedByMemberId",
                table: "ScreenValidations");

            migrationBuilder.DropColumn(
                name: "ClosedOn",
                table: "ScreenValidations");
        }
    }
}
