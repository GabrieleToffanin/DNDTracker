using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace DNDTracker.Outbound.PostgresDb.Migrations
{
    /// <inheritdoc />
    public partial class AddAlignmentToMonsterStatBlock : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.AddColumn<string>(
                name: "Alignment",
                table: "MonsterStatBlockModel",
                type: "text",
                nullable: false,
                defaultValue: "");
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropColumn(
                name: "Alignment",
                table: "MonsterStatBlockModel");
        }
    }
}
