namespace recipe_suggestions.Services;

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

        if (pantry.Contains(recipe, StringComparison.Ordinal) ||
            recipe.Contains(pantry, StringComparison.Ordinal))
            return true;

        var pantryTokens = pantry.Split(' ', StringSplitOptions.RemoveEmptyEntries);
        var recipeTokens = recipe.Split(' ', StringSplitOptions.RemoveEmptyEntries);

        if (pantryTokens.Length == 0 || recipeTokens.Length == 0)
            return false;

        return pantryTokens.Any(pt => recipeTokens.Any(rt => pt == rt || pt.StartsWith(rt) || rt.StartsWith(pt)));
    }

    public static int CountMatches(IEnumerable<string> pantryNames, IEnumerable<string> recipeIngredients)
    {
        var pantryList = pantryNames.ToList();
        var recipeList = recipeIngredients.ToList();

        return recipeList.Count(recipeIngredient =>
            pantryList.Any(pantryName => Matches(pantryName, recipeIngredient)));
    }
}
