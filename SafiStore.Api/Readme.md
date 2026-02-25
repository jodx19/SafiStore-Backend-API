# 🛒 SafiStore API - E-commerce Backend System

SafiStore is a robust, scalable Backend API built with **ASP.NET Core** and **EF Core**, following clean architecture principles. It handles the complete e-commerce lifecycle from user authentication to order processing and product reviews.

## 🚀 Key Features

* **Secure Authentication**: Integrated JWT Token-based authentication for secure user sessions.
* **Advanced Ordering System**: Implements **Database Transactions** to ensure atomic operations (deducting stock, clearing cart, and creating orders).
* **Smart Review System**: A logic-driven review system that only allows users to review products they have successfully purchased.
* **Data Integrity**: Full relational database schema with optimized SQL queries and JOINs.
* **Global Exception Handling**: Custom middlewares to handle errors and return consistent API responses.

## 🛠️ Tech Stack

* **Framework**: .NET 8 / ASP.NET Core Web API
* **ORM**: Entity Framework Core
* **Database**: SQL Server (SSMS)
* **Documentation**: Swagger & Postman

## 🏗️ Database Schema
The project uses a structured relational model including:
* **Users**: Identity management.
* **Products**: Inventory and stock management.
* **Orders & OrderItems**: Tracking sales and item details.
* **Reviews**: User feedback linked to purchases.

## 📖 API Documentation (Postman)
You can explore and test the API endpoints through the published Postman documentation:
🔗 **[رابط الـ Postman Documentation بتاعك هنا]**

## 📸 Screenshots

### 1. Successful Order Processing (200 OK)
![Order Success]
*Shows the transaction-safe order creation flow.*

### 2. Product Reviews with User Identity
![Review Success]
*Displays user reviews with full names fetched via SQL Joins.*

## 🏁 Getting Started

1. Clone the repository: `git clone https://github.com/YourUsername/SafiStore-Backend-API.git`
2. Update the connection string in `appsettings.json`.
3. Run `Update-Database` in Package Manager Console.
4. Hit `F5` to run the project.

---
Developed with ❤️ by Mahmoud Mostafa