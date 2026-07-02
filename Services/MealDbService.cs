using System.Net.Http.Json;
using System.Text.Json;

namespace recipe_suggestions.Services;

public class MealDbService
{
    private readonly HttpClient _http;
    public bool LastRequestFailed { get; private set; }

    public MealDbService(HttpClient http)
    {
        _http = http;
    }

    public void ResetRequestFailure()
    {
        LastRequestFailed = false;
    }

    public async Task<List<MealDbMealSummary>> SearchByIngredientAsync(string ingredient)
    {
        if (string.IsNullOrWhiteSpace(ingredient))
            return new List<MealDbMealSummary>();

        try
        {
            // TheMealDB filter endpoint returns a compact list of meals that include one ingredient.
            // Spaces are converted to underscores because TheMealDB expects values like "chicken_breast".
            var safeIngredient = Uri.EscapeDataString(ingredient.Trim().Replace(" ", "_").ToLowerInvariant());

            var result = await _http.GetFromJsonAsync<MealDbSearchResponse>(
                $"filter.php?i={safeIngredient}");

            return result?.Meals ?? new List<MealDbMealSummary>();
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

        try
        {
            // TheMealDB lookup endpoint returns the full instructions, metadata, and image for one meal id.
            var safeMealId = Uri.EscapeDataString(mealId.Trim());
            var result = await _http.GetFromJsonAsync<MealDbDetailResponse>(
                $"lookup.php?i={safeMealId}");

            return result?.Meals?.FirstOrDefault();
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
