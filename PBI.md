# Product Backlog

## Product Goal

Build and deploy a responsive full-stack application where authenticated users can browse and manage books and maintain their own collection of favorite quotes.

## Definition of Done

- The implementation follows the existing modular backend structure.
- The API and Angular client build without errors.
- Acceptance criteria are verified locally through the UI or API.
- No secrets, build artifacts, or dependency folders are committed.
- The feature is documented and committed to the `main` branch.

## PBI-001: Establish the Backend Data Layer

**Priority:** High
**Estimate:** 4 points
**Dependencies:** None

## Description

As a developer, I want a configured SQLite database and domain model so that the application can persist users, books, and quotes.

## Acceptance Criteria

- [ ] `BookAppDbContext` is registered with the `DefaultConnection` SQLite connection string.
- [ ] `AppUser` contains `Id` (`int` primary key), `Username` (required, unique, max 50), `PasswordHash` (required), and `CreatedAtUtc` (required UTC timestamp).
- [ ] `Book` contains `Id` (`int` primary key), `Title` (required, max 200), `Author` (required, max 150), and `PublishedDate` (required `DateOnly`).
- [ ] `Quote` contains `Id` (`int` primary key), `UserId` (required foreign key), `Text` (required, max 2,000), and `CreatedAtUtc` (required UTC timestamp).
- [ ] Each entity has a Fluent API configuration under `Infrastructure/Data/Configurations`.
- [ ] `Username` has a unique database index.
- [ ] `AppUser` to `Quote` is one-to-many; deleting a user deletes that user's quotes.
- [ ] An `InitialCreate` migration is created under `Infrastructure/Data/Migrations`.
- [ ] `dotnet ef database update` creates the database and all three tables without errors.
- [ ] `dotnet build` succeeds.

## Suggested Solution

Implement the data layer in `BookApp-api/Infrastructure/Data`. Keep domain entities in their feature modules, use `IEntityTypeConfiguration<T>` for constraints, and let EF Core create the SQLite schema from the model. The first action is to add `BookAppDbContext` and the three entity configurations.

## Scope / Tasks

- [ ] Add `BookAppDbContext` with `DbSet<AppUser>`, `DbSet<Book>`, and `DbSet<Quote>`.
- [ ] Configure keys, required fields, lengths, indexes, date/time handling, and the user-to-quote relationship.
- [ ] Add `ConnectionStrings:DefaultConnection` using a local `bookapp.db` file.
- [ ] Keep local database files ignored by Git and do not add credentials to configuration.
- [ ] Create and apply the `InitialCreate` migration.

## Notes

```bash
cd BookApp-api
dotnet ef migrations add InitialCreate
dotnet ef database update
dotnet build
```

## PBI-002: Implement User Registration and JWT Login

**Priority:** High
**Estimate:** 8 points
**Dependencies:** PBI-001

## Description

As a visitor, I want to register and log in securely so that I can access protected application features.

## Acceptance Criteria

- [x] `POST /api/auth/register` accepts a username and password and returns HTTP 201 for a new user.
- [x] Passwords are stored only as BCrypt hashes.
- [x] Duplicate usernames return HTTP 409 with a stable error response.
- [x] `POST /api/auth/login` returns HTTP 200 and a signed JWT for valid credentials.
- [x] The JWT contains the authenticated user's ID in a documented claim.
- [x] Invalid credentials return HTTP 401 without revealing whether the username exists.
- [x] JWT issuer, audience, expiry, and signing key are loaded from configuration.

## Suggested Solution

Implement `RegisterUser` and `LoginUser` inside `Modules/Users`, with thin controllers delegating to handlers. Hash passwords with `BCrypt.Net-Next`, issue HMAC-signed bearer tokens from a dedicated token service, and validate the same issuer, audience, and signing key in the JWT middleware. The first action is to define request/response DTOs and the auth configuration contract.

## Scope / Tasks

- [x] Add request and response DTOs with username/password validation.
- [x] Add registration and login commands, handlers, and `AuthController`.
- [x] Add BCrypt password hashing and verification.
- [x] Add a token service that creates tokens with user ID and username claims.
- [x] Add JWT bearer authentication and `UseAuthentication()` before authorization.
- [x] Add consistent 400, 401, and 409 error responses.

## Notes

- Register a new user.
- Log in with valid credentials and receive a token.
- Verify invalid credentials return 401.
- Verify the password hash is not the plain-text password.

## PBI-003: Implement Authenticated Books CRUD

**Priority:** High
**Estimate:** 8 points
**Dependencies:** PBI-001 and PBI-002

## Description

As an authenticated user, I want to create, view, edit, and delete books so that the application maintains a useful book catalog.

## Acceptance Criteria

- [ ] `GET /api/books` returns all books for an authenticated request.
- [ ] `GET /api/books/{id}` returns one book or HTTP 404.
- [ ] `POST /api/books` creates a book and returns HTTP 201.
- [ ] `PUT /api/books/{id}` updates a book and returns HTTP 204 or the updated resource.
- [ ] `DELETE /api/books/{id}` deletes a book and returns HTTP 204.
- [ ] Every books endpoint requires `[Authorize]`; missing or invalid JWT returns HTTP 401.
- [ ] Title, author, and published date are validated against PBI-001 constraints.
- [ ] Missing books return HTTP 404 and invalid payloads return HTTP 400.

