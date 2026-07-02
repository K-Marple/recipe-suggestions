namespace recipe_suggestions.Models;

public class Ingredient
{
    public int Id { get; set; }
    public string Name { get; set; } = "";
    public string Category { get; set; } = "Other";
    public bool IsDefault { get; set; } = true;
    public string? CreatedByUserId { get; set; }
}