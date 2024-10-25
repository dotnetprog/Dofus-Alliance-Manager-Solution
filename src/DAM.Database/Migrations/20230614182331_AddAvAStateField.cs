﻿using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace DAM.Database.Migrations
{
    /// <inheritdoc />
    public partial class AddAvAStateField : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.AddColumn<int>(
                name: "State",
                table: "AvA",
                type: "int",
                nullable: true);
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropColumn(
                name: "State",
                table: "AvA");
        }
    }
}
