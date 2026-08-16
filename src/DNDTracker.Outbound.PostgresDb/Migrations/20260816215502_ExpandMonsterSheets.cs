using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace DNDTracker.Outbound.PostgresDb.Migrations
{
    /// <inheritdoc />
    public partial class ExpandMonsterSheets : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.AddColumn<string>(
                name: "Actions",
                table: "MonsterStatBlockModel",
                type: "text",
                nullable: false,
                defaultValue: "");

            migrationBuilder.AddColumn<string>(
                name: "BonusActions",
                table: "MonsterStatBlockModel",
                type: "text",
                nullable: false,
                defaultValue: "");

            migrationBuilder.AddColumn<string>(
                name: "Description",
                table: "MonsterStatBlockModel",
                type: "text",
                nullable: false,
                defaultValue: "");

            migrationBuilder.AddColumn<string>(
                name: "LairActions",
                table: "MonsterStatBlockModel",
                type: "text",
                nullable: false,
                defaultValue: "");

            migrationBuilder.AddColumn<string>(
                name: "LegendaryActions",
                table: "MonsterStatBlockModel",
                type: "text",
                nullable: false,
                defaultValue: "");

            migrationBuilder.AddColumn<string>(
                name: "Reactions",
                table: "MonsterStatBlockModel",
                type: "text",
                nullable: false,
                defaultValue: "");

            migrationBuilder.AddColumn<string>(
                name: "Spells",
                table: "MonsterStatBlockModel",
                type: "text",
                nullable: false,
                defaultValue: "");

            migrationBuilder.AddColumn<string>(
                name: "Statistics",
                table: "MonsterStatBlockModel",
                type: "text",
                nullable: false,
                defaultValue: "");
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropColumn(
                name: "Actions",
                table: "MonsterStatBlockModel");

            migrationBuilder.DropColumn(
                name: "BonusActions",
                table: "MonsterStatBlockModel");

            migrationBuilder.DropColumn(
                name: "Description",
                table: "MonsterStatBlockModel");

            migrationBuilder.DropColumn(
                name: "LairActions",
                table: "MonsterStatBlockModel");

            migrationBuilder.DropColumn(
                name: "LegendaryActions",
                table: "MonsterStatBlockModel");

            migrationBuilder.DropColumn(
                name: "Reactions",
                table: "MonsterStatBlockModel");

            migrationBuilder.DropColumn(
                name: "Spells",
                table: "MonsterStatBlockModel");

            migrationBuilder.DropColumn(
                name: "Statistics",
                table: "MonsterStatBlockModel");
        }
    }
}
