# OrisMall E-Commerce API

A scalable, multi-tier C# ASP.NET Core service with RESTful APIs for an e-commerce platform, built with Clean Architecture and .NET 8.

## 🏗️ Architecture

```
OrisMall.sln
├── OrisMall.API/           # Web API Layer
├── OrisMall.Core/          # Business Logic Layer  
├── OrisMall.Infrastructure/ # Data Access Layer
├── OrisMall.MockPaymentGateway/ # Payment Gateway (Mock)
└── OrisMall.Tests/         # Test Project
```

## 🚀 Features

- **Product Management** - CRUD operations
- **Category Management** - CRUD operations
- **Shopping Cart Management** - Session-based cart operations
- **User Management** - Registration, login, authentication
- **Payment Processing Management** - Mock payment gateway
- **Caching** - In-memory caching with LRU eviction
- **Logging** - Global comprehensive HTTP request logging with performance monitoring


## 🚀 Setup & Run

### Prerequisites Check
- **Microsoft SQL Server Express LocalDB Installed**:
  ```bash
  # Check if LocalDB is installed
  sqllocaldb info
  ```
  - **If not installed**: Download SQL Server Express LocalDB from [Microsoft](https://docs.microsoft.com/en-us/sql/database-engine/configure-windows/sql-server-express-localdb)

- **Port 5000 free**: 
  ```bash
  netstat -ano | findstr :5000
  ```
  *(Should return nothing)*
  
  **If port is in use:**
  ```bash
  taskkill /PID [process_id] /F
  ```
  *(Replace [process_id] with actual PID from netstat output)*

### Run Application

#### Option 1: Pre-built Executable (Easiest - No Development Setup Required)
```bash
# Download the build folder from this repository

# Start LocalDB
sqllocaldb start mssqllocaldb

# Environment Setup
$env:ASPNETCORE_ENVIRONMENT="Development" (for PowerShell)
set ASPNETCORE_ENVIRONMENT=Development (for Command Prompt)

# Navigate to publish directory and run the application
cd publish
OrisMall.API.exe

# Access Swagger UI: http://localhost:5000/swagger
```

#### Option 2: Visual Studio (Developer GUI)
1. **Open Solution**: Open `OrisMall.sln` in Visual Studio
2. **Start LocalDB**: `sqllocaldb start mssqllocaldb`
3. **Set Startup Project**: Right-click `OrisMall.API` → Set as Startup Project
4. **Run**: Press F5 or click Start Debugging
5. **Access Swagger UI**: `https://localhost:5001/swagger`

**Note**: Visual Studio defaults to the HTTPS profile (port 5001) for secure development

#### Option 3: Run from Source Code (.NET 8 SDK Required)
```bash
# Clone and navigate
git clone <repository-url>
cd <your-local-repo-name>

# Start LocalDB
sqllocaldb start mssqllocaldb

# Restore and build
dotnet restore
dotnet build

# Run the API
dotnet run --project OrisMall.API

# Access Swagger UI: http://localhost:5000/swagger
```

#### Option 4: Build Executable (Advanced/Deployment)
```bash
# Clone and navigate
git clone <repository-url>
cd <your-local-repo-name>

# Start LocalDB
sqllocaldb start mssqllocaldb

# Restore dependencies
dotnet restore

# Build executable for your platform
dotnet publish OrisMall.API -c Release -o ./publish

# Environment Setup
$env:ASPNETCORE_ENVIRONMENT="Development" (for PowerShell)
set ASPNETCORE_ENVIRONMENT=Development (for Command Prompt)

# Run the executable
cd publish
OrisMall.API.exe

# Access Swagger UI: http://localhost:5000/swagger
```

#### Option 5: Docker (Recommended for Production)
```bash
# Navigate to project directory
cd <your-local-repo-name>

# Build and run with Docker Compose
docker-compose up --build

# Access: http://localhost:8080/swagger
# SQL Server: localhost:1433 (sa/Test123!)

# Stop containers
docker-compose down
```

## 🐳 Docker

**Prerequisites:** Docker Desktop, ports 8080 & 1433 available

**Quick Commands:**
```bash
# View logs
docker-compose logs

# Restart API
docker-compose restart orismall-api

# Clean restart
docker-compose down && docker-compose up --build
```

**Note:** First startup takes 2-3 minutes (SQL Server initialization)

## 🛠️ Technology Stack

- **.NET 8** - ASP.NET Core Web API
- **Entity Framework Core** - ORM with SQL Server LocalDB/Docker
- **Docker & Docker Compose** - Containerization
- **JWT** - Authentication
- **Serilog** - Logging
- **xUnit + Moq** - Testing

## 📋 Important Notes
- **Database**: LocalDB (data lost on restart) or Docker SQL Server (persistent)
- **Security**: Change JWT key and SQL password (`Test123!`) for production