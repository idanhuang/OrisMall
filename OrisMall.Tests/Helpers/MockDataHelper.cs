using Microsoft.AspNetCore.Http;
using Moq;
using OrisMall.Core.DTOs;
using OrisMall.Core.Entities;

namespace OrisMall.Tests.Helpers;

public static class MockDataHelper
{
    public static List<CategoryDto> GetMockCategoryDtos()
    {
        return new List<CategoryDto>
        {
            new CategoryDto
            {
                Id = 1,
                Name = "Electronics",
                Description = "Electronic devices",
                ImageUrl = "https://example.com/electronics.jpg",
                IsActive = true,
                CreatedAt = DateTime.UtcNow.AddDays(-30)
            },
            new CategoryDto
            {
                Id = 2,
                Name = "Clothing",
                Description = "Fashion and apparel",
                ImageUrl = "https://example.com/clothing.jpg",
                IsActive = true,
                CreatedAt = DateTime.UtcNow.AddDays(-25)
            }
        };
    }

    public static List<ProductDto> GetMockProductDtos()
    {
        return new List<ProductDto>
        {
            new ProductDto
            {
                Id = 1,
                Name = "Smartphone",
                Description = "Smartphone with advanced features",
                Price = 699.99m,
                StockQuantity = 50,
                SKU = "PHONE001",
                ImageUrl = "https://example.com/smartphone.jpg",
                IsActive = true,
                CreatedAt = DateTime.UtcNow.AddDays(-15),
                CategoryId = 1,
                CategoryName = "Electronics"
            },
            new ProductDto
            {
                Id = 2,
                Name = "Laptop",
                Description = "Laptop for work and gaming",
                Price = 1299.99m,
                StockQuantity = 25,
                SKU = "LAPTOP001",
                ImageUrl = "https://example.com/laptop.jpg",
                IsActive = true,
                CreatedAt = DateTime.UtcNow.AddDays(-10),
                CategoryId = 1,
                CategoryName = "Electronics"
            }
        };
    }

    // Entity Mock Data - For service testing
    public static List<Category> GetMockCategories()
    {
        return new List<Category>
        {
            new Category
            {
                Id = 1,
                Name = "Electronics",
                Description = "Electronic devices",
                ImageUrl = "https://example.com/electronics.jpg",
                IsActive = true,
                CreatedAt = DateTime.UtcNow.AddDays(-30),
                UpdatedAt = null
            },
            new Category
            {
                Id = 2,
                Name = "Clothing",
                Description = "Fashion and apparel",
                ImageUrl = "https://example.com/clothing.jpg",
                IsActive = true,
                CreatedAt = DateTime.UtcNow.AddDays(-25),
                UpdatedAt = null
            }
        };
    }

    public static List<Product> GetMockProducts()
    {
        return new List<Product>
        {
            new Product
            {
                Id = 1,
                Name = "Smartphone",
                Description = "Latest smartphone",
                Price = 699.99m,
                StockQuantity = 50,
                SKU = "PHONE001",
                ImageUrl = "https://example.com/smartphone.jpg",
                IsActive = true,
                CreatedAt = DateTime.UtcNow.AddDays(-15),
                UpdatedAt = null,
                CategoryId = 1
            },
            new Product
            {
                Id = 2,
                Name = "Laptop",
                Description = "Laptop for work and gaming",
                Price = 1299.99m,
                StockQuantity = 25,
                SKU = "LAPTOP001",
                ImageUrl = "https://example.com/laptop.jpg",
                IsActive = true,
                CreatedAt = DateTime.UtcNow.AddDays(-10),
                UpdatedAt = null,
                CategoryId = 1
            },
            new Product
            {
                Id = 3,
                Name = "T-Shirt",
                Description = "Cotton t-shirt",
                Price = 19.99m,
                StockQuantity = 100,
                SKU = "TSHIRT001",
                ImageUrl = "https://example.com/tshirt.jpg",
                IsActive = true,
                CreatedAt = DateTime.UtcNow.AddDays(-5),
                UpdatedAt = null,
                CategoryId = 2
            }
        };
    }

