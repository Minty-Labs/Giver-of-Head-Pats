﻿using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace HeadPats.Migrations
{
    /// <inheritdoc />
    public partial class DailyPatChannelID : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.AddColumn<decimal>(
                name: "DailyPatChannelId",
                table: "Guilds",
                type: "numeric(20,0)",
                nullable: false,
                defaultValue: 0m);
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropColumn(
                name: "DailyPatChannelId",
                table: "Guilds");
        }
    }
}
