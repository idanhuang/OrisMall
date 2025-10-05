# Use the official .NET 8 runtime as base image
FROM mcr.microsoft.com/dotnet/aspnet:8.0 AS base
WORKDIR /app
EXPOSE 8080
EXPOSE 8081

# Use the official .NET 8 SDK for building
FROM mcr.microsoft.com/dotnet/sdk:8.0 AS build
WORKDIR /src

# Copy project files
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

# Publish the application
FROM build AS publish
RUN dotnet publish "OrisMall.API.csproj" -c Release -o /app/publish /p:UseAppHost=false

# Final stage/image
FROM base AS final
WORKDIR /app

# Create logs directory
RUN mkdir -p /app/logs

# Copy published application
COPY --from=publish /app/publish .

# Set environment variables
ENV ASPNETCORE_ENVIRONMENT=Production
ENV ASPNETCORE_URLS=http://+:8080

# Create non-root user for security
RUN adduser --disabled-password --gecos '' appuser && chown -R appuser /app
USER appuser

# Health check
HEALTHCHECK --interval=30s --timeout=3s --start-period=5s --retries=3 \
  CMD curl -f http://localhost:8080/health || exit 1

# Start the application
ENTRYPOINT ["dotnet", "OrisMall.API.dll"]
