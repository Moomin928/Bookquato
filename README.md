# Book Quote App

Book Quote App is a responsive full-stack web application for managing books and personal favorite quotes. It is being developed as a two-week take-home project with an Angular frontend and a .NET REST API backend.

## Planned Features

- User registration and login with JWT authentication
- Password hashing with BCrypt
- Authenticated CRUD operations for books
- Personal quote management: add, edit, delete, and list quotes
- SQLite persistence through Entity Framework Core
- Responsive UI built with Bootstrap and Font Awesome
- Optional light and dark theme support

## Technology Stack

### Frontend

- Angular 22
- TypeScript
- SCSS
- Angular Router

### Backend

- ASP.NET Core 9 Web API
- Entity Framework Core with SQLite
- MediatR for application request handling
- JWT Bearer authentication
- BCrypt.Net-Next for password hashing
- Swagger/OpenAPI

## Project Structure

```text
BookApp-api/
  Modules/
    Books/
    Quotes/
    Users/
  Infrastructure/
  Middleware/
  Extension/
BookApp-client/
  src/
book-quote-app.slnx
```

## Getting Started

### Prerequisites

- .NET 9 SDK
- Node.js and npm

### Run the API

```bash
cd BookApp-api
dotnet restore
dotnet run
```

### Run the Angular client

```bash
cd BookApp-client
npm install
npm start
```

The Angular development server runs at `http://localhost:4200` by default.

## Development Status

The project currently contains the initial Angular and ASP.NET Core scaffolding, the modular backend structure, and the core package setup. Books, quotes, authentication, database migrations, and the connected frontend workflows are planned next.

## License

This project is intended for portfolio and educational purposes.
