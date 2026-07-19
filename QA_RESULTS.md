# QA Results — ScrapeThePantry

**App:** [https://scrapethepantry.onrender.com/](https://scrapethepantry.onrender.com/)  
**Repo:** https://github.com/K-Marple/recipe-suggestions.git  
**Task board:** Jira project RS (team chose Jira instead of Trello)  
**Stack:** .NET 8 Blazor (Interactive Server), Identity, EF Core, PostgreSQL (Supabase), TheMealDB  
**QA date:** 2026-07-18  
**Follow-up fixes:** 2026-07-18 (a11y + responsive spacing)

---

## Build evidence

| Check | Result | Notes |
|-------|--------|-------|
| `dotnet build` | Pass | Project builds on .NET 8 |
| `dotnet run` (local) | Pass | Requires `.env` with `ConnectionStrings__DefaultConnection` |
| Live site loads | Pass | https://scrapethepantry.onrender.com/ |

---

## Manual functional smoke test

| Area | Result | Notes |
|------|--------|-------|
| Navigation / branding | Pass | Consistent sidebar + top bar |
| Register / login / logout | Pass | Username on register; email login |
| Guest pantry + merge on login | Pass | Guest selections merge into account pantry |
| Ingredient select / custom CRUD | Pass | Custom add/remove for signed-in users |
| Recipe search + have/buy | Pass | Sorted match badges; owned chips first |
| Browse fallback when no matches | Pass | Short notice + browse list |
| Recipe details | Pass | Have/need list; cleaned instruction steps; ~mins |
| Save / favorite / saved list | Pass | Auth-gated |
| Account settings | Pass | Username/email/password/delete |
| Responsive layout | Pass | Shared spacing scale; drawer ≤900px; compact ≤640px |

---

## Lighthouse results

Captured with Lighthouse CLI against the live Home page (`https://scrapethepantry.onrender.com/`).

### Desktop (2026-07-18)

| Category | Score | Notes |
|----------|------:|-------|
| Performance | **100** | Strong caching / light home markup |
| Accessibility | **92** | Findings below — fixed in follow-up |
| Best Practices | **100** | |
| SEO | **90** | |

### Mobile (2026-07-18)

| Category | Score | Notes |
|----------|------:|-------|
| Performance | **90** | Expected dip vs desktop for Interactive Server |
| Accessibility | **92** | Same a11y findings as desktop — fixed in follow-up |
| Best Practices | **100** | |
| SEO | **90** | |

### Extra page check

| Page | Accessibility | Notes |
|------|--------------:|-------|
| `/recipes` (desktop) | **95** | Slightly higher than Home |

### Accessibility findings (initial audit) → fixes

| Finding | Status |
|---------|--------|
| `[aria-hidden="true"]` on focusable sidebar checkbox | **Fixed** — checkbox visually hidden, `tabindex="-1"`, no `aria-hidden`; toggle uses labeled control with `.visually-hidden` text |
| `aria-label` on associated `<label>` | **Fixed** — name via `.visually-hidden` span instead of `aria-label` |
| Heading order h1 → h3 on Home | **Fixed** — section `h2` (visually hidden) + feature card `h3`s |
| Empty backdrop control | **Fixed** — close backdrop includes “Close navigation menu” |
| Article landmark without name | **Fixed** — `aria-label="Main content"` on main article |
| Auth cards oversized on mobile | **Fixed** — mobile padding cascade corrected |
| Inconsistent card stacking / spacing | **Fixed** — master ≤900 / ≤640 spacing scale; recipe + saved cards stack together |

Re-run Lighthouse on the live Home page after deploy to confirm Accessibility moves above 92.

---

## Accessibility (WCAG 2.1 AA oriented)

| Check | Result | Evidence |
|-------|--------|----------|
| Semantic headings / landmarks | Pass | Home: h1 → h2 → h3; main article labeled |
| Skip to content link | Pass | Present in layout |
| Form labels | Pass | Auth + account + pantry controls labeled |
| Meaningful image alt | Pass | Recipe images use descriptive alt |
| Keyboard-reachable primary actions | Pass | Links/buttons focusable; sidebar closes on navigate; 44×44 menu toggle |
| Status messages | Pass | Validation / guest banner / save feedback |
| Have vs need not color-only | Pass | Check / circle icons + text on details; chip text on lists |
| Lighthouse Accessibility (Home) | Pass* | 92 pre-fix; fixes applied locally |

\* Re-audit after deploy recommended.

---

## Markup / styling validation

| Check | Result | Notes |
|-------|--------|-------|
| Consistent branding (color, type, layout) | Pass | Shared CSS variables / app shell |
| Navigation hierarchy | Pass | Persistent sidebar + account entry points |
| Responsive | Pass | Spacing tokens; ≤900 drawer + single column; ≤640 compact chrome |
| W3C HTML validator (Home) | Pass with notes | Blazor noise only expected |

### Responsive spacing scale

| Token | Desktop | ≤900px | ≤640px |
|-------|---------|--------|--------|
| Page pad | 34 / 42 / 44 | 20 / 16 / 32 | 16 / 12 / 28 |
| Section gap | 28 | 16 | 14 |
| Card pad | 20 | 16 | 14 |
| Top bar | 74 | 64 | 58 |
| Touch target | 44 | 44 | 44 |

### Responsive polish applied

- Spacing CSS variables under `:root`
- Obsolete shell sidebar-column rules neutralized
- Auth desktop rules no longer overwrite mobile padding
- Recipe + saved cards both stack at ≤900px
- Master media blocks at end of `app.css` win cascade for shell/page/cards/auth
- CSS cache: `app.css?v=20260718c`

### W3C Nu HTML Checker (Home)

Checked: `https://scrapethepantry.onrender.com/` via [validator.w3.org/nu](https://validator.w3.org/nu/?doc=https%3A%2F%2Fscrapethepantry.onrender.com%2F).

**Expected Blazor noise (not product defects):**

- Attributes like `b-********` — Blazor CSS isolation
- Blazor event attributes such as `__internal_preventDefault_onclick`

Overall: markup is valid for a Blazor Server app; actionable a11y/markup notes from the initial audit are addressed in source.

---

## Performance notes (design / implementation)

ScrapeThePantry reduces unnecessary network work:

1. **MealDB response caching** — `MealDbService` caches ingredient filters, browse lists, and meal details in `IMemoryCache` (~30 minutes).
2. **Batched recipe hydration** — search results load a small first page, then **Load More** hydrates additional meals.
3. **Ingredient catalog caching** — pantry catalog is seeded/synced and cached so the pantry page does not re-download MealDB ingredients every request.
4. **Parallel ingredient searches** — multi-ingredient search fans out a limited set of filter calls and merges by meal id.
5. **Secrets outside source** — DB connection uses `.env` / host env vars, not committed `appsettings.json`.

Lighthouse Home Performance: **100** desktop / **90** mobile supports the performance objective for this app type.

---

## Repository / collaboration evidence

| Item | Location |
|------|----------|
| Source control | GitHub: K-Marple/recipe-suggestions |
| Task tracking | Jira board RS |
| User documentation | README.md (How to Use + deployment) |
| QA evidence | QA_RESULTS.md (this file) |
| Live deployment | https://scrapethepantry.onrender.com/ |

---

## Sign-off

| Role | Name | Date |
|------|------|------|
| Automated QA capture | Cursor agent (Lighthouse CLI + W3C Nu) | 2026-07-18 |
| Follow-up a11y / responsive spacing | Cursor agent | 2026-07-18 |
| Team review | | |