    public static List<User> GetMockUsers()
    {
        return new List<User>
        {
            new User
            {
                Id = 1,
                Email = "dan.huang@example.com",
                FirstName = "Dan",
                LastName = "Huang",
                PasswordHash = BCrypt.Net.BCrypt.HashPassword("password123", BCrypt.Net.BCrypt.GenerateSalt(12)), // BCrypt hash of "password123"
                PhoneNumber = "+1234567890",
                IsActive = true,
                IsEmailVerified = true,
                Role = "User",
                CreatedAt = DateTime.UtcNow.AddDays(-30),
                LastLoginAt = DateTime.UtcNow.AddDays(-1)
            },
            new User
            {
                Id = 2,
                Email = "admin@example.com",
                FirstName = "Admin",
                LastName = "User",
                PasswordHash = BCrypt.Net.BCrypt.HashPassword("admin123", BCrypt.Net.BCrypt.GenerateSalt(12)), // BCrypt hash of "admin123"
                PhoneNumber = "+1234567891",
                IsActive = true,
                IsEmailVerified = true,
                Role = "Admin",
                CreatedAt = DateTime.UtcNow.AddDays(-60),
                LastLoginAt = DateTime.UtcNow.AddHours(-2)
            }
        };
    }

    public static HttpContext CreateMockHttpContext(
        string method = "GET",
        string path = "/api/products",
        int statusCode = 200,
        Dictionary<string, string>? headers = null)
    {
        var context = new Mock<HttpContext>();
        var request = new Mock<HttpRequest>();
        var response = new Mock<HttpResponse>();
        var headerDict = new Mock<IHeaderDictionary>();
        var connection = new Mock<ConnectionInfo>();

        // Setup request
        request.Setup(r => r.Method).Returns(method);
        request.Setup(r => r.Path).Returns(new PathString(path));
        request.Setup(r => r.QueryString).Returns(new QueryString());
        request.Setup(r => r.Headers).Returns(headerDict.Object);

        // Setup headers
        var defaultHeaders = new Dictionary<string, string>
        {
            ["Accept"] = "application/json",
            ["User-Agent"] = "Mozilla/5.0 (Test Browser)",
            ["Host"] = "localhost:5000"
        };

        if (headers != null)
        {
            foreach (var header in headers)
            {
                defaultHeaders[header.Key] = header.Value;
            }
        }

        foreach (var header in defaultHeaders)
        {
            headerDict.Setup(h => h[header.Key]).Returns(new Microsoft.Extensions.Primitives.StringValues(header.Value));
        }

        headerDict.Setup(h => h.GetEnumerator()).Returns(
            defaultHeaders.Select(kvp => new KeyValuePair<string, Microsoft.Extensions.Primitives.StringValues>(kvp.Key, kvp.Value)).GetEnumerator());

        // Setup response
        response.Setup(r => r.StatusCode).Returns(statusCode);
        response.Setup(r => r.Headers).Returns(new Mock<IHeaderDictionary>().Object);
        response.Setup(r => r.ContentType).Returns("application/json; charset=utf-8");
        response.Setup(r => r.HasStarted).Returns(false);

        // Setup connection
        connection.Setup(c => c.RemoteIpAddress).Returns(System.Net.IPAddress.Parse("127.0.0.1"));

        // Setup context
        context.Setup(c => c.Request).Returns(request.Object);
        context.Setup(c => c.Response).Returns(response.Object);
        context.Setup(c => c.Connection).Returns(connection.Object);
        context.Setup(c => c.Items).Returns(new Dictionary<object, object?>());

        return context.Object;
    }

    // Cart Mock Data
    public static AddToCartDto GetMockAddToCartDto()
    {
        return new AddToCartDto
        {
            ProductId = 1,
            Quantity = 2
        };
    }
}