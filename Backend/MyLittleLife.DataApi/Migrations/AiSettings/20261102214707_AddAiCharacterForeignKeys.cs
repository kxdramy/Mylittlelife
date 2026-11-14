using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace MyLittleLife.DataApi.Migrations.AiSettings
{
    /// <inheritdoc />
    public partial class AddAiCharacterForeignKeys : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.AddForeignKey(
                name: "FK_ai_agent_settings_characters_character_id",
                table: "ai_agent_settings",
                column: "character_id",
                principalTable: "characters",
                principalColumn: "id",
                onDelete: ReferentialAction.Restrict);

            migrationBuilder.AddForeignKey(
                name: "FK_ai_decision_metrics_characters_character_id",
                table: "ai_decision_metrics",
                column: "character_id",
                principalTable: "characters",
                principalColumn: "id",
                onDelete: ReferentialAction.Restrict);
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropForeignKey(
                name: "FK_ai_agent_settings_characters_character_id",
                table: "ai_agent_settings");

            migrationBuilder.DropForeignKey(
                name: "FK_ai_decision_metrics_characters_character_id",
                table: "ai_decision_metrics");
        }
    }
}
