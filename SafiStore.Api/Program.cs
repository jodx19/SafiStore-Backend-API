using System.Text;
using System.Text.Json;
using System.Text.Json.Serialization;
using Microsoft.AspNetCore.Authentication.JwtBearer;
using Microsoft.AspNetCore.Diagnostics.HealthChecks;
using Microsoft.AspNetCore.Identity;
using Microsoft.EntityFrameworkCore;
using Microsoft.IdentityModel.Tokens;
using Microsoft.OpenApi.Models;
using AspNetCoreRateLimit;
using SafiStore.Api.Data;
using SafiStore.Api.Infrastructure.Services;
using SafiStore.Api.Models.Domain;
using Microsoft.AspNetCore.HttpOverrides;

var builder = WebApplication.CreateBuilder(args);

// Controllers + JSON
builder.Services.AddControllers()
    .AddJsonOptions(options =>
    {
        options.JsonSerializerOptions.PropertyNamingPolicy =
            JsonNamingPolicy.CamelCase;
        options.JsonSerializerOptions.DefaultIgnoreCondition =
            JsonIgnoreCondition.WhenWritingNull;
    });

// Database
builder.Services.AddDbContext<AppDbContext>(options =>
    options.UseSqlServer(
        builder.Configuration.GetConnectionString("DefaultConnection"),
        sqlOptions => sqlOptions.EnableRetryOnFailure(
            maxRetryCount: 3,
            maxRetryDelay: TimeSpan.FromSeconds(10),
            errorNumbersToAdd: null)));

// Identity
builder.Services.AddIdentity<ApplicationUser, IdentityRole<int>>(options =>
{
    // Password - Strong validation matching DTO requirements
    options.Password.RequireDigit           = true;      // Must contain at least one digit
    options.Password.RequireLowercase       = false;     // Lowercase not required
    options.Password.RequireUppercase       = true;      // Must contain at least one uppercase
    options.Password.RequireNonAlphanumeric = true;      // Must contain at least one special character
    options.Password.RequiredLength         = 8;         // Minimum 8 characters

    options.Lockout.DefaultLockoutTimeSpan  = TimeSpan.FromMinutes(5);
    options.Lockout.MaxFailedAccessAttempts = 5;
    options.User.RequireUniqueEmail         = true;
})
.AddEntityFrameworkStores<AppDbContext>()
.AddDefaultTokenProviders();

// JWT
var jwtSettings = builder.Configuration.GetSection("JwtSettings");
var jwtSecret = jwtSettings["SecretKey"]
    ?? Environment.GetEnvironmentVariable("JwtSettings__SecretKey")
    ?? throw new InvalidOperationException(
        "JwtSettings:SecretKey is not configured. Set the JwtSettings__SecretKey environment variable.");

builder.Services.AddAuthentication(options =>
{
    options.DefaultAuthenticateScheme = JwtBearerDefaults.AuthenticationScheme;
    options.DefaultChallengeScheme = JwtBearerDefaults.AuthenticationScheme;
})
.AddJwtBearer(options =>
{
    options.TokenValidationParameters = new TokenValidationParameters
    {
        ValidateIssuerSigningKey = true,
        IssuerSigningKey = new SymmetricSecurityKey(Encoding.UTF8.GetBytes(jwtSecret)),
        ValidateIssuer = true,
        ValidIssuer = jwtSettings["Issuer"] ?? "SafiStoreAPI",
        ValidateAudience = true,
        ValidAudience = jwtSettings["Audience"] ?? "SafiStoreClient",
        ValidateLifetime = true,
        ClockSkew = TimeSpan.Zero
    };
    options.Events = new JwtBearerEvents
    {
        OnAuthenticationFailed = context =>
        {
            if (context.Exception is SecurityTokenExpiredException)
                context.Response.Headers.Add("Token-Expired", "true");
            return Task.CompletedTask;
        }
    };
});

// Dynamic CORS configuration from appsettings or Environment Variables
var allowedOrigins = builder.Configuration.GetSection("Cors:AllowedOrigins").Get<string[]>() ?? Array.Empty<string>();
var allowedOriginsList = new List<string>(allowedOrigins);

// Always allow Vercel production by default if not in config
if (!allowedOriginsList.Contains("https://safistore.vercel.app"))
{
    allowedOriginsList.Add("https://safistore.vercel.app");
}

// Only allow localhost in development
if (builder.Environment.IsDevelopment())
{
    if (!allowedOriginsList.Contains("http://localhost:4200"))
    {
        allowedOriginsList.Add("http://localhost:4200");
    }
}

builder.Services.AddCors(options =>
{
    options.AddPolicy("SafiStorePolicy", policy =>
        policy.WithOrigins(allowedOriginsList.ToArray())
              .AllowAnyMethod()
              .AllowAnyHeader()
              .AllowCredentials());
});

// Rate Limiting Configuration
builder.Services.Configure<IpRateLimitOptions>(options =>
{
    builder.Configuration.GetSection("IpRateLimiting").Bind(options);
    options.EnableEndpointRateLimiting = true;

    options.GeneralRules = new List<RateLimitRule>
    {
        new RateLimitRule
        {
            Endpoint = "*",
            Limit = 100,
            Period = "1m"
        },
        new RateLimitRule
        {
            Endpoint = "POST:/api/v1/auth/login",
            Limit = 5,
            Period = "1m"
        },
        new RateLimitRule
        {
            Endpoint = "POST:/api/v1/auth/register",
            Limit = 5,
            Period = "1m"
        },
        new RateLimitRule
        {
            Endpoint = "POST:/api/v1/auth/forgot-password",
            Limit = 5,
            Period = "1m"
        },
        new RateLimitRule
        {
            Endpoint = "POST:/api/v1/auth/reset-password",
            Limit = 5,
            Period = "1m"
        },
        new RateLimitRule
        {
            Endpoint = "POST:/api/v1/auth/change-password",
            Limit = 5,
            Period = "1m"
        }
    };
});

