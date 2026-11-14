using System;
using System.Collections.Generic;
using Microsoft.EntityFrameworkCore;
using MyLittleLife.DataApi.Models;

namespace MyLittleLife.DataApi.Data;

public partial class GameDataContext : DbContext
{
    public GameDataContext(DbContextOptions<GameDataContext> options)
        : base(options)
    {
    }

    public virtual DbSet<ActionHistory> ActionHistories { get; set; }

    public virtual DbSet<ActionType> ActionTypes { get; set; }

    public virtual DbSet<Character> Characters { get; set; }

    public virtual DbSet<CharacterAppearance> CharacterAppearances { get; set; }

    public virtual DbSet<CharacterNeed> CharacterNeeds { get; set; }

    public virtual DbSet<Furniture> Furnitures { get; set; }

    public virtual DbSet<FurnitureCustomization> FurnitureCustomizations { get; set; }

    public virtual DbSet<FurnitureType> FurnitureTypes { get; set; }

    public virtual DbSet<GameSession> GameSessions { get; set; }

    public virtual DbSet<HairStyle> HairStyles { get; set; }

    public virtual DbSet<NeedHistory> NeedHistories { get; set; }

    public virtual DbSet<NeedType> NeedTypes { get; set; }

    public virtual DbSet<Outfit> Outfits { get; set; }

    public virtual DbSet<Room> Rooms { get; set; }

    public virtual DbSet<RoomSurface> RoomSurfaces { get; set; }

    public virtual DbSet<SaveSlot> SaveSlots { get; set; }

    public virtual DbSet<SurfaceMaterial> SurfaceMaterials { get; set; }

