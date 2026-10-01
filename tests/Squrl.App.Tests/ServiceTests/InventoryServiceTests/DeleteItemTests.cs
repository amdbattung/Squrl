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
public sealed class DeleteItemTests : IntegrationTest
{
    public DeleteItemTests(SqurlWebApplicationFactory factory) : base(factory)
    {
        
    }

    [Fact]
    public async Task DeleteItem_ShouldDeleteAndReturnItem()
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
        Result<GetItemDto> actual = await service.DeleteItemAsync(seed.Id);
        
        // Assert
        Assert.True(actual.IsSuccess);
        Assert.NotNull(actual.Value);
        Assert.Empty(actual.Errors);
        Assert.Null(actual.Exception);
        Assert.Null(actual.FailureType);
        
        Assert.Equal(seed.Id, actual.Value.Id);
        Assert.Equal(seed.Name, actual.Value.Name);
        Assert.Equal(seed.Uom.Id, actual.Value.UomId);
        Assert.Equal(seed.Description, actual.Value.Description);
        Assert.Equal(seed.Image, actual.Value.Image);
        Assert.Equal(seed.ListPrice, actual.Value.ListPrice);
        Assert.Equal(seed.RetailPrice, actual.Value.RetailPrice);
        Assert.Equal(seed.CostPrice, actual.Value.CostPrice);
        Assert.Equal(seed.LowQuantityAlertThreshold, actual.Value.LowQuantityAlertThreshold);
        Assert.Equal(seed.Locations, actual.Value.Locations);
        
        bool exists = await dataContext.Items
            .AnyAsync(i => i.Id == seed.Id);
        
        Assert.False(exists);
    }

    [Fact]
    public async Task DeleteItem_ShouldFailIfIdIsEmpty()
    {
        using IServiceScope scope = CreateScope();

        IInventoryService service = scope.ServiceProvider.GetRequiredService<IInventoryService>();
        
        // Arrange
        
        // Act
        Result<GetItemDto> actual = await service.DeleteItemAsync(Guid.Empty);
        
        // Assert
        Assert.False(actual.IsSuccess);
        Assert.Null(actual.Value);
        Assert.Contains("Null or invalid ID.", actual.Errors);
        Assert.Null(actual.Exception);
        Assert.Equal(FailureType.Validation, actual.FailureType);
    }

    [Fact]
    public async Task DeleteItem_ShouldFailIfItemDoesNotExist()
    {
        using IServiceScope scope = CreateScope();

        IInventoryService service = scope.ServiceProvider.GetRequiredService<IInventoryService>();
        
        // Arrange
        
        // Act
        Result<GetItemDto> actual = await service.DeleteItemAsync(Guid.NewGuid());
        
        // Assert
        Assert.False(actual.IsSuccess);
        Assert.Null(actual.Value);
        Assert.Contains("Item not found.", actual.Errors);
        Assert.Null(actual.Exception);
        Assert.Equal(FailureType.NotFound, actual.FailureType);
    }

    // TODO
    // [Fact]
    // public async Task DeleteItem_ShouldDeleteImage()
    // {
    //     
    // }
    //
    // [Fact]
    // public async Task DeleteItem_ShouldNotDeleteImageWhenItemDeletionFails()
    // {
    //     
    // }
}