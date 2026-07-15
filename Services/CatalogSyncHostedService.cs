namespace recipe_suggestions.Services;

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
