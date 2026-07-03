using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace recipe_suggestions.Migrations
{
    /// <inheritdoc />
    public partial class AddMealDbColumnsToSavedRecipe : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropForeignKey(
                name: "FK_SavedRecipes_Recipes_RecipeId",
                table: "SavedRecipes");

            migrationBuilder.AlterColumn<int>(
                name: "RecipeId",
                table: "SavedRecipes",
                type: "INTEGER",
                nullable: true,
                oldClrType: typeof(int),
                oldType: "INTEGER");

            migrationBuilder.AddColumn<string>(
                name: "MealDbId",
                table: "SavedRecipes",
                type: "TEXT",
                nullable: true);

            migrationBuilder.AddColumn<string>(
                name: "MealDbImageUrl",
                table: "SavedRecipes",
                type: "TEXT",
                nullable: true);

            migrationBuilder.AddColumn<string>(
                name: "MealDbName",
                table: "SavedRecipes",
                type: "TEXT",
                nullable: true);

            migrationBuilder.AddForeignKey(
                name: "FK_SavedRecipes_Recipes_RecipeId",
                table: "SavedRecipes",
                column: "RecipeId",
                principalTable: "Recipes",
                principalColumn: "Id");
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropForeignKey(
                name: "FK_SavedRecipes_Recipes_RecipeId",
                table: "SavedRecipes");

            migrationBuilder.DropColumn(
                name: "MealDbId",
                table: "SavedRecipes");

            migrationBuilder.DropColumn(
                name: "MealDbImageUrl",
                table: "SavedRecipes");

            migrationBuilder.DropColumn(
                name: "MealDbName",
                table: "SavedRecipes");

            migrationBuilder.AlterColumn<int>(
                name: "RecipeId",
                table: "SavedRecipes",
                type: "INTEGER",
                nullable: false,
                defaultValue: 0,
                oldClrType: typeof(int),
                oldType: "INTEGER",
                oldNullable: true);

            migrationBuilder.AddForeignKey(
                name: "FK_SavedRecipes_Recipes_RecipeId",
                table: "SavedRecipes",
                column: "RecipeId",
                principalTable: "Recipes",
                principalColumn: "Id",
                onDelete: ReferentialAction.Cascade);
        }
    }
}
