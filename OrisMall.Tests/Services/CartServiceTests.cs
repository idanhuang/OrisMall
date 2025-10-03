using Microsoft.AspNetCore.Http;
using Moq;
using OrisMall.Core.Interfaces;
using OrisMall.Infrastructure.Services;
using OrisMall.Tests.Helpers;

namespace OrisMall.Tests.Services;

public class CartServiceTests
{
    private readonly Mock<IProductService> _mockProductService;
    private readonly Mock<IHttpContextAccessor> _mockHttpContextAccessor;
    private readonly CartService _cartService;

    public CartServiceTests()
    {
        _mockProductService = new Mock<IProductService>();
        _mockHttpContextAccessor = new Mock<IHttpContextAccessor>();
        _cartService = new CartService(_mockProductService.Object, _mockHttpContextAccessor.Object);
        
        // Clear cart storage before each test to avoid interference
        ClearCartStorage();
    }

    private void ClearCartStorage()
    {
        // Use reflection to clear the static _cartStorage dictionary
        var cartServiceType = typeof(CartService);
        var storageField = cartServiceType.GetField("_cartStorage", 
            System.Reflection.BindingFlags.NonPublic | System.Reflection.BindingFlags.Static);
        
        if (storageField?.GetValue(null) is Dictionary<string, string> storage)
        {
            storage.Clear();
        }
    }

    [Fact]
    public async Task AddToCartAsync_ShouldAddProductToCart_WhenValidProduct()
    {
        // Arrange
        var sessionId = "test-session-123";
        var addToCartDto = MockDataHelper.GetMockAddToCartDto();
        var mockProduct = MockDataHelper.GetMockProductDtos().First();
        
        _mockProductService.Setup(s => s.GetProductByIdAsync(addToCartDto.ProductId))
            .ReturnsAsync(mockProduct);

        // Act
        var result = await _cartService.AddToCartAsync(sessionId, addToCartDto);

        // Assert
        Assert.NotNull(result);
        Assert.Equal(sessionId, result.SessionId);
        Assert.Single(result.Items);
        Assert.Equal(addToCartDto.ProductId, result.Items.First().ProductId);
        Assert.Equal(addToCartDto.Quantity, result.Items.First().Quantity);
        Assert.Equal(mockProduct.Price * addToCartDto.Quantity, result.TotalAmount);
        _mockProductService.Verify(s => s.GetProductByIdAsync(addToCartDto.ProductId), Times.Once);
    }
}