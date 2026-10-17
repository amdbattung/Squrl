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
public sealed class DynamicStockUpdateTests : IntegrationTest
{
    public DynamicStockUpdateTests(SqurlWebApplicationFactory factory) : base(factory)
    {
        
    }

    [Theory]
    [InlineData("+0.05", 10.05)]
    [InlineData("+1", 11)]
    [InlineData("+5", 15)]
    public async Task DynamicStockUpdate_ShouldAddStocksAndReturnItem(string quantity, decimal expected)
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
        Result<GetItemDto> actual = await service.DynamicStockUpdateAsync(seed.Id, quantity);
        
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
    
    [Theory]
    [InlineData("−0.05", 9.95)]
    [InlineData("−1", 9)]
    [InlineData("-5", 5)]
    public async Task DynamicStockUpdate_ShouldRemoveStocksAndReturnItem(string quantity, decimal expected)
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
        Result<GetItemDto> actual = await service.DynamicStockUpdateAsync(seed.Id, quantity);
        
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

    [Theory]
    [InlineData("×0.05", 0.5)]
    [InlineData("*1", 10)]
    [InlineData("x2", 20)]
    [InlineData("X5", 50)]
    public async Task DynamicStockUpdate_ShouldMultiplyStocksAndReturnItem(string quantity, decimal expected)
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
        Result<GetItemDto> actual = await service.DynamicStockUpdateAsync(seed.Id, quantity);
        
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

    [Theory]
    [InlineData("÷0.05", 200)]
    [InlineData("÷1", 10)]
    [InlineData("/5", 2)]
    public async Task DynamicStockUpdate_ShouldDivideStocksAndReturnItem(string quantity, decimal expected)
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
        Result<GetItemDto> actual = await service.DynamicStockUpdateAsync(seed.Id, quantity);
        
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

    [Theory]
    [InlineData("0.05", 0.05)]
    [InlineData("1", 1)]
    [InlineData("=5", 5)]
    public async Task DynamicStockUpdate_ShouldSetStockAndReturnItem(string quantity, decimal expected)
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
        Result<GetItemDto> actual = await service.DynamicStockUpdateAsync(seed.Id, quantity);
        
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
    public async Task DynamicStockUpdate_ShouldFailIfIdIsEmpty()
    {
        using IServiceScope scope = CreateScope();

        IInventoryService service = scope.ServiceProvider.GetRequiredService<IInventoryService>();
        
        // Arrange
        
        // Act
        Result<GetItemDto> actual = await service.DynamicStockUpdateAsync(Guid.Empty, "+5");
        
        // Assert
        Assert.False(actual.IsSuccess);
        Assert.Null(actual.Value);
        Assert.Contains("Null or invalid ID.", actual.Errors);
        Assert.Null(actual.Exception);
        Assert.Equal(FailureType.Validation, actual.FailureType);
    }

    [Fact]
    public async Task DynamicStockUpdate_ShouldFailIfItemDoesNotExists()
    {
        using IServiceScope scope = CreateScope();

        IInventoryService service = scope.ServiceProvider.GetRequiredService<IInventoryService>();
        
        // Arrange
        
        // Act
        Result<GetItemDto> actual = await service.DynamicStockUpdateAsync(Guid.NewGuid(), "+5");
        
        // Assert
        Assert.False(actual.IsSuccess);
        Assert.Null(actual.Value);
        Assert.Contains("Item not found.", actual.Errors);
        Assert.Null(actual.Exception);
        Assert.Equal(FailureType.NotFound, actual.FailureType);
    }

    [Theory]
    [InlineData("+-5")]
    [InlineData("--1")]
    public async Task DynamicStockUpdate_ShouldRejectInvalidQuantity(string quantity)
    {
        using IServiceScope scope = CreateScope();

        IInventoryService service = scope.ServiceProvider.GetRequiredService<IInventoryService>();
        
        // Arrange
        
        // Act
        Result<GetItemDto> actual = await service.DynamicStockUpdateAsync(Guid.NewGuid(), quantity);
        
        // Assert
        Assert.False(actual.IsSuccess);
        Assert.Null(actual.Value);
        Assert.Contains("Invalid quantity.", actual.Errors);
        Assert.Null(actual.Exception);
        Assert.Equal(FailureType.Validation, actual.FailureType);
    }

