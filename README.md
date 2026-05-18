<p align="center">
  <img src="https://img.shields.io/badge/ASP.NET_Core_8-512BD4?style=for-the-badge&logo=dotnet&logoColor=white" alt="ASP.NET Core 8" />
  <img src="https://img.shields.io/badge/Entity_Framework_Core-512BD4?style=for-the-badge&logo=entity-framework&logoColor=white" alt="EF Core" />
  <img src="https://img.shields.io/badge/SQL_Server-CC2927?style=for-the-badge&logo=microsoft-sql-server&logoColor=white" alt="SQL Server" />
  <img src="https://img.shields.io/badge/JWT_Auth-000000?style=for-the-badge&logo=json-web-tokens&logoColor=white" alt="JWT Auth" />
  <img src="https://img.shields.io/badge/Docker-2496ED?style=for-the-badge&logo=docker&logoColor=white" alt="Docker" />
</p>

<h1 align="center">🛡️ SafiStore — Backend API</h1>

<p align="center">
  <strong>Production-ready RESTful API powering a modern e-commerce platform</strong>
  <br />
  Secure · Scalable · Clean Architecture
</p>

---

## 📋 Table of Contents

- [Overview](#-overview)
- [Tech Stack](#-tech-stack)
- [Architecture](#-architecture)
- [Authentication & Authorization](#-authentication--authorization)
- [API Endpoints](#-api-endpoints)
- [Database Schema](#-database-schema)
- [Getting Started](#-getting-started)
- [Docker Support](#-docker-support)
- [Environment Variables](#-environment-variables)
- [Production Checklist](#-production-checklist)
- [Deployment](#-deployment)
- [Security](#-security)

---

## 🚀 Overview

SafiStore Backend is a **production-hardened** REST API built with ASP.NET Core 8. It powers the full e-commerce lifecycle — from product browsing and cart management to order processing, reviews, and an admin panel.

### Key Features

| Feature | Description |
|---------|-------------|
| 🔐 **JWT Auth** | Access + Refresh tokens with automatic expiry handling |
| 👥 **Role-Based Access** | Admin / Customer roles with `[Authorize]` filters |
| 🛍️ **Product Management** | CRUD with pagination, search, category filtering |
| 🛒 **Cart System** | Add/update/remove items with quantity validation |
| 📦 **Order Processing** | Transactional order creation with stock deduction |
| ⭐ **Reviews** | Purchase-verified reviews with ratings |
| 🛡️ **Rate Limiting** | 100 req/min general, 5 req/min on auth endpoints |
| 📊 **Health Checks** | Database connectivity monitoring |
| 🐳 **Docker Support** | Multi-stage build ready |

---

## 🛠 Tech Stack

| Layer | Technology |
|-------|-----------|
| **Runtime** | ASP.NET Core 8 |
| **ORM** | Entity Framework Core 8 |
| **Database** | SQL Server (localdb / production) |
| **Auth** | JWT Bearer + ASP.NET Core Identity |
| **API Docs** | Swagger / OpenAPI |
| **Rate Limiting** | AspNetCoreRateLimit |
| **Containerization** | Docker, Docker Compose |
| **Deployment** | Render / Railway / Azure / Monster |

---

## 🏗 Architecture

The project follows **Clean Architecture** principles with clear separation of concerns:

```
SafiStore.Api/
├── Application/           # DTOs, Validators, Mappings
│   ├── DTOs/             # Request/Response data transfer objects
│   └── Validators/       # FluentValidation rules
├── Common/               # Shared utilities & extensions
├── Controllers/          # API endpoints
│   ├── AuthController    # Registration, login, profile
│   ├── ProductsController# Product CRUD + categories
│   ├── CartController    # Cart management
│   ├── OrdersController  # Order placement & history
│   ├── ReviewsController # Product reviews
│   └── AdminController   # Admin operations
├── Data/                 # DbContext, Migrations, DbSeeder
├── Filters/              # Action filters
├── Infrastructure/       # Business logic services
│   └── Services/         # JwtService, ProductService, CartService,
│                         # OrderService, ReviewService, UserService
├── Middleware/           # Custom middleware pipeline
├── Models/               # Domain entities
│   └── Domain/           # Product, Order, Cart, Review, etc.
└── Program.cs            # App startup & DI configuration
```

### Design Principles

- **Separation of Concerns** — Each layer has a single responsibility
- **Strongly Typed DTOs** — No raw `IQueryable` exposure
- **Fail-Fast Security** — Startup validation of JWT secret length
- **Consistent Responses** — Unified `ApiResponse<T>` envelope
- **Transaction Safety** — Order creation uses DB transactions

---

## 🔐 Authentication & Authorization

### Flow

```
Register / Login
      ↓
JWT Access Token (60 min) + Refresh Token (7 days)
      ↓
[Authorize] endpoints extract claims from Bearer token
      ↓
Role-based access via ClaimTypes.Role
```

### Implementation

- **JWT Generation**: `JwtService.GenerateAccessToken()` adds `ClaimTypes.NameIdentifier`, `ClaimTypes.Email`, `ClaimTypes.Role`
- **Token Refresh**: `POST /api/v1/auth/refresh` validates expired token and issues new pair
- **Role Sync**: `Users.Role` string is synced with ASP.NET Identity roles on registration/login
- **Security**: Clock skew is zeroed; expired tokens receive `Token-Expired` header

### Environment Variables

| Variable | Required | Description |
|----------|----------|-------------|
| `JwtSettings:SecretKey` | ✅ | JWT signing key (min 32 chars) |
| `JWT_SECRET` | ✅ | Alternative env var for JWT secret |
| `ConnectionStrings:DefaultConnection` | ✅ | SQL Server connection string |

---

## 📡 API Endpoints

### Authentication `/api/v1/auth`

| Method | Endpoint | Auth | Description |
|--------|----------|------|-------------|
| `POST` | `/register` | — | Register new customer |
| `POST` | `/login` | — | Login, returns JWT + refresh token |
| `POST` | `/refresh` | — | Refresh expired access token |
| `POST` | `/logout` | ✅ | Invalidate refresh token |
| `GET` | `/me` | ✅ | Get current user profile |
| `POST` | `/forgot-password` | — | Generate password reset token |
| `POST` | `/reset-password` | — | Reset password with token |
| `POST` | `/change-password` | ✅ | Change password (requires current) |
| `PUT` | `/profile` | ✅ | Update profile fields |

### Products `/api/v1/products`

| Method | Endpoint | Auth | Description |
|--------|----------|------|-------------|
| `GET` | `/` | — | List products (pagination, search, category) |
| `GET` | `/{id}` | — | Get product by ID |
| `GET` | `/categories` | — | List all categories |
| `POST` | `/` | Admin | Create product |
| `PUT` | `/{id}` | Admin | Update product |
| `DELETE` | `/{id}` | Admin | Soft-delete product |

### Cart `/api/v1/cart`

| Method | Endpoint | Auth | Description |
|--------|----------|------|-------------|
| `GET` | `/` | ✅ | Get user's cart with items |
| `POST` | `/items` | ✅ | Add item to cart |
| `PUT` | `/items/{id}` | ✅ | Update item quantity |
| `DELETE` | `/items/{id}` | ✅ | Remove item from cart |
| `DELETE` | `/` | ✅ | Clear entire cart |

### Orders `/api/v1/orders`

| Method | Endpoint | Auth | Description |
|--------|----------|------|-------------|
| `POST` | `/` | ✅ | Create order from cart (transactional) |
| `GET` | `/{id}` | ✅ | Get order by ID (owner or admin) |
| `GET` | `/` | ✅ | List user's orders |

### Reviews `/api/v1/reviews`

| Method | Endpoint | Auth | Description |
|--------|----------|------|-------------|
| `GET` | `/product/{productId}` | — | Get product reviews (paginated) |
| `GET` | `/product/{productId}/summary` | — | Get average rating + count |
| `GET` | `/my-reviews` | ✅ | Get current user's reviews |
| `POST` | `/` | ✅ | Add review (purchase verified) |
| `PUT` | `/{id}` | ✅ | Update own review |
| `DELETE` | `/{id}` | ✅ | Delete own review |

### Admin `/api/v1/admin`

| Method | Endpoint | Auth | Description |
|--------|----------|------|-------------|
| `GET` | `/orders` | Admin | List all orders (paginated) |
| `PUT` | `/orders/{id}/status` | Admin | Update order status |
| `POST` | `/orders/{id}/cancel` | Admin | Cancel order (restores stock) |
| `GET` | `/users` | Admin | List all users (paginated) |
| `DELETE` | `/users/{id}` | Admin | Delete user (checks constraints) |
| `POST` | `/users` | Admin | Create new admin user |

### Health & Utility

| Method | Endpoint | Description |
|--------|----------|-------------|
| `GET` | `/health` | Health check with DB context status |
| `GET` | `/ping` | Simple alive check |
| `GET` | `/swagger` | Swagger UI |

---

## 🗄 Database Schema

```
Users ──┬── Orders ── OrderItems ── Product
        │                          
        └── Reviews ─────────────── Product
        └── Cart ─── CartItems ─── Product

Category ── Products
```

### Key Entities

| Entity | Table | Key Relationships |
|--------|-------|-------------------|
| `ApplicationUser` | `Users` | HasMany Orders, Reviews |
| `Product` | `Products` | BelongsTo Category, HasMany Reviews |
| `Order` | `Orders` | BelongsTo User, HasMany OrderItems |
| `OrderItem` | `OrderItems` | BelongsTo Order, BelongsTo Product |
| `Cart` | `Carts` | BelongsTo User, HasMany CartItems |
| `CartItem` | `CartItems` | BelongsTo Cart, BelongsTo Product |
| `Review` | `Reviews` | BelongsTo Product, BelongsTo User |

---

## 🧪 Getting Started

### Prerequisites

- [.NET 8 SDK](https://dotnet.microsoft.com/download/dotnet/8.0)
- [SQL Server](https://www.microsoft.com/en-us/sql-server/sql-server-downloads) (LocalDB or full instance)
- [Docker](https://www.docker.com/) (optional)

### Local Development

```bash
# 1. Clone the repository
git clone https://github.com/jodx19/SafiStore-Backend-API.git
cd SafiStore-Backend-API

# 2. Restore dependencies
dotnet restore

# 3. Update database (creates/migrates localdb)
dotnet ef database update

# 4. Run the application
dotnet run
```

The API will be available at:
- **HTTPS**: `https://localhost:7111`
- **HTTP**: `http://localhost:5084`
- **Swagger**: `https://localhost:7111/swagger`

### Docker

```bash
# Build
docker build -t safistore-api .

# Run with Docker Compose
docker-compose up
```

### Default Admin Account

When the database is seeded for the first time, a default admin account is created:

| Field | Value |
|-------|-------|
| **Email** | `admin@safistore.com` |
| **Password** | `Admin@123` |

> ⚠️ **Change this password immediately after first login.**

---

## 🔧 Environment Variables

Create an `appsettings.Production.json` or set environment variables:

```json
{
  "ConnectionStrings": {
    "DefaultConnection": "Server=your-server;Database=SafiStore;User Id=...;Password=...;"
  },
  "JwtSettings": {
    "SecretKey": "your-32-char-min-secret-key-here!!",
    "Issuer": "SafiStoreAPI",
    "Audience": "SafiStoreClient",
    "AccessTokenExpiryMinutes": 60,
    "RefreshTokenExpiryDays": 7
  },
  "Cors": {
    "AllowedOrigins": ["https://safistore.vercel.app"]
  }
}
```

Or use environment variables (useful for IIS/containers):

```bash
ConnectionStrings__DefaultConnection="..."
JwtSettings__SecretKey="..."
ASPNETCORE_ENVIRONMENT=Production
```

---

## ✅ Production Checklist

- [ ] Set `ASPNETCORE_ENVIRONMENT=Production`
- [ ] Configure JWT secret (min 32 chars) via environment variable
- [ ] Set production database connection string (secure)
- [ ] Configure CORS with specific frontend URL
- [ ] Enable HTTPS with valid certificate
- [ ] Configure reverse proxy (Nginx / IIS / Azure)
- [ ] Set up logging provider (Serilog recommended)
- [ ] Run database migrations before deployment
- [ ] Remove any hardcoded secrets from config files
- [ ] Enable rate limiting (already configured by default)

---

## 🚀 Deployment

The API is compatible with:

| Platform | Notes |
|----------|-------|
| **IIS / Windows Server** | Use dotnet-hosting bundle |
| **Azure App Service** | Configure connection strings in portal |
| **Docker / Container** | Multi-stage build included |
| **Linux / Nginx** | Use Kestrel behind reverse proxy |
| **Render / Railway** | Set env vars in dashboard |

---

## 🛡 Security

- **JWT Validation**: HS256 signing, issuer/audience validation, zero clock skew
- **Password Policy**: Min 8 chars, requires uppercase, digit, and special character
- **Rate Limiting**: 100 req/min general, 5 req/min on auth endpoints
- **CORS**: Restricted to specific origins
- **Error Handling**: Production errors never expose stack traces
- **SQL Injection**: EF Core parameterized queries
- **XSS**: Anti-forgery headers via middleware
- **Soft Deletes**: Products use `IsDeleted` flag instead of hard deletion

---

## 📄 License

This project is licensed under the MIT License.

---

<p align="center">
  <br />
  Made with 💜 by <strong>الصافي</strong>
  <br />
  <sub>Built with production standards and security-first design.</sub>
</p>
