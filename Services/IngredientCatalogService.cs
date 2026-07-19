using System.Net.Http.Json;
using System.Text.Json.Serialization;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Caching.Memory;
using recipe_suggestions.Data;
using recipe_suggestions.Models;

namespace recipe_suggestions.Services;

/// <summary>
/// Loads and caches the ingredient catalog (defaults + MealDB sync + per-user custom items).
/// Cache keys are per-user so custom ingredients stay private while defaults stay shared.
/// </summary>
public class IngredientCatalogService
{
    private static readonly TimeSpan CatalogCacheDuration = TimeSpan.FromMinutes(10);

    private readonly ApplicationDbContext _db;
    private readonly HttpClient _http;
    private readonly IMemoryCache _cache;

    public IngredientCatalogService(ApplicationDbContext db, HttpClient http, IMemoryCache cache)
    {
        _db = db;
        _http = http;
        _cache = cache;
    }

    public void InvalidateCatalogCache(string? userId)
    {
        _cache.Remove(CatalogCacheKey(userId));
        _cache.Remove(CatalogCacheKey(null));
    }

    private static string CatalogCacheKey(string? userId) => $"ingredient-catalog:{userId ?? "default"}";

    public async Task EnsureCatalogSeededAsync()
    {
        if (await _db.Ingredients.AsNoTracking().AnyAsync())
            return;

        _db.Ingredients.AddRange(GetDefaultCatalog());
        await _db.SaveChangesAsync();
        InvalidateCatalogCache(null);
    }

    public async Task SyncMealDbCatalogAsync()
    {
        try
        {
            var response = await _http.GetFromJsonAsync<MealDbIngredientListResponse>("list.php?i=list");
            var remoteNames = response?.Meals?
                .Select(m => m.StrIngredient?.Trim())
                .Where(n => !string.IsNullOrWhiteSpace(n))
                .Select(n => n!)
                .Distinct(StringComparer.OrdinalIgnoreCase)
                .ToList() ?? new List<string>();

            if (remoteNames.Count == 0)
                return;

            var existing = await _db.Ingredients
                .AsNoTracking()
                .Select(i => i.Name.ToLower())
                .ToListAsync();

            var existingSet = existing.ToHashSet();
            var added = false;

            foreach (var name in remoteNames)
            {
                if (existingSet.Contains(name.ToLowerInvariant()))
                    continue;

                _db.Ingredients.Add(new Ingredient
                {
                    Name = name,
                    Category = GuessCategory(name),
                    IsDefault = true
                });
                added = true;
            }

            if (!added)
                return;

            await _db.SaveChangesAsync();
            InvalidateCatalogCache(null);
        }
        catch
        {
            // Catalog sync is best-effort; seeded defaults still work offline.
        }
    }

    public async Task<List<Ingredient>> GetCatalogAsync(string? userId, string? search = null, string? category = null)
    {
        var hasFilters = !string.IsNullOrWhiteSpace(search) || (!string.IsNullOrWhiteSpace(category) && category != "All");
        if (!hasFilters)
        {
            var cached = await _cache.GetOrCreateAsync(CatalogCacheKey(userId), async entry =>
            {
                entry.AbsoluteExpirationRelativeToNow = CatalogCacheDuration;
                return await QueryCatalogAsync(userId, null, null);
            });

            return cached?.ToList() ?? new List<Ingredient>();
        }

        return await QueryCatalogAsync(userId, search, category);
    }

    private async Task<List<Ingredient>> QueryCatalogAsync(string? userId, string? search, string? category)
    {
        var query = _db.Ingredients
            .AsNoTracking()
            .Where(i => i.IsDefault || i.CreatedByUserId == userId);

        if (!string.IsNullOrWhiteSpace(search))
            query = query.Where(i => EF.Functions.ILike(i.Name, $"%{search.Trim()}%"));

        if (!string.IsNullOrWhiteSpace(category) && category != "All")
            query = query.Where(i => i.Category == category);

        return await query
            .OrderBy(i => i.Category)
            .ThenBy(i => i.Name)
            .ToListAsync();
    }

    public async Task<List<string>> GetCategoriesAsync(string? userId)
    {
        return await _db.Ingredients
            .AsNoTracking()
            .Where(i => i.IsDefault || i.CreatedByUserId == userId)
            .Select(i => i.Category)
            .Distinct()
            .OrderBy(c => c)
            .ToListAsync();
    }

