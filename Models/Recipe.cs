namespace recipe_suggestions.Models;

public class Recipe
{
    public int Id { get; set; }
    public string Name { get; set; } = "";
    public string Description { get; set; } = "";
    public int PrepTimeMinutes { get; set; }
    public string Difficulty { get; set; } = "Easy";
    public string Instructions { get; set; } = "";

    public List<RecipeIngredient> RecipeIngredients { get; set; } = new();
}