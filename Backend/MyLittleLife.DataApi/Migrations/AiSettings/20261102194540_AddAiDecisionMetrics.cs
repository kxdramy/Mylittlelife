using System;
using Microsoft.EntityFrameworkCore.Migrations;
using Npgsql.EntityFrameworkCore.PostgreSQL.Metadata;

#nullable disable

namespace MyLittleLife.DataApi.Migrations.AiSettings
{
    /// <inheritdoc />
    public partial class AddAiDecisionMetrics : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.CreateTable(
                name: "ai_decision_metrics",
                columns: table => new
                {
                    id = table.Column<long>(type: "bigint", nullable: false)
                        .Annotation("Npgsql:ValueGenerationStrategy", NpgsqlValueGenerationStrategy.IdentityByDefaultColumn),
                    character_id = table.Column<long>(type: "bigint", nullable: false),
                    action_name = table.Column<string>(type: "character varying(64)", maxLength: 64, nullable: false),
                    strategy_code = table.Column<string>(type: "character varying(32)", maxLength: 32, nullable: false),
                    utility_score = table.Column<double>(type: "double precision", nullable: false),
                    decision_time_ms = table.Column<double>(type: "double precision", nullable: false),
                    created_at = table.Column<DateTime>(type: "timestamp with time zone", nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_ai_decision_metrics", x => x.id);
                });

            migrationBuilder.CreateIndex(
                name: "IX_ai_decision_metrics_character_id_created_at",
                table: "ai_decision_metrics",
                columns: new[] { "character_id", "created_at" });
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropTable(
                name: "ai_decision_metrics");
        }
    }
}
