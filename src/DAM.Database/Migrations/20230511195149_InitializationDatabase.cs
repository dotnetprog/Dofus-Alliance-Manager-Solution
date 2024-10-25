using System;
using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace DAM.Database.Migrations
{
    /// <inheritdoc />
    public partial class InitializationDatabase : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.CreateTable(
                name: "AllianceConfigurations",
                columns: table => new
                {
                    Id = table.Column<Guid>(type: "uniqueidentifier", nullable: false),
                    AtkScreen_DiscordChannelId = table.Column<decimal>(type: "decimal(20,0)", nullable: false),
                    DefScreen_DiscordChannelId = table.Column<decimal>(type: "decimal(20,0)", nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_AllianceConfigurations", x => x.Id);
                });

            migrationBuilder.CreateTable(
                name: "Alliances",
                columns: table => new
                {
                    Id = table.Column<Guid>(type: "uniqueidentifier", nullable: false),
                    Alias = table.Column<string>(type: "nvarchar(max)", nullable: true),
                    DiscordGuildId = table.Column<string>(type: "nvarchar(max)", nullable: true),
                    RegisteredOn = table.Column<DateTime>(type: "datetime2", nullable: true),
                    AllianceConfigurationId = table.Column<Guid>(type: "uniqueidentifier", nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_Alliances", x => x.Id);
                    table.ForeignKey(
                        name: "FK_Alliances_AllianceConfigurations_AllianceConfigurationId",
                        column: x => x.AllianceConfigurationId,
                        principalTable: "AllianceConfigurations",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Cascade);
                });

            migrationBuilder.CreateTable(
                name: "Members",
                columns: table => new
                {
                    Id = table.Column<Guid>(type: "uniqueidentifier", nullable: false),
                    AllianceId = table.Column<Guid>(type: "uniqueidentifier", nullable: true),
                    DiscordId = table.Column<string>(type: "nvarchar(max)", nullable: true),
                    Alias = table.Column<string>(type: "nvarchar(max)", nullable: true),
                    CreatedOn = table.Column<DateTime>(type: "datetime2", nullable: false),
                    ModifiedOn = table.Column<DateTime>(type: "datetime2", nullable: false),
                    State = table.Column<int>(type: "int", nullable: false),
                    Status = table.Column<int>(type: "int", nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_Members", x => x.Id);
                    table.ForeignKey(
                        name: "FK_Members_Alliances_AllianceId",
                        column: x => x.AllianceId,
                        principalTable: "Alliances",
                        principalColumn: "Id");
                });

            migrationBuilder.CreateTable(
                name: "ScreenPosts",
                columns: table => new
                {
                    Id = table.Column<Guid>(type: "uniqueidentifier", nullable: false),
                    Description = table.Column<string>(type: "nvarchar(max)", nullable: true),
                    Base64Image = table.Column<string>(type: "nvarchar(max)", nullable: true),
                    ImageUrl = table.Column<string>(type: "nvarchar(max)", nullable: true),
                    Filesize = table.Column<int>(type: "int", nullable: true),
                    EnemyCount = table.Column<int>(type: "int", nullable: false),
                    Type = table.Column<int>(type: "int", nullable: false),
                    Target = table.Column<int>(type: "int", nullable: false),
                    CreatedOn = table.Column<DateTime>(type: "datetime2", nullable: false),
                    CreatedByMemberId = table.Column<Guid>(type: "uniqueidentifier", nullable: true)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_ScreenPosts", x => x.Id);
                    table.ForeignKey(
                        name: "FK_ScreenPosts_Members_CreatedByMemberId",
                        column: x => x.CreatedByMemberId,
                        principalTable: "Members",
                        principalColumn: "Id");
                });

            migrationBuilder.CreateTable(
                name: "Member_ScreenPosts",
                columns: table => new
                {
                    AllianceMemberId = table.Column<Guid>(type: "uniqueidentifier", nullable: false),
                    ScreenPostId = table.Column<Guid>(type: "uniqueidentifier", nullable: false),
                    CharacterCount = table.Column<int>(type: "int", nullable: true)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_Member_ScreenPosts", x => new { x.ScreenPostId, x.AllianceMemberId });
                    table.ForeignKey(
                        name: "FK_Member_ScreenPosts_Members_AllianceMemberId",
                        column: x => x.AllianceMemberId,
                        principalTable: "Members",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Cascade);
                    table.ForeignKey(
                        name: "FK_Member_ScreenPosts_ScreenPosts_ScreenPostId",
                        column: x => x.ScreenPostId,
                        principalTable: "ScreenPosts",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Cascade);
                });

            migrationBuilder.CreateTable(
                name: "ScreenValidations",
                columns: table => new
                {
                    Id = table.Column<Guid>(type: "uniqueidentifier", nullable: false),
                    ScreenPostId = table.Column<Guid>(type: "uniqueidentifier", nullable: false),
                    OCRScreenText = table.Column<string>(type: "nvarchar(max)", nullable: false),
                    ProcessingState = table.Column<int>(type: "int", nullable: false),
                    ResultState = table.Column<int>(type: "int", nullable: true)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_ScreenValidations", x => x.Id);
                    table.ForeignKey(
                        name: "FK_ScreenValidations_ScreenPosts_ScreenPostId",
                        column: x => x.ScreenPostId,
                        principalTable: "ScreenPosts",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Cascade);
                });

            migrationBuilder.CreateIndex(
                name: "IX_Alliances_AllianceConfigurationId",
                table: "Alliances",
                column: "AllianceConfigurationId");

            migrationBuilder.CreateIndex(
                name: "IX_Member_ScreenPosts_AllianceMemberId",
                table: "Member_ScreenPosts",
                column: "AllianceMemberId");

            migrationBuilder.CreateIndex(
                name: "IX_Members_AllianceId",
                table: "Members",
                column: "AllianceId");

            migrationBuilder.CreateIndex(
                name: "IX_ScreenPosts_CreatedByMemberId",
                table: "ScreenPosts",
                column: "CreatedByMemberId");

            migrationBuilder.CreateIndex(
                name: "IX_ScreenValidations_ScreenPostId",
                table: "ScreenValidations",
                column: "ScreenPostId");
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropTable(
                name: "Member_ScreenPosts");

            migrationBuilder.DropTable(
                name: "ScreenValidations");

            migrationBuilder.DropTable(
                name: "ScreenPosts");

            migrationBuilder.DropTable(
                name: "Members");

            migrationBuilder.DropTable(
                name: "Alliances");

            migrationBuilder.DropTable(
                name: "AllianceConfigurations");
        }
    }
}
