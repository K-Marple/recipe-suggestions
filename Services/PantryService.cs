using Microsoft.EntityFrameworkCore;
using recipe_suggestions.Data;
using recipe_suggestions.Models;

namespace recipe_suggestions.Services;

public class PantryService
{
    private readonly ApplicationDbContext _db;

    public PantryService(ApplicationDbContext db)
    {
        _db = db;
    }

    public async Task<List<Ingredient>> GetPantryIngredientsAsync(string userId)
    {
        return await _db.PantryItems
            .Where(p => p.UserId == userId)
            .Include(p => p.Ingredient)
            .Where(p => p.Ingredient != null)
            .OrderBy(p => p.Ingredient!.Name)
            .Select(p => p.Ingredient)
            .OfType<Ingredient>()
            .ToListAsync();
    }

    public async Task<HashSet<int>> GetPantryIngredientIdsAsync(string userId)
    {
        var ids = await _db.PantryItems
            .Where(p => p.UserId == userId)
            .Select(p => p.IngredientId)
            .ToListAsync();

        return ids.ToHashSet();
    }

    public async Task<bool> IsInPantryAsync(string userId, int ingredientId)
    {
        return await _db.PantryItems
            .AnyAsync(p => p.UserId == userId && p.IngredientId == ingredientId);
    }

    public async Task AddToPantryAsync(string userId, int ingredientId)
    {
        if (await IsInPantryAsync(userId, ingredientId))
            return;

        _db.PantryItems.Add(new PantryItem
        {
            UserId = userId,
            IngredientId = ingredientId
        });

        await _db.SaveChangesAsync();
    }

    public async Task AddManyToPantryAsync(string userId, IEnumerable<int> ingredientIds)
    {
        var ids = ingredientIds.Distinct().ToList();
        if (ids.Count == 0)
            return;

        var existing = await _db.PantryItems
            .Where(p => p.UserId == userId && ids.Contains(p.IngredientId))
            .Select(p => p.IngredientId)
            .ToListAsync();

        var validIds = await _db.Ingredients
            .Where(i => ids.Contains(i.Id))
            .Select(i => i.Id)
            .ToListAsync();

        var toAdd = validIds.Except(existing).ToList();
        if (toAdd.Count == 0)
            return;

        _db.PantryItems.AddRange(toAdd.Select(id => new PantryItem
        {
            UserId = userId,
            IngredientId = id
        }));

        await _db.SaveChangesAsync();
    }

    public async Task RemoveFromPantryAsync(string userId, int ingredientId)
    {
        var item = await _db.PantryItems
            .FirstOrDefaultAsync(p => p.UserId == userId && p.IngredientId == ingredientId);

        if (item == null)
            return;

        _db.PantryItems.Remove(item);
        await _db.SaveChangesAsync();
    }

    public async Task<int> GetPantryCountAsync(string userId)
    {
        return await _db.PantryItems.CountAsync(p => p.UserId == userId);
    }
}
