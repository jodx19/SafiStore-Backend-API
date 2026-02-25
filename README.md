📦 SafiStore Backend API

Production-ready ASP.NET Core Web API powering the SafiStore e-commerce platform.

🚀 Overview

SafiStore Backend is a secure, scalable, and production-hardened REST API built with:

ASP.NET Core 8

Entity Framework Core

SQL Server

JWT Authentication (Access + Refresh Tokens)

Role-Based Authorization (Admin / Customer)

Clean Architecture layering

DTO validation

Pagination support

Centralized error handling

Security headers middleware

🏗 Architecture
SafiStore.Api
│
├── Application
│   ├── DTOs
│   ├── Validators
│   └── Mappings
│
├── Controllers
├── Infrastructure
│   ├── Services
│   └── Repositories
│
├── Middleware
├── Data
├── Models
└── Common
Principles Used

Separation of Concerns

Strongly Typed DTOs

Fail-Fast Security Checks

Zero Breaking Changes policy

Clean Response Wrapper (ApiResponse<T>)

🔐 Authentication

JWT Access Token

Refresh Token support

Role-based route protection

Fail-fast startup if JWT secret is weak (< 32 chars)

Required Environment Variables
Jwt__Secret=your_super_secure_32+_char_secret
ConnectionStrings__DefaultConnection=your_connection_string

⚠️ The application will not start if the JWT secret is missing or invalid.

📌 API Features
🛍 Products

Get paginated products

Create / Update / Delete (Admin)

Category filtering

Reviews support

📦 Orders

Create order from cart

Admin pagination:

GET /api/v1/admin/orders?page=1&pageSize=20
👤 Users

Role-based authorization

Admin user management with pagination

🛒 Cart

Add / Update / Remove items

Quantity validation

Secure user ownership

🛡 Security Hardening

JWT Secret length validation (HS256 safe)

Centralized exception handling middleware

Secure headers middleware

Strong DTO validation attributes

Password minimum length enforced (10+ characters)

📊 Pagination Format
{
  "success": true,
  "data": {
    "items": [],
    "pagination": {
      "page": 1,
      "pageSize": 20,
      "total": 42,
      "totalPages": 3
    }
  }
}
🧪 Local Development
1️⃣ Restore
dotnet restore
2️⃣ Apply Migrations
dotnet ef database update
3️⃣ Run
dotnet run

API will be available at:

https://localhost:7111

Swagger:

https://localhost:7111/swagger
🐳 Docker Support

Build:

docker build -t safistore-api .

Run:

docker-compose up
🧹 Production Checklist

 Set environment variables (NO secrets in appsettings.json)

 Use HTTPS only

 Configure CORS properly

 Enable logging provider (Serilog recommended)

 Run migrations before deployment

 Verify JWT secret length ≥ 32 chars

 Configure reverse proxy (Nginx / IIS / Azure / Render)

🧠 Deployment Notes

The project is compatible with:

Render

Railway

Azure App Service

Docker-based hosting

VPS with reverse proxy

Make sure:

ASPNETCORE_ENVIRONMENT=Production

HTTPS is enabled

Database connection string is secure

📄 License

MIT License

👨‍💻 Author

SafiStore Backend API
Built with production standards and security-first design.
