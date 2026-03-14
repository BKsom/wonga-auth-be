# WongaAuth Backend

A microservices-based authentication and user management backend built with C# .NET 8, following Clean Architecture, DDD, and CQRS principles.

## Architecture Overview

```
                        ┌─────────────────────┐
                        │     API Gateway      │
                        │  (YARP - Port 5000)  │
                        └──────────┬──────────┘
                                   │
                    ┌──────────────┴──────────────┐
                    │                             │
          ┌─────────▼─────────┐       ┌──────────▼──────────┐
          │   Auth Service    │──────▶│   User Service      │
          │   (Port 5001)     │  HTTP │   (Port 5002)        │
          └─────────┬─────────┘       └──────────┬──────────┘
                    │                             │
          ┌─────────▼─────────┐       ┌──────────▼──────────┐
          │  auth_db (PG)     │       │  user_db (PG)        │
          │  Port 5432        │       │  Port 5433           │
          └───────────────────┘       └─────────────────────┘
```

## Tech Stack

- **Runtime**: .NET 8 / ASP.NET Core 8
- **ORM**: Entity Framework Core 8 with Npgsql (PostgreSQL)
- **CQRS**: MediatR 12
- **Validation**: FluentValidation 11
- **Authentication**: JWT Bearer tokens
- **Password Hashing**: BCrypt.Net-Next
- **API Gateway**: YARP (Yet Another Reverse Proxy)
- **API Docs**: Swagger / OpenAPI (Swashbuckle)
- **Testing**: xUnit, Moq, FluentAssertions
- **Containers**: Docker + Docker Compose

## Project Structure

```
WongaAuth.sln
├── src/
│   ├── ApiGateway/                    # YARP reverse proxy
│   └── Services/
│       ├── Auth/
│       │   ├── Auth.Domain/           # Entities, ValueObjects, Repository interfaces
│       │   ├── Auth.Application/      # CQRS Commands, DTOs, Application interfaces
│       │   ├── Auth.Infrastructure/   # EF Core, JWT, BCrypt implementations
│       │   └── Auth.API/              # ASP.NET Core controllers, middleware
│       └── User/
│           ├── User.Domain/           # UserProfile entity, repository interfaces
│           ├── User.Application/      # CQRS Commands/Queries, DTOs
│           ├── User.Infrastructure/   # EF Core implementations
│           └── User.API/              # ASP.NET Core controllers, middleware
└── tests/
    ├── Auth.UnitTests/                # xUnit tests for Auth service
    └── User.UnitTests/                # xUnit tests for User service
```

## Quick Start with Docker

### Prerequisites
- Docker Desktop installed and running
- Ports 5000, 5001, 5002, 5432, 5433 must be free

### Run with Docker Compose

```bash
# Clone or navigate to this directory, then:
./build.sh docker

# Or manually:
docker-compose up --build -d
```

Services will be available at:

| Service | URL |
|---------|-----|
| API Gateway | http://localhost:5000 |
| Auth Service | http://localhost:5001 |
| User Service | http://localhost:5002 |
| Auth Swagger | http://localhost:5001/swagger |
| User Swagger | http://localhost:5002/swagger |

Databases auto-migrate on startup — no manual migration step required.

## Running Locally (without Docker)

### Prerequisites
- .NET 8 SDK
- PostgreSQL running locally

### Steps

1. Update connection strings in `appsettings.json` for each service to point to your local PostgreSQL.

2. Restore and build:
```bash
./build.sh local
# Or:
dotnet restore WongaAuth.sln
dotnet build WongaAuth.sln
```

3. Run each service:
```bash
# Terminal 1 - Auth Service
cd src/Services/Auth/Auth.API
dotnet run

# Terminal 2 - User Service
cd src/Services/User/User.API
dotnet run

# Terminal 3 - API Gateway
cd src/ApiGateway
dotnet run
```

## API Endpoints

All requests go through the API Gateway at `http://localhost:5000`.

### Auth Endpoints

#### Register a new user
```http
POST /api/auth/register
Content-Type: application/json

{
  "firstName": "John",
  "lastName": "Doe",
  "email": "john@example.com",
  "password": "password123"
}
```

**Response (201 Created):**
```json
{
  "id": "3fa85f64-5717-4562-b3fc-2c963f66afa6",
  "firstName": "John",
  "lastName": "Doe",
  "email": "john@example.com"
}
```

#### Login
```http
POST /api/auth/login
Content-Type: application/json

{
  "email": "john@example.com",
  "password": "password123"
}
```

**Response (200 OK):**
```json
{
  "token": "eyJhbGciOiJIUzI1NiIsInR5cCI6IkpXVCJ9...",
  "user": {
    "id": "3fa85f64-5717-4562-b3fc-2c963f66afa6",
    "firstName": "John",
    "lastName": "Doe",
    "email": "john@example.com"
  }
}
```

### User Endpoints

#### Get current user profile (requires JWT)
```http
GET /api/users/me
Authorization: Bearer <your-jwt-token>
```

**Response (200 OK):**
```json
{
  "id": "3fa85f64-5717-4562-b3fc-2c963f66afa6",
  "firstName": "John",
  "lastName": "Doe",
  "email": "john@example.com"
}
```

## Running Tests

```bash
# Unit tests only
./build.sh test
# Or:
dotnet test WongaAuth.sln --filter "Category!=Integration"

# All tests
./build.sh test-all
# Or:
dotnet test WongaAuth.sln
```

## Environment Variables

### Auth Service

| Variable | Description | Default |
|----------|-------------|---------|
| `ConnectionStrings__AuthDatabase` | PostgreSQL connection string | See appsettings.json |
| `JwtSettings__SecretKey` | JWT signing secret (min 32 chars) | WongaAuthSuperSecretKey2024... |
| `JwtSettings__Issuer` | JWT issuer | WongaAuth |
| `JwtSettings__Audience` | JWT audience | WongaAuth |
| `JwtSettings__ExpiryMinutes` | Token expiry in minutes | 60 |
| `Services__UserService` | URL of the User Service | http://user-service:8080 |

### User Service

| Variable | Description | Default |
|----------|-------------|---------|
| `ConnectionStrings__UserDatabase` | PostgreSQL connection string | See appsettings.json |
| `JwtSettings__SecretKey` | JWT signing secret (must match Auth Service) | WongaAuthSuperSecretKey2024... |
| `JwtSettings__Issuer` | JWT issuer | WongaAuth |
| `JwtSettings__Audience` | JWT audience | WongaAuth |

## Service-to-Service Communication

When a user registers via the Auth Service, the Auth Service automatically calls the User Service's internal endpoint (`POST /api/users/internal/create`) to create a corresponding user profile. This keeps the two databases in sync.

## Design Patterns

- **Clean Architecture**: Domain -> Application -> Infrastructure -> API
- **DDD**: Rich domain entities with encapsulated business logic
- **CQRS**: Separate Command and Query handlers via MediatR
- **Repository Pattern**: Abstracts data access behind interfaces
- **Dependency Injection**: All services registered via DI container
