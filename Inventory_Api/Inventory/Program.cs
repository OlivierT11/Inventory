using Inventory.Data;
using Inventory.Repositories;
using Inventory.Services;
using Inventory.Models;
using Microsoft.AspNetCore.Authentication.JwtBearer;
using Microsoft.AspNetCore.Identity;
using Microsoft.EntityFrameworkCore;
using Microsoft.IdentityModel.Tokens;
using System.Text;

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

// jwt authentication service
var jwtSettings = builder.Configuration.GetSection("Jwt");
var signingKey = jwtSettings["Key"]
    ?? throw new InvalidOperationException("JWT key is missing.");

builder.Services.AddAuthentication(JwtBearerDefaults.AuthenticationScheme)
    .AddJwtBearer(options =>
    {
        options.TokenValidationParameters = new TokenValidationParameters
        {
            ValidateIssuerSigningKey = true,
            IssuerSigningKey = new SymmetricSecurityKey(
                Encoding.UTF8.GetBytes(signingKey)),

            ValidateIssuer = true,
            ValidIssuer = jwtSettings["Issuer"],

            ValidateAudience = true,
            ValidAudience = jwtSettings["Audience"],

            ValidateLifetime = true,
            ClockSkew = TimeSpan.FromMinutes(1)
        };
    });

builder.Services.AddAuthorization(); //jwt
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
builder.Services.AddScoped<JwtTokenService>();
builder.Services.AddScoped<IAuthRepository, AuthRepository>();
builder.Services.AddScoped<IAuthService, AuthService>();
builder.Services.AddScoped<IPasswordHasher<User>, PasswordHasher<User>>();

// IMemoryCache service for caching
builder.Services.AddMemoryCache();

builder.Logging.ClearProviders();
builder.Logging.AddConsole();
builder.Logging.AddDebug();

var app = builder.Build();

// Apply database migrations automatically in Docker
if (app.Environment.IsEnvironment("Docker"))
{
    using (var scope = app.Services.CreateScope())
    {
        var dbContext = scope.ServiceProvider.GetRequiredService<AppDbContext>();
        dbContext.Database.Migrate();
    }
}

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

app.UseAuthentication(); //jwt
app.UseAuthorization();

// health check
app.MapGet("/api/health", () => Results.Ok(new { status = "healthy" }));

app.MapControllers();

app.Run();