## Suggested Solution

Implement the Books module with one handler per command/query and a thin controller under `Modules/Books/Controllers`. Use DTOs at the HTTP boundary, async EF Core queries with cancellation tokens, and `[Authorize]` on the controller. The first action is to define the book request/response DTOs and endpoint contract.

## Scope / Tasks

- [ ] Add `AddBook`, `UpdateBook`, and `DeleteBook` commands and handlers.
- [ ] Add `GetAllBooks` and `GetBookById` queries and handlers.
- [ ] Add request/response DTOs and model validation.
- [ ] Add an authorized `BooksController` with the five endpoints above.
- [ ] Register the Books module services.
- [ ] Return appropriate HTTP status codes and response payloads.

## Notes

- Test every endpoint with and without a valid JWT.
- Verify the complete add, list, edit, and delete flow through Swagger or an HTTP client.

## PBI-004: Implement Personal Quotes Management

**Priority:** High
**Estimate:** 8 points
**Dependencies:** PBI-001 and PBI-002

## Description

As an authenticated user, I want to manage my favorite quotes so that my saved quotes remain private and available to me.

## Acceptance Criteria

- [ ] `GET /api/quotes` returns only quotes belonging to the authenticated user.
- [ ] `POST /api/quotes` creates a quote using the user ID from JWT claims.
- [ ] `PUT /api/quotes/{id}` updates only a quote owned by the current user.
- [ ] `DELETE /api/quotes/{id}` deletes only a quote owned by the current user.
- [ ] Unauthenticated requests return HTTP 401.
- [ ] Another user's quote is never returned and cannot be modified or deleted.
- [ ] Empty or overlong quote text returns HTTP 400.

## Suggested Solution

Implement the Quotes module with ownership enforced in every handler query, not only in the controller. Resolve the user ID from the validated `NameIdentifier` claim and never accept `UserId` from a client DTO. The first action is to define the ownership-aware query and command contracts.

## Scope / Tasks

- [ ] Add the Quote entity, DTOs, commands, queries, handlers, and controller.
- [ ] Resolve the user ID from the JWT `NameIdentifier` claim rather than request input.
- [ ] Enforce ownership checks in list, update, and delete operations.
- [ ] Return 404 for a quote that does not belong to the current user.
- [ ] Register the Quotes module services.

## Notes

- Create two users and confirm that each account sees only its own quotes.
- Test unauthorized, missing-resource, and ownership-failure cases.

## PBI-005: Build the Angular Application Shell

**Priority:** High
**Estimate:** 4 points
**Dependencies:** None

## Description

As a user, I want a clear responsive application shell so that I can navigate between books, quotes, and authentication screens on desktop and mobile.

## Acceptance Criteria

- [ ] Routes exist for `/login`, `/register`, `/`, `/books/new`, `/books/:id/edit`, and `/quotes`.
- [ ] Protected routes redirect unauthenticated users to `/login`.
- [ ] The navigation shows Books and My Quotes only for authenticated users.
- [ ] The navigation includes a mobile collapse button with an accessible label.
- [ ] Bootstrap CSS, Bootstrap bundle JS, and Font Awesome are loaded consistently.

## Suggested Solution

Build the shell with standalone Angular components and the existing Angular 22 project conventions. Put shared navigation in `src/app/shared/nav`, route protection in `src/app/core/auth`, and feature pages under `src/app/features`. The first action is to install Bootstrap and Font Awesome and define the route tree.

## Scope / Tasks

- [ ] Add the shared navigation component.
- [ ] Add route configuration, auth guard, and placeholder feature components.
- [ ] Add Bootstrap and Font Awesome dependencies, CSS, and bundle JS configuration.
- [ ] Add a consistent page layout and responsive base styles.
- [ ] Ensure all navigation controls are keyboard accessible.

## Notes

- Run `npm start` and manually check every route.
- Resize the browser and verify the navigation remains usable.

## PBI-006: Connect Authentication to the API

**Priority:** High
**Estimate:** 8 points
**Dependencies:** PBI-002 and PBI-005

## Description

As a user, I want to register, log in, and log out from the Angular client so that my session controls access to protected features.

## Acceptance Criteria

- [ ] Registration and login forms validate required input before sending requests.
- [ ] Successful login stores the JWT and exposes the authenticated state.
- [ ] The HTTP interceptor adds `Authorization: Bearer <token>` to API requests only when a token exists.
- [ ] Logout removes the token and returns the user to `/login`.
- [ ] HTTP 401 responses clear the invalid token and redirect to `/login`.
- [ ] Password values are never logged or displayed after submission.

## Suggested Solution

Implement an `AuthService`, functional interceptor, and functional route guard under `src/app/core/auth`. Store only the access token in `localStorage`, centralize the API base URL, and keep navigation state derived from the token. The first action is to define the auth service methods and API DTOs.

## Scope / Tasks

