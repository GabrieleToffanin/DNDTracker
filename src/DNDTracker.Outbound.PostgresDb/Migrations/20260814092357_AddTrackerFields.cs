using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace DNDTracker.Outbound.PostgresDb.Migrations
{
    /// <inheritdoc />
    public partial class AddTrackerFields : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.AddColumn<int>(
                name: "ArmorClass",
                table: "HeroModel",
                type: "integer",
                nullable: false,
                defaultValue: 10);

            migrationBuilder.AddColumn<string>(
                name: "Background",
                table: "HeroModel",
                type: "text",
                nullable: false,
                defaultValue: "");

            migrationBuilder.AddColumn<int>(
                name: "Charisma",
                table: "HeroModel",
                type: "integer",
                nullable: false,
                defaultValue: 0);

            migrationBuilder.AddColumn<string>(
                name: "ConditionsJson",
                table: "HeroModel",
                type: "text",
                nullable: false,
                defaultValue: "[]");

            migrationBuilder.AddColumn<int>(
                name: "Constitution",
                table: "HeroModel",
                type: "integer",
                nullable: false,
                defaultValue: 0);

            migrationBuilder.AddColumn<int>(
                name: "CurrentHitPoints",
                table: "HeroModel",
                type: "integer",
                nullable: false,
                defaultValue: 0);

            migrationBuilder.AddColumn<int>(
                name: "Dexterity",
                table: "HeroModel",
                type: "integer",
                nullable: false,
                defaultValue: 0);

            migrationBuilder.AddColumn<string>(
                name: "EquipmentJson",
                table: "HeroModel",
                type: "text",
                nullable: false,
                defaultValue: "[]");

            migrationBuilder.AddColumn<int>(
                name: "Initiative",
                table: "HeroModel",
                type: "integer",
                nullable: false,
                defaultValue: 0);

            migrationBuilder.AddColumn<int>(
                name: "Intelligence",
                table: "HeroModel",
                type: "integer",
                nullable: false,
                defaultValue: 0);

            migrationBuilder.AddColumn<string>(
                name: "InventoryJson",
                table: "HeroModel",
                type: "text",
                nullable: false,
                defaultValue: "[]");

            migrationBuilder.AddColumn<bool>(
                name: "IsNonPlayerCharacter",
                table: "HeroModel",
                type: "boolean",
                nullable: false,
                defaultValue: false);

            migrationBuilder.AddColumn<int>(
                name: "MaxHitPoints",
                table: "HeroModel",
                type: "integer",
                nullable: false,
                defaultValue: 0);

            migrationBuilder.Sql("UPDATE \"HeroModel\" SET \"MaxHitPoints\" = \"HitPoints\", \"CurrentHitPoints\" = \"HitPoints\" WHERE \"MaxHitPoints\" = 0 AND \"CurrentHitPoints\" = 0;");

            migrationBuilder.AddColumn<string>(
                name: "Notes",
                table: "HeroModel",
                type: "text",
                nullable: false,
                defaultValue: "");

            migrationBuilder.AddColumn<int>(
                name: "Speed",
                table: "HeroModel",
                type: "integer",
                nullable: false,
                defaultValue: 30);

            migrationBuilder.AddColumn<string>(
                name: "SpellSlotsJson",
                table: "HeroModel",
                type: "text",
                nullable: false,
                defaultValue: "[]");

            migrationBuilder.AddColumn<string>(
                name: "SpellbookJson",
                table: "HeroModel",
                type: "text",
                nullable: false,
                defaultValue: "[]");

            migrationBuilder.AddColumn<int>(
                name: "Strength",
                table: "HeroModel",
                type: "integer",
                nullable: false,
                defaultValue: 0);

            migrationBuilder.AddColumn<int>(
                name: "TemporaryHitPoints",
                table: "HeroModel",
                type: "integer",
                nullable: false,
                defaultValue: 0);

            migrationBuilder.AddColumn<int>(
                name: "Wisdom",
                table: "HeroModel",
                type: "integer",
                nullable: false,
                defaultValue: 0);

            migrationBuilder.AddColumn<string>(
                name: "ActiveCombatJson",
                table: "CampaignModel",
                type: "text",
                nullable: true);

            migrationBuilder.AddColumn<string>(
                name: "LocationsJson",
                table: "CampaignModel",
                type: "text",
                nullable: false,
                defaultValue: "[]");

            migrationBuilder.AddColumn<string>(
                name: "LootJson",
                table: "CampaignModel",
                type: "text",
                nullable: false,
                defaultValue: "[]");

            migrationBuilder.AddColumn<string>(
                name: "MembersJson",
                table: "CampaignModel",
                type: "text",
                nullable: false,
                defaultValue: "[]");

            migrationBuilder.AddColumn<string>(
                name: "MonsterLibraryJson",
                table: "CampaignModel",
                type: "text",
                nullable: false,
                defaultValue: "[]");

            migrationBuilder.AddColumn<string>(
                name: "NpcsJson",
                table: "CampaignModel",
                type: "text",
                nullable: false,
                defaultValue: "[]");

            migrationBuilder.AddColumn<string>(
                name: "QuestsJson",
                table: "CampaignModel",
                type: "text",
                nullable: false,
                defaultValue: "[]");

            migrationBuilder.AddColumn<string>(
                name: "SessionLogsJson",
                table: "CampaignModel",
                type: "text",
                nullable: false,
                defaultValue: "[]");

            migrationBuilder.AddColumn<string>(
                name: "TimelineEntriesJson",
                table: "CampaignModel",
                type: "text",
                nullable: false,
                defaultValue: "[]");
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropColumn(
                name: "ArmorClass",
                table: "HeroModel");

            migrationBuilder.DropColumn(
                name: "Background",
                table: "HeroModel");

            migrationBuilder.DropColumn(
                name: "Charisma",
                table: "HeroModel");

            migrationBuilder.DropColumn(
                name: "ConditionsJson",
                table: "HeroModel");

            migrationBuilder.DropColumn(
                name: "Constitution",
                table: "HeroModel");

            migrationBuilder.DropColumn(
                name: "CurrentHitPoints",
                table: "HeroModel");

            migrationBuilder.DropColumn(
                name: "Dexterity",
                table: "HeroModel");

            migrationBuilder.DropColumn(
                name: "EquipmentJson",
                table: "HeroModel");

            migrationBuilder.DropColumn(
                name: "Initiative",
                table: "HeroModel");

            migrationBuilder.DropColumn(
                name: "Intelligence",
                table: "HeroModel");

            migrationBuilder.DropColumn(
                name: "InventoryJson",
                table: "HeroModel");

            migrationBuilder.DropColumn(
                name: "IsNonPlayerCharacter",
                table: "HeroModel");

            migrationBuilder.DropColumn(
                name: "MaxHitPoints",
                table: "HeroModel");

            migrationBuilder.DropColumn(
                name: "Notes",
                table: "HeroModel");

            migrationBuilder.DropColumn(
                name: "Speed",
                table: "HeroModel");

            migrationBuilder.DropColumn(
                name: "SpellSlotsJson",
                table: "HeroModel");

            migrationBuilder.DropColumn(
                name: "SpellbookJson",
                table: "HeroModel");

            migrationBuilder.DropColumn(
                name: "Strength",
                table: "HeroModel");

            migrationBuilder.DropColumn(
                name: "TemporaryHitPoints",
                table: "HeroModel");

            migrationBuilder.DropColumn(
                name: "Wisdom",
                table: "HeroModel");

            migrationBuilder.DropColumn(
                name: "ActiveCombatJson",
                table: "CampaignModel");

            migrationBuilder.DropColumn(
                name: "LocationsJson",
                table: "CampaignModel");

            migrationBuilder.DropColumn(
                name: "LootJson",
                table: "CampaignModel");

            migrationBuilder.DropColumn(
                name: "MembersJson",
                table: "CampaignModel");

            migrationBuilder.DropColumn(
                name: "MonsterLibraryJson",
                table: "CampaignModel");

            migrationBuilder.DropColumn(
                name: "NpcsJson",
                table: "CampaignModel");

            migrationBuilder.DropColumn(
                name: "QuestsJson",
                table: "CampaignModel");

            migrationBuilder.DropColumn(
                name: "SessionLogsJson",
                table: "CampaignModel");

            migrationBuilder.DropColumn(
                name: "TimelineEntriesJson",
                table: "CampaignModel");
        }
    }
}
