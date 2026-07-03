# ScrapeThePantry Jira Task Breakdown

## Setup

- Create Blazor web application project.
- Configure project structure for Components, Data, Models, Services, and wwwroot assets.
- Add GitHub repository with `.gitignore`, README, and buildable source.

## Authentication

- Add ASP.NET Core Identity.
- Create register, login, logout, and account management flow.
- Protect user-specific saved recipe and profile data.

## Database

- Configure Entity Framework Core with SQLite.
- Create Identity schema migration.
- Create recipe, ingredient, and saved recipe models.
- Add and apply migrations.

## Ingredient CRUD

- Display seeded default ingredients.
- Add custom ingredient create behavior.
- Add custom ingredient edit behavior.
- Add custom ingredient delete behavior.
- Prevent default ingredients from being edited or deleted.

## MealDB API

- Register typed `HttpClient` for TheMealDB.
- Search meals by ingredient.
- Load meal details by MealDB id.
- Add safe API failure handling for offline or empty responses.

## Recipe Results

- Pass selected ingredient ids to recipe results page.
- Search TheMealDB once per selected ingredient.
- Merge duplicate MealDB results by meal id.
- Display match count and matched ingredient chips.
- Add empty and API unavailable states.

## Saved Recipes

- Save MealDB recipes to the signed-in user account.
- Display saved recipes.
- Sort favorites first.
- Toggle favorite/unfavorite.
- Remove saved recipes.
- Add empty state with Find Recipes action.

## UI/UX

- Create responsive app layout.
- Add navigation links for Ingredients, Recipes, Saved Recipes, Profile, and Account.
- Create recipe cards and detail layout.
- Add mobile-friendly spacing and wrapping.

## Validation

- Reject blank custom ingredient names.
- Reject duplicate ingredient names ignoring capitalization.
- Limit ingredient names to 40 characters.
- Show clear success and error messages.
- Validate malformed recipe query ids safely.

## Testing

- Build project with `dotnet build`.
- Run manual authentication tests.
- Run manual ingredient CRUD tests.
- Run manual recipe search and details tests.
- Run manual saved recipe tests.
- Run mobile and accessibility checks.

## Documentation

- Update README with purpose, audience, features, setup, CRUD, API, accessibility, testing, and deployment notes.
- Add TESTING.md manual QA checklist.
- Add Jira task breakdown.
- Add helpful code comments around API, CRUD, recipe merging, and saved recipe behavior.

## Deployment

- Configure production connection string.
- Apply migrations in the target environment.
- Publish Release build.
- Verify HTTPS and outbound TheMealDB API access.
- Smoke test authentication, ingredients, recipe search, saved recipes, and profile after deployment.
