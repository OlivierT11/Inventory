using Inventory.Data;
using Inventory.Services;
using Microsoft.EntityFrameworkCore;

var builder = WebApplication.CreateBuilder(args);

// Add services to the container.

var allowedOrigins =
    builder.Configuration
        .GetSection("Cors:AllowedOrigins") // appsettings.env.json
        .Get<string[]>() ?? [];

// database service
builder.Services.AddDbContext<AppDbContext>(options =>
    options.UseSqlServer(
        builder.Configuration.GetConnectionString("DefaultConnection")));

// Only set the container URL when running in Docker
var inContainer = Environment.GetEnvironmentVariable("DOTNET_RUNNING_IN_CONTAINER") == "true";
if (inContainer)
{
    builder.WebHost.UseUrls("http://0.0.0.0:8080");
}

builder.Services.AddControllers();
builder.Services.AddOpenApi();

// CORS (for frontend calls)
builder.Services.AddCors(options =>
{
    options.AddPolicy("ReactApp", policy =>
    {
        policy
            .WithOrigins(allowedOrigins)  
            .AllowAnyHeader()
            .AllowAnyMethod();
    });
});

builder.Services.AddScoped<IProductService, ProductService>();

builder.Services.AddMemoryCache();

builder.Logging.ClearProviders();
builder.Logging.AddConsole();
builder.Logging.AddDebug();

var app = builder.Build();

// Configure the HTTP request pipeline.
if (app.Environment.IsDevelopment())
{
    // See the JSON describing the api at http://localhost:5293/openapi/v1.json (based on ProductController.cs)
    app.MapOpenApi();

    // enable swagger ui
    app.UseSwaggerUI(options =>
    {
        options.SwaggerEndpoint("/openapi/v1.json", "Inventory API v1");
    });
}

// Avoid forcing HTTPS when running in container (prevents broken redirects)
if (!inContainer)
{
    app.UseHttpsRedirection();
}

if (!app.Environment.IsDevelopment())
{
    // Centralized exception handling rather than logging errors in every controller
    app.UseExceptionHandler("/error");
}

app.UseCors("ReactApp");

app.UseAuthorization();

app.MapControllers();

app.Run();
