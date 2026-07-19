using System.Net.Http.Json;
using System.Text.Json;
using Microsoft.Extensions.Caching.Memory;

namespace recipe_suggestions.Services;

/// <summary>
/// Calls TheMealDB and caches responses to avoid repeat network traffic within a short window.
/// </summary>
public class MealDbService
{
    private readonly HttpClient _http;
    private readonly IMemoryCache _cache;
    private static readonly TimeSpan CacheDuration = TimeSpan.FromMinutes(30);
    public bool LastRequestFailed { get; private set; }

    public MealDbService(HttpClient http, IMemoryCache cache)
    {
        _http = http;
        _cache = cache;
    }

    public void ResetRequestFailure()
    {
        LastRequestFailed = false;
    }

    public async Task<List<MealDbMealSummary>> SearchByIngredientAsync(string ingredient)
    {
        if (string.IsNullOrWhiteSpace(ingredient))
            return new List<MealDbMealSummary>();

        var normalizedIngredient = ingredient.Trim().ToLowerInvariant();
        var cacheKey = $"mealdb:ingredient:{normalizedIngredient}";
        if (_cache.TryGetValue(cacheKey, out List<MealDbMealSummary>? cachedMeals) && cachedMeals != null)
            return cachedMeals;

        try
        {
            // TheMealDB filter endpoint returns a compact list of meals that include one ingredient.
            // Spaces become underscores (e.g. "chicken breast" → "chicken_breast").
            var safeIngredient = Uri.EscapeDataString(normalizedIngredient.Replace(" ", "_"));

            var result = await _http.GetFromJsonAsync<MealDbSearchResponse>(
                $"filter.php?i={safeIngredient}");

            var meals = result?.Meals ?? new List<MealDbMealSummary>();
            _cache.Set(cacheKey, meals, CacheDuration);
            return meals;
        }
        catch (HttpRequestException)
        {
            LastRequestFailed = true;
            return new List<MealDbMealSummary>();
        }
        catch (NotSupportedException)
        {
            LastRequestFailed = true;
            return new List<MealDbMealSummary>();
        }
        catch (JsonException)
        {
            LastRequestFailed = true;
            return new List<MealDbMealSummary>();
        }
    }

    public async Task<List<MealDbMealSummary>> GetBrowseRecipesAsync()
    {
        const string cacheKey = "mealdb:browse";
        if (_cache.TryGetValue(cacheKey, out List<MealDbMealSummary>? cachedMeals) && cachedMeals != null)
            return cachedMeals;

        try
        {
            // Empty search returns a broad meal list used when browsing or when pantry search has no hits.
            var result = await _http.GetFromJsonAsync<MealDbDetailResponse>("search.php?s=");

            var meals = result?.Meals?
                .Select(meal => new MealDbMealSummary
                {
                    IdMeal = meal.IdMeal,
                    StrMeal = meal.StrMeal,
                    StrMealThumb = meal.StrMealThumb
                })
                .Where(meal => !string.IsNullOrWhiteSpace(meal.IdMeal))
                .DistinctBy(meal => meal.IdMeal)
                .ToList() ?? new List<MealDbMealSummary>();

            _cache.Set(cacheKey, meals, CacheDuration);
            return meals;
        }
        catch (HttpRequestException)
        {
            LastRequestFailed = true;
            return new List<MealDbMealSummary>();
        }
        catch (NotSupportedException)
        {
            LastRequestFailed = true;
            return new List<MealDbMealSummary>();
        }
        catch (JsonException)
        {
            LastRequestFailed = true;
            return new List<MealDbMealSummary>();
        }
    }

    public async Task<MealDbMealDetail?> GetMealDetailsAsync(string mealId)
    {
        if (string.IsNullOrWhiteSpace(mealId))
            return null;

        var normalizedMealId = mealId.Trim();
        var cacheKey = $"mealdb:detail:{normalizedMealId}";
        if (_cache.TryGetValue(cacheKey, out MealDbMealDetail? cachedMeal) && cachedMeal != null)
            return cachedMeal;

        try
        {
            // TheMealDB lookup endpoint returns the full instructions, metadata, and image for one meal id.
            var safeMealId = Uri.EscapeDataString(normalizedMealId);
            var result = await _http.GetFromJsonAsync<MealDbDetailResponse>(
                $"lookup.php?i={safeMealId}");

            var meal = result?.Meals?.FirstOrDefault();
            if (meal != null)
                _cache.Set(cacheKey, meal, CacheDuration);
            return meal;
        }
        catch (HttpRequestException)
        {
            LastRequestFailed = true;
            return null;
        }
        catch (NotSupportedException)
        {
            LastRequestFailed = true;
            return null;
        }
        catch (JsonException)
        {
            LastRequestFailed = true;
            return null;
        }
    }
}
