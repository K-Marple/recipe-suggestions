using System.Text.Json;
using Microsoft.JSInterop;

namespace recipe_suggestions.Services;

// Stores an anonymous user's pantry (ingredient ids) in the browser's localStorage
// so guests can build a pantry and search recipes without an account. When a guest
// logs in, these ids are migrated into their persistent database pantry.
public static class LocalPantryStore
{
    private const string Key = "stp_pantry_ids";

    public static async Task<List<int>> GetAsync(IJSRuntime js)
    {
        try
        {
            var json = await js.InvokeAsync<string?>("localStorage.getItem", Key);
            if (string.IsNullOrWhiteSpace(json))
                return new();

            return JsonSerializer.Deserialize<List<int>>(json) ?? new();
        }
        catch
        {
            return new();
        }
    }

    public static async Task SetAsync(IJSRuntime js, IEnumerable<int> ids)
    {
        try
        {
            var json = JsonSerializer.Serialize(ids.Distinct().ToList());
            await js.InvokeVoidAsync("localStorage.setItem", Key, json);
        }
        catch
        {
            // localStorage may be unavailable during prerender; ignore.
        }
    }

    public static async Task ClearAsync(IJSRuntime js)
    {
        try
        {
            await js.InvokeVoidAsync("localStorage.removeItem", Key);
        }
        catch
        {
            // Ignore storage errors.
        }
    }
}
