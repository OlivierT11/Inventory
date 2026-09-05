# Inventory API

A lightweight RESTful API for managing products. Built with ASP.NET Core (.NET 10), EF Core, JWT authentication, in-memory caching and health checks. Designed for use with a React or other SPA front-end.

## Key features
- CRUD operations for products via `ProductsController`
- Repository & service layers (`IProductRepository`, `ProductRepository`, `IProductService`, `ProductService`)
- EF Core `AppDbContext` with SQL Server support
- JWT authentication and authorization
- In-memory caching for product reads
- Health checks (`/api/health`) (API + database)
- Automatic DB migrations when running in Docker environment
- xUnit + GitHub Actions for automated unit testing
- Unit tests for database calls (including cancellation tokens) and API calls
- Docker for deployment and containerization
- OpenAPI/Swagger for API documentation
- Input validation at front and API level
- React frontend + TypeScript
    Product creation form
    Product viewing
    Search with pagination
    JWT login
- Cache management to optimize performance
- GitHub
- Transactions and error handling (SQL Server)
- Separate development/production environments (frontend, backend, database)
- Azure Cloud for application deployment
    Azure SQL
    Azure Key Vault
- Server-side and client-side data validation
- Error logging (ILogger)
- Frontend notifications after CRUD operations
- GitHub Actions pipeline for continuous integration and continuous deployment (CI/CD):
    Build and run
    xUnit tests
    Docker Compose
    Code quality checks with CodeQL, SonarCloud, and dotnet format
    Docker images
    Azure Cloud deployment

## Tech stack
- .NET 10
- ASP.NET Core Web API
- Entity Framework Core (SQL Server)
- Microsoft Identity for password hashing
- JWT Bearer authentication
- IMemoryCache for caching
- Serilog/Console/Debug logging (console & debug configured)

## Requirements
- .NET 10 SDK
- SQL Server instance (or change to another provider in `AppDbContext`)
- PowerShell (recommended; Visual Studio 2026 supported)

## Quick start (local)
1. Clone the repo
   - git clone https://github.com/OlivierT11/Inventory
2. Update configuration
   - Edit `appsettings.json` or environment variables:
     - `ConnectionStrings:DefaultConnection` — SQL Server connection string
     - `Jwt:Key`, `Jwt:Issuer`, `Jwt:Audience` — JWT settings
3. Restore and build
   - dotnet restore
   - dotnet build
4. Apply EF migrations and run
   - dotnet ef database update
   - dotnet run --project Inventory_Api/Inventory

Note: When running in Visual Studio, open the `Inventory.slnx` solution and run the API project.

## Docker
- Set `ASPNETCORE_ENVIRONMENT=Docker` (migrations are applied automatically in that environment).
- Expose ports and provide the `DOTNET_RUNNING_IN_CONTAINER=true` environment variable if running in container mode.

## Configuration examples
appsettings.json (example) :
appsettings.json { "ConnectionStrings": { "DefaultConnection": "Server=.;Database=InventoryDb;Trusted_Connection=True;" }, "Jwt": { "Key": "your-very-long-secret-key", "Issuer": "InventoryApi", "Audience": "InventoryClients" }, "Cors": { "AllowedOrigins": [ "http://localhost:3000" ] } }

## Authentication
- The API uses JWT bearer tokens. Include the header:
  - `Authorization: Bearer <token>`
- An authentication service and repository are provided (`JwtTokenService`, `IAuthService`, `IAuthRepository`).

## Endpoints (examples)
- GET `/api/products` — list products
- GET `/api/products/{id}` — get product by id
- POST `/api/products` — create product
- PUT `/api/products/{id}` — update product
- DELETE `/api/products/{id}` — delete product
- GET `/api/health` — health status

(See controllers for full details and required authorization per endpoint.)

## Business rules 
- GET returns 200 OK with products.
- GET returns 404 NotFound when the product does not exist.
- POST returns 201 Created for a valid product.
- POST returns 400 BadRequest for invalid input.
- PUT returns 204 NoContent or 200 OK.
- DELETE returns 204 NoContent.
- Service exceptions are translated to the correct HTTP response.
- The cancellation token is passed to the service.

## Health checks & Swagger
- Health checks: `GET /api/health`
- Swagger/OpenAPI is enabled in Development:
  - OpenAPI JSON: `/openapi/v1.json`
  - Swagger UI: configured when `ASPNETCORE_ENVIRONMENT=Development`

## Logging & diagnostics
- Console and Debug logging providers are configured. Adjust logging in `Program.cs` or `appsettings.json` as needed.

## Contributing
- Fork the repository and create a feature branch.
- Add tests for new features.
- Open a pull request with a clear description.

## License & contact
- See repository for license details.
- Repository: https://github.com/OlivierT11/Inventory