using Microsoft.AspNetCore.Components.Server.ProtectedBrowserStorage;

namespace recipe_suggestions.Services;

/// <summary>
/// Persists guest pantry/search ingredient IDs across login so they can be merged into the account pantry.
/// </summary>
public class GuestPantrySession
{
    private const string StorageKey = "guest_pantry_ids";
    private readonly ProtectedLocalStorage _localStorage;

    public GuestPantrySession(ProtectedLocalStorage localStorage)
    {
        _localStorage = localStorage;
    }

    public async Task SaveAsync(IEnumerable<int> ingredientIds)
    {
        var ids = ingredientIds.Where(id => id > 0).Distinct().OrderBy(id => id).ToList();
        if (ids.Count == 0)
            return;

        try
        {
            await _localStorage.SetAsync(StorageKey, ids);
        }
        catch (InvalidOperationException)
        {
            // JS interop not available yet (prerender) — ignore.
        }
    }

    public async Task<List<int>> TakeAsync()
    {
        try
        {
            var result = await _localStorage.GetAsync<List<int>>(StorageKey);
            await ClearAsync();
            return result.Success && result.Value != null
                ? result.Value.Where(id => id > 0).Distinct().ToList()
                : new List<int>();
        }
        catch (InvalidOperationException)
        {
            return new List<int>();
        }
    }

    public async Task ClearAsync()
    {
        try
        {
            await _localStorage.DeleteAsync(StorageKey);
        }
        catch (InvalidOperationException)
        {
        }
    }
}
