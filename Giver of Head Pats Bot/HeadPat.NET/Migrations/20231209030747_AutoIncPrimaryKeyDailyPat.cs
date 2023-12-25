using Microsoft.EntityFrameworkCore.Migrations;
using Npgsql.EntityFrameworkCore.PostgreSQL.Metadata;

#nullable disable

namespace HeadPats.Migrations
{
    /// <inheritdoc />
    public partial class AutoIncPrimaryKeyDailyPat : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropPrimaryKey(
                name: "PK_DailyPats",
                table: "DailyPats");

            migrationBuilder.AddColumn<long>(
                name: "Id",
                table: "DailyPats",
                type: "bigint",
                nullable: false,
                defaultValue: 0L)
                .Annotation("Npgsql:ValueGenerationStrategy", NpgsqlValueGenerationStrategy.IdentityByDefaultColumn);

            migrationBuilder.AddPrimaryKey(
                name: "PK_DailyPats",
                table: "DailyPats",
                column: "Id");
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropPrimaryKey(
                name: "PK_DailyPats",
                table: "DailyPats");

            migrationBuilder.DropColumn(
                name: "Id",
                table: "DailyPats");

            migrationBuilder.AddPrimaryKey(
                name: "PK_DailyPats",
                table: "DailyPats",
                column: "UserId");
        }
    }
}
