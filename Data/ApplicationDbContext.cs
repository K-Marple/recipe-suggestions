using recipe_suggestions.Models;
using Microsoft.AspNetCore.Identity.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore;

namespace recipe_suggestions.Data;

public class ApplicationDbContext(DbContextOptions<ApplicationDbContext> options)
    : IdentityDbContext<ApplicationUser>(options)
{
    public DbSet<Ingredient> Ingredients => Set<Ingredient>();

    public DbSet<Recipe> Recipes => Set<Recipe>();

    public DbSet<RecipeIngredient> RecipeIngredients => Set<RecipeIngredient>();

    public DbSet<SavedRecipe> SavedRecipes => Set<SavedRecipe>();

    public DbSet<PantryItem> PantryItems => Set<PantryItem>();

    protected override void OnModelCreating(ModelBuilder modelBuilder)
    {
        base.OnModelCreating(modelBuilder);

        modelBuilder.Entity<PantryItem>(entity =>
        {
            entity.HasIndex(p => new { p.UserId, p.IngredientId }).IsUnique();
            entity.HasIndex(p => p.UserId);

            entity.HasOne(p => p.Ingredient)
                .WithMany(i => i.PantryItems)
                .HasForeignKey(p => p.IngredientId)
                .OnDelete(DeleteBehavior.Cascade);
        });

        modelBuilder.Entity<Ingredient>(entity =>
        {
            entity.HasIndex(i => i.Name);
        });
    }
}
