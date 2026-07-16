using recipe_suggestions.Models;

namespace recipe_suggestions.Services;

public class RecipeSearchState
{
    public string IngredientQuery { get; set; } = string.Empty;
    public string FilterQuery { get; set; } = string.Empty;
    public string BrowseSearch { get; set; } = string.Empty;
    public bool SearchStarted { get; set; }
    public bool NoSearchResults { get; set; }
    public bool ShowBackToBrowse { get; set; }
    public List<int> SearchIngredientIds { get; } = new();
    public HashSet<int> PantryIngredientIds { get; } = new();
    public List<string> PantryNames { get; } = new();
    public List<string> SearchNames { get; } = new();
    public List<EnrichedRecipeMatch> EnrichedMatches { get; } = new();
    public List<EnrichedRecipeMatch> RecipeCandidates { get; } = new();
    public HashSet<string> FavoritedMealIds { get; } = new(StringComparer.OrdinalIgnoreCase);
    public HashSet<string> SavedMealIds { get; } = new(StringComparer.OrdinalIgnoreCase);
    public RecipeFilter FilterMode { get; set; } = RecipeFilter.All;
    public bool IsLoading { get; set; } = true;
    public string LoadingProgress { get; set; } = "0";
    public int AllCount { get; set; }
    public int PantryCompleteCount { get; set; }
    public int SearchCompleteCount { get; set; }
    public int NextCandidateIndex { get; set; }
    public string? UserId { get; set; }
    public List<MealDbMealSummary> BrowseRecipesCache { get; } = new();

    public void Clear(bool keepBrowseCache = true, bool keepPantry = true)
    {
        if (!keepBrowseCache)
            BrowseRecipesCache.Clear();

        IngredientQuery = string.Empty;
        FilterQuery = string.Empty;
        BrowseSearch = string.Empty;
        SearchStarted = false;
        NoSearchResults = false;
        ShowBackToBrowse = false;
        SearchIngredientIds.Clear();
        if (!keepPantry)
        {
            PantryIngredientIds.Clear();
            PantryNames.Clear();
        }
        SearchNames.Clear();
        EnrichedMatches.Clear();
        RecipeCandidates.Clear();
        FavoritedMealIds.Clear();
        SavedMealIds.Clear();
        FilterMode = RecipeFilter.All;
        IsLoading = true;
        LoadingProgress = "0";
        AllCount = 0;
        PantryCompleteCount = 0;
        SearchCompleteCount = 0;
        NextCandidateIndex = 0;
        UserId = null;
    }
}
