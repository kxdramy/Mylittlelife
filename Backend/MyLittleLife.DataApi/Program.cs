
using MyLittleLife.DataApi.Repositories;
using Microsoft.EntityFrameworkCore;
using MyLittleLife.DataApi.Data;

var builder = WebApplication.CreateBuilder(args);

// ========================================
// ПОДКЛЮЧЕНИЕ К POSTGRESQL
// ========================================

var connectionString =
    builder.Configuration.GetConnectionString("GameDatabase")
    ?? throw new InvalidOperationException(
        "Не настроено подключение GameDatabase"
    );

// Основной контекст существующих таблиц игры.
builder.Services.AddDbContext<GameDataContext>(options =>
    options.UseNpgsql(connectionString));

// Отдельный контекст для настроек и истории Utility AI.
// Использует отдельную таблицу истории миграций.
builder.Services.AddDbContext<AiSettingsDataContext>(options =>
    options.UseNpgsql(
        connectionString,
        npgsql => npgsql.MigrationsHistoryTable(
            "__EFMigrationsHistory_AiSettings"
        )
    ));

// ========================================
// РЕГИСТРАЦИЯ REPOSITORIES
// ========================================

// Repository для потребностей персонажа.
builder.Services.AddScoped<
    ICharacterNeedsRepository,
    CharacterNeedsRepository>();

// Repository для стратегий Utility AI.
builder.Services.AddScoped<
    IAiSettingsRepository,
    AiSettingsRepository>();

// НОВОЕ: Repository для истории решений Utility AI.
builder.Services.AddScoped<
    IAiDecisionMetricsRepository,
    AiDecisionMetricsRepository>();

builder.Services.AddOpenApi();

var app = builder.Build();

// ========================================
// НАСТРОЙКА HTTP API
// ========================================

if (app.Environment.IsDevelopment())
{
    app.MapOpenApi();
}

// ========================================
// ТЕСТОВЫЙ API ПОГОДЫ
// ========================================

var summaries = new[]
{
    "Freezing", "Bracing", "Chilly", "Cool", "Mild",
    "Warm", "Balmy", "Hot", "Sweltering", "Scorching"
};

app.MapGet("/weatherforecast", () =>
{
    var forecast = Enumerable.Range(1, 5)
        .Select(index =>
            new WeatherForecast
            (
                DateOnly.FromDateTime(
                    DateTime.Now.AddDays(index)),
                Random.Shared.Next(-20, 55),
                summaries[Random.Shared.Next(summaries.Length)]
            ))
        .ToArray();

    return forecast;
})
.WithName("GetWeatherForecast");

// ========================================
// ПРОВЕРКА ПОДКЛЮЧЕНИЯ К POSTGRESQL
// ========================================

if (app.Environment.IsDevelopment())
{
    app.MapGet("/db-check", async (GameDataContext db) =>
    {
        bool connected =
            await db.Database.CanConnectAsync();

        return Results.Ok(new
        {
            database = "mylittlelife_db",
            connected
        });
    });
}

// ========================================
// ПОТРЕБНОСТИ ПЕРСОНАЖА
// ========================================

// GET: загрузка потребностей персонажа.
app.MapGet(
    "/api/characters/{id:long}/needs",
    async (
        long id,
        ICharacterNeedsRepository repository) =>
    {
        var needs = await repository.GetNeedsAsync(id);

        return Results.Ok(needs);
    });

// PUT: сохранение потребностей персонажа.
app.MapPut(
    "/api/characters/{id:long}/needs",
    async (
        long id,
        List<NeedValueDto> values,
        ICharacterNeedsRepository repository) =>
    {
        // Проверяем переданные значения.
        if (values == null ||
            values.Count != 5 ||
            values.Select(n => n.NeedTypeId)
                .Distinct().Count() != 5 ||
            values.Any(n => n.Value < 0 || n.Value > 100))
        {
            return Results.BadRequest(
                "Нужно передать 5 разных потребностей " +
                "со значениями от 0 до 100.");
        }

        await repository.SaveNeedsAsync(id, values);

        var saved =
            await repository.GetNeedsAsync(id);

        return Results.Ok(saved);
    });