    public static string GuessCategory(string name)
    {
        var lower = name.ToLowerInvariant();

        if (ContainsAny(lower, "chicken", "beef", "pork", "lamb", "bacon", "sausage", "turkey", "ham"))
            return "Meat & Poultry";

        if (ContainsAny(lower, "salmon", "tuna", "shrimp", "fish", "cod", "crab", "prawn", "anchovy"))
            return "Seafood";

        if (ContainsAny(lower, "milk", "cheese", "butter", "cream", "yogurt", "egg"))
            return "Dairy & Eggs";

        if (ContainsAny(lower, "rice", "pasta", "noodle", "flour", "bread", "oat", "quinoa", "couscous"))
            return "Grains & Pasta";

        if (ContainsAny(lower, "salt", "pepper", "cumin", "paprika", "oregano", "basil", "thyme", "cinnamon", "ginger", "garlic powder", "chili"))
            return "Herbs & Spices";

        if (ContainsAny(lower, "sugar", "honey", "vanilla", "baking", "yeast", "cocoa"))
            return "Baking";

        if (ContainsAny(lower, "oil", "vinegar", "stock", "broth", "sauce", "tomato paste", "can ", "beans", "lentil"))
            return "Pantry Staples";

        if (ContainsAny(lower, "apple", "banana", "lemon", "lime", "orange", "berry", "mango", "fruit"))
            return "Fruits";

        if (ContainsAny(lower, "tomato", "onion", "pepper", "carrot", "potato", "lettuce", "spinach", "broccoli", "mushroom", "celery", "cucumber", "zucchini", "garlic", "corn"))
            return "Vegetables";

        return "Other";
    }

    private static bool ContainsAny(string value, params string[] terms) =>
        terms.Any(term => value.Contains(term, StringComparison.Ordinal));

    private static IEnumerable<Ingredient> GetDefaultCatalog()
    {
        var items = new (string Name, string Category)[]
        {
            ("Chicken Breast", "Meat & Poultry"), ("Chicken Thighs", "Meat & Poultry"), ("Ground Beef", "Meat & Poultry"),
            ("Beef Steak", "Meat & Poultry"), ("Pork Chops", "Meat & Poultry"), ("Bacon", "Meat & Poultry"),
            ("Turkey", "Meat & Poultry"), ("Salmon", "Seafood"), ("Shrimp", "Seafood"), ("Tuna", "Seafood"),
            ("Cod", "Seafood"), ("Rice", "Grains & Pasta"), ("Pasta", "Grains & Pasta"), ("Spaghetti", "Grains & Pasta"),
            ("Flour", "Grains & Pasta"), ("Bread", "Grains & Pasta"), ("Oats", "Grains & Pasta"),
            ("Eggs", "Dairy & Eggs"), ("Milk", "Dairy & Eggs"), ("Butter", "Dairy & Eggs"),
            ("Cheddar Cheese", "Dairy & Eggs"), ("Mozzarella", "Dairy & Eggs"), ("Yogurt", "Dairy & Eggs"),
            ("Olive Oil", "Pantry Staples"), ("Vegetable Oil", "Pantry Staples"), ("Chicken Stock", "Pantry Staples"),
            ("Tomato Sauce", "Pantry Staples"), ("Black Beans", "Pantry Staples"), ("Chickpeas", "Pantry Staples"),
            ("Garlic", "Vegetables"), ("Onion", "Vegetables"), ("Tomato", "Vegetables"), ("Bell Pepper", "Vegetables"),
            ("Carrot", "Vegetables"), ("Potato", "Vegetables"), ("Spinach", "Vegetables"), ("Broccoli", "Vegetables"),
            ("Mushrooms", "Vegetables"), ("Celery", "Vegetables"), ("Lemon", "Fruits"), ("Lime", "Fruits"),
            ("Salt", "Herbs & Spices"), ("Black Pepper", "Herbs & Spices"), ("Paprika", "Herbs & Spices"),
            ("Cumin", "Herbs & Spices"), ("Oregano", "Herbs & Spices"), ("Basil", "Herbs & Spices"),
            ("Sugar", "Baking"), ("Honey", "Baking"), ("Baking Powder", "Baking")
        };

        return items.Select(i => new Ingredient
        {
            Name = i.Name,
            Category = i.Category,
            IsDefault = true
        });
    }

    private class MealDbIngredientListResponse
    {
        [JsonPropertyName("meals")]
        public List<MealDbIngredientListItem>? Meals { get; set; }
    }

    private class MealDbIngredientListItem
    {
        [JsonPropertyName("strIngredient")]
        public string? StrIngredient { get; set; }
    }
}
