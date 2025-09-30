using OrisMall.Core.DTOs;

namespace OrisMall.Core.Interfaces;

public interface IProductService
{
    Task<IEnumerable<ProductDto>> GetAllProductsAsync();
    Task<ProductDto?> GetProductByIdAsync(int id);
    Task<IEnumerable<ProductDto>> GetProductsByCategoryAsync(int categoryId);
    Task<IEnumerable<ProductDto>> SearchProductsAsync(string searchTerm);
    Task<ProductDto> CreateProductAsync(CreateProductDto createProductDto);
    Task<ProductDto?> UpdateProductAsync(int id, UpdateProductDto updateProductDto);
    Task<bool> DeleteProductAsync(int id);
    Task<bool> ProductExistsAsync(int id);
}
