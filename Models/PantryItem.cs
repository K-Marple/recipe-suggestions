namespace recipe_suggestions.Models;

public class PantryItem
{
    public int Id { get; set; }

    public string UserId { get; set; } = "";

    public int IngredientId { get; set; }
    public Ingredient Ingredient { get; set; } = null!;

    public DateTime AddedAt { get; set; } = DateTime.UtcNow;
}