    protected override void OnModelCreating(ModelBuilder modelBuilder)
    {
        modelBuilder.Entity<ActionHistory>(entity =>
        {
            entity.HasKey(e => e.Id).HasName("action_history_pkey");

            entity.ToTable("action_history");

            entity.Property(e => e.Id).HasColumnName("id");
            entity.Property(e => e.ActionTypeId).HasColumnName("action_type_id");
            entity.Property(e => e.ControlMode)
                .HasMaxLength(6)
                .HasColumnName("control_mode");
            entity.Property(e => e.DurationSeconds)
                .HasPrecision(7, 2)
                .HasColumnName("duration_seconds");
            entity.Property(e => e.ExecutedAt).HasColumnName("executed_at");
            entity.Property(e => e.FurnitureId).HasColumnName("furniture_id");
            entity.Property(e => e.GameSessionId).HasColumnName("game_session_id");

            entity.HasOne(d => d.ActionType).WithMany(p => p.ActionHistories)
                .HasForeignKey(d => d.ActionTypeId)
                .OnDelete(DeleteBehavior.ClientSetNull)
                .HasConstraintName("fk_action_history_action_type");

            entity.HasOne(d => d.Furniture).WithMany(p => p.ActionHistories)
                .HasForeignKey(d => d.FurnitureId)
                .OnDelete(DeleteBehavior.ClientSetNull)
                .HasConstraintName("fk_action_history_furniture");

            entity.HasOne(d => d.GameSession).WithMany(p => p.ActionHistories)
                .HasForeignKey(d => d.GameSessionId)
                .OnDelete(DeleteBehavior.ClientSetNull)
                .HasConstraintName("fk_action_history_game_session");
        });

        modelBuilder.Entity<ActionType>(entity =>
        {
            entity.HasKey(e => e.Id).HasName("action_types_pkey");

            entity.ToTable("action_types");

            entity.HasIndex(e => e.Code, "action_types_code_key").IsUnique();

            entity.Property(e => e.Id).HasColumnName("id");
            entity.Property(e => e.Code)
                .HasMaxLength(40)
                .HasColumnName("code");
            entity.Property(e => e.DurationSeconds)
                .HasPrecision(7, 2)
                .HasColumnName("duration_seconds");
            entity.Property(e => e.Name)
                .HasMaxLength(80)
                .HasColumnName("name");
            entity.Property(e => e.NeedTypeId).HasColumnName("need_type_id");
            entity.Property(e => e.RestoreAmount)
                .HasPrecision(5, 2)
                .HasColumnName("restore_amount");

            entity.HasOne(d => d.NeedType).WithMany(p => p.ActionTypes)
                .HasForeignKey(d => d.NeedTypeId)
                .OnDelete(DeleteBehavior.ClientSetNull)
                .HasConstraintName("fk_action_types_need_type");
        });

        modelBuilder.Entity<Character>(entity =>
        {
            entity.HasKey(e => e.Id).HasName("characters_pkey");

            entity.ToTable("characters");

            entity.HasIndex(e => e.SaveSlotId, "characters_save_slot_id_key").IsUnique();

            entity.Property(e => e.Id).HasColumnName("id");
            entity.Property(e => e.CreatedAt)
                .HasDefaultValueSql("CURRENT_TIMESTAMP")
                .HasColumnName("created_at");
            entity.Property(e => e.Name)
                .HasMaxLength(80)
                .HasColumnName("name");
            entity.Property(e => e.PositionX).HasColumnName("position_x");
            entity.Property(e => e.PositionY).HasColumnName("position_y");
            entity.Property(e => e.PositionZ).HasColumnName("position_z");
            entity.Property(e => e.SaveSlotId).HasColumnName("save_slot_id");

            entity.HasOne(d => d.SaveSlot).WithOne(p => p.Character)
                .HasForeignKey<Character>(d => d.SaveSlotId)
                .OnDelete(DeleteBehavior.ClientSetNull)
                .HasConstraintName("fk_characters_save_slot");
        });

        modelBuilder.Entity<CharacterAppearance>(entity =>
        {
            entity.HasKey(e => e.CharacterId).HasName("character_appearances_pkey");

            entity.ToTable("character_appearances");

            entity.Property(e => e.CharacterId)
                .ValueGeneratedNever()
                .HasColumnName("character_id");
            entity.Property(e => e.HairColor)
                .HasMaxLength(7)
                .HasColumnName("hair_color");
            entity.Property(e => e.HairStyleId).HasColumnName("hair_style_id");
            entity.Property(e => e.OutfitColor)
                .HasMaxLength(7)
                .HasColumnName("outfit_color");
            entity.Property(e => e.OutfitId).HasColumnName("outfit_id");
            entity.Property(e => e.SkinColor)
                .HasMaxLength(7)
                .HasColumnName("skin_color");

            entity.HasOne(d => d.Character).WithOne(p => p.CharacterAppearance)
                .HasForeignKey<CharacterAppearance>(d => d.CharacterId)
                .OnDelete(DeleteBehavior.ClientSetNull)
                .HasConstraintName("fk_appearances_character");

            entity.HasOne(d => d.HairStyle).WithMany(p => p.CharacterAppearances)
                .HasForeignKey(d => d.HairStyleId)
                .OnDelete(DeleteBehavior.ClientSetNull)
                .HasConstraintName("fk_appearances_hair_style");

            entity.HasOne(d => d.Outfit).WithMany(p => p.CharacterAppearances)
                .HasForeignKey(d => d.OutfitId)
                .OnDelete(DeleteBehavior.ClientSetNull)
                .HasConstraintName("fk_appearances_outfit");
        });

        modelBuilder.Entity<CharacterNeed>(entity =>
        {
            entity.HasKey(e => new { e.CharacterId, e.NeedTypeId }).HasName("pk_character_needs");

            entity.ToTable("character_needs");

            entity.Property(e => e.CharacterId).HasColumnName("character_id");
            entity.Property(e => e.NeedTypeId).HasColumnName("need_type_id");
            entity.Property(e => e.Value)
                .HasPrecision(5, 2)
                .HasColumnName("value");

            entity.HasOne(d => d.Character).WithMany(p => p.CharacterNeeds)
                .HasForeignKey(d => d.CharacterId)
                .OnDelete(DeleteBehavior.ClientSetNull)
                .HasConstraintName("fk_character_needs_character");

            entity.HasOne(d => d.NeedType).WithMany(p => p.CharacterNeeds)
                .HasForeignKey(d => d.NeedTypeId)
                .OnDelete(DeleteBehavior.ClientSetNull)
                .HasConstraintName("fk_character_needs_need_type");
        });

        modelBuilder.Entity<Furniture>(entity =>
        {
            entity.HasKey(e => e.Id).HasName("furniture_pkey");

            entity.ToTable("furniture");

            entity.Property(e => e.Id).HasColumnName("id");
            entity.Property(e => e.FurnitureTypeId).HasColumnName("furniture_type_id");
            entity.Property(e => e.IsActive)
                .HasDefaultValue(true)
                .HasColumnName("is_active");
            entity.Property(e => e.PositionX).HasColumnName("position_x");
            entity.Property(e => e.PositionY).HasColumnName("position_y");
            entity.Property(e => e.PositionZ).HasColumnName("position_z");
            entity.Property(e => e.RoomId).HasColumnName("room_id");
            entity.Property(e => e.RotationY).HasColumnName("rotation_y");

            entity.HasOne(d => d.FurnitureType).WithMany(p => p.Furnitures)
                .HasForeignKey(d => d.FurnitureTypeId)
                .OnDelete(DeleteBehavior.ClientSetNull)
                .HasConstraintName("fk_furniture_type");

            entity.HasOne(d => d.Room).WithMany(p => p.Furnitures)
                .HasForeignKey(d => d.RoomId)
                .OnDelete(DeleteBehavior.ClientSetNull)
                .HasConstraintName("fk_furniture_room");
        });

        modelBuilder.Entity<FurnitureCustomization>(entity =>
        {
            entity.HasKey(e => e.FurnitureId).HasName("furniture_customizations_pkey");

            entity.ToTable("furniture_customizations");

            entity.Property(e => e.FurnitureId)
                .ValueGeneratedNever()
                .HasColumnName("furniture_id");
            entity.Property(e => e.ColorHex)
                .HasMaxLength(7)
                .HasColumnName("color_hex");
            entity.Property(e => e.MaterialVariant)
                .HasMaxLength(80)
                .HasColumnName("material_variant");

            entity.HasOne(d => d.Furniture).WithOne(p => p.FurnitureCustomization)
                .HasForeignKey<FurnitureCustomization>(d => d.FurnitureId)
                .OnDelete(DeleteBehavior.ClientSetNull)
                .HasConstraintName("fk_furniture_customizations_furniture");
        });

        modelBuilder.Entity<FurnitureType>(entity =>
        {
            entity.HasKey(e => e.Id).HasName("furniture_types_pkey");

            entity.ToTable("furniture_types");

            entity.HasIndex(e => e.Code, "furniture_types_code_key").IsUnique();

            entity.Property(e => e.Id).HasColumnName("id");
            entity.Property(e => e.Code)
                .HasMaxLength(40)
                .HasColumnName("code");
            entity.Property(e => e.Name)
                .HasMaxLength(80)
                .HasColumnName("name");
            entity.Property(e => e.PrefabKey)
                .HasMaxLength(150)
                .HasColumnName("prefab_key");

            entity.HasMany(d => d.ActionTypes).WithMany(p => p.FurnitureTypes)
                .UsingEntity<Dictionary<string, object>>(
                    "FurnitureAction",
                    r => r.HasOne<ActionType>().WithMany()
                        .HasForeignKey("ActionTypeId")
                        .OnDelete(DeleteBehavior.ClientSetNull)
                        .HasConstraintName("fk_furniture_actions_action_type"),
                    l => l.HasOne<FurnitureType>().WithMany()
                        .HasForeignKey("FurnitureTypeId")
                        .OnDelete(DeleteBehavior.ClientSetNull)
                        .HasConstraintName("fk_furniture_actions_furniture_type"),
                    j =>
                    {
                        j.HasKey("FurnitureTypeId", "ActionTypeId").HasName("pk_furniture_actions");
                        j.ToTable("furniture_actions");
                        j.IndexerProperty<long>("FurnitureTypeId").HasColumnName("furniture_type_id");
                        j.IndexerProperty<long>("ActionTypeId").HasColumnName("action_type_id");
                    });
        });

        modelBuilder.Entity<GameSession>(entity =>
        {
            entity.HasKey(e => e.Id).HasName("game_sessions_pkey");

            entity.ToTable("game_sessions");

            entity.Property(e => e.Id).HasColumnName("id");
            entity.Property(e => e.EndedAt).HasColumnName("ended_at");
            entity.Property(e => e.SaveSlotId).HasColumnName("save_slot_id");
            entity.Property(e => e.StartedAt).HasColumnName("started_at");

            entity.HasOne(d => d.SaveSlot).WithMany(p => p.GameSessions)
                .HasForeignKey(d => d.SaveSlotId)
                .OnDelete(DeleteBehavior.ClientSetNull)
                .HasConstraintName("fk_game_sessions_save_slot");
        });

        modelBuilder.Entity<HairStyle>(entity =>
        {
            entity.HasKey(e => e.Id).HasName("hair_styles_pkey");

            entity.ToTable("hair_styles");

            entity.HasIndex(e => e.Code, "hair_styles_code_key").IsUnique();

            entity.Property(e => e.Id).HasColumnName("id");
            entity.Property(e => e.AssetKey)
                .HasMaxLength(150)
                .HasColumnName("asset_key");
            entity.Property(e => e.Code)
                .HasMaxLength(40)
                .HasColumnName("code");
            entity.Property(e => e.Name)
                .HasMaxLength(80)
                .HasColumnName("name");
        });

        modelBuilder.Entity<NeedHistory>(entity =>
        {
            entity.HasKey(e => e.Id).HasName("need_history_pkey");

            entity.ToTable("need_history");

            entity.Property(e => e.Id).HasColumnName("id");
            entity.Property(e => e.GameSessionId).HasColumnName("game_session_id");
            entity.Property(e => e.NeedTypeId).HasColumnName("need_type_id");
            entity.Property(e => e.RecordedAt).HasColumnName("recorded_at");
            entity.Property(e => e.Value)
                .HasPrecision(5, 2)
                .HasColumnName("value");

            entity.HasOne(d => d.GameSession).WithMany(p => p.NeedHistories)
                .HasForeignKey(d => d.GameSessionId)
                .OnDelete(DeleteBehavior.ClientSetNull)
                .HasConstraintName("fk_need_history_game_session");

            entity.HasOne(d => d.NeedType).WithMany(p => p.NeedHistories)
                .HasForeignKey(d => d.NeedTypeId)
                .OnDelete(DeleteBehavior.ClientSetNull)
                .HasConstraintName("fk_need_history_need_type");
        });

        modelBuilder.Entity<NeedType>(entity =>
        {
            entity.HasKey(e => e.Id).HasName("need_types_pkey");

            entity.ToTable("need_types");

            entity.HasIndex(e => e.Code, "need_types_code_key").IsUnique();

            entity.Property(e => e.Id).HasColumnName("id");
            entity.Property(e => e.Code)
                .HasMaxLength(40)
                .HasColumnName("code");
            entity.Property(e => e.DecayPerSecond)
                .HasPrecision(8, 4)
                .HasColumnName("decay_per_second");
            entity.Property(e => e.Name)
                .HasMaxLength(80)
                .HasColumnName("name");
        });

        modelBuilder.Entity<Outfit>(entity =>
        {
            entity.HasKey(e => e.Id).HasName("outfits_pkey");

            entity.ToTable("outfits");

            entity.HasIndex(e => e.Code, "outfits_code_key").IsUnique();

            entity.Property(e => e.Id).HasColumnName("id");
            entity.Property(e => e.AssetKey)
                .HasMaxLength(150)
                .HasColumnName("asset_key");
            entity.Property(e => e.Code)
                .HasMaxLength(40)
                .HasColumnName("code");
            entity.Property(e => e.Name)
                .HasMaxLength(80)
                .HasColumnName("name");
        });

        modelBuilder.Entity<Room>(entity =>
        {
            entity.HasKey(e => e.Id).HasName("rooms_pkey");

            entity.ToTable("rooms");

            entity.Property(e => e.Id).HasColumnName("id");
            entity.Property(e => e.Name)
                .HasMaxLength(80)
                .HasColumnName("name");
            entity.Property(e => e.RoomType)
                .HasMaxLength(40)
                .HasColumnName("room_type");
            entity.Property(e => e.SaveSlotId).HasColumnName("save_slot_id");

            entity.HasOne(d => d.SaveSlot).WithMany(p => p.Rooms)
                .HasForeignKey(d => d.SaveSlotId)
                .OnDelete(DeleteBehavior.ClientSetNull)
                .HasConstraintName("fk_rooms_save_slot");
        });

        modelBuilder.Entity<RoomSurface>(entity =>
        {
            entity.HasKey(e => new { e.RoomId, e.SurfaceKind }).HasName("pk_room_surfaces");

            entity.ToTable("room_surfaces");

            entity.Property(e => e.RoomId).HasColumnName("room_id");
            entity.Property(e => e.SurfaceKind)
                .HasMaxLength(10)
                .HasColumnName("surface_kind");
            entity.Property(e => e.SurfaceMaterialId).HasColumnName("surface_material_id");

            entity.HasOne(d => d.Room).WithMany(p => p.RoomSurfaces)
                .HasForeignKey(d => d.RoomId)
                .OnDelete(DeleteBehavior.ClientSetNull)
                .HasConstraintName("fk_room_surfaces_room");

            entity.HasOne(d => d.SurfaceMaterial).WithMany(p => p.RoomSurfaces)
                .HasPrincipalKey(p => new { p.Id, p.SurfaceKind })
                .HasForeignKey(d => new { d.SurfaceMaterialId, d.SurfaceKind })
                .OnDelete(DeleteBehavior.ClientSetNull)
                .HasConstraintName("fk_room_surfaces_material_kind");
        });

        modelBuilder.Entity<SaveSlot>(entity =>
        {
            entity.HasKey(e => e.Id).HasName("save_slots_pkey");

            entity.ToTable("save_slots");

            entity.Property(e => e.Id).HasColumnName("id");
            entity.Property(e => e.CreatedAt)
                .HasDefaultValueSql("CURRENT_TIMESTAMP")
                .HasColumnName("created_at");
            entity.Property(e => e.LastSavedAt)
                .HasDefaultValueSql("CURRENT_TIMESTAMP")
                .HasColumnName("last_saved_at");
            entity.Property(e => e.Name)
                .HasMaxLength(80)
                .HasColumnName("name");
        });

        modelBuilder.Entity<SurfaceMaterial>(entity =>
        {
            entity.HasKey(e => e.Id).HasName("surface_materials_pkey");

            entity.ToTable("surface_materials");

            entity.HasIndex(e => e.Code, "surface_materials_code_key").IsUnique();

            entity.HasIndex(e => new { e.Id, e.SurfaceKind }, "uq_surface_materials_id_kind").IsUnique();

            entity.Property(e => e.Id).HasColumnName("id");
            entity.Property(e => e.AssetKey)
                .HasMaxLength(150)
                .HasColumnName("asset_key");
            entity.Property(e => e.Code)
                .HasMaxLength(40)
                .HasColumnName("code");
            entity.Property(e => e.Name)
                .HasMaxLength(80)
                .HasColumnName("name");
            entity.Property(e => e.SurfaceKind)
                .HasMaxLength(10)
                .HasColumnName("surface_kind");
        });

        OnModelCreatingPartial(modelBuilder);
    }

    partial void OnModelCreatingPartial(ModelBuilder modelBuilder);
}
