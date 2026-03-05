using System.Text;
using Microsoft.AspNetCore.Authentication.JwtBearer;
using Microsoft.EntityFrameworkCore;
using Microsoft.IdentityModel.Tokens;
using Microsoft.OpenApi.Models;
using SafiStore.Api.Common.Helpers;
using SafiStore.Api.Data;
using SafiStore.Api.Infrastructure.Services;
using SafiStore.Api.Middleware;

var builder = WebApplication.CreateBuilder(args);

var configuration = builder.Configuration;

#region Controllers & JSON
builder.Services.AddControllers(opts =>
    {
        opts.Filters.Add<SafiStore.Api.Filters.ApiResponseResultFilter>();
    })
    .AddJsonOptions(opts =>
    {
        opts.JsonSerializerOptions.PropertyNamingPolicy =
            System.Text.Json.JsonNamingPolicy.CamelCase;

        opts.JsonSerializerOptions.DefaultIgnoreCondition =
            System.Text.Json.Serialization.JsonIgnoreCondition.WhenWritingNull;
    });
#endregion

#region Database
builder.Services.AddDbContext<ApplicationDbContext>(options =>
    options.UseSqlServer(builder.Configuration.GetConnectionString("DefaultConnection")));
#endregion

#region Application Services
builder.Services.AddScoped<IProductService, ProductService>();
builder.Services.AddScoped<ICartService, CartService>();
builder.Services.AddScoped<IOrderService, OrderService>();
builder.Services.AddScoped<IAuthService, AuthService>();
builder.Services.AddScoped<IJwtService, JwtService>();
builder.Services.AddScoped<IUserService, UserService>();
builder.Services.AddScoped<IReviewService, ReviewService>();
#endregion

#region Health Checks
builder.Services.AddHealthChecks();
#endregion

#region Performance
builder.Services.AddResponseCompression(options =>
{
    options.EnableForHttps = true;
});
#endregion

#region CORS
var allowedOrigins = configuration.GetSection("Cors:AllowedOrigins")
    .Get<string[]>() ?? new[] { "http://localhost:4200" };

if (allowedOrigins.Length == 0)
{
    throw new InvalidOperationException("CORS AllowedOrigins must not be empty when credentials are allowed.");
}

builder.Services.AddCors(options =>
{
    options.AddPolicy("DefaultCorsPolicy", policy =>
    {
        policy.WithOrigins(allowedOrigins)
              .AllowAnyHeader()
              .AllowAnyMethod();

        if (!allowedOrigins.Contains("*"))
        {
            policy.AllowCredentials();
        }
    });
});
#endregion

#region Swagger
builder.Services.AddEndpointsApiExplorer();

builder.Services.AddSwaggerGen(options =>
{
    options.SwaggerDoc("v1", new OpenApiInfo
    {
        Title = "SafiStore API",
        Version = "v1",
        Description = "Backend API for SafiStore"
    });

    options.AddSecurityDefinition("Bearer", new OpenApiSecurityScheme
    {
        Name = "Authorization",
        Type = SecuritySchemeType.Http,
        Scheme = "bearer",
        BearerFormat = "JWT",
        In = ParameterLocation.Header,
        Description = "Enter: Bearer {your JWT token}"
    });

    options.AddSecurityRequirement(new OpenApiSecurityRequirement
    {
        {
            new OpenApiSecurityScheme
            {
                Reference = new OpenApiReference
                {
                    Type = ReferenceType.SecurityScheme,
                    Id = "Bearer"
                }
            },
            Array.Empty<string>()
        }
    });
});
#endregion

#region JWT Authentication
var jwtSection = configuration.GetSection("Jwt");
// ── Security verification ──────────────────────────────────────────
var secretKey = jwtSection["Secret"];

if (string.IsNullOrWhiteSpace(secretKey) || secretKey.Contains("REPLACE_WITH_ENV_VAR"))
{
    throw new InvalidOperationException(
        "CRITICAL: JWT Secret is not configured or still using placeholder. " +
        "Ensure 'Jwt__Secret' environment variable is set in production.");
}

if (secretKey.Length < 32)
{
    throw new InvalidOperationException(
        "CRITICAL: Jwt:Secret is too short for HS256 (minimum 32 characters required).");
}
// ───────────────────────────────────────────────────────────────────

var issuer   = jwtSection["Issuer"]   ?? "SafiStoreApi";
var audience = jwtSection["Audience"] ?? "SafiStoreClient";

var key = Encoding.UTF8.GetBytes(secretKey);

builder.Services.AddAuthentication(options =>
{
    options.DefaultAuthenticateScheme = JwtBearerDefaults.AuthenticationScheme;
    options.DefaultChallengeScheme = JwtBearerDefaults.AuthenticationScheme;
})
.AddJwtBearer(options =>
{
    // ── Force HTTPS in production ────────────────────────────────────
    options.RequireHttpsMetadata = !builder.Environment.IsDevelopment();
    options.SaveToken = false; // Don't store raw token in AuthenticationProperties (security)
    options.TokenValidationParameters = new TokenValidationParameters
    {
        ValidateIssuerSigningKey = true,
        IssuerSigningKey        = new SymmetricSecurityKey(key),
        ValidateIssuer          = true,
        ValidIssuer             = issuer,
        ValidateAudience        = true,
        ValidAudience           = audience,
        ValidateLifetime        = true,
        RequireExpirationTime   = true,
        ClockSkew               = TimeSpan.Zero  // No grace period — expired = rejected immediately
    };
});

builder.Services.Configure<JwtSettings>(options =>
{
    options.Secret              = secretKey;
    options.Issuer              = issuer;
    options.Audience            = audience;
    options.AccessTokenExpiration  = jwtSection.GetValue<int>("AccessTokenExpiration", 15);
    options.RefreshTokenExpiration = jwtSection.GetValue<int>("RefreshTokenExpiration", 30);
});
#endregion

var app = builder.Build();

#region Middleware Pipeline

// DEV: detailed exception page
if (app.Environment.IsDevelopment())
{
    app.UseDeveloperExceptionPage();
}
else
{
    // The default HSTS value is 30 days. You may want to change this for production scenarios, see https://aka.ms/aspnetcore-hsts.
    app.UseHsts();
}

// 1) Global error handling (first middleware — catches everything below)
app.UseMiddleware<ErrorHandlingMiddleware>();


// 2️⃣ Infrastructure & Security
app.UseHttpsRedirection();
app.UseResponseCompression();
app.UseCors("DefaultCorsPolicy");
app.UseMiddleware<SecurityHeadersMiddleware>();

// 3️⃣ Authentication & Authorization
app.UseAuthentication();
app.UseAuthorization();

// 4️⃣ Response formatting (Disabled for performance: buffering middleware is a bottleneck)
// app.UseMiddleware<ResponseFormattingMiddleware>();

// 5️⃣ Swagger (DEV only)
if (app.Environment.IsDevelopment())
{
    app.UseSwagger();
    app.UseSwaggerUI(options =>
    {
        options.SwaggerEndpoint(
            "/swagger/v1/swagger.json",
            "SafiStore API v1");

        options.RoutePrefix = "swagger";
    });
}


// Endpoints
app.MapHealthChecks("/health");

// RISK-2 fix: requires Admin JWT — was previously anonymous, leaking traffic metrics
app.MapGet("/metrics", () => Results.Json(MetricsService.GetMetrics()))
   .RequireAuthorization(policy => policy.RequireRole("Admin"));


app.MapControllers();

app.Run();

#endregion
