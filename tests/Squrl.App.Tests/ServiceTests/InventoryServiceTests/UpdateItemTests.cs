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
public sealed class UpdateItemTests : IntegrationTest
{
    public UpdateItemTests(SqurlWebApplicationFactory factory) : base(factory)
    {
        
    }

    [Fact]
    public async Task UpdateItem_ShouldUpdateAndPersistItem()
    {
        using IServiceScope scope = CreateScope();

        IInventoryService service = scope.ServiceProvider.GetRequiredService<IInventoryService>();
        
        DataContext dataContext = scope.ServiceProvider.GetRequiredService<DataContext>();
        
        // Arrange
        UnitOfMeasure uomSeed = new()
        {
            Id = Guid.NewGuid(),
            Name = "Test UOM 1",
            Code = "u 1"
        };

        dataContext.UnitOfMeasures.Add(uomSeed);
        
        Item seed = new()
        {
            Id = Guid.NewGuid(),
            Name = "Test Item",
            Uom = new()
            {
                Id = Guid.NewGuid(),
                Name = "Test UOM 2",
                Code = "u 2"
            },
            Description = "A valid description.",
            ListPrice = 1000m,
            RetailPrice = 1500.10m,
            CostPrice = 600m,
            Quantity = 10m,
            Locations = ["Shelf A", "Shelf B"]
        };

        dataContext.Items.Add(seed);
        await dataContext.SaveChangesAsync();
        
        UpdateItemDto dto = new()
        {
            Name = "Updated Test Item",
            UomId =  uomSeed.Id,
            Description = "An updated description.",
            ListPrice = 1100m,
            RetailPrice = 1600.10m,
            CostPrice = 700m,
            Quantity = 10m,
            Locations = ["Shelf A", "Shelf B", "Shelf C"]
        };
        
        // Act
        Result<GetItemDto> actual = await service.UpdateItemAsync(seed.Id, dto);
        
        // Assert
        Assert.True(actual.IsSuccess);
        Assert.NotNull(actual.Value);
        Assert.Empty(actual.Errors);
        Assert.Null(actual.Exception);
        Assert.Null(actual.FailureType);
        
        Assert.Equal(seed.Id, actual.Value.Id);
        Assert.Equal(dto.Name, actual.Value.Name);
        Assert.Equal(dto.UomId, actual.Value.UomId);
        Assert.Equal(dto.Description, actual.Value.Description);
        Assert.Equal(dto.Image, actual.Value.Image);
        Assert.Equal(dto.ListPrice, actual.Value.ListPrice);
        Assert.Equal(dto.RetailPrice, actual.Value.RetailPrice);
        Assert.Equal(dto.CostPrice, actual.Value.CostPrice);
        Assert.Equal(dto.LowQuantityAlertThreshold, actual.Value.LowQuantityAlertThreshold);
        Assert.Equal(dto.Locations, actual.Value.Locations);
        
        Item? updated = await dataContext.Items
            .AsNoTracking()
            .Include(i => i.Uom)
            .SingleOrDefaultAsync(i => i.Id == seed.Id);
        
        Assert.NotNull(updated);
        
        Assert.Equal(seed.Id, updated.Id);
        Assert.Equal(dto.Name, updated.Name);
        Assert.Equal(dto.UomId, updated.Uom.Id);
        Assert.Equal(dto.Description, updated.Description);
        Assert.Equal(dto.Image, updated.Image);
        Assert.Equal(dto.ListPrice, updated.ListPrice);
        Assert.Equal(dto.RetailPrice, updated.RetailPrice);
        Assert.Equal(dto.CostPrice, updated.CostPrice);
        Assert.Equal(dto.LowQuantityAlertThreshold, updated.LowQuantityAlertThreshold);
        Assert.Equal(dto.Locations, updated.Locations);
    }

    [Fact]
    public async Task UpdateItem_ShouldFailIfIdIsEmpty()
    {
        using IServiceScope scope = CreateScope();

        IInventoryService service = scope.ServiceProvider.GetRequiredService<IInventoryService>();
        
        // Arrange
        UpdateItemDto dto = new()
        {
            Name = "Test Item",
            UomId = Guid.NewGuid(),
            Quantity = 10m,
            Locations = ["Shelf A", "Shelf B"]
        };
        
        // Act
        Result<GetItemDto> actual = await service.UpdateItemAsync(Guid.Empty, dto);
        
        // Assert
        Assert.False(actual.IsSuccess);
        Assert.Null(actual.Value);
        Assert.Contains("Null or invalid ID.", actual.Errors);
        Assert.Null(actual.Exception);
        Assert.Equal(FailureType.Validation, actual.FailureType);
    }

    [Fact]
    public async Task UpdateItem_ShouldFailIfItemDoesNotExist()
    {
        using IServiceScope scope = CreateScope();

        IInventoryService service = scope.ServiceProvider.GetRequiredService<IInventoryService>();
        
        // Arrange
        UpdateItemDto dto = new()
        {
            Name = "Test Item",
            UomId = Guid.NewGuid(),
            Quantity = 10m,
            Locations = ["Shelf A", "Shelf B"]
        };
        
        // Act
        Result<GetItemDto> actual = await service.UpdateItemAsync(Guid.NewGuid(), dto);
        
        // Assert
        Assert.False(actual.IsSuccess);
        Assert.Null(actual.Value);
        Assert.Contains("Item not found.", actual.Errors);
        Assert.Null(actual.Exception);
        Assert.Equal(FailureType.NotFound, actual.FailureType);
    }

    [Fact]
    public async Task UpdateItem_ShouldFailWhenDtoIsInvalid()
    {
        using IServiceScope scope = CreateScope();

        IInventoryService service = scope.ServiceProvider.GetRequiredService<IInventoryService>();
        
        // Arrange
        UpdateItemDto dto = new()
        {
            Name = "",
            UomId = Guid.NewGuid(),
            Quantity = 10m,
            Locations = ["Shelf A", "Shelf B"]
        };
        
        // Act
        Result<GetItemDto> actual = await service.UpdateItemAsync(Guid.NewGuid(), dto);
        
        // Assert
        Assert.False(actual.IsSuccess);
        Assert.Null(actual.Value);
        Assert.Contains("Name is required.", actual.Errors);
        Assert.Null(actual.Exception);
        Assert.Equal(FailureType.Validation, actual.FailureType);
    }

    [Fact]
    public async Task UpdateItem_ShouldNotPersistWhenValidationFails()
    {
        using IServiceScope scope = CreateScope();

        IInventoryService service = scope.ServiceProvider.GetRequiredService<IInventoryService>();
        
        DataContext dataContext = scope.ServiceProvider.GetRequiredService<DataContext>();
        
        // Arrange
        UpdateItemDto dto = new()
        {
            Name = "",
            UomId = Guid.NewGuid(),
            Quantity = 10m,
            Locations = ["Shelf A", "Shelf B"]
        };
        
        // Act
        Result<GetItemDto> actual = await service.UpdateItemAsync(Guid.NewGuid(), dto);
        
        // Assert
        Assert.False(actual.IsSuccess);
        Assert.Null(actual.Value);
        Assert.NotEmpty(actual.Errors);
        Assert.Null(actual.Exception);
        Assert.Equal(FailureType.Validation, actual.FailureType);
        
        bool exists = await dataContext.Items
            .AnyAsync(i => i.Uom.Id == dto.UomId);

        Assert.False(exists);
    }
    
    // TODO
    // [Fact]
    // public async Task UpdateItem_ShouldReplaceOldImageWithANewOne()
    // {
    //     
    // }
    //
    // [Fact]
    // public async Task UpdateItem_ShouldFailWhenImageIsInvalid()
    // {
    //     
    // }
    //
    // [Fact]
    // public async Task UpdateItem_ShouldNotUpdateItemWhenImageIsInvalid()
    // {
    //     
    // }
    //
    // [Fact]
    // public async Task UpdateItem_ShouldNotReplaceImageWhenItemUpdateFails()
    // {
    //     
    // }
}