# DeveloperStore Sales API

Sales Management API built with .NET 8.0, implementing Domain-Driven Design (DDD) principles and CQRS pattern using MediatR.

## Getting Started

### Prerequisites

- [.NET 8.0 SDK](https://dotnet.microsoft.com/download/dotnet/8.0)
- [Docker Desktop](https://www.docker.com/products/docker-desktop/)
- [PostgreSQL](https://www.postgresql.org/) (if running without Docker)
- [MongoDB](https://www.mongodb.com/) (if running without Docker)
- [Redis](https://redis.io/) (if running without Docker)

### Running the Project

#### Option 1: Using Docker Compose (CLI)

1. Build the solution
   
2. Navigate to the project root directory

3. Start the containers:
   ```bash
   docker-compose up -d
   ```

4. The API will be available at `http://localhost:8080`, Swagger UI: `http://localhost:8080/swagger`

#### Option 2: Using Visual Studio

1. Build the solution

2. Set `docker-compose` as the startup project and run it.

The API will start with all required dependencies (PostgreSQL, MongoDB, Redis)

### Running Migrations

After starting the application, you need to apply database migrations to create the PostgreSQL schema:

```bash
dotnet ef database update --project src/DeveloperStore.ORM --startup-project src/DeveloperStore.WebApi
```

This command will:
- Create the database if it doesn't exist
- Apply all pending migrations
- Set up the required tables and relationships

## Testing the API

**Swagger:**
- Go to `http://localhost:8080/swagger` in your browser.
- Click "Try it out" on any endpoint and execute requests directly.

**Postman:**
- Import `DeveloperStore Sales API.postman_collection.json` into Postman.
- Use the pre-configured requests to test all endpoints.

If you only want to run and test the project, you can stop here. The sections below provide details on architecture, techniques, and design decisions for those interested.

## Architecture & Techniques

### Design Patterns

- **Domain-Driven Design (DDD)**: The project is organized around business domains with separation between domain logic, application logic, and infrastructure concerns.
  
- **CQRS (Command Query Responsibility Segregation)**: Implemented using MediatR to separate read and write operations, improving scalability and maintainability.
  
- **Repository Pattern**: Abstracts data access logic, providing a separation between domain and data layers.
  
- **External Identities Pattern**: Used for referencing entities from other domains with denormalization of descriptions, following DDD practices.
  
- **Event Sourcing**: Domain events are published for key operations:
  - `SaleCreatedEvent`
  - `SaleModifiedEvent`
  - `SaleCancelledEvent`
  - `SaleItemCancelledEvent`

   Events are stored in MongoDB as an example implementation. This allows for future integration with a message broker, where these events can be consumed and processed asynchronously by other services.

### Frameworks & Libraries

- **MediatR**: CQRS and request/response handling
- **AutoMapper**: Object mapping
- **FluentValidation**: Validation rules
- **Entity Framework Core**: PostgreSQL ORM
- **MongoDB Driver**: Event store
- **Redis**: Distributed caching
- **xUnit**: Unit testing
- **NSubstitute**: Mocking
- **Bogus (Faker)**: Test data generation


### Database & Caching Strategy

- **PostgreSQL**: Main relational database for sales data.
- **MongoDB**: Stores domain events and supports flexible queries.
- **Redis**: Caches frequent queries for fast API responses.

   Caching with Redis is demonstrated on a single endpoint as an example.
   
### Additional Techniques

- **Clean Architecture**: Layers are organized to depend on abstractions, with dependencies pointing inward toward the domain.
- **Dependency Injection**: Built-in .NET DI container manages all dependencies.
- **Validation Pipeline**: MediatR behaviors validate commands before handler execution.
- **Health Checks**: Monitors application and database health.
- **Structured Logging**: Comprehensive logging for debugging and monitoring.

## Project Structure

```
src/
├── DeveloperStore.WebApi/          # API Layer (Controllers, Middleware)
├── DeveloperStore.Application/     # Application Layer (Use Cases, Handlers)
├── DeveloperStore.Domain/          # Domain Layer (Entities, Value Objects, Rules)
├── DeveloperStore.ORM/             # Infrastructure (Repositories, Migrations)
├── DeveloperStore.IoC/             # Dependency Injection Configuration
└── DeveloperStore.Common/          # Shared Utilities (Validation, Logging)

tests/
├── DeveloperStore.Unit/            # Unit Tests
├── DeveloperStore.Integration/     # Integration Tests
└── DeveloperStore.Functional/      # Functional Tests
```

## Features

✅ **Sales CRUD**
- Create sales with multiple items
- Retrieve sales with pagination, filtering, and sorting
- Update sale information
- Cancel entire sales or individual items

✅ **Business Logic Validation**
- Quantity-based discount rules
- Maximum quantity restrictions
- Domain-driven validation

✅ **Event Publishing**
- Events logged for all major operations
- Event store in MongoDB for audit trail

✅ **RESTful API Design**
- Standard HTTP methods and status codes
- Consistent response formats
- Comprehensive error handling

## Business Rules

The API enforces the following discount and quantity rules:

| Quantity Range | Discount | Status               |
| -------------- | -------- | -------------------- |
| 1-3 items      | 0%       | No discount allowed  |
| 4-9 items      | 10%      | Standard discount    |
| 10-20 items    | 20%      | Bulk discount        |
| 21+ items      | ❌        | Not allowed (max 20) |

## Possible Improvements

For a production-ready solution, consider these short-term enhancements:

- Add authentication and authorization (JWT)
- Implement API versioning
- Expand caching to more endpoints beyond the example
- Add integration and functional tests (test projects are scaffolded but empty)
- Enhance Swagger documentation with request/response examples
- Implement rate limiting to prevent API abuse
- Connect Rebus to a real message broker (RabbitMQ/Azure Service Bus)
- Add CI/CD pipeline for automated testing and deployment

