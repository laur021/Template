using System.Reflection;
using API.Middleware;
using API.Services;
using Application;
using Application.Interfaces;
using Infrastructure;
using Microsoft.OpenApi;

var builder = WebApplication.CreateBuilder(args);

// ======================================
// Service Registration
// ======================================

// Add Application layer services (Cortex.Mediator, FluentValidation)
builder.Services.AddApplicationServices(builder.Configuration);

// Add Infrastructure layer services (Identity, JWT, EF Core, etc.)
builder.Services.AddInfrastructureServices(builder.Configuration);

// Add current user service (requires HttpContext)
builder.Services.AddHttpContextAccessor();
builder.Services.AddScoped<ICurrentUserService, CurrentUserService>();

// Register Exception Middleware
builder.Services.AddScoped<ExceptionMiddleware>();

// Add Controllers
builder.Services.AddControllers();

// ======================================
// CORS Configuration for Angular SPA
// ======================================
builder.Services.AddCors(options =>
{
    options.AddPolicy("AngularApp", policy =>
    {
        var allowedOrigins = builder.Configuration
            .GetSection("Cors:AllowedOrigins")
            .Get<string[]>() ?? new[] { "http://localhost:4200" };

        policy
            .WithOrigins(allowedOrigins)
            .AllowAnyHeader()
            .AllowAnyMethod()
            // IMPORTANT: AllowCredentials is required for cookies to be sent cross-origin
            .AllowCredentials();
    });
});

// ======================================
// Swagger/OpenAPI Configuration
// ======================================
builder.Services.AddEndpointsApiExplorer();
builder.Services.AddSwaggerGen(options =>
{
    options.SwaggerDoc("v1", new OpenApiInfo
    {
        Title = "API Template",
        Version = "v1",
        Description = "ASP.NET Core Web API with Clean Architecture, JWT Authentication, and CQRS"
    });

    // Add JWT authentication to Swagger
    options.AddSecurityDefinition("bearer", new OpenApiSecurityScheme
    {
        Type = SecuritySchemeType.Http,
        Scheme = "bearer",
        BearerFormat = "JWT",
        Description = "JWT Authorization header using the Bearer scheme. Enter your token."
    });

    options.AddSecurityRequirement(document => new OpenApiSecurityRequirement
    {
        [new OpenApiSecuritySchemeReference("bearer", document)] = []
    });

    // Include XML comments for API documentation
    var xmlFile = $"{Assembly.GetExecutingAssembly().GetName().Name}.xml";
    var xmlPath = Path.Combine(AppContext.BaseDirectory, xmlFile);
    if (File.Exists(xmlPath))
    {
        options.IncludeXmlComments(xmlPath);
    }
});

// ======================================
// Build the Application
// ======================================
var app = builder.Build();

// ======================================
// Middleware Pipeline
// ======================================

// Global exception handling (must be first)
app.UseMiddleware<ExceptionMiddleware>();

// Development-only middleware
if (app.Environment.IsDevelopment())
{
    app.UseSwagger();
    app.UseSwaggerUI(options =>
    {
        options.SwaggerEndpoint("/swagger/v1/swagger.json", "API Template v1");
    });
}

app.UseHttpsRedirection();

// CORS must come before authentication
app.UseCors("AngularApp");

// Authentication & Authorization
app.UseAuthentication();
app.UseAuthorization();

// Map Controllers
app.MapControllers();

// Health check endpoint
app.MapGet("/health", () => Results.Ok(new { Status = "Healthy", Timestamp = DateTime.UtcNow }))
    .WithTags("Health")
    .AllowAnonymous();

// ======================================
// Database Initialization
// ======================================
await DbInitializer.InitializeAsync(app.Services);

// ======================================
// Run the Application
// ======================================
app.Run();

// Make Program accessible for integration tests
public partial class Program { }