    [Fact]
    public async Task DynamicStockUpdate_ShouldRejectEmptyQuantity()
    {
        using IServiceScope scope = CreateScope();

        IInventoryService service = scope.ServiceProvider.GetRequiredService<IInventoryService>();
        
        // Arrange
        
        // Act
        Result<GetItemDto> actual = await service.DynamicStockUpdateAsync(Guid.NewGuid(), "");
        
        // Assert
        Assert.False(actual.IsSuccess);
        Assert.Null(actual.Value);
        Assert.Contains("Invalid quantity.", actual.Errors);
        Assert.Null(actual.Exception);
        Assert.Equal(FailureType.Validation, actual.FailureType);
    }
    
    [Fact]
    public async Task DynamicStockUpdate_ShouldAllowZeroQuantityWhenDoingSetOperations()
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
        Result<GetItemDto> actual = await service.DynamicStockUpdateAsync(seed.Id, "0");
        
        // Assert
        Assert.True(actual.IsSuccess);
        Assert.NotNull(actual.Value);
        Assert.Empty(actual.Errors);
        Assert.Null(actual.Exception);
        Assert.Null(actual.FailureType);
        
        Assert.Equal(seed.Id, actual.Value.Id);
        Assert.Equal(0, actual.Value.Quantity);
        
        Item? updated = await dataContext.Items
            .AsNoTracking()
            .SingleOrDefaultAsync(i => i.Id == seed.Id);
        
        Assert.NotNull(updated);
        
        Assert.Equal(seed.Id, updated.Id);
        Assert.Equal(0, updated.Quantity);
    }

    [Theory]
    [InlineData("+0")]
    [InlineData("-0")]
    [InlineData("*0")]
    [InlineData("/0")]
    public async Task DynamicStockUpdate_ShouldRejectZeroQuantityWhenDoingNonSetOperations(string quantity)
    {
        using IServiceScope scope = CreateScope();

        IInventoryService service = scope.ServiceProvider.GetRequiredService<IInventoryService>();
        
        // Arrange
        
        // Act
        Result<GetItemDto> actual = await service.DynamicStockUpdateAsync(Guid.NewGuid(), quantity);
        
        // Assert
        Assert.False(actual.IsSuccess);
        Assert.Null(actual.Value);
        Assert.Contains("Invalid quantity.", actual.Errors);
        Assert.Null(actual.Exception);
        Assert.Equal(FailureType.Validation, actual.FailureType);
    }

    [Fact]
    public async Task DynamicStockUpdate_ShouldRejectInvalidOperator()
    {
        using IServiceScope scope = CreateScope();

        IInventoryService service = scope.ServiceProvider.GetRequiredService<IInventoryService>();
        
        // Arrange
        
        // Act
        Result<GetItemDto> actual = await service.DynamicStockUpdateAsync(Guid.NewGuid(), "!5");
        
        // Assert
        Assert.False(actual.IsSuccess);
        Assert.Null(actual.Value);
        Assert.Contains("Invalid operator.", actual.Errors);
        Assert.Null(actual.Exception);
        Assert.Equal(FailureType.Validation, actual.FailureType);
    }

    [Theory]
    [InlineData("-10.01")]
    [InlineData("-15")]
    public async Task DynamicStockUpdate_ShouldFailWhenNotEnoughStocksToRemove(string quantity)
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
        Result<GetItemDto> actual = await service.DynamicStockUpdateAsync(seed.Id, quantity);
        
        // Assert
        Assert.False(actual.IsSuccess);
        Assert.Null(actual.Value);
        // TODO: Change Error Message
        Assert.Contains("Item not found.", actual.Errors);
        Assert.Null(actual.Exception);
        Assert.Equal(FailureType.NotFound, actual.FailureType);
    }
}