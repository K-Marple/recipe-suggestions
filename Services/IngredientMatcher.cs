namespace recipe_suggestions.Services;

/// <summary>
/// Fuzzy matching between pantry/search names and MealDB ingredient strings.
/// Tuned to avoid common false positives (egg↔eggplant, pea↔peanut).
/// </summary>
public static class IngredientMatcher
{
    public static string Normalize(string? value)
    {
        if (string.IsNullOrWhiteSpace(value))
            return "";

        return value
            .Trim()
            .ToLowerInvariant()
            .Replace("_", " ")
            .Replace("-", " ")
            .Replace("  ", " ");
    }

    public static bool Matches(string pantryName, string recipeIngredient)
    {
        var pantry = Normalize(pantryName);
        var recipe = Normalize(recipeIngredient);

        if (string.IsNullOrEmpty(pantry) || string.IsNullOrEmpty(recipe))
            return false;

        if (pantry == recipe)
            return true;

        // Prefer whole-word / whole-phrase containment (avoids egg↔eggplant, pea↔peanut).
        if (ContainsAsWords(recipe, pantry) || ContainsAsWords(pantry, recipe))
            return true;

        var pantryTokens = SignificantTokens(pantry);
        var recipeTokens = SignificantTokens(recipe);
        if (pantryTokens.Count == 0 || recipeTokens.Count == 0)
            return false;

        return pantryTokens.Any(pt => recipeTokens.Contains(pt));
    }

    public static int CountMatches(IEnumerable<string> pantryNames, IEnumerable<string> recipeIngredients)
    {
        var pantryList = pantryNames.ToList();
        var recipeList = recipeIngredients.ToList();

        return recipeList.Count(recipeIngredient =>
            pantryList.Any(pantryName => Matches(pantryName, recipeIngredient)));
    }

    /// <summary>
    /// Higher score when earlier items in the ordered pantry/search list appear in the recipe.
    /// </summary>
    public static int PriorityScore(IReadOnlyList<string> orderedOwnedNames, IEnumerable<string> recipeIngredients)
    {
        if (orderedOwnedNames.Count == 0)
            return 0;

        var recipeList = recipeIngredients.ToList();
        var score = 0;
        var weight = orderedOwnedNames.Count;

        for (var i = 0; i < orderedOwnedNames.Count; i++)
        {
            if (recipeList.Any(recipeIngredient => Matches(orderedOwnedNames[i], recipeIngredient)))
                score += weight - i;
        }

        return score;
    }

    private static bool ContainsAsWords(string haystack, string needle)
    {
        if (needle.Length < 3 || !haystack.Contains(needle, StringComparison.Ordinal))
            return false;

        return $" {haystack} ".Contains($" {needle} ", StringComparison.Ordinal);
    }

    private static HashSet<string> SignificantTokens(string value) =>
        value.Split(' ', StringSplitOptions.RemoveEmptyEntries)
            .Where(token => token.Length >= 3)
            .ToHashSet(StringComparer.Ordinal);
}
