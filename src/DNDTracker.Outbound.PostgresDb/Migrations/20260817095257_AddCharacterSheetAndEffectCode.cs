using System;
using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace DNDTracker.Outbound.PostgresDb.Migrations
{
    /// <inheritdoc />
    public partial class AddCharacterSheetAndEffectCode : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.AddColumn<string>(
                name: "EffectCode",
                table: "SpellModel",
                type: "text",
                nullable: true);

            migrationBuilder.AddColumn<string>(
                name: "EffectCode",
                table: "InventoryItemModel",
                type: "text",
                nullable: true);

            migrationBuilder.AddColumn<string>(
                name: "Bonds",
                table: "HeroModel",
                type: "text",
                nullable: false,
                defaultValue: "");

            migrationBuilder.AddColumn<int>(
                name: "DeathSaveFailures",
                table: "HeroModel",
                type: "integer",
                nullable: false,
                defaultValue: 0);

            migrationBuilder.AddColumn<int>(
                name: "DeathSaveSuccesses",
                table: "HeroModel",
                type: "integer",
                nullable: false,
                defaultValue: 0);

            migrationBuilder.AddColumn<string>(
                name: "Flaws",
                table: "HeroModel",
                type: "text",
                nullable: false,
                defaultValue: "");

            migrationBuilder.AddColumn<string>(
                name: "Ideals",
                table: "HeroModel",
                type: "text",
                nullable: false,
                defaultValue: "");

            migrationBuilder.AddColumn<string>(
                name: "PersonalityTraits",
                table: "HeroModel",
                type: "text",
                nullable: false,
                defaultValue: "");

            migrationBuilder.AddColumn<int>(
                name: "SpellcastingAbility",
                table: "HeroModel",
                type: "integer",
                nullable: true);

            migrationBuilder.AddColumn<string>(
                name: "EffectCode",
                table: "HeroConditionModel",
                type: "text",
                nullable: true);

            migrationBuilder.AddColumn<string>(
                name: "EffectCode",
                table: "EquipmentItemModel",
                type: "text",
                nullable: true);

            migrationBuilder.CreateTable(
                name: "HeroFeatModel",
                columns: table => new
                {
                    Id = table.Column<Guid>(type: "uuid", nullable: false),
                    HeroId = table.Column<Guid>(type: "uuid", nullable: false),
                    FeatName = table.Column<string>(type: "character varying(200)", maxLength: 200, nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_HeroFeatModel", x => x.Id);
                    table.ForeignKey(
                        name: "FK_HeroFeatModel_HeroModel_HeroId",
                        column: x => x.HeroId,
                        principalTable: "HeroModel",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Cascade);
                });

            migrationBuilder.CreateTable(
                name: "HeroSavingThrowProficiencyModel",
                columns: table => new
                {
                    Id = table.Column<Guid>(type: "uuid", nullable: false),
                    HeroId = table.Column<Guid>(type: "uuid", nullable: false),
                    Ability = table.Column<int>(type: "integer", nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_HeroSavingThrowProficiencyModel", x => x.Id);
                    table.ForeignKey(
                        name: "FK_HeroSavingThrowProficiencyModel_HeroModel_HeroId",
                        column: x => x.HeroId,
                        principalTable: "HeroModel",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Cascade);
                });

            migrationBuilder.CreateTable(
                name: "HeroSkillProficiencyModel",
                columns: table => new
                {
                    Id = table.Column<Guid>(type: "uuid", nullable: false),
                    HeroId = table.Column<Guid>(type: "uuid", nullable: false),
                    Skill = table.Column<int>(type: "integer", nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_HeroSkillProficiencyModel", x => x.Id);
                    table.ForeignKey(
                        name: "FK_HeroSkillProficiencyModel_HeroModel_HeroId",
                        column: x => x.HeroId,
                        principalTable: "HeroModel",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Cascade);
                });

            migrationBuilder.CreateIndex(
                name: "IX_HeroFeatModel_HeroId",
                table: "HeroFeatModel",
                column: "HeroId");

            migrationBuilder.CreateIndex(
                name: "IX_HeroSavingThrowProficiencyModel_HeroId",
                table: "HeroSavingThrowProficiencyModel",
                column: "HeroId");

            migrationBuilder.CreateIndex(
                name: "IX_HeroSkillProficiencyModel_HeroId",
                table: "HeroSkillProficiencyModel",
                column: "HeroId");
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropTable(
                name: "HeroFeatModel");

            migrationBuilder.DropTable(
                name: "HeroSavingThrowProficiencyModel");

            migrationBuilder.DropTable(
                name: "HeroSkillProficiencyModel");

            migrationBuilder.DropColumn(
                name: "EffectCode",
                table: "SpellModel");

            migrationBuilder.DropColumn(
                name: "EffectCode",
                table: "InventoryItemModel");

            migrationBuilder.DropColumn(
                name: "Bonds",
                table: "HeroModel");

            migrationBuilder.DropColumn(
                name: "DeathSaveFailures",
                table: "HeroModel");

            migrationBuilder.DropColumn(
                name: "DeathSaveSuccesses",
                table: "HeroModel");

            migrationBuilder.DropColumn(
                name: "Flaws",
                table: "HeroModel");

            migrationBuilder.DropColumn(
                name: "Ideals",
                table: "HeroModel");

            migrationBuilder.DropColumn(
                name: "PersonalityTraits",
                table: "HeroModel");

            migrationBuilder.DropColumn(
                name: "SpellcastingAbility",
                table: "HeroModel");

            migrationBuilder.DropColumn(
                name: "EffectCode",
                table: "HeroConditionModel");

            migrationBuilder.DropColumn(
                name: "EffectCode",
                table: "EquipmentItemModel");
        }
    }
}
