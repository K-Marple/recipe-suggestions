namespace recipe_suggestions.Models;

public class SavedRecipe
{
    public int Id { get; set; }

    public string UserId { get; set; } = "";

    public string? MealDbId { get; set; }
    public string? MealDbName { get; set; }
    public string? MealDbImageUrl { get; set; }

    public bool IsFavorite { get; set; }

    public DateTime SavedAt { get; set; } = DateTime.Now;
}
