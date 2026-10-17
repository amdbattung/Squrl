using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.DependencyInjection;
using Squrl.App.Common;
using Squrl.App.Data;
using Squrl.App.Enums;
using Squrl.App.Features.Items.DTOs;
using Squrl.App.Models;
using Squrl.App.Services.Inventory;

namespace Squrl.App.Tests.ServiceTests.InventoryServiceTests;

[Collection("IntegrationTests")]
public sealed class RemoveStocksTests : IntegrationTest
{
    public RemoveStocksTests(SqurlWebApplicationFactory factory) : base(factory)
    {
        
    }

    [Theory]
    [InlineData(0.05, 9.95)]
    [InlineData(1, 9)]
    [InlineData(5, 5)]
    public async Task RemoveStocks_ShouldRemoveStocksAndReturnItem(decimal quantity, decimal expected)
    {
        using IServiceScope scope = CreateScope();

        IInventoryService service = scope.ServiceProvider.GetRequiredService<IInventoryService>();
        
        DataContext dataContext = scope.ServiceProvider.GetRequiredService<DataContext>();
        
        // Arrange
        Item seed = new()
        {
            Id = Guid.NewGuid(),
            Name = "Test Item",
            Uom = new()
            {
                Id = Guid.NewGuid(),
                Name = "Test UOM",
                Code = "u"
            },
            Quantity = 10m,
            Locations = ["Shelf A", "Shelf B"]
        };

        dataContext.Items.Add(seed);
        await dataContext.SaveChangesAsync();
        
        // Act
        Result<GetItemDto> actual = await service.RemoveStocksAsync(seed.Id, quantity);
        
        // Assert
        Assert.True(actual.IsSuccess);
        Assert.NotNull(actual.Value);
        Assert.Empty(actual.Errors);
        Assert.Null(actual.Exception);
        Assert.Null(actual.FailureType);
        
        Assert.Equal(seed.Id, actual.Value.Id);
        Assert.Equal(expected, actual.Value.Quantity);
        
        Item? updated = await dataContext.Items
            .AsNoTracking()
            .SingleOrDefaultAsync(i => i.Id == seed.Id);
        
        Assert.NotNull(updated);
        
        Assert.Equal(seed.Id, updated.Id);
        Assert.Equal(expected, updated.Quantity);
    }

    [Fact]
    public async Task RemoveStocks_ShouldFailIfIdIsEmpty()
    {
        using IServiceScope scope = CreateScope();

        IInventoryService service = scope.ServiceProvider.GetRequiredService<IInventoryService>();
        
        // Arrange
        
        // Act
        Result<GetItemDto> actual = await service.RemoveStocksAsync(Guid.Empty, 5m);
        
        // Assert
        Assert.False(actual.IsSuccess);
        Assert.Null(actual.Value);
        Assert.Contains("Null or invalid ID.", actual.Errors);
        Assert.Null(actual.Exception);
        Assert.Equal(FailureType.Validation, actual.FailureType);
    }

    [Fact]
    public async Task RemoveStocks_ShouldFailIfItemDoesNotExists()
    {
        using IServiceScope scope = CreateScope();

        IInventoryService service = scope.ServiceProvider.GetRequiredService<IInventoryService>();
        
        // Arrange
        
        // Act
        Result<GetItemDto> actual = await service.RemoveStocksAsync(Guid.NewGuid(), 5m);
        
        // Assert
        Assert.False(actual.IsSuccess);
        Assert.Null(actual.Value);
        Assert.Contains("Item not found.", actual.Errors);
        Assert.Null(actual.Exception);
        Assert.Equal(FailureType.NotFound, actual.FailureType);
    }

    [Theory]
    [InlineData(-5)]
    [InlineData(-0.01)]
    public async Task RemoveStocks_ShouldRejectNegativeQuantity(decimal quantity)
    {
        using IServiceScope scope = CreateScope();

        IInventoryService service = scope.ServiceProvider.GetRequiredService<IInventoryService>();
        
        // Arrange
        
        // Act
        Result<GetItemDto> actual = await service.RemoveStocksAsync(Guid.NewGuid(), quantity);
        
        // Assert
        Assert.False(actual.IsSuccess);
        Assert.Null(actual.Value);
        Assert.Contains("Invalid quantity.", actual.Errors);
        Assert.Null(actual.Exception);
        Assert.Equal(FailureType.Validation, actual.FailureType);
    }

    [Fact]
    public async Task RemoveStocks_ShouldRejectZeroQuantity()
    {
        using IServiceScope scope = CreateScope();

        IInventoryService service = scope.ServiceProvider.GetRequiredService<IInventoryService>();
        
        // Arrange
        
        // Act
        Result<GetItemDto> actual = await service.AddStocksAsync(Guid.NewGuid(), 0m);
        
        // Assert
        Assert.False(actual.IsSuccess);
        Assert.Null(actual.Value);
        Assert.Contains("Invalid quantity.", actual.Errors);
        Assert.Null(actual.Exception);
        Assert.Equal(FailureType.Validation, actual.FailureType);
    }

    [Theory]
    [InlineData(10.01)]
    [InlineData(15)]
    public async Task RemoveStocks_ShouldFailWhenNotEnoughStocksToRemove(decimal quantity)
    {
        using IServiceScope scope = CreateScope();

        IInventoryService service = scope.ServiceProvider.GetRequiredService<IInventoryService>();
        
        DataContext dataContext = scope.ServiceProvider.GetRequiredService<DataContext>();
        
        // Arrange
        Item seed = new()
        {
            Id = Guid.NewGuid(),
            Name = "Test Item",
            Uom = new()
            {
                Id = Guid.NewGuid(),
                Name = "Test UOM",
                Code = "u"
            },
            Quantity = 10m,
            Locations = ["Shelf A", "Shelf B"]
        };

        dataContext.Items.Add(seed);
        await dataContext.SaveChangesAsync();
        
        // Act
        Result<GetItemDto> actual = await service.RemoveStocksAsync(seed.Id, quantity);
        
        // Assert
        Assert.False(actual.IsSuccess);
        Assert.Null(actual.Value);
        // TODO: Change Error Message
        Assert.Contains("Item not found.", actual.Errors);
        Assert.Null(actual.Exception);
        Assert.Equal(FailureType.NotFound, actual.FailureType);
    }
}