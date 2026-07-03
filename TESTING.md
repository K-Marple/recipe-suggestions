# ScrapeThePantry Manual QA Checklist

Use this checklist before submitting or deploying the app.

## Recorded Automated Checks

- [x] Build passed with 0 warnings and 0 errors using `dotnet build -o .\build-check-evidence2 /p:UseAppHost=false`.
- [x] Desktop smoke check completed for Home, Ingredients, Recipes, Saved Recipes, Profile, Login, and Register.
- [x] Mobile smoke check completed for Home, Ingredients, Recipes, Saved Recipes, Profile, Login, and Register.
- [x] Checked routes had no horizontal overflow in the tested desktop/mobile viewports.
- [x] Checked routes had no images missing `alt` attributes.
- [x] Checked routes had no unnamed buttons.
- [x] Checked routes had no unlabeled visible inputs after account form label fixes.

See `QA_RESULTS.md` for details.

## Manual Checks Still Needed

These require a person to run through the app with test accounts and confirm behavior.

## Authentication

- [ ] Register with a new email and valid password.
- [ ] Confirm the registered user can reach authenticated pages.
- [ ] Log in with a registered account.
- [ ] Log out and confirm saved/profile pages require login.

## Ingredient CRUD

- [ ] Add a custom ingredient with a valid name.
- [ ] Confirm blank custom ingredients are rejected.
- [ ] Confirm duplicate ingredients are rejected when capitalization differs.
- [ ] Confirm ingredient names longer than 40 characters are rejected or capped.
- [ ] Edit a custom ingredient using the kebab menu.
- [ ] Confirm default ingredients cannot be edited.
- [ ] Delete a custom ingredient with confirmation dialog.
- [ ] Confirm default ingredients cannot be deleted.
- [ ] Search for an ingredient and confirm the list filters correctly.
- [ ] Filter by All, Selected, and Custom tabs.
- [ ] Confirm custom ingredients are scoped to the signed-in user.
- [ ] Confirm selected ingredients persist when returning from Recipes via Edit Ingredients.

## Recipe Search

- [ ] Select one ingredient.
- [ ] Select multiple ingredients.
- [ ] Confirm the 15 ingredient selection limit.
- [ ] Click Find Recipes.
- [ ] Confirm loading state appears while recipes load.
- [ ] Confirm recipe cards display names, images, match counts, and matched ingredient labels.
- [ ] Confirm results are sorted by match count (highest first).
- [ ] Quick-save a recipe to favorites from the heart button on a recipe card.
- [ ] Confirm empty/error state appears if no recipes or API access is unavailable.

## Recipe Details and Saved Recipes

- [ ] Open a recipe from the results page.
- [ ] Confirm recipe image, title, category, area, ingredient tags, and numbered instructions display.
- [ ] Save a recipe while logged in.
- [ ] Confirm unauthenticated users are prompted to log in before saving.
- [ ] Favorite a saved recipe from the details page.
- [ ] Open Saved Recipes.
- [ ] Filter between All Recipes and Favorites tabs.
- [ ] Confirm favorites appear first in the All tab.
- [ ] Favorite and unfavorite a saved recipe.
- [ ] Remove a saved recipe with confirmation.
- [ ] Confirm the empty state shows a Find Recipes button when no recipes are saved.
- [ ] Confirm Profile favorites stat links to `/savedrecipes?tab=favorites`.

## Profile

- [ ] Confirm profile shows account email.
- [ ] Confirm saved recipe count is correct.
- [ ] Confirm favorite count is correct.
- [ ] Confirm custom ingredient count is correct (user-scoped only).
- [ ] Confirm Manage Account link works.
- [ ] Confirm stat cards link to the correct pages.

## UI, Mobile, and Accessibility

- [ ] Test desktop layout at full width.
- [ ] Test mobile responsiveness using browser dev tools (375px and 768px).
- [ ] Confirm mobile navigation toggle opens and closes the sidebar.
- [ ] Confirm skip-to-content link appears on keyboard focus.
- [ ] Confirm buttons and links are keyboard reachable.
- [ ] Confirm visible focus states are usable on all interactive elements.
- [ ] Confirm tab filters use proper ARIA roles and selected state.
- [ ] Confirm kebab menus are keyboard operable.
- [ ] Confirm modals can be closed and have accessible labels.
- [ ] Confirm images have useful alt text.
- [ ] Run a Lighthouse accessibility check on Home, Ingredients, Recipes, and Recipe Details.
- [ ] Fix any critical accessibility or contrast issues before submission.
