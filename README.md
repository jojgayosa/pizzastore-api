# Pizza Store API

A RESTful backend API for managing pizzas and toppings, built with C# and ASP.NET Core 8.

## Technologies

* C#
* ASP.NET Core 8 Web API
* Entity Framework Core 8
* PostgreSQL
* FluentValidation
* Swagger / OpenAPI
* Clean Architecture
* Vertical Slice Architecture

## Project Structure

```text
PizzaStore
├── PizzaStore.API
├── PizzaStore.Application
├── PizzaStore.Domain
├── PizzaStore.Infrastructure
```

### Architecture

The project follows a Clean Architecture approach with feature-based (vertical slice) organization.

* **API** – HTTP/API concerns, controllers, middleware, and exception handling.
* **Application** – Business use cases, commands, queries, validators, handlers, and application interfaces.
* **Domain** – Core business entities and domain behavior.
* **Infrastructure** – Entity Framework Core, PostgreSQL, database configuration, and persistence.

Application use cases are organized by feature rather than by technical layer. Each use case contains its command/query, validator, and handler where applicable.

## Features

### Pizza

* Create a pizza
* Get all pizzas
* Get a pizza by ID
* Update pizza details
* Update pizza toppings
* Delete a pizza
* Many-to-many relationship between pizzas and toppings

### Topping

* Create a topping
* Get all toppings
* Get a topping by ID
* Update a topping
* Delete a topping
* Prevent duplicate topping names
* Prevent deletion of toppings currently used by pizzas

### Validation and Error Handling

The API uses FluentValidation for request validation and a global exception handler for consistent HTTP responses.

The API handles:

* `400 Bad Request` – Validation or invalid request
* `404 Not Found` – Requested resource does not exist
* `409 Conflict` – Duplicate or conflicting resource
* `500 Internal Server Error` – Unexpected server errors

Responses use the `ProblemDetails` format where applicable.

## Requirements

Before running the application, install:

* .NET 8 SDK
* PostgreSQL
* Visual Studio 2022/2026 or another compatible .NET IDE

## Database Configuration

Update the PostgreSQL connection string in:

```text
PizzaStore.API/appsettings.json
```

Example:

```json
{
  "ConnectionStrings": {
    "DefaultConnection": "Host=localhost;Port=5432;Database=pizzastore;Username=postgres;Password=YOUR_PASSWORD"
  }
}
```

Replace `YOUR_PASSWORD` with the password for your local PostgreSQL installation.


## Database Setup

The project includes Entity Framework Core migrations.

From Visual Studio Package Manager Console:

```powershell
Update-Database -Project PizzaStore.Infrastructure -StartupProject PizzaStore.API
```

Or using the .NET CLI:

```bash
dotnet ef database update --project PizzaStore.Infrastructure --startup-project PizzaStore.API
```

This will create/update the `pizzastore` database using the existing EF Core migrations.

## Running the API

Open the solution:

```text
PizzaStore.sln
```

Set `PizzaStore.API` as the startup project and run the application.

Alternatively:

```bash
dotnet run --project PizzaStore.API
```

## Swagger

Once the API is running, open the Swagger UI using the HTTPS URL shown by the application, for example:

```text
https://localhost:<port>/swagger
```

Swagger provides an interactive interface for testing all API endpoints.

## API Endpoints

### Toppings

| Method | Endpoint            | Description         |
| ------ | ------------------- | ------------------- |
| POST   | `/api/topping`      | Create a topping    |
| GET    | `/api/topping`      | Get all toppings    |
| GET    | `/api/topping/{id}` | Get a topping by ID |
| PUT    | `/api/topping/{id}` | Update a topping    |
| DELETE | `/api/topping/{id}` | Delete a topping    |

### Pizzas

| Method | Endpoint                   | Description           |
| ------ | -------------------------- | --------------------- |
| POST   | `/api/pizza`               | Create a pizza        |
| GET    | `/api/pizza`               | Get all pizzas        |
| GET    | `/api/pizza/{id}`          | Get a pizza by ID     |
| PUT    | `/api/pizza/{id}`          | Update pizza details  |
| PUT    | `/api/pizza/{id}/toppings` | Update pizza toppings |
| DELETE | `/api/pizza/{id}`          | Delete a pizza        |

## HTTP Status Codes

Successful operations use appropriate REST status codes:

* `200 OK` – Successful GET/PUT operations
* `201 Created` – Successful pizza/topping creation
* `204 No Content` – Successful deletion

Error responses are handled centrally by the global exception handler.

## Notes

* Pizza and topping names are validated to prevent duplicates.
* Pizza toppings use a many-to-many relationship through `PizzaTopping`.
* Database access is performed through Entity Framework Core.
* The Application layer depends on the `IApplicationDbContext` abstraction rather than directly depending on the Infrastructure DbContext.
* EF Core migrations are included in the project for database initialization.
