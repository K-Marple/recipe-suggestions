namespace recipe_suggestions.Services;

/// <summary>
/// MealDB does not publish prep/cook times, so we approximate from ingredient count,
/// category hints, and a stable name hash. Display as "~N mins" (rounded to 5).
/// </summary>
public static class RecipeTimeEstimator
{
    public static RecipeTimeEstimate Estimate(
        int ingredientCount,
        string? mealName = null,
        string? category = null)
    {
        var count = Math.Max(ingredientCount, 1);
        var total = count <= 6 ? 25 : count <= 11 ? 35 : 45;

        if (!string.IsNullOrWhiteSpace(category))
        {
            if (category.Contains("Dessert", StringComparison.OrdinalIgnoreCase) ||
                category.Contains("Baking", StringComparison.OrdinalIgnoreCase))
                total += 10;
            else if (category.Contains("Beef", StringComparison.OrdinalIgnoreCase) ||
                     category.Contains("Lamb", StringComparison.OrdinalIgnoreCase))
                total += 10;
            else if (category.Contains("Seafood", StringComparison.OrdinalIgnoreCase) ||
                     category.Contains("Starter", StringComparison.OrdinalIgnoreCase))
                total -= 5;
        }

        if (!string.IsNullOrWhiteSpace(mealName))
            total += Math.Abs(StringComparer.OrdinalIgnoreCase.GetHashCode(mealName)) % 3 * 5;

        total = RoundApprox(Math.Max(15, total));
        var prep = RoundApprox(Math.Max(10, (total * 2 + 2) / 5));
        var cook = RoundApprox(Math.Max(10, total - prep));
        total = prep + cook;

        var difficulty = count >= 12 || total >= 50 ? "Medium" : "Easy";
        return new RecipeTimeEstimate(prep, cook, total, difficulty);
    }

    public static string FormatMinutes(int minutes) => $"~{RoundApprox(minutes)} mins";

    public static int RoundApprox(int minutes)
    {
        if (minutes <= 10)
            return 10;

        return (int)Math.Round(minutes / 5.0, MidpointRounding.AwayFromZero) * 5;
    }
}

public record RecipeTimeEstimate(int PrepMinutes, int CookMinutes, int TotalMinutes, string Difficulty);
