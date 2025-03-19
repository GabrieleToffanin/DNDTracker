using System;
using Microsoft.EntityFrameworkCore.Migrations;
using Npgsql.EntityFrameworkCore.PostgreSQL.Metadata;

#nullable disable

namespace DNDTracker.Outbounx.PostgresDb.Migrations
{
    /// <inheritdoc />
    public partial class UpdateEntities : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropTable(
                name: "DomainEvent");

            migrationBuilder.DropTable(
                name: "Spell");

            migrationBuilder.DropTable(
                name: "Hero");

            migrationBuilder.DropTable(
                name: "Campaign");

            migrationBuilder.CreateTable(
                name: "CampaignModel",
                columns: table => new
                {
                    Id = table.Column<Guid>(type: "uuid", nullable: false),
                    CampaignName = table.Column<string>(type: "text", nullable: false),
                    CampaignDescription = table.Column<string>(type: "text", nullable: false),
                    CampaignImage = table.Column<string>(type: "text", nullable: false),
                    IsActive = table.Column<bool>(type: "boolean", nullable: false),
                    CreatedDate = table.Column<DateTime>(type: "timestamp with time zone", nullable: false),
                    UpdatedDate = table.Column<DateTime>(type: "timestamp with time zone", nullable: false),
                    DeletedDate = table.Column<DateTime>(type: "timestamp with time zone", nullable: true)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_CampaignModel", x => x.Id);
                });

            migrationBuilder.CreateTable(
                name: "SpellModel",
                columns: table => new
                {
                    Id = table.Column<int>(type: "integer", nullable: false)
                        .Annotation("Npgsql:ValueGenerationStrategy", NpgsqlValueGenerationStrategy.IdentityByDefaultColumn),
                    Name = table.Column<string>(type: "text", nullable: true),
                    Description = table.Column<string>(type: "text", nullable: true),
                    Source = table.Column<string>(type: "text", nullable: true),
                    Level = table.Column<int>(type: "integer", nullable: false),
                    School = table.Column<string>(type: "text", nullable: true),
                    Time = table.Column<string>(type: "text", nullable: true),
                    Range = table.Column<string>(type: "text", nullable: true),
                    Components = table.Column<string>(type: "text", nullable: true),
                    Material = table.Column<string>(type: "text", nullable: true),
                    IsRitual = table.Column<bool>(type: "boolean", nullable: false),
                    Duration = table.Column<string>(type: "text", nullable: true),
                    Concentration = table.Column<string>(type: "text", nullable: true),
                    CastingTime = table.Column<string>(type: "text", nullable: true),
                    Damage = table.Column<string>(type: "text", nullable: true),
                    Save = table.Column<string>(type: "text", nullable: true)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_SpellModel", x => x.Id);
                });

