namespace recipe_suggestions.Services;

/// <summary>
/// Computes pantry "have / buy" stats and ranking helpers for a hydrated MealDB recipe.
/// Have counts use the user's pantry; priority scoring prefers earlier selected search ingredients.
/// </summary>
public class RecipeMatchAnalyzer
{
    public static RecipeMatchStats Analyze(
        MealDbMealDetail meal,
        IEnumerable<string> pantryNames,
        IEnumerable<string> searchNames)
    {
        var recipeIngredients = meal.GetIngredientsWithMeasures()
            .Select(p => p.Ingredient)
            .ToList();

        var pantryList = pantryNames.ToList();
        var searchList = searchNames.ToList();

        var haveFromPantry = IngredientMatcher.CountMatches(pantryList, recipeIngredients);
        var haveFromSearch = IngredientMatcher.CountMatches(searchList, recipeIngredients);
        var total = recipeIngredients.Count;
        var orderedOwned = searchList.Count > 0 ? searchList : pantryList;

        return new RecipeMatchStats
        {
            TotalIngredients = total,
            HaveFromPantry = haveFromPantry,
            HaveFromSearch = haveFromSearch,
            BuyCount = Math.Max(0, total - haveFromPantry),
            PriorityScore = IngredientMatcher.PriorityScore(orderedOwned, recipeIngredients),
            MissingFromPantry = recipeIngredients
                .Where(ri => !pantryList.Any(p => IngredientMatcher.Matches(p, ri)))
                .ToList(),
            CanMakeWithPantry = total > 0 && haveFromPantry == total,
            CanMakeWithSearch = total > 0 && haveFromSearch == total
        };
    }
}

public class RecipeMatchStats
{
    public int TotalIngredients { get; set; }
    public int HaveFromPantry { get; set; }
    public int HaveFromSearch { get; set; }
    public int BuyCount { get; set; }
    public int PriorityScore { get; set; }
    public List<string> MissingFromPantry { get; set; } = new();
    public bool CanMakeWithPantry { get; set; }
    public bool CanMakeWithSearch { get; set; }
}

public class EnrichedRecipeMatch
{
    public MealDbMealSummary Meal { get; set; } = new();
    public MealDbMealDetail? Detail { get; set; }
    public RecipeMatchStats Stats { get; set; } = new();
    public int SearchHitCount { get; set; }
    public List<string> MatchedSearchNames { get; set; } = new();
}
