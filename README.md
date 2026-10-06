# Inventory API

A lightweight RESTful API for managing products and inventory.

The backend is built with ASP.NET Core and .NET 10, using Entity Framework Core, SQL Server, JWT authentication, in-memory caching, and health checks. The project also includes a React and TypeScript frontend designed as a single-page application.

## Table of Contents

- [Features](#features)
- [Architecture](#architecture)
- [Technology Stack](#technology-stack)
- [Requirements](#requirements)
- [Getting Started](#getting-started)
- [Docker](#docker)
- [Configuration](#configuration)
- [Authentication](#authentication)
- [API Endpoints](#api-endpoints)
- [Health Checks and Swagger](#health-checks-and-swagger)
- [Testing and CI/CD](#testing-and-cicd)
- [Logging and Diagnostics](#logging-and-diagnostics)
- [Deployment](#deployment)
- [Contributing](#contributing)
- [License and Contact](#license-and-contact)

## Features

### Product Management

- Create products
- Update products
- Delete products
- View product details
- Browse products with pagination
- Search products

### Authentication and Authorization

- Admin login and logout
- JWT bearer authentication
- Role-based authorization
- Password hashing with Microsoft Identity

### Performance and Reliability

- In-memory caching for database calls
- API and database health checks
- Transactions and error handling for SQL Server
- Server-side and client-side validation
- Error logging with `ILogger`

## Architecture

### Backend Architecture

The backend follows a layered architecture based on separation of concerns.

- **Controllers** handle HTTP requests and return HTTP responses.
- **Services** contain business logic and application rules.
- **Repositories** abstract data access and interact with Entity Framework Core.
- **Entity Framework Core** provides database access through `AppDbContext`.

The dependency flow is:
Controller → Service → Repository → Database

### Frontend Architecture

The React frontend follows a separation-of-concerns architecture organized by technical responsibility.

- **Components** Reusable UI components
- **Pages** Route-level application views
- **Hooks** Reusable stateful logic
- **Services** API communication
- **Context** Shared application state

## Technology Stack

### Backend

- .NET 10
- ASP.NET Core Web API
- Entity Framework Core
- SQL Server
- Microsoft Identity password hashing
- JWT Bearer authentication
- IMemoryCache
- OpenAPI and Swagger
- ILogger
- Serilog, Console, and Debug logging providers

### Frontend

- React
- TypeScript

## Infrastructure and Deployment

- Docker
- Docker Compose
- Azure Container Apps
- Azure SQL Database
- Azure Key Vault
- GitHub Actions
- CodeQL
- SonarCloud
- dotnet format

## Testing

- xUnit
- Unit tests for database calls
- Unit tests involving cancellation tokens
- API tests
- Automated test execution through GitHub Actions

## Requirements

- .NET 10 SDK
- SQL Server, either locally or remotely
- Entity Framework Core CLI tools
- Docker, if running the application in containers
- Visual Studio 2026 or another compatible IDE

## Getting Started

1. Clone the Repository
git clone https://github.com/OlivierT11/Inventory.git
cd Inventory

3. Configure the Application
Update appsettings.json or configure the required environment variables.

Required settings include:
- ConnectionStrings:DefaultConnection
- Jwt:Key
- Jwt:Issuer
- Jwt:Audience
- Cors:AllowedOrigins

3. Restore and Build the Project
dotnet restore
dotnet build

4. Apply Entity Framework Core Migrations
dotnet ef database update

5. Run the API
dotnet run --project Inventory_Api/Inventory

When using Visual Studio, open Inventory.slnx and run the API project.

## Docker

When running in Docker, use the Docker environment:
ASPNETCORE\_ENVIRONMENT=Docker
DOTNET\_RUNNING\_IN\_CONTAINER=true

Database migrations are applied automatically when the application runs in the Docker environment.

Make sure that the required database connection string, JWT settings, and exposed ports are configured correctly.

## Configuration

Example appsettings.json configuration:

{
  "ConnectionStrings": {
    "DefaultConnection": "Server=.;Database=InventoryDb;Trusted\_Connection=True;TrustServerCertificate=True;"
  },
  "Jwt": {
    "Key": "your-very-long-secret-key",
    "Issuer": "InventoryApi",
    "Audience": "InventoryClients"
  },
  "Cors": {
    "AllowedOrigins": [
      "http://localhost:3000"
    ]
  }
}

Do not commit production secrets, JWT keys, database passwords, or other sensitive values to the repository. Use environment variables or Azure Key Vault for production configuration.

## Authentication

The API uses JWT bearer tokens.
Include the token in authenticated requests:
Authorization: Bearer <token>
Authentication-related components include:
- JwtTokenService
- IAuthService
- IAuthRepository

## API Endpoints
GET	/api/products	  Get a paginated list of products
GET	/api/products/{id}	  Get a product by ID
POST /api/products	  Create a product
PUT	/api/products/{id}	  Update a product
DELETE /api/products/{id}	  Delete a product
GET	/api/health	  Check API and database health

See the controllers and OpenAPI documentation for request models, response formats, and authorization requirements.

## Health Checks and Swagger

Health checks are available at:
GET /api/health

OpenAPI documentation is enabled in the Development environment:
/openapi/v1.json

Swagger UI is available when:
ASPNETCORE\_ENVIRONMENT=Development

## Testing and CI/CD

Run the test suite with:
dotnet test

The GitHub Actions pipeline includes:
- Building the application
- Running xUnit tests
- Running API tests
- Running Docker Compose
- Code quality checks with dotnet format
- Code scanning with CodeQL
- Static analysis with SonarCloud
- Building Docker images
- Deploying to Azure

## Logging and Diagnostics

The application is configured with Console and Debug logging providers.

Logging settings can be adjusted in:
Program.cs
appsettings.json

## Deployment

The application can be deployed to Azure using:
- Azure Container Apps
- Azure SQL Database
- Azure Key Vault

## Deployed Applications

Frontend:
https://inventory-frontend-container-app.gentledune-2e10108a.francecentral.azurecontainerapps.io

## Business Rules

The business rules are documented progressively as the application evolves.

Examples may include:
    Only authenticated administrators can manage products.
    Product names must be provided.
    Product prices and quantities must be valid.
    Product identifiers must be unique.

## Contributing

- Fork the repository.
- Create a feature branch:
    git switch -c feature/my-feature
- Implement your changes.
- Add or update tests.
- Run the test suite:
    dotnet test
- Commit and push your changes.
- Open a pull request with a clear description.

## License and Contact

See the repository for license details.

Repository:
https://github.com/OlivierT11/Inventory