            migrationBuilder.CreateTable(
                name: "HeroModel",
                columns: table => new
                {
                    Id = table.Column<Guid>(type: "uuid", nullable: false),
                    Name = table.Column<string>(type: "text", nullable: false),
                    Class = table.Column<int>(type: "integer", nullable: false),
                    Race = table.Column<int>(type: "integer", nullable: false),
                    Alignment = table.Column<int>(type: "integer", nullable: false),
                    Level = table.Column<int>(type: "integer", nullable: false),
                    Experience = table.Column<int>(type: "integer", nullable: false),
                    HitPoints = table.Column<int>(type: "integer", nullable: false),
                    HitDice = table.Column<int>(type: "integer", nullable: false),
                    CampaignId = table.Column<Guid>(type: "uuid", nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_HeroModel", x => x.Id);
                    table.ForeignKey(
                        name: "FK_HeroModel_CampaignModel_CampaignId",
                        column: x => x.CampaignId,
                        principalTable: "CampaignModel",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Cascade);
                });

            migrationBuilder.CreateTable(
                name: "HeroModelSpellModel",
                columns: table => new
                {
                    HeroesId = table.Column<Guid>(type: "uuid", nullable: false),
                    SpellsId = table.Column<int>(type: "integer", nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_HeroModelSpellModel", x => new { x.HeroesId, x.SpellsId });
                    table.ForeignKey(
                        name: "FK_HeroModelSpellModel_HeroModel_HeroesId",
                        column: x => x.HeroesId,
                        principalTable: "HeroModel",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Cascade);
                    table.ForeignKey(
                        name: "FK_HeroModelSpellModel_SpellModel_SpellsId",
                        column: x => x.SpellsId,
                        principalTable: "SpellModel",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Cascade);
                });

            migrationBuilder.CreateIndex(
                name: "IX_HeroModel_CampaignId",
                table: "HeroModel",
                column: "CampaignId");

            migrationBuilder.CreateIndex(
                name: "IX_HeroModelSpellModel_SpellsId",
                table: "HeroModelSpellModel",
                column: "SpellsId");
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropTable(
                name: "HeroModelSpellModel");

            migrationBuilder.DropTable(
                name: "HeroModel");

            migrationBuilder.DropTable(
                name: "SpellModel");

            migrationBuilder.DropTable(
                name: "CampaignModel");

            migrationBuilder.CreateTable(
                name: "Campaign",
                columns: table => new
                {
                    Id = table.Column<string>(type: "text", nullable: false),
                    CampaignDescription = table.Column<string>(type: "text", nullable: false),
                    CampaignImage = table.Column<string>(type: "text", nullable: false),
                    CampaignName = table.Column<string>(type: "text", nullable: false),
                    CreatedDate = table.Column<DateTime>(type: "timestamp with time zone", nullable: false),
                    DeletedDate = table.Column<DateTime>(type: "timestamp with time zone", nullable: true),
                    IsActive = table.Column<bool>(type: "boolean", nullable: false),
                    UpdatedDate = table.Column<DateTime>(type: "timestamp with time zone", nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_Campaign", x => x.Id);
                });

            migrationBuilder.CreateTable(
                name: "Hero",
                columns: table => new
                {
                    Id = table.Column<string>(type: "text", nullable: false),
                    CampaignId = table.Column<string>(type: "text", nullable: false),
                    Alignment = table.Column<int>(type: "integer", nullable: false),
                    Class = table.Column<int>(type: "integer", nullable: false),
                    Experience = table.Column<int>(type: "integer", nullable: false),
                    HitDice = table.Column<int>(type: "integer", nullable: false),
                    HitPoints = table.Column<int>(type: "integer", nullable: false),
                    Level = table.Column<int>(type: "integer", nullable: false),
                    Name = table.Column<string>(type: "text", nullable: false),
                    Race = table.Column<int>(type: "integer", nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_Hero", x => x.Id);
                    table.ForeignKey(
                        name: "FK_Hero_Campaign_CampaignId",
                        column: x => x.CampaignId,
                        principalTable: "Campaign",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Cascade);
                });

            migrationBuilder.CreateTable(
                name: "DomainEvent",
                columns: table => new
                {
                    Id = table.Column<Guid>(type: "uuid", nullable: false),
                    CampaignId = table.Column<string>(type: "text", nullable: true),
                    HeroId = table.Column<string>(type: "text", nullable: true),
                    OccuredOn = table.Column<DateTime>(type: "timestamp with time zone", nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_DomainEvent", x => x.Id);
                    table.ForeignKey(
                        name: "FK_DomainEvent_Campaign_CampaignId",
                        column: x => x.CampaignId,
                        principalTable: "Campaign",
                        principalColumn: "Id");
                    table.ForeignKey(
                        name: "FK_DomainEvent_Hero_HeroId",
                        column: x => x.HeroId,
                        principalTable: "Hero",
                        principalColumn: "Id");
                });

            migrationBuilder.CreateTable(
                name: "Spell",
                columns: table => new
                {
                    Id = table.Column<int>(type: "integer", nullable: false)
                        .Annotation("Npgsql:ValueGenerationStrategy", NpgsqlValueGenerationStrategy.IdentityByDefaultColumn),
                    CastingTime = table.Column<string>(type: "text", nullable: true),
                    Components = table.Column<string>(type: "text", nullable: true),
                    Concentration = table.Column<string>(type: "text", nullable: true),
                    Damage = table.Column<string>(type: "text", nullable: true),
                    Description = table.Column<string>(type: "text", nullable: true),
                    Duration = table.Column<string>(type: "text", nullable: true),
                    HeroId = table.Column<string>(type: "text", nullable: true),
                    IsRitual = table.Column<bool>(type: "boolean", nullable: false),
                    Level = table.Column<int>(type: "integer", nullable: false),
                    Material = table.Column<string>(type: "text", nullable: true),
                    Name = table.Column<string>(type: "text", nullable: true),
                    Range = table.Column<string>(type: "text", nullable: true),
                    Save = table.Column<string>(type: "text", nullable: true),
                    School = table.Column<string>(type: "text", nullable: true),
                    Source = table.Column<string>(type: "text", nullable: true),
                    Time = table.Column<string>(type: "text", nullable: true)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_Spell", x => x.Id);
                    table.ForeignKey(
                        name: "FK_Spell_Hero_HeroId",
                        column: x => x.HeroId,
                        principalTable: "Hero",
                        principalColumn: "Id");
                });

            migrationBuilder.CreateIndex(
                name: "IX_DomainEvent_CampaignId",
                table: "DomainEvent",
                column: "CampaignId");

            migrationBuilder.CreateIndex(
                name: "IX_DomainEvent_HeroId",
                table: "DomainEvent",
                column: "HeroId");

            migrationBuilder.CreateIndex(
                name: "IX_Hero_CampaignId",
                table: "Hero",
                column: "CampaignId");

            migrationBuilder.CreateIndex(
                name: "IX_Spell_HeroId",
                table: "Spell",
                column: "HeroId");
        }
    }
}
