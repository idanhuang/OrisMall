using Moq;
using OrisMall.Core.Interfaces;
using OrisMall.Infrastructure.Services;
using OrisMall.Tests.Helpers;

namespace OrisMall.Tests.Services;

public class ProductServiceTests
{
    private readonly Mock<IProductRepository> _mockProductRepository;
    private readonly Mock<ICategoryRepository> _mockCategoryRepository;
    private readonly ProductService _productService;

    public ProductServiceTests()
    {
        _mockProductRepository = new Mock<IProductRepository>();
        _mockCategoryRepository = new Mock<ICategoryRepository>();
        _productService = new ProductService(_mockProductRepository.Object, _mockCategoryRepository.Object);
    }

    [Fact]
    public async Task GetAllProductsAsync_ShouldReturnAllProducts_WhenProductsExist()
    {
        // Arrange
        var mockProducts = MockDataHelper.GetMockProducts();
        _mockProductRepository.Setup(r => r.GetAllAsync()).ReturnsAsync(mockProducts);

        // Act
        var result = await _productService.GetAllProductsAsync();

        // Assert
        Assert.NotNull(result);
        Assert.Equal(mockProducts.Count, result.Count());
        _mockProductRepository.Verify(r => r.GetAllAsync(), Times.Once);
    }
}