using Microsoft.EntityFrameworkCore;
using Microsoft.AspNetCore.Authentication.JwtBearer;
using Microsoft.IdentityModel.Tokens;
using Microsoft.Extensions.Caching.Memory;
using System.Text;
using System.Security.Claims;
using OrisMall.Infrastructure.Data;
using OrisMall.Infrastructure.Repositories;
using OrisMall.Infrastructure.Services;
using OrisMall.Core.Interfaces;
using OrisMall.Core.Configuration;
using OrisMall.API.Middleware;
using Serilog;

var builder = WebApplication.CreateBuilder(args);

// ===== LOGGING CONFIGURATION =====
Log.Logger = new LoggerConfiguration()
    .ReadFrom.Configuration(builder.Configuration)
    .CreateLogger();
builder.Host.UseSerilog(); 

// ===== CORE SERVICES =====
builder.Services.AddControllers();
builder.Services.AddEndpointsApiExplorer();
builder.Services.AddHttpContextAccessor();

// ===== SESSION =====
builder.Services.AddDistributedMemoryCache(); // Register IDistributedCache in DI container
builder.Services.AddSession(options =>
{
    options.IdleTimeout = TimeSpan.FromMinutes(30);
    options.Cookie.HttpOnly = true;
    options.Cookie.IsEssential = true;
});

// ===== CACHING =====
// Load Memory cache configuration & Register IMemoryCache in DI container
builder.Services.AddMemoryCache(options =>
{
    var cacheConfig = builder.Configuration.GetSection(CacheOptions.SectionName).Get<CacheOptions>() ?? new CacheOptions();
    options.SizeLimit = cacheConfig.SizeLimit;
    options.CompactionPercentage = cacheConfig.CompactionPercentage;
});

// ===== SWAGGER DOCUMENTATION =====
builder.Services.AddSwaggerGen(c =>
{
    c.SwaggerDoc("v1", new Microsoft.OpenApi.Models.OpenApiInfo
    {
        Title = "OrisMall API",
        Version = "v1"
    });

    // Add JWT Authentication to Swagger
    c.AddSecurityDefinition("Bearer", new Microsoft.OpenApi.Models.OpenApiSecurityScheme
    {
        Description = "JWT Authorization header using the Bearer scheme. Example: \"Authorization: Bearer {token}\"",
        Name = "Authorization",
        In = Microsoft.OpenApi.Models.ParameterLocation.Header,
        Type = Microsoft.OpenApi.Models.SecuritySchemeType.ApiKey,
        Scheme = "Bearer"
    });

    c.AddSecurityRequirement(new Microsoft.OpenApi.Models.OpenApiSecurityRequirement
    {
        {
            new Microsoft.OpenApi.Models.OpenApiSecurityScheme
            {
                Reference = new Microsoft.OpenApi.Models.OpenApiReference
                {
                    Type = Microsoft.OpenApi.Models.ReferenceType.SecurityScheme,
                    Id = "Bearer"
                }
            },
            new string[] {}
        }
    });
});

// ===== REPOSITORIES =====
builder.Services.AddScoped<IProductRepository, ProductRepository>();
builder.Services.AddScoped<ICategoryRepository, CategoryRepository>();
builder.Services.AddScoped<IUserRepository, UserRepository>();
builder.Services.AddScoped<ICartRepository, CartRepository>();

// ===== BUSINESS SERVICES =====
// Concrete services (for dependency injection)
builder.Services.AddScoped<ProductService>();
builder.Services.AddScoped<CategoryService>();

// Cached services (decorators)
builder.Services.AddScoped<ICategoryService>(provider =>
{
    var categoryService = provider.GetRequiredService<CategoryService>();
    var cache = provider.GetRequiredService<IMemoryCache>();
    return new CachedCategoryService(categoryService, cache);
});
builder.Services.AddScoped<IProductService>(provider =>
{
    var productService = provider.GetRequiredService<ProductService>();
    var cache = provider.GetRequiredService<IMemoryCache>();
    return new CachedProductService(productService, cache);
});

