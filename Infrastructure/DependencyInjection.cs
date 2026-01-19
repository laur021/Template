using System.Text;
using Application.Interfaces;
using Infrastructure.Configuration;
using Infrastructure.Identity;
using Infrastructure.Services;
using Microsoft.AspNetCore.Authentication.JwtBearer;
using Microsoft.AspNetCore.Identity;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.IdentityModel.Tokens;

namespace Infrastructure;

/// <summary>
/// Extension methods for registering Infrastructure layer services.
/// </summary>
public static class DependencyInjection
{
    public static IServiceCollection AddInfrastructureServices(
        this IServiceCollection services,
        IConfiguration configuration)
    {
        // ======================================
        // Database Configuration
        // ======================================
        var connectionString = configuration.GetConnectionString("DefaultConnection");

        services.AddDbContext<AppDbContext>(options =>
        {
            // Determine database provider based on connection string
            // SQLite connection strings typically contain "Data Source" and end with .db
            // SQL Server connection strings contain "Server=" or "Data Source=" with server name
            var useSqlite = string.IsNullOrEmpty(connectionString) ||
                            connectionString.Contains(".db", StringComparison.OrdinalIgnoreCase) ||
                            connectionString.StartsWith("Data Source=", StringComparison.OrdinalIgnoreCase);

            if (useSqlite)
            {
                options.UseSqlite(connectionString ?? "Data Source=app.db");
            }
            else
            {
                options.UseSqlServer(connectionString);
            }
        });

        // ======================================
        // ASP.NET Identity Configuration
        // ======================================
        services.AddIdentity<AppUser, AppRole>(options =>
        {
            // Password requirements
            options.Password.RequireDigit = true;
            options.Password.RequireLowercase = true;
            options.Password.RequireUppercase = true;
            options.Password.RequireNonAlphanumeric = true;
            options.Password.RequiredLength = 8;
            options.Password.RequiredUniqueChars = 4;

            // Lockout settings - protects against brute force attacks
            options.Lockout.DefaultLockoutTimeSpan = TimeSpan.FromMinutes(15);
            options.Lockout.MaxFailedAccessAttempts = 5;
            options.Lockout.AllowedForNewUsers = true;

            // User settings
            options.User.RequireUniqueEmail = true;
            options.User.AllowedUserNameCharacters =
                "abcdefghijklmnopqrstuvwxyzABCDEFGHIJKLMNOPQRSTUVWXYZ0123456789-._@+";

            // Sign-in settings
            options.SignIn.RequireConfirmedEmail = true;
            options.SignIn.RequireConfirmedAccount = false;
        })
        .AddEntityFrameworkStores<AppDbContext>()
        .AddDefaultTokenProviders();

        // ======================================
        // JWT Authentication Configuration
        // ======================================
        var jwtSettings = configuration.GetSection(JwtSettings.SectionName).Get<JwtSettings>()
            ?? throw new InvalidOperationException("JWT settings are not configured");

        services.Configure<JwtSettings>(configuration.GetSection(JwtSettings.SectionName));

        services.AddAuthentication(options =>
        {
            // SECURITY NOTE: We use JWT Bearer as the default scheme
            // This means all [Authorize] attributes will require a valid JWT
            options.DefaultAuthenticateScheme = JwtBearerDefaults.AuthenticationScheme;
            options.DefaultChallengeScheme = JwtBearerDefaults.AuthenticationScheme;
            options.DefaultScheme = JwtBearerDefaults.AuthenticationScheme;
        })
        .AddJwtBearer(options =>
        {
            options.TokenValidationParameters = new TokenValidationParameters
            {
                // Validate the signing key
                ValidateIssuerSigningKey = true,
                IssuerSigningKey = new SymmetricSecurityKey(
                    Encoding.UTF8.GetBytes(jwtSettings.SecretKey)),

                // Validate the issuer (who created the token)
                ValidateIssuer = true,
                ValidIssuer = jwtSettings.Issuer,

                // Validate the audience (who the token is for)
                ValidateAudience = true,
                ValidAudience = jwtSettings.Audience,

                // Validate the token expiration
                ValidateLifetime = true,

                // Set clock skew to zero for more precise expiration
                // Default is 5 minutes which may be too lenient
                ClockSkew = TimeSpan.FromSeconds(30)
            };

            // SECURITY NOTE: For SignalR or other scenarios where you need
            // to pass the token in the query string, configure it here:
            // options.Events = new JwtBearerEvents
            // {
            //     OnMessageReceived = context =>
            //     {
            //         var accessToken = context.Request.Query["access_token"];
            //         if (!string.IsNullOrEmpty(accessToken))
            //         {
            //             context.Token = accessToken;
            //         }
            //         return Task.CompletedTask;
            //     }
            // };
        });

        services.AddAuthorization();

        // ======================================
        // Configuration Options
        // ======================================
        services.Configure<GoogleAuthSettings>(
            configuration.GetSection(GoogleAuthSettings.SectionName));
        services.Configure<EmailSettings>(
            configuration.GetSection(EmailSettings.SectionName));

        // ======================================
        // Application Services
        // ======================================
        services.AddScoped<IJwtTokenService, JwtTokenService>();
        services.AddScoped<IAuthService, AuthService>();
        services.AddScoped<IEmailService, EmailService>();
        services.AddScoped<IExternalAuthService, ExternalAuthService>();
        services.AddScoped<IUserService, UserService>();

        return services;
    }
}
