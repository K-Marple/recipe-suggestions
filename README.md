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
- PostgreSQL (Supabase)
- TheMealDB public API
- Bootstrap and custom CSS
- Deployed on Render

## Run Locally

1. Install the .NET 8 SDK.
2. Clone the repository.
3. Copy `.env.example` to `.env` and set `ConnectionStrings__DefaultConnection` (Supabase/Postgres).
4. Open a terminal in the project folder.
5. Restore and build:

```powershell
dotnet restore
dotnet build
```

6. Run the app:

```powershell
dotnet run
```

7. Open the localhost URL shown in the terminal.

## Source Repository

GitHub remote:

```text
https://github.com/K-Marple/recipe-suggestions.git
```

Task tracking: Jira project **RS** (team board).

## Database and Migrations

The app uses PostgreSQL (Supabase) through Entity Framework Core. The connection string is loaded from `.env` as `ConnectionStrings__DefaultConnection` (see `.env.example`). Do not put secrets in `appsettings.json`.

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

## How to Use (user guide)

### Quick start (guest)

1. Open [ScrapeThePantry](https://scrapethepantry.onrender.com/) (or your local URL).
2. Go to **My Pantry**.
3. Search and select ingredients you have (guest mode is temporary until you log in).
4. Click **Find Recipes**.
5. Open a recipe to see details, approximated times, and which ingredients you already have.

### Account

1. Click **Log in / Sign up** (or **Account**).
2. Create an account with a **username**, email, and password.
3. After login, guest pantry selections merge into your saved pantry.
4. Use **Account** to change username/email/password or delete the account.
5. The top bar shows **Hello, {username}** (click to open Account).

### Saved recipes

1. While logged in, save or favorite recipes from results or details.
2. Open **Saved Recipes** to browse all saved items or the Favorites tab.
3. Unsave or unfavorite from that page.

### Tips

- Green chips on recipe cards are ingredients you have; they appear first.
- If no recipes match your pantry, the app shows a short note and lets you browse other recipes.
- Custom ingredients require an account.

## CRUD Explanation

- Create: users add custom ingredients and save MealDB recipes.
- Read: users view ingredient lists, recipe results, recipe details, saved recipes, and profile stats.
- Update: users change account username/email/password and toggle recipe favorite status.
- Delete: users delete custom ingredients, remove saved recipes, and can delete their account.

Default seeded ingredients are read-only so the shared pantry list remains stable.

## API Explanation

ScrapeThePantry uses TheMealDB public API:

- `filter.php?i=ingredient_name` searches meals by one ingredient.
- `lookup.php?i=meal_id` loads full recipe details.
- `search.php?s=` supports browse when no pantry match is available.

The recipe results page searches each selected ingredient separately (capped), merges duplicate meals by MealDB id, ranks by pantry overlap, and shows have/buy chips. API failures are handled safely so the app shows friendly empty/error states instead of crashing.

## Performance

The app is designed to limit unnecessary network work:

- **MealDB caching** — ingredient filters, browse lists, and meal details are cached in memory (~30 minutes).
- **Batched hydration** — recipe cards load a small first page, then more on demand (Load More).
- **Catalog cache / background sync** — pantry ingredients are seeded and synced without re-fetching MealDB on every page view.
- **Parallel capped search** — multi-ingredient search runs a limited number of filter calls in parallel and merges results.

## Accessibility Notes

- Pages use semantic headings and clear button/link text.
- Recipe and saved recipe images include descriptive alt text.
- Validation and status messages use alert regions.
- Form controls include labels or accessible labels.
- Layout is responsive for desktop and mobile use.
- Have vs need uses icons/text in addition to color.

## Testing Checklist

See `QA_RESULTS.md` for the recorded QA evidence: build checks, functional smoke tests, Lighthouse scores, accessibility notes, responsive checks, performance notes, and repository links.

## Code comments (for maintainers)

Non-obvious logic is commented in:

- `Services/MealDbService.cs` — API + cache behavior
- `Services/IngredientMatcher.cs` — fuzzy match rules
- `Services/RecipeMatchAnalyzer.cs` — have/buy / ranking stats
- `Services/GuestPantrySession.cs` — guest → login pantry merge
- `Services/RecipeTimeEstimator.cs` — approximate times
- `Services/CatalogSyncHostedService.cs` — startup catalog sync
- `Program.cs` — env secrets + Identity setup

UI pages (`Components/Pages/*.razor`) stay light on comments; business rules live in services.

## Deployment Notes

- Configure a production database connection string via environment secrets (do not commit `.env`).
- Run EF migrations during deployment.
- Keep Identity files and authentication endpoints enabled.
- Ensure outbound HTTPS access to `https://www.themealdb.com`.
- Use HTTPS in production.
- Set environment-specific connection strings in deployment secrets or app settings.
- Run `dotnet build` before publishing.

**Live deployment:** [https://scrapethepantry.onrender.com/](https://scrapethepantry.onrender.com/)

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