// ========================================
// НАСТРОЙКИ UTILITY AI
// ========================================

// GET: загрузка выбранной стратегии ИИ.
app.MapGet(
    "/api/characters/{id:long}/ai-settings",
    async (
        long id,
        IAiSettingsRepository repository) =>
    {
        if (id <= 0)
        {
            return Results.BadRequest(
                "Некорректный ID персонажа.");
        }

        string? strategy =
            await repository.GetStrategyAsync(id);

        // Если настройки ещё нет,
        // возвращаем стандартную стратегию.
        return Results.Ok(new
        {
            characterId = id,
            strategyCode = strategy ?? "FirstOnTie"
        });
    });

// PUT: сохранение выбранной стратегии ИИ.
app.MapPut(
    "/api/characters/{id:long}/ai-settings",
    async (
        long id,
        SaveAiStrategyRequest request,
        IAiSettingsRepository repository) =>
    {
        if (id <= 0)
        {
            return Results.BadRequest(
                "Некорректный ID персонажа.");
        }

        if (request.StrategyCode != "FirstOnTie" &&
            request.StrategyCode != "LastOnTie")
        {
            return Results.BadRequest(
                "Допустимые стратегии: " +
                "FirstOnTie и LastOnTie.");
        }

        await repository.SaveStrategyAsync(
            id,
            request.StrategyCode);

        return Results.Ok(new
        {
            characterId = id,
            strategyCode = request.StrategyCode,
            saved = true
        });
    });

// ========================================
// НОВОЕ: ИСТОРИЯ РЕШЕНИЙ UTILITY AI
// ========================================

// POST: сохранение нового решения ИИ.
app.MapPost(
    "/api/characters/{id:long}/ai-decisions",
    async (
        long id,
        SaveAiDecisionRequest request,
        IAiDecisionMetricsRepository repository) =>
    {
        // Проверяем корректность входных данных.
        if (id <= 0 ||
            string.IsNullOrWhiteSpace(request.ActionName) ||
            request.ActionName.Length > 64 ||
            (request.StrategyCode != "FirstOnTie" &&
             request.StrategyCode != "LastOnTie") ||
            !double.IsFinite(request.UtilityScore) ||
            request.UtilityScore < 0 ||
            request.UtilityScore > 100 ||
            !double.IsFinite(request.DecisionTimeMs) ||
            request.DecisionTimeMs < 0)
        {
            return Results.BadRequest(
                "Некорректные данные решения ИИ.");
        }

        // Сохраняем новое решение в PostgreSQL.
        long savedId =
            await repository.SaveDecisionAsync(
                id,
                request.ActionName.Trim(),
                request.StrategyCode,
                request.UtilityScore,
                request.DecisionTimeMs);

        return Results.Ok(new
        {
            id = savedId,
            saved = true
        });
    });

// GET: получение истории решений персонажа.
app.MapGet(
    "/api/characters/{id:long}/ai-decisions",
    async (
        long id,
        int? limit,
        IAiDecisionMetricsRepository repository) =>
    {
        if (id <= 0)
        {
            return Results.BadRequest(
                "Некорректный ID персонажа.");
        }

        // По умолчанию получаем последние 50 записей.
        // Максимальное количество — 100.
        int count = Math.Clamp(limit ?? 50, 1, 100);

        var history =
            await repository.GetRecentDecisionsAsync(
                id,
                count);

        return Results.Ok(history);
    });

// ========================================
// ЗАПУСК СЕРВЕРА
// ========================================

app.Run();

// ========================================
// МОДЕЛИ HTTP-ЗАПРОСОВ
// ========================================

// Модель для тестового API погоды.
record WeatherForecast(
    DateOnly Date,
    int TemperatureC,
    string? Summary)
{
    public int TemperatureF =>
        32 + (int)(TemperatureC / 0.5556);
}

// Модель запроса на сохранение стратегии ИИ.
record SaveAiStrategyRequest(
    string StrategyCode);

// НОВОЕ: модель запроса для истории решений ИИ.
record SaveAiDecisionRequest(
    string ActionName,
    string StrategyCode,
    double UtilityScore,
    double DecisionTimeMs);