- [ ] Add the auth service and API models.
- [ ] Add login and registration components.
- [ ] Add the JWT interceptor and route guard.
- [ ] Add user-facing loading, validation, and API error states.
- [ ] Add logout behavior to the shared navigation.

## Notes

- Complete registration, login, logout, and failed-login flows in the browser.
- Confirm protected API calls include the bearer token.

## PBI-007: Connect Books and Quotes Features to the API

**Priority:** High
**Estimate:** 8 points
**Dependencies:** PBI-003, PBI-004, and PBI-006

## Description

As an authenticated user, I want to manage books and my quotes from the Angular UI so that I can use the complete application without calling the API directly.

## Acceptance Criteria

- [ ] The books screen lists books returned by `GET /api/books`.
- [ ] A user can add, edit, and delete a book from the UI.
- [ ] The quotes screen lists only the logged-in user's quotes.
- [ ] A user can add, edit, and delete a quote from the UI.
- [ ] Success, loading, empty, validation, and API error states are visible and usable.
- [ ] The UI refreshes or updates after each successful mutation.
- [ ] API errors are mapped to user-readable messages without exposing stack traces.

## Suggested Solution

Implement typed Angular services for the agreed API contracts and keep components responsible for presentation and user interaction. Use reactive forms for book and auth validation, refresh list data after mutations, and route back to the list after successful create/edit. The first action is to add the book service and connect the books list to the API.

## Scope / Tasks

- [ ] Add book and quote services with typed request/response models.
- [ ] Add the book list and shared add/edit form components.
- [ ] Add the personal quotes component with create/edit/delete interactions.
- [ ] Add API base URL configuration for local and production environments.
- [ ] Handle loading, empty, validation, success, and API error states.

## Notes

- Walk through the complete book CRUD flow.
- Walk through the complete quote CRUD flow.
- Confirm a second user cannot see the first user's quotes.

## PBI-008: Complete Responsive Design and Theme Support

**Priority:** Medium
**Estimate:** 4 points
**Dependencies:** PBI-007

## Description

As a user, I want the application to work comfortably on different screen sizes and optionally switch themes.

## Acceptance Criteria

- [ ] Books, quotes, forms, navigation, and feedback states work at mobile, tablet, and desktop widths.
- [ ] No primary action or content is clipped or overlaps another element.
- [ ] The navbar collapse works with keyboard and touch input.
- [ ] The theme toggle updates `data-bs-theme` on the document root.
- [ ] The selected theme persists after route changes and page reload.

## Suggested Solution

Use Bootstrap responsive utilities and a small set of app-level SCSS rules rather than duplicating component-specific breakpoints. Persist the theme preference in `localStorage` and apply it before the first rendered view when possible. The first action is to test the existing shell at mobile, tablet, and desktop widths and record layout defects.

## Scope / Tasks

- [ ] Review layouts at mobile, tablet, and desktop breakpoints.
- [ ] Add the Bootstrap light/dark theme toggle.
- [ ] Persist and restore the selected theme.
- [ ] Fix spacing, typography, focus states, and empty/error states.
- [ ] Verify keyboard focus is visible for all interactive controls.

## Notes

- Manually test representative mobile and desktop viewport sizes.
- Test keyboard navigation and theme switching.

## PBI-009: Prepare Deployment and Submission

**Priority:** Medium
**Estimate:** 8 points
**Dependencies:** PBI-007 and PBI-008

## Description

As a reviewer, I want accessible deployed frontend and backend applications so that I can evaluate the project from the submitted links.

## Acceptance Criteria

- [ ] The production Angular build completes successfully.
- [ ] The API is deployed and reachable over HTTPS.
- [ ] CORS allows only the deployed frontend origin.
- [ ] Production configuration contains no development secrets.
- [ ] The deployed frontend uses the deployed API URL.
- [ ] SQLite uses persistent storage, or the deployment uses an appropriate production database.
- [ ] The README contains setup instructions and the live URLs.
- [ ] A fresh-browser smoke test passes for registration, login, book CRUD, and quote CRUD.

## Suggested Solution

Deploy the Angular static build to Netlify or Vercel and deploy the ASP.NET Core API to a host with HTTPS and persistent database storage. Configure production API URL and CORS through environment-specific settings, then run the same user journey in a private browser window. The first action is to produce a clean production build and document the required environment variables.

## Scope / Tasks

- [ ] Configure production API settings and restrictive CORS.
- [ ] Document required production environment variables and secrets.
- [ ] Deploy the frontend to a static hosting provider.
- [ ] Deploy the API with persistent SQLite storage or an appropriate production database.
- [ ] Run the production smoke test in a fresh browser session.
- [ ] Update the README with deployment details and live URLs.

## Notes

```bash
cd BookApp-client
npm run build

cd ../BookApp-api
dotnet build
```

## Recommended Delivery Order

1. PBI-001: Backend data layer
2. PBI-005: Angular application shell
3. PBI-002: Authentication
4. PBI-003: Books CRUD
5. PBI-004: Personal quotes
6. PBI-006: Client authentication
7. PBI-007: Client API integration
8. PBI-008: Responsive design and themes
9. PBI-009: Deployment and submission
