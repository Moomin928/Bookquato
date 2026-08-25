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
**Estimate:** 3 points  
**Dependencies:** None

### User Story

As a developer, I want a configured SQLite database and domain model so that the application can persist users, books, and quotes.

### Acceptance Criteria

- `BookAppDbContext` is registered with the SQLite connection string.
- `AppUser`, `Book`, and `Quote` entities have the required properties and relationships.
- Entity configurations use the existing `Infrastructure/Data/Configurations` structure.
- An initial EF Core migration is created successfully.
- The database can be created or updated locally without errors.

### Implementation Tasks

- Add `BookAppDbContext`.
- Configure keys, required fields, lengths, indexes, and the user-to-quote relationship.
- Add the SQLite connection string to local configuration without committing secrets.
- Create and apply the `InitialCreate` migration.

### Verification

```bash
cd BookApp-api
dotnet ef migrations add InitialCreate
dotnet ef database update
dotnet build
```

## PBI-002: Implement User Registration and JWT Login

**Priority:** High  
**Estimate:** 5 points  
**Dependencies:** PBI-001

### User Story

As a visitor, I want to register and log in securely so that I can access protected application features.

### Acceptance Criteria

- A user can register with a unique username and password.
- Passwords are stored only as BCrypt hashes.
- Duplicate usernames return a clear client error.
- A valid login returns a signed JWT containing the user identity.
- Invalid credentials return HTTP 401 without revealing sensitive details.
- JWT configuration is loaded from application configuration and is not hard-coded.

### Implementation Tasks

- Add registration and login commands, DTOs, handlers, and controllers.
- Add BCrypt password hashing and verification.
- Add JWT bearer authentication configuration.
- Add authentication service registration and token creation.
- Add consistent error handling for validation and authentication failures.

### Verification

- Register a new user.
- Log in with valid credentials and receive a token.
- Verify invalid credentials return 401.
- Verify the password hash is not the plain-text password.

## PBI-003: Implement Authenticated Books CRUD

**Priority:** High  
**Estimate:** 5 points  
**Dependencies:** PBI-001 and PBI-002

### User Story

As an authenticated user, I want to create, view, edit, and delete books so that the application maintains a useful book catalog.

### Acceptance Criteria

- Unauthenticated requests to book CRUD endpoints return HTTP 401.
- Authenticated users can list all books.
- Authenticated users can retrieve a book by ID.
- Authenticated users can add, update, and delete books.
- Required fields are validated.
- Missing books return HTTP 404.

### Implementation Tasks

- Add book commands, queries, handlers, DTOs, and controllers.
- Protect endpoints with `[Authorize]`.
- Register the Books module services.
- Return appropriate HTTP status codes and response payloads.

### Verification

- Test every endpoint with and without a valid JWT.
- Verify the complete add, list, edit, and delete flow through Swagger or an HTTP client.

## PBI-004: Implement Personal Quotes Management

**Priority:** High  
**Estimate:** 5 points  
**Dependencies:** PBI-001 and PBI-002

### User Story

As an authenticated user, I want to manage my favorite quotes so that my saved quotes remain private and available to me.

### Acceptance Criteria

- Unauthenticated requests to quote endpoints return HTTP 401.
- A user can list only their own quotes.
- A user can add a quote associated with their authenticated user ID.
- A user can edit and delete only their own quotes.
- A user cannot read, edit, or delete another user's quote.
- Empty quote text is rejected with a validation error.

### Implementation Tasks

- Add the Quote entity, DTOs, commands, queries, handlers, and controller.
- Resolve the user ID from JWT claims rather than request input.
- Enforce ownership checks in update, delete, and list operations.
- Register the Quotes module services.

### Verification

- Create two users and confirm that each account sees only its own quotes.
- Test unauthorized, missing-resource, and ownership-failure cases.

## PBI-005: Build the Angular Application Shell

**Priority:** High  
**Estimate:** 3 points  
**Dependencies:** None

### User Story

As a user, I want a clear responsive application shell so that I can navigate between books, quotes, and authentication screens on desktop and mobile.

### Acceptance Criteria

