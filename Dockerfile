# Multi-stage Dockerfile for OrisMall .NET 8 API

# Base runtime image
FROM mcr.microsoft.com/dotnet/aspnet:8.0 AS base
WORKDIR /app
EXPOSE 80
EXPOSE 443

# Build stage
FROM mcr.microsoft.com/dotnet/sdk:8.0 AS build
WORKDIR /src

# Copy project files for better layer caching
COPY ["OrisMall.API/OrisMall.API.csproj", "OrisMall.API/"]
COPY ["OrisMall.Core/OrisMall.Core.csproj", "OrisMall.Core/"]
COPY ["OrisMall.Infrastructure/OrisMall.Infrastructure.csproj", "OrisMall.Infrastructure/"]
COPY ["OrisMall.MockPaymentGateway/OrisMall.MockPaymentGateway.csproj", "OrisMall.MockPaymentGateway/"]

# Restore dependencies
RUN dotnet restore "OrisMall.API/OrisMall.API.csproj"

# Copy all source code
COPY . .

# Build the application
WORKDIR "/src/OrisMall.API"
RUN dotnet build "OrisMall.API.csproj" -c Release -o /app/build

# Publish stage
FROM build AS publish
RUN dotnet publish "OrisMall.API.csproj" -c Release -o /app/publish /p:UseAppHost=false

# Final stage
FROM base AS final
WORKDIR /app
COPY --from=publish /app/publish .

# Create non-root user for security
RUN adduser --disabled-password --gecos '' appuser && chown -R appuser /app
USER appuser

ENTRYPOINT ["dotnet", "OrisMall.API.dll"]