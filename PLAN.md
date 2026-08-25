# BookApp — Angular 20 + .NET 9 take-home test plan

## Context

This is a timed (2-week) take-home coding test for an internship application, not a freeform product build. The brief (pasted by the user, in Swedish) fixes almost everything: a responsive CRUD web app — Angular 20 frontend, .NET 9 C# REST API backend — with JWT-based auth, a Books CRUD screen, a "My quotes" screen (add/edit/delete, list of favorite quotes), Bootstrap + Font Awesome styling, full responsive behavior, an optional light/dark toggle, and a submission consisting of a live deployed link + GitHub repo link.

The user already scaffolded `book-quote-app/BookApp-api/` following the `Blotz-Task-App` backend's modular-CQRS convention (`Modules/<Feature>/{Commands,Queries,Controllers,Domain,DTOs}` + `Extension/`, `Infrastructure/Data/`, `Middleware/`, `Shared/`) and confirmed (this session) they want to keep that pattern. Two things in the existing scaffold need correcting against the brief:
- `BookApp-api.csproj` targets `net10.0` — the brief requires **.NET 9**. Both SDKs are installed locally (`9.0.304`, `10.0.201`), so retargeting is safe.
- The csproj already references `MediatR`, but Blotz's actual pattern (confirmed by reading `Modules/Referrals/*`, `Modules/Tasks/DependencyInjection.cs`) does **not** use MediatR — handlers are plain classes, manually registered in each module's `DependencyInjection.cs`, and called directly from thin controllers (`handler.Handle(command, ct)`). Since the user asked to mirror Blotz's actual pattern, MediatR should be dropped to avoid a second, inconsistent dispatch style.
- EF Core provider is already `Microsoft.EntityFrameworkCore.Sqlite` — confirmed as the right call for this plan (zero external DB service to provision, works locally and in most free-tier hosts, matches the 2-week deadline).

Angular project doesn't exist yet (the `bookapp-client` folder was proposed then explicitly not created this session) — it needs to be scaffolded from scratch with `ng new`.

## Backend plan (`BookApp-api/`)

**Entities / modules** (mirroring the `Modules/<Feature>/{Commands,Queries,Controllers,Domain,DTOs}` layout already scaffolded):

