using System;
using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace DNDTracker.Outbound.PostgresDb.Migrations
{
    /// <inheritdoc />
    public partial class AddCampaignTrackingModels : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropColumn(
                name: "ConditionsJson",
                table: "HeroModel");

            migrationBuilder.DropColumn(
                name: "EquipmentJson",
                table: "HeroModel");

            migrationBuilder.DropColumn(
                name: "InventoryJson",
                table: "HeroModel");

            migrationBuilder.DropColumn(
                name: "SpellSlotsJson",
                table: "HeroModel");

            migrationBuilder.DropColumn(
                name: "SpellbookJson",
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

            migrationBuilder.CreateTable(
                name: "ActiveCombatModel",
                columns: table => new
                {
                    CampaignId = table.Column<Guid>(type: "uuid", nullable: false),
                    Round = table.Column<int>(type: "integer", nullable: false),
                    TurnIndex = table.Column<int>(type: "integer", nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_ActiveCombatModel", x => x.CampaignId);
                    table.ForeignKey(
                        name: "FK_ActiveCombatModel_CampaignModel_CampaignId",
                        column: x => x.CampaignId,
                        principalTable: "CampaignModel",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Cascade);
                });

            migrationBuilder.CreateTable(
                name: "CampaignMemberModel",
                columns: table => new
                {
                    Id = table.Column<Guid>(type: "uuid", nullable: false),
                    CampaignId = table.Column<Guid>(type: "uuid", nullable: false),
                    UserId = table.Column<Guid>(type: "uuid", nullable: false),
                    DisplayName = table.Column<string>(type: "text", nullable: false),
                    Role = table.Column<int>(type: "integer", nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_CampaignMemberModel", x => x.Id);
                    table.ForeignKey(
                        name: "FK_CampaignMemberModel_CampaignModel_CampaignId",
                        column: x => x.CampaignId,
                        principalTable: "CampaignModel",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Cascade);
                });

            migrationBuilder.CreateTable(
                name: "CampaignTimelineEntryModel",
                columns: table => new
                {
                    Id = table.Column<Guid>(type: "uuid", nullable: false),
                    CampaignId = table.Column<Guid>(type: "uuid", nullable: false),
                    OccurredAt = table.Column<DateTime>(type: "timestamp with time zone", nullable: false),
                    Description = table.Column<string>(type: "text", nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_CampaignTimelineEntryModel", x => x.Id);
                    table.ForeignKey(
                        name: "FK_CampaignTimelineEntryModel_CampaignModel_CampaignId",
                        column: x => x.CampaignId,
                        principalTable: "CampaignModel",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Cascade);
                });

            migrationBuilder.CreateTable(
                name: "EquipmentItemModel",
                columns: table => new
                {
                    Id = table.Column<Guid>(type: "uuid", nullable: false),
                    HeroId = table.Column<Guid>(type: "uuid", nullable: false),
                    Name = table.Column<string>(type: "text", nullable: false),
                    Quantity = table.Column<int>(type: "integer", nullable: false),
                    Notes = table.Column<string>(type: "text", nullable: true)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_EquipmentItemModel", x => x.Id);
                    table.ForeignKey(
                        name: "FK_EquipmentItemModel_HeroModel_HeroId",
                        column: x => x.HeroId,
                        principalTable: "HeroModel",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Cascade);
                });

            migrationBuilder.CreateTable(
                name: "HeroConditionModel",
                columns: table => new
                {
                    Id = table.Column<Guid>(type: "uuid", nullable: false),
                    HeroId = table.Column<Guid>(type: "uuid", nullable: false),
                    Name = table.Column<string>(type: "text", nullable: false),
                    RemainingRounds = table.Column<int>(type: "integer", nullable: true)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_HeroConditionModel", x => x.Id);
                    table.ForeignKey(
                        name: "FK_HeroConditionModel_HeroModel_HeroId",
                        column: x => x.HeroId,
                        principalTable: "HeroModel",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Cascade);
                });

            migrationBuilder.CreateTable(
                name: "InventoryItemModel",
                columns: table => new
                {
                    Id = table.Column<Guid>(type: "uuid", nullable: false),
                    HeroId = table.Column<Guid>(type: "uuid", nullable: false),
                    Name = table.Column<string>(type: "text", nullable: false),
                    Quantity = table.Column<int>(type: "integer", nullable: false),
                    Notes = table.Column<string>(type: "text", nullable: true)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_InventoryItemModel", x => x.Id);
                    table.ForeignKey(
                        name: "FK_InventoryItemModel_HeroModel_HeroId",
                        column: x => x.HeroId,
                        principalTable: "HeroModel",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Cascade);
                });

            migrationBuilder.CreateTable(
                name: "LocationResourceModel",
                columns: table => new
                {
                    Id = table.Column<Guid>(type: "uuid", nullable: false),
                    CampaignId = table.Column<Guid>(type: "uuid", nullable: false),
                    Name = table.Column<string>(type: "text", nullable: false),
                    Description = table.Column<string>(type: "text", nullable: false),
                    MapUrl = table.Column<string>(type: "text", nullable: true)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_LocationResourceModel", x => x.Id);
                    table.ForeignKey(
                        name: "FK_LocationResourceModel_CampaignModel_CampaignId",
                        column: x => x.CampaignId,
                        principalTable: "CampaignModel",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Cascade);
                });

            migrationBuilder.CreateTable(
                name: "LootResourceModel",
                columns: table => new
                {
                    Id = table.Column<Guid>(type: "uuid", nullable: false),
                    CampaignId = table.Column<Guid>(type: "uuid", nullable: false),
                    Name = table.Column<string>(type: "text", nullable: false),
                    IsMagicItem = table.Column<bool>(type: "boolean", nullable: false),
                    Notes = table.Column<string>(type: "text", nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_LootResourceModel", x => x.Id);
                    table.ForeignKey(
                        name: "FK_LootResourceModel_CampaignModel_CampaignId",
                        column: x => x.CampaignId,
                        principalTable: "CampaignModel",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Cascade);
                });

            migrationBuilder.CreateTable(
                name: "MonsterStatBlockModel",
                columns: table => new
                {
                    Id = table.Column<Guid>(type: "uuid", nullable: false),
                    CampaignId = table.Column<Guid>(type: "uuid", nullable: false),
                    Name = table.Column<string>(type: "text", nullable: false),
                    CreatureType = table.Column<string>(type: "text", nullable: false),
                    ArmorClass = table.Column<int>(type: "integer", nullable: false),
                    HitPoints = table.Column<int>(type: "integer", nullable: false),
                    ChallengeRating = table.Column<int>(type: "integer", nullable: false),
                    ExperiencePoints = table.Column<int>(type: "integer", nullable: false),
                    InitiativeModifier = table.Column<int>(type: "integer", nullable: false),
                    Speed = table.Column<int>(type: "integer", nullable: false),
                    Notes = table.Column<string>(type: "text", nullable: true)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_MonsterStatBlockModel", x => x.Id);
                    table.ForeignKey(
                        name: "FK_MonsterStatBlockModel_CampaignModel_CampaignId",
                        column: x => x.CampaignId,
                        principalTable: "CampaignModel",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Cascade);
                });

            migrationBuilder.CreateTable(
                name: "NpcResourceModel",
                columns: table => new
                {
                    Id = table.Column<Guid>(type: "uuid", nullable: false),
                    CampaignId = table.Column<Guid>(type: "uuid", nullable: false),
                    Name = table.Column<string>(type: "text", nullable: false),
                    Role = table.Column<string>(type: "text", nullable: false),
                    Notes = table.Column<string>(type: "text", nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_NpcResourceModel", x => x.Id);
                    table.ForeignKey(
                        name: "FK_NpcResourceModel_CampaignModel_CampaignId",
                        column: x => x.CampaignId,
                        principalTable: "CampaignModel",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Cascade);
                });

            migrationBuilder.CreateTable(
                name: "QuestResourceModel",
                columns: table => new
                {
                    Id = table.Column<Guid>(type: "uuid", nullable: false),
                    CampaignId = table.Column<Guid>(type: "uuid", nullable: false),
                    Title = table.Column<string>(type: "text", nullable: false),
                    Status = table.Column<int>(type: "integer", nullable: false),
                    Description = table.Column<string>(type: "text", nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_QuestResourceModel", x => x.Id);
                    table.ForeignKey(
                        name: "FK_QuestResourceModel_CampaignModel_CampaignId",
                        column: x => x.CampaignId,
                        principalTable: "CampaignModel",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Cascade);
                });

            migrationBuilder.CreateTable(
                name: "SessionLogEntryModel",
                columns: table => new
                {
                    Id = table.Column<Guid>(type: "uuid", nullable: false),
                    CampaignId = table.Column<Guid>(type: "uuid", nullable: false),
                    Date = table.Column<DateTime>(type: "timestamp with time zone", nullable: false),
                    DurationMinutes = table.Column<int>(type: "integer", nullable: false),
                    Summary = table.Column<string>(type: "text", nullable: false),
                    DungeonMasterNotes = table.Column<string>(type: "text", nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_SessionLogEntryModel", x => x.Id);
                    table.ForeignKey(
                        name: "FK_SessionLogEntryModel_CampaignModel_CampaignId",
                        column: x => x.CampaignId,
                        principalTable: "CampaignModel",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Cascade);
                });

            migrationBuilder.CreateTable(
                name: "SpellbookEntryModel",
                columns: table => new
                {
                    Id = table.Column<Guid>(type: "uuid", nullable: false),
                    HeroId = table.Column<Guid>(type: "uuid", nullable: false),
                    SpellId = table.Column<int>(type: "integer", nullable: false),
                    SpellName = table.Column<string>(type: "text", nullable: false),
                    IsPrepared = table.Column<bool>(type: "boolean", nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_SpellbookEntryModel", x => x.Id);
                    table.ForeignKey(
                        name: "FK_SpellbookEntryModel_HeroModel_HeroId",
                        column: x => x.HeroId,
                        principalTable: "HeroModel",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Cascade);
                });

            migrationBuilder.CreateTable(
                name: "SpellSlotUsageModel",
                columns: table => new
                {
                    Id = table.Column<Guid>(type: "uuid", nullable: false),
                    HeroId = table.Column<Guid>(type: "uuid", nullable: false),
                    SlotLevel = table.Column<int>(type: "integer", nullable: false),
                    SlotsTotal = table.Column<int>(type: "integer", nullable: false),
                    SlotsSpent = table.Column<int>(type: "integer", nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_SpellSlotUsageModel", x => x.Id);
                    table.ForeignKey(
                        name: "FK_SpellSlotUsageModel_HeroModel_HeroId",
                        column: x => x.HeroId,
                        principalTable: "HeroModel",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Cascade);
                });

            migrationBuilder.CreateTable(
                name: "CombatantStateModel",
                columns: table => new
                {
                    Id = table.Column<Guid>(type: "uuid", nullable: false),
                    ActiveCombatCampaignId = table.Column<Guid>(type: "uuid", nullable: false),
                    Name = table.Column<string>(type: "text", nullable: false),
                    Type = table.Column<int>(type: "integer", nullable: false),
                    Initiative = table.Column<int>(type: "integer", nullable: false),
                    CurrentHitPoints = table.Column<int>(type: "integer", nullable: false),
                    MaxHitPoints = table.Column<int>(type: "integer", nullable: false),
                    TemporaryHitPoints = table.Column<int>(type: "integer", nullable: false),
                    HideHitPointsFromPlayers = table.Column<bool>(type: "boolean", nullable: false),
                    TurnOrder = table.Column<int>(type: "integer", nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_CombatantStateModel", x => x.Id);
                    table.ForeignKey(
                        name: "FK_CombatantStateModel_ActiveCombatModel_ActiveCombatCampaignId",
                        column: x => x.ActiveCombatCampaignId,
                        principalTable: "ActiveCombatModel",
                        principalColumn: "CampaignId",
                        onDelete: ReferentialAction.Cascade);
                });

            migrationBuilder.CreateTable(
                name: "CombatantConditionModel",
                columns: table => new
                {
                    Id = table.Column<Guid>(type: "uuid", nullable: false),
                    CombatantId = table.Column<Guid>(type: "uuid", nullable: false),
                    Name = table.Column<string>(type: "text", nullable: false),
                    RemainingRounds = table.Column<int>(type: "integer", nullable: true)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_CombatantConditionModel", x => x.Id);
                    table.ForeignKey(
                        name: "FK_CombatantConditionModel_CombatantStateModel_CombatantId",
                        column: x => x.CombatantId,
                        principalTable: "CombatantStateModel",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Cascade);
                });

            migrationBuilder.CreateIndex(
                name: "IX_CampaignMemberModel_CampaignId",
                table: "CampaignMemberModel",
                column: "CampaignId");

            migrationBuilder.CreateIndex(
                name: "IX_CampaignTimelineEntryModel_CampaignId",
                table: "CampaignTimelineEntryModel",
                column: "CampaignId");

            migrationBuilder.CreateIndex(
                name: "IX_CombatantConditionModel_CombatantId",
                table: "CombatantConditionModel",
                column: "CombatantId");

            migrationBuilder.CreateIndex(
                name: "IX_CombatantStateModel_ActiveCombatCampaignId",
                table: "CombatantStateModel",
                column: "ActiveCombatCampaignId");

            migrationBuilder.CreateIndex(
                name: "IX_EquipmentItemModel_HeroId",
                table: "EquipmentItemModel",
                column: "HeroId");

            migrationBuilder.CreateIndex(
                name: "IX_HeroConditionModel_HeroId",
                table: "HeroConditionModel",
                column: "HeroId");

            migrationBuilder.CreateIndex(
                name: "IX_InventoryItemModel_HeroId",
                table: "InventoryItemModel",
                column: "HeroId");

            migrationBuilder.CreateIndex(
                name: "IX_LocationResourceModel_CampaignId",
                table: "LocationResourceModel",
                column: "CampaignId");

            migrationBuilder.CreateIndex(
                name: "IX_LootResourceModel_CampaignId",
                table: "LootResourceModel",
                column: "CampaignId");

            migrationBuilder.CreateIndex(
                name: "IX_MonsterStatBlockModel_CampaignId",
                table: "MonsterStatBlockModel",
                column: "CampaignId");

            migrationBuilder.CreateIndex(
                name: "IX_NpcResourceModel_CampaignId",
                table: "NpcResourceModel",
                column: "CampaignId");

            migrationBuilder.CreateIndex(
                name: "IX_QuestResourceModel_CampaignId",
                table: "QuestResourceModel",
                column: "CampaignId");

            migrationBuilder.CreateIndex(
                name: "IX_SessionLogEntryModel_CampaignId",
                table: "SessionLogEntryModel",
                column: "CampaignId");

            migrationBuilder.CreateIndex(
                name: "IX_SpellbookEntryModel_HeroId",
                table: "SpellbookEntryModel",
                column: "HeroId");

            migrationBuilder.CreateIndex(
                name: "IX_SpellSlotUsageModel_HeroId",
                table: "SpellSlotUsageModel",
                column: "HeroId");
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropTable(
                name: "CampaignMemberModel");

            migrationBuilder.DropTable(
                name: "CampaignTimelineEntryModel");

            migrationBuilder.DropTable(
                name: "CombatantConditionModel");

            migrationBuilder.DropTable(
                name: "EquipmentItemModel");

            migrationBuilder.DropTable(
                name: "HeroConditionModel");

            migrationBuilder.DropTable(
                name: "InventoryItemModel");

            migrationBuilder.DropTable(
                name: "LocationResourceModel");

            migrationBuilder.DropTable(
                name: "LootResourceModel");

            migrationBuilder.DropTable(
                name: "MonsterStatBlockModel");

            migrationBuilder.DropTable(
                name: "NpcResourceModel");

            migrationBuilder.DropTable(
                name: "QuestResourceModel");

            migrationBuilder.DropTable(
                name: "SessionLogEntryModel");

            migrationBuilder.DropTable(
                name: "SpellbookEntryModel");

            migrationBuilder.DropTable(
                name: "SpellSlotUsageModel");

            migrationBuilder.DropTable(
                name: "CombatantStateModel");

            migrationBuilder.DropTable(
                name: "ActiveCombatModel");

            migrationBuilder.AddColumn<string>(
                name: "ConditionsJson",
                table: "HeroModel",
                type: "text",
                nullable: false,
                defaultValue: "[]");

            migrationBuilder.AddColumn<string>(
                name: "EquipmentJson",
                table: "HeroModel",
                type: "text",
                nullable: false,
                defaultValue: "[]");

            migrationBuilder.AddColumn<string>(
                name: "InventoryJson",
                table: "HeroModel",
                type: "text",
                nullable: false,
                defaultValue: "[]");

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
    }
}
