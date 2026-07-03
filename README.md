# ScrapeThePantry

ScrapeThePantry is a .NET Blazor web application that helps users choose ingredients they already have and find matching recipes from TheMealDB.

## Purpose

The app reduces meal-planning friction by turning pantry ingredients into recipe suggestions. Users can add custom ingredients, search and select ingredients, view recipe details, and save/favorite recipes for later.

## Target Audience

- Students and families planning meals with food they already have
- Home cooks who want quick recipe ideas
- Users who want a simple saved recipe list tied to their account

## Features

- ASP.NET Identity registration, login, logout, and account management
- Ingredient CRUD for custom ingredients
- Ingredient search and up to 15 selected ingredients for recipe search
- TheMealDB API integration for recipe search and details
- Matched ingredient chips on recipe cards
- Save recipes to a user account
- Favorite/unfavorite saved recipes
- Remove saved recipes
- Profile page with email and activity stats
- Responsive custom CSS layout
- Validation, empty states, and API failure handling

## Tech Stack

- .NET 8
- Blazor Web App with Interactive Server rendering
- ASP.NET Core Identity
- Entity Framework Core
- SQLite
- TheMealDB public API
- Bootstrap and custom CSS

## Run Locally

1. Install the .NET 8 SDK.
2. Clone the repository.
3. Open a terminal in the project folder.
4. Restore and build:

```powershell
dotnet restore
dotnet build
```

5. Run the app:

```powershell
dotnet run
```

6. Open the localhost URL shown in the terminal.

## Source Repository

GitHub remote:

```text
https://github.com/K-Marple/recipe-suggestions.git
```

## Database and Migrations

The app uses SQLite through Entity Framework Core. The default connection string is configured in `appsettings.json`.

Create a migration after model changes:

```powershell
dotnet ef migrations add MigrationName
```

Apply migrations to the database:

```powershell
dotnet ef database update
```

If `dotnet ef` is not available, install the EF Core CLI tool:

```powershell
dotnet tool install --global dotnet-ef
```

## How to Use

1. Register for an account or log in.
2. Go to Ingredients.
3. Search existing ingredients or add custom ingredients.
4. Select up to 15 ingredients.
5. Click Find Recipes.
6. Open a recipe card to view details.
7. Save recipes while logged in.
8. Favorite, unfavorite, or remove recipes from Saved Recipes.
9. View account stats on the Profile page.

## CRUD Explanation

- Create: users add custom ingredients and save MealDB recipes.
- Read: users view ingredient lists, recipe results, recipe details, saved recipes, and profile stats.
- Update: users edit custom ingredient names and toggle recipe favorite status.
- Delete: users delete custom ingredients and remove saved recipes.

Default seeded ingredients are read-only so the shared pantry list remains stable.

## API Explanation

ScrapeThePantry uses TheMealDB public API:

- `filter.php?i=ingredient_name` searches meals by one ingredient.
- `lookup.php?i=meal_id` loads full recipe details.

The recipe results page searches each selected ingredient separately, merges duplicate meals by MealDB id, counts matches, and displays the matched ingredient chips. API failures are handled safely so the app shows friendly empty/error states instead of crashing.

## Accessibility Notes

- Pages use semantic headings and clear button/link text.
- Recipe and saved recipe images include descriptive alt text.
- Validation and status messages use alert regions.
- Form controls include labels or accessible labels.
- Layout is responsive for desktop and mobile use.

## Testing Checklist

See `TESTING.md` for the full manual QA checklist. At minimum, verify registration, login, ingredient CRUD, recipe search, recipe details, saved recipe actions, profile stats, mobile layout, and accessibility/Lighthouse checks.

See `QA_RESULTS.md` for recorded build, responsive smoke-test, accessibility smoke-test, and repository evidence.

## Deployment Notes

- Configure a production SQLite path or managed database connection string.
- Run EF migrations during deployment.
- Keep Identity files and authentication endpoints enabled.
- Ensure outbound HTTPS access to `https://www.themealdb.com`.
- Use HTTPS in production.
- Set environment-specific connection strings in deployment secrets or app settings.
- Run `dotnet build` before publishing.

Deployment status: add the final cloud deployment URL here after publishing.

Publish example:

```powershell
dotnet publish -c Release
```

## Team Members

- Avery Jones
- Jared Harper
- Mike Olson
- William Delgado Florez
- Kayli Marple
