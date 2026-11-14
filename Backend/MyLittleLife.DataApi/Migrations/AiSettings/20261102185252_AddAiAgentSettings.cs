using System;
using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace MyLittleLife.DataApi.Migrations.AiSettings
{
    /// <inheritdoc />
    public partial class AddAiAgentSettings : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.CreateTable(
                name: "ai_agent_settings",
                columns: table => new
                {
                    character_id = table.Column<long>(type: "bigint", nullable: false),
                    strategy_code = table.Column<string>(type: "character varying(32)", maxLength: 32, nullable: false),
                    updated_at = table.Column<DateTime>(type: "timestamp with time zone", nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_ai_agent_settings", x => x.character_id);
                });
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropTable(
                name: "ai_agent_settings");
        }
    }
}
