namespace recipe_suggestions.Services;

/// <summary>
/// One-shot background job: sync MealDB ingredient names into the local catalog after startup
/// so the pantry page can serve from Postgres/cache instead of hitting MealDB every request.
/// </summary>
public sealed class CatalogSyncHostedService(
    IServiceScopeFactory scopeFactory,
    ILogger<CatalogSyncHostedService> logger) : BackgroundService
{
    protected override async Task ExecuteAsync(CancellationToken stoppingToken)
    {
        await Task.Yield();

        try
        {
            await using var scope = scopeFactory.CreateAsyncScope();
            var catalog = scope.ServiceProvider.GetRequiredService<IngredientCatalogService>();
            await catalog.SyncMealDbCatalogAsync();
        }
        catch (Exception exception) when (!stoppingToken.IsCancellationRequested)
        {
            logger.LogWarning(exception, "The background ingredient catalog sync did not complete.");
        }
    }
}