- Routes exist for login, registration, books, book creation, book editing, and quotes.
- Protected routes redirect unauthenticated users to login.
- The navigation shows links appropriate to the current authentication state.
- The navigation collapses correctly at mobile breakpoints.
- Bootstrap and Font Awesome are loaded consistently.

### Implementation Tasks

- Add the shared navigation component.
- Add route configuration, auth guard, and placeholder feature components.
- Add Bootstrap and Font Awesome dependencies and styles.
- Add a consistent page layout and responsive base styles.

### Verification

- Run `npm start` and manually check every route.
- Resize the browser and verify the navigation remains usable.

## PBI-006: Connect Authentication to the API

**Priority:** High  
**Estimate:** 5 points  
**Dependencies:** PBI-002 and PBI-005

### User Story

As a user, I want to register, log in, and log out from the Angular client so that my session controls access to protected features.

### Acceptance Criteria

- Registration and login forms validate required input.
- Successful login stores the JWT locally.
- The HTTP interceptor attaches the JWT to protected API requests.
- Logout removes the stored token and returns the user to login.
- Expired or rejected tokens redirect the user to login.

### Implementation Tasks

- Add the auth service and API models.
- Add login and registration components.
- Add the JWT interceptor and route guard.
- Add user-facing loading and error states.

### Verification

- Complete registration, login, logout, and failed-login flows in the browser.
- Confirm protected API calls include the bearer token.

## PBI-007: Connect Books and Quotes Features to the API

**Priority:** High  
**Estimate:** 8 points  
**Dependencies:** PBI-003, PBI-004, and PBI-006

### User Story

As an authenticated user, I want to manage books and my quotes from the Angular UI so that I can use the complete application without calling the API directly.

### Acceptance Criteria

- The books screen lists books returned by the API.
- A user can add, edit, and delete a book from the UI.
- The quotes screen lists only the logged-in user's quotes.
- A user can add, edit, and delete a quote from the UI.
- Success, loading, empty, validation, and API error states are visible and usable.
- The UI refreshes or updates after each successful mutation.

### Implementation Tasks

- Add book and quote services.
- Add book list and shared add/edit form components.
- Add the personal quotes component with create/edit/delete interactions.
- Add API base URL configuration for local and production environments.

### Verification

- Walk through the complete book CRUD flow.
- Walk through the complete quote CRUD flow.
- Confirm a second user cannot see the first user's quotes.

## PBI-008: Complete Responsive Design and Theme Support

**Priority:** Medium  
**Estimate:** 3 points  
**Dependencies:** PBI-007

### User Story

As a user, I want the application to work comfortably on different screen sizes and optionally switch themes.

### Acceptance Criteria

- Books, quotes, forms, navigation, and feedback states work at mobile and desktop widths.
- No primary action or content is clipped or overlaps another element.
- The navbar collapse works with keyboard and touch input.
- The theme toggle updates the application consistently and persists during navigation.

### Implementation Tasks

- Review layouts at mobile, tablet, and desktop breakpoints.
- Add the Bootstrap light/dark theme toggle.
- Fix spacing, typography, focus states, and empty/error states.

### Verification

- Manually test representative mobile and desktop viewport sizes.
- Test keyboard navigation and theme switching.

## PBI-009: Prepare Deployment and Submission

**Priority:** Medium  
**Estimate:** 5 points  
**Dependencies:** PBI-007 and PBI-008

### User Story

As a reviewer, I want accessible deployed frontend and backend applications so that I can evaluate the project from the submitted links.

### Acceptance Criteria

- The production Angular build completes successfully.
- The API is deployed and reachable over HTTPS.
- CORS allows only the deployed frontend origin.
- Production configuration contains no development secrets.
- The deployed frontend uses the deployed API URL.
- The README contains setup instructions and the live URLs when available.
- A fresh-browser smoke test passes for registration, login, book CRUD, and quote CRUD.

### Implementation Tasks

- Configure production API settings and CORS.
- Deploy the frontend to a static hosting provider.
- Deploy the API with persistent SQLite storage or an appropriate production database.
- Run the production smoke test.
- Update the README with deployment details.

### Verification

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