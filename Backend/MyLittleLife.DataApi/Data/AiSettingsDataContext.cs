
using Microsoft.EntityFrameworkCore;
using MyLittleLife.DataApi.Models;

namespace MyLittleLife.DataApi.Data;

public class AiSettingsDataContext : DbContext
{
    public AiSettingsDataContext(
        DbContextOptions<AiSettingsDataContext> options)
        : base(options)
    {
    }

    public DbSet<AiAgentSetting> AiAgentSettings =>
        Set<AiAgentSetting>();

    public DbSet<AiDecisionMetric> AiDecisionMetrics =>
        Set<AiDecisionMetric>();

    protected override void OnModelCreating(
        ModelBuilder modelBuilder)
    {
        // =====================================
        // СУЩЕСТВУЮЩАЯ ТАБЛИЦА CHARACTERS
        // =====================================

        modelBuilder.Entity<AiCharacterReference>(entity =>
        {
            // Таблица уже создана ранее.
            // EF Core не должен создавать её повторно.
            entity.ToTable(
                "characters",
                table => table.ExcludeFromMigrations()
            );

            entity.HasKey(e => e.Id);

            entity.Property(e => e.Id)
                .HasColumnName("id");
        });

        // =====================================
        // НАСТРОЙКИ UTILITY AI
        // =====================================

        modelBuilder.Entity<AiAgentSetting>(entity =>
        {
            entity.ToTable("ai_agent_settings");

            entity.HasKey(e => e.CharacterId);

            entity.Property(e => e.CharacterId)
                .HasColumnName("character_id")
                .ValueGeneratedNever();

            entity.Property(e => e.StrategyCode)
                .HasColumnName("strategy_code")
                .HasMaxLength(32)
                .IsRequired();

            entity.Property(e => e.UpdatedAt)
                .HasColumnName("updated_at")
                .HasColumnType("timestamp with time zone")
                .IsRequired();

            // Внешний ключ:
            // ai_agent_settings.character_id
            // -> characters.id
            entity.HasOne<AiCharacterReference>()
                .WithMany()
                .HasForeignKey(e => e.CharacterId)
                .OnDelete(DeleteBehavior.Restrict)
                .HasConstraintName(
                    "FK_ai_agent_settings_characters_character_id"
                );
        });

        // =====================================
        // ИСТОРИЯ РЕШЕНИЙ UTILITY AI
        // =====================================

        modelBuilder.Entity<AiDecisionMetric>(entity =>
        {
            entity.ToTable("ai_decision_metrics");

            entity.HasKey(e => e.Id);

            entity.Property(e => e.Id)
                .HasColumnName("id")
                .ValueGeneratedOnAdd();

            entity.Property(e => e.CharacterId)
                .HasColumnName("character_id")
                .IsRequired();

            entity.Property(e => e.ActionName)
                .HasColumnName("action_name")
                .HasMaxLength(64)
                .IsRequired();

            entity.Property(e => e.StrategyCode)
                .HasColumnName("strategy_code")
                .HasMaxLength(32)
                .IsRequired();

            entity.Property(e => e.UtilityScore)
                .HasColumnName("utility_score")
                .HasColumnType("double precision")
                .IsRequired();

            entity.Property(e => e.DecisionTimeMs)
                .HasColumnName("decision_time_ms")
                .HasColumnType("double precision")
                .IsRequired();

            entity.Property(e => e.CreatedAt)
                .HasColumnName("created_at")
                .HasColumnType("timestamp with time zone")
                .IsRequired();

            entity.HasIndex(e => new
            {
                e.CharacterId,
                e.CreatedAt
            });

            // Внешний ключ:
            // ai_decision_metrics.character_id
            // -> characters.id
            entity.HasOne<AiCharacterReference>()
                .WithMany()
                .HasForeignKey(e => e.CharacterId)
                .OnDelete(DeleteBehavior.Restrict)
                .HasConstraintName(
                    "FK_ai_decision_metrics_characters_character_id"
                );
        });
    }
}