builder.Services.AddSingleton<IRateLimitConfiguration, RateLimitConfiguration>();
builder.Services.AddMemoryCache(); // Required for rate limiting
builder.Services.AddInMemoryRateLimiting();

// Swagger
builder.Services.AddEndpointsApiExplorer();
builder.Services.AddSwaggerGen(c =>
{
    c.SwaggerDoc("v1", new() { Title = "SafiStore API", Version = "v1" });
    c.AddSecurityDefinition("Bearer", new OpenApiSecurityScheme
    {
        Description = "JWT Authorization: Bearer {token}",
        Name = "Authorization",
        In = ParameterLocation.Header,
        Type = SecuritySchemeType.ApiKey,
        Scheme = "Bearer"
    });
    c.AddSecurityRequirement(new OpenApiSecurityRequirement
    {{
        new OpenApiSecurityScheme
        {
            Reference = new OpenApiReference
                { Type = ReferenceType.SecurityScheme, Id = "Bearer" }
        },
        Array.Empty<string>()
    }});
});

// Services
builder.Services.AddScoped<IJwtService, JwtService>();
builder.Services.AddScoped<IProductService, ProductService>();
builder.Services.AddScoped<ICartService, CartService>();
builder.Services.AddScoped<IOrderService, OrderService>();
builder.Services.AddScoped<IUserService, UserService>();
builder.Services.AddScoped<IReviewService, ReviewService>();

builder.Services.AddHealthChecks()
    .AddDbContextCheck<AppDbContext>();

var app = builder.Build();

// Enable Forwarded Headers for IIS/Reverse Proxy (Crucial for HTTPS detection)
app.UseForwardedHeaders(new ForwardedHeadersOptions
{
    ForwardedHeaders = ForwardedHeaders.XForwardedFor | ForwardedHeaders.XForwardedProto
});

// Middleware pipeline ORDER IS CRITICAL
app.UseExceptionHandler(appError =>
{
    appError.Run(async context =>
    {
        context.Response.StatusCode = 500;
        context.Response.ContentType = "application/json";
        
        var contextFeature = context.Features.Get<Microsoft.AspNetCore.Diagnostics.IExceptionHandlerFeature>();
        if (contextFeature != null)
        {
            var logger = context.RequestServices.GetRequiredService<ILogger<Program>>();
            logger.LogError(contextFeature.Error, "Unhandled exception occurred.");
            
            // In production, we NEVER expose the full stack trace or internal messages.
            await context.Response.WriteAsJsonAsync(new 
            {
                success = false,
                message = "An unexpected error occurred. Please contact support.",
                timestamp = DateTime.UtcNow
            });
        }
    });
});

if (app.Environment.IsDevelopment())
{
    app.UseSwagger();
    app.UseSwaggerUI(c =>
    {
        c.SwaggerEndpoint("/swagger/v1/swagger.json", "SafiStore API v1");
        c.RoutePrefix = "swagger";
    });
}

app.UseRouting(); // explicitly add UseRouting before UseCors
app.UseCors("SafiStorePolicy");

// Rate Limiting Middleware (must be after UseRouting)
app.UseIpRateLimiting();

app.UseHttpsRedirection();
app.UseAuthentication();
app.UseAuthorization();
app.MapControllers();

app.MapHealthChecks("/health", new HealthCheckOptions
{
    ResponseWriter = async (context, report) =>
    {
        context.Response.ContentType = "application/json";
        var result = JsonSerializer.Serialize(new
        {
            status = report.Status.ToString(),
            timestamp = DateTime.UtcNow,
            service = "SafiStore API",
            version = "1.0.0",
            checks = report.Entries.Select(e => new
            {
                name = e.Key,
                status = e.Value.Status.ToString(),
                duration = e.Value.Duration.TotalMilliseconds
            })
        });
        await context.Response.WriteAsync(result);
    }
});

app.MapGet("/ping", () => Results.Ok(new
{
    message = "SafiStore API is alive",
    timestamp = DateTime.UtcNow,
    environment = Environment.GetEnvironmentVariable("ASPNETCORE_ENVIRONMENT")
}));

// Auto migrate on startup — wrapped so a DB failure doesn't kill the whole app
try
{
    using var scope = app.Services.CreateScope();
    var logger = scope.ServiceProvider.GetRequiredService<ILogger<Program>>();
    var db = scope.ServiceProvider.GetRequiredService<AppDbContext>();

    logger.LogInformation("Applying database migrations...");
    await db.Database.MigrateAsync();
    logger.LogInformation("Migrations applied successfully.");

    // Seed roles (DISABLED: Using 'Role' column in Users table instead of separate Roles table)
    /*
    var roleManager = scope.ServiceProvider.GetRequiredService<RoleManager<IdentityRole<int>>>();
    foreach (var role in new[] { "Admin", "Customer", "Vendor" })
    {
        if (!await roleManager.RoleExistsAsync(role))
            await roleManager.CreateAsync(new IdentityRole<int>(role));
    }
    logger.LogInformation("Roles seeded successfully.");
    */
}
catch (Exception ex)
{
    // Log the error but don't crash the app — it may still serve non-DB endpoints
    var logger = app.Services.GetRequiredService<ILogger<Program>>();
    logger.LogError(ex, "Database migration/seeding failed on startup.");
}

app.Run();