- `Modules/Users` — `AppUser` (Id, Username, PasswordHash, CreatedAtUtc). Commands: `RegisterUser`, `LoginUser` (verifies BCrypt hash, issues JWT). No Queries needed for MVP.
- `Modules/Books` — `Book` (Id, Title, Author, PublishedDate). Global list, not user-scoped, but every CRUD endpoint requires a valid JWT (per brief: "only authenticated users can access CRUD operations"). Commands: `AddBook`, `UpdateBook`, `DeleteBook`. Queries: `GetAllBooks`, `GetBookById`.
- `Modules/Quotes` — `Quote` (Id, UserId FK, Text, CreatedAtUtc). Personal to the logged-in user ("Mina citat"/"My quotes" is not a global list). Commands: `AddQuote`, `UpdateQuote`, `DeleteQuote`. Queries: `GetMyQuotes` (filtered by the JWT's user id).

**Auth wiring** (`Program.cs`, modeled on `blotztask-api/Program.cs`'s extension-method style, but self-issued JWT instead of Auth0):
- `Extension/AuthServiceExtensions.cs` — `AddJwtAuthentication(config)`: configures `AddAuthentication().AddJwtBearer(...)` with a signing key from config (`appsettings.Development.json`, not committed with a real secret).
- `Extension/DatabaseServiceExtensions.cs` — `AddDatabaseContext(config)`: registers `BookAppDbContext` with the SQLite connection string.
- `Middleware/ErrorHandlingMiddleware.cs` — same shape as Blotz's (catch `NotFoundException`/`UnauthorizedAccessException`/`DbUpdateException`/generic → consistent JSON error body), since the brief expects real error handling around auth failures.
- `Program.cs` — same shape as Blotz's: `builder.Services.AddDatabaseContext(...)`, `.AddJwtAuthentication(...)`, `.AddUserModule()`, `.AddBookModule()`, `.AddQuoteModule()`; pipeline: `UseMiddleware<ErrorHandlingMiddleware>()` → `UseAuthentication()` → `UseAuthorization()` → `MapControllers()`.

**Data**: `Infrastructure/Data/BookAppDbContext.cs` + `Infrastructure/Data/Configurations/{AppUserConfiguration,BookConfiguration,QuoteConfiguration}.cs` (Fluent API, one file per entity, matching Blotz's `Infrastructure/Data/Configurations/ReferralCodeConfiguration.cs` style). One initial EF Core migration (`dotnet ef migrations add InitialCreate`) against SQLite.

## Frontend plan (new `book-quote-app/bookapp-client/`)

Scaffold with `npx @angular/cli@20 new bookapp-client --routing --style=scss` (already verified `npx @angular/cli@20` works in this environment). Add `bootstrap` + `@fortawesome/fontawesome-free` via npm, import in `angular.json` styles array (no separate Bootstrap JS framework needed beyond what components require — a Bootstrap navbar's collapse behavior needs `bootstrap`'s JS bundle too, so include both `bootstrap.min.css` and `bootstrap.bundle.min.js`).

**Structure** (standalone components, Angular 20 default):
- `core/auth/` — `auth.service.ts` (login/register calls, stores JWT in `localStorage`), `auth.interceptor.ts` (attaches `Authorization: Bearer <token>` to outgoing requests), `auth.guard.ts` (route guard redirecting to `/login` when no token).
- `features/auth/` — `login.component.ts`, `register.component.ts`.
- `features/books/` — `book-list.component.ts` (home page), `book-form.component.ts` (shared add/edit form, route param decides mode), `book.service.ts`.
- `features/quotes/` — `quote-list.component.ts` ("My quotes" view — list + inline add/edit/delete), `quote.service.ts`.
- `shared/nav/nav.component.ts` — top navbar (Bootstrap `navbar-expand-lg` + `navbar-toggler` for the required mobile collapse), links to Books/Quotes, login/logout state, the light/dark toggle (toggles a `data-bs-theme` attribute on `<html>`, which Bootstrap 5.3+ reads natively — no custom CSS variables needed).
- `app.routes.ts` — `/login`, `/register`, `''` (books list, guarded), `/books/new`, `/books/:id/edit`, `/quotes` (guarded).

Responsive verification is manual (brief explicitly asks to resize the browser and test breakpoints) — no automated viewport tests needed for this scope.

## Deployment

- Frontend: `ng build` → deploy `dist/bookapp-client` to Netlify or Vercel (either's free static-site tier is a one-command CLI deploy; Azure Static Web Apps is heavier to wire up solo in this timeframe, so only fall back to it if Netlify/Vercel deploy hits a blocker).
- Backend: the brief doesn't mandate a specific backend host. Recommend **Azure App Service (free F1 tier)** — the user already has Azure familiarity from Blotz's infra, and a self-issued-JWT + SQLite API deploys there without needing a managed DB service. Render/Fly.io are viable fallbacks if Azure App Service's free tier has cold-start or file-persistence issues with SQLite (SQLite on App Service needs the file on persistent storage, not the ephemeral `/tmp`-style path some PaaS use — worth a quick smoke test before committing to it for the final submission).
- Both API base URL (frontend `environment.prod.ts`) and CORS (backend, `AllowSpecificOrigin`-style policy matching Blotz's `CorsServiceExtensions.cs`) need updating once both are deployed.

## Suggested 2-week order of work

1. Backend: entities + migrations + plain CRUD (no auth yet) for Books and Quotes — get `dotnet run` + Swagger working end to end.
2. Backend: JWT auth (register/login/BCrypt/token issuance/`[Authorize]` on Book & Quote controllers).
3. Frontend: scaffold + routing shell + Bootstrap/Font Awesome wired + nav.
4. Frontend: Books list/add/edit/delete wired to the real API.
5. Frontend: auth (login/register pages, interceptor, guard) wired to the real API.
6. Frontend: Quotes view wired to the real API (now behind auth).
7. Responsive pass + light/dark toggle (the explicit "additional challenge").
8. Deploy both, wire prod API URL + CORS, smoke-test the live links, push to GitHub.

Steps 1–2 and 3 can happen in parallel since the frontend shell doesn't need the API yet.

## Verification

- Backend: `dotnet build` after every module lands; `dotnet run` + hit endpoints via the generated Swagger UI (`/swagger`) or the `.http` file already in the project, including a manual JWT flow (register → login → copy token → call a protected Book endpoint with/without the header to confirm 401 vs 200).
- Frontend: `ng serve` locally against the local API; manually walk the exact user flows the brief describes (add book → redirected to list → edit → delete; add/edit/delete a quote; resize the browser at each breakpoint; toggle dark mode).
- Before submission: open both deployed URLs fresh (private/incognito window) and redo the same manual walkthrough against production, since that's what the grader will actually click through.