// Other business services
builder.Services.AddScoped<IUserService, UserService>();
builder.Services.AddScoped<ICartService, CartService>();

// ===== PAYMENT SERVICES =====
if (builder.Environment.IsDevelopment())
{
    builder.Services.AddScoped<IPaymentService, OrisMall.MockPaymentGateway.MockPaymentGateway>();
}
else
{
    // TODO: For production, integrate with 3rd party payment gateway
    // builder.Services.AddScoped<IPaymentService, PayPalPaymentService>();
    // Use MockPaymentGateway to avoid application crash
    builder.Services.AddScoped<IPaymentService, OrisMall.MockPaymentGateway.MockPaymentGateway>();
}

// ===== INFRASTRUCTURE SERVICES =====
builder.Services.AddScoped<ILoggingService, LoggingService>();

// ===== SECURITY & AUTHENTICATION =====
builder.Services.AddAuthentication(JwtBearerDefaults.AuthenticationScheme)
    .AddJwtBearer(options =>
    {
        options.TokenValidationParameters = new TokenValidationParameters
        {
            ValidateIssuer = true,
            ValidateAudience = true,
            ValidateLifetime = true,
            ValidateIssuerSigningKey = true,
            ValidIssuer = builder.Configuration["Jwt:Issuer"],
            ValidAudience = builder.Configuration["Jwt:Audience"],
            IssuerSigningKey = new SymmetricSecurityKey(
                Encoding.UTF8.GetBytes(builder.Configuration["Jwt:Key"]!)),
            RoleClaimType = ClaimTypes.Role
        };
    });

builder.Services.AddAuthorization(options =>
{
    options.AddPolicy("AdminOnly", policy => policy.RequireRole("Admin"));
});

// CORS configuration
builder.Services.AddCors(options =>
{
    options.AddPolicy("AllowAll", policy =>
    {
        if (builder.Environment.IsDevelopment())
        {
            // Development: Allow all origins for testing
            policy.AllowAnyOrigin()
                  .AllowAnyMethod()
                  .AllowAnyHeader();
        }
        else
        {
            // Production: Only allow specific origins
            policy.WithOrigins("https://orismall.com")
                  .WithMethods("GET", "POST", "PUT", "DELETE")
                  .WithHeaders("Content-Type", "Authorization");
        }
    });
});

// ===== DATABASE =====
builder.Services.AddDbContext<OrisMallDbContext>(options =>
    options.UseSqlServer(builder.Configuration.GetConnectionString("DefaultConnection")));

var app = builder.Build();

// ===== HTTP REQUEST PIPELINE =====
// Development tools
if (app.Environment.IsDevelopment())
{
    app.UseSwagger();
    app.UseSwaggerUI();
}

// Security & routing
app.UseHttpsRedirection();
app.UseCors("AllowAll");

// Custom middleware
app.UseMiddleware<RequestResponseLoggingMiddleware>();
app.UseMiddleware<GlobalExceptionHandlingMiddleware>();

// Authentication & authorization
app.UseSession();
app.UseAuthentication();
app.UseAuthorization();

// Endpoints
app.MapControllers();

// Development-only test endpoints
if (app.Environment.IsDevelopment())
{
    // Test controllers are automatically mapped with [ApiController] attribute
    // No additional mapping needed
}

// ===== DATABASE INITIALIZATION =====
using (var scope = app.Services.CreateScope())
{
    var context = scope.ServiceProvider.GetRequiredService<OrisMallDbContext>();
    
    if (app.Environment.IsDevelopment())
    {
        // Development: Delete and recreate (safe for test data)
        context.Database.EnsureDeleted();
        context.Database.EnsureCreated();
    }
    else
    {
        // Production: Use migrations (preserves customer data)
        context.Database.Migrate();
    }
}

// ===== APPLICATION STARTUP =====
try
{
    Log.Information("Starting OrisMall API");
    app.Run();
}
catch (Exception ex)
{
    Log.Fatal(ex, "Application terminated unexpectedly");
}
finally
{
    Log.CloseAndFlush();
}