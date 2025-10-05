using OrisMall.Core.DTOs;
using OrisMall.Core.Entities;
using OrisMall.Core.Interfaces;

namespace OrisMall.Infrastructure.Services;

public class ProductService : IProductService
{
    private readonly IProductRepository _productRepository;
    private readonly ICategoryRepository _categoryRepository;

    public ProductService(IProductRepository productRepository, ICategoryRepository categoryRepository)
    {
        _productRepository = productRepository;
        _categoryRepository = categoryRepository;
    }

    public async Task<IEnumerable<ProductDto>> GetAllProductsAsync()
    {
        var products = await _productRepository.GetAllAsync();
        return products.Select(MapToDto);
    }

    public async Task<ProductDto?> GetProductByIdAsync(int id)
    {
        var product = await _productRepository.GetByIdAsync(id);
        return product != null ? MapToDto(product) : null;
    }

    public async Task<IEnumerable<ProductDto>> GetProductsByCategoryAsync(int categoryId)
    {
        var products = await _productRepository.GetByCategoryIdAsync(categoryId);
        return products.Select(MapToDto);
    }

    public async Task<IEnumerable<ProductDto>> SearchProductsAsync(string searchTerm)
    {
        var products = await _productRepository.SearchAsync(searchTerm);
        return products.Select(MapToDto);
    }

    public async Task<ProductDto> CreateProductAsync(CreateProductDto createProductDto)
    {
        var category = await _categoryRepository.GetByIdAsync(createProductDto.CategoryId);
        if (category == null)
            throw new ArgumentException("Category not found");

        var product = new Product
        {
            Name = createProductDto.Name,
            Description = createProductDto.Description,
            Price = createProductDto.Price,
            StockQuantity = createProductDto.StockQuantity,
            SKU = createProductDto.SKU,
            ImageUrl = createProductDto.ImageUrl,
            CategoryId = createProductDto.CategoryId
        };

        var createdProduct = await _productRepository.AddAsync(product);
        return MapToDto(createdProduct);
    }

    public async Task<ProductDto> UpdateProductAsync(int id, UpdateProductDto updateProductDto)
    {
        var product = await _productRepository.GetByIdAsync(id);
        if (product == null)
            throw new ArgumentException("Product not found");

        var category = await _categoryRepository.GetByIdAsync(updateProductDto.CategoryId);
        if (category == null)
            throw new ArgumentException("Category not found");

        product.Name = updateProductDto.Name;
        product.Description = updateProductDto.Description;
        product.Price = updateProductDto.Price;
        product.StockQuantity = updateProductDto.StockQuantity;
        product.SKU = updateProductDto.SKU;
        product.ImageUrl = updateProductDto.ImageUrl;
        product.CategoryId = updateProductDto.CategoryId;

        var updatedProduct = await _productRepository.UpdateAsync(product);
        return MapToDto(updatedProduct);
    }

    public async Task DeleteProductAsync(int id)
    {
        // Check if product exists
        var product = await _productRepository.GetByIdAsync(id);
        if (product == null)
            throw new ArgumentException("Product not found");

        await _productRepository.DeleteAsync(id);
    }


    private static ProductDto MapToDto(Product product)
    {
        return new ProductDto
        {
            Id = product.Id,
            Name = product.Name,
            Description = product.Description,
            Price = product.Price,
            StockQuantity = product.StockQuantity,
            SKU = product.SKU,
            ImageUrl = product.ImageUrl,
            CategoryId = product.CategoryId,
            CategoryName = product.Category?.Name ?? string.Empty,
            CreatedAt = product.CreatedAt
        };
    }

    public async Task<(IEnumerable<ProductDto> Items, int TotalCount)> FilterProductsAsync(ProductFilterDto filter)
    {
        var (items, total) = await _productRepository.FilterAsync(
            filter.Name,
            filter.CategoryId,
            filter.MinPrice,
            filter.MaxPrice,
            filter.InStock,
            filter.SortBy,
            filter.SortDirection,
            filter.Page,
            filter.PageSize);

        return (items.Select(MapToDto), total);
    }
}

