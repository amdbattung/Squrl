using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.DependencyInjection;
using Squrl.App.Common;
using Squrl.App.Data;
using Squrl.App.Enums;
using Squrl.App.Features.Items.DTOs;
using Squrl.App.Infrastructure.ImageStorage;
using Squrl.App.Models;
using Squrl.App.Services.Inventory;
using Xunit.Abstractions;

namespace Squrl.App.Tests.ServiceTests.InventoryServiceTests;

[Collection("IntegrationTests")]
public sealed class CreateItemTests : IntegrationTest
{
    public CreateItemTests(SqurlWebApplicationFactory factory) : base(factory)
    {
        
    }
    
    [Fact]
    public async Task CreateItem_ShouldCreateItem()
    {
        using IServiceScope scope = CreateScope();

        IInventoryService service = scope.ServiceProvider.GetRequiredService<IInventoryService>();
        
        DataContext dataContext = scope.ServiceProvider.GetRequiredService<DataContext>();

        // Arrange
        UnitOfMeasure uomSeed = new()
        {
            Id = Guid.NewGuid(),
            Name = "Test UOM",
            Code = "u"
        };

        dataContext.UnitOfMeasures.Add(uomSeed);
        await dataContext.SaveChangesAsync();
        
        CreateItemDto dto = new()
        {
            Name = "Test Item",
            UomId = uomSeed.Id,
            Description = "A valid description.",
            ListPrice = 1000m,
            RetailPrice = 1500.10m,
            CostPrice = 600m,
            Quantity = 10m,
            LowQuantityAlertThreshold = 2m,
            Locations = ["Shelf A", "Shelf B"]
        };
        
        // Act
        Result<GetItemDto> actual = await service.CreateItemAsync(dto);

        // Assert
        Assert.True(actual.IsSuccess);
        Assert.NotNull(actual.Value);
        Assert.Empty(actual.Errors);
        Assert.Null(actual.Exception);
        Assert.Null(actual.FailureType);

        Assert.Equal(dto.Name, actual.Value.Name);
        Assert.Equal(dto.UomId, actual.Value.UomId);
        Assert.Equal(dto.Description, actual.Value.Description);
        Assert.Equal(dto.Image, actual.Value.Image);
        Assert.Equal(dto.ListPrice, actual.Value.ListPrice);
        Assert.Equal(dto.RetailPrice, actual.Value.RetailPrice);
        Assert.Equal(dto.CostPrice, actual.Value.CostPrice);
        Assert.Equal(dto.LowQuantityAlertThreshold, actual.Value.LowQuantityAlertThreshold);
        Assert.Equal(dto.Locations, actual.Value.Locations);

        Item? created = await dataContext.Items
            .AsNoTracking()
            .Include(i => i.Uom)
            .SingleOrDefaultAsync(i => i.Id == actual.Value.Id);

        Assert.NotNull(created);

        Assert.Equal(actual.Value.Id, created.Id);
        Assert.Equal(dto.Name, created.Name);
        Assert.Equal(dto.UomId, created.Uom.Id);
        Assert.Equal(dto.Description, created.Description);
        Assert.Equal(dto.Image, created.Image);
        Assert.Equal(dto.ListPrice, created.ListPrice);
        Assert.Equal(dto.RetailPrice, created.RetailPrice);
        Assert.Equal(dto.CostPrice, created.CostPrice);
        Assert.Equal(dto.LowQuantityAlertThreshold, created.LowQuantityAlertThreshold);
        Assert.Equal(dto.Locations, created.Locations);
    }

    [Fact]
    public async Task CreateItem_ShouldFailWhenDtoIsInvalid()
    {
        using IServiceScope scope = CreateScope();

        IInventoryService service = scope.ServiceProvider.GetRequiredService<IInventoryService>();

        // Arrange
        CreateItemDto dto = new()
        {
            Name = "",
            UomId = Guid.NewGuid(),
            Quantity = 10m,
            Locations = ["Shelf A", "Shelf B"]
        };
        
        // Act
        Result<GetItemDto> actual = await service.CreateItemAsync(dto);

        // Assert
        Assert.False(actual.IsSuccess);
        Assert.Null(actual.Value);
        Assert.Contains("Name is required.", actual.Errors);
        Assert.Null(actual.Exception);
        Assert.Equal(FailureType.Validation, actual.FailureType);
    }

    [Fact]
    public async Task CreateItem_ShouldNotPersistWhenValidationFails()
    {
        using IServiceScope scope = CreateScope();

        IInventoryService service = scope.ServiceProvider.GetRequiredService<IInventoryService>();
        
        DataContext dataContext = scope.ServiceProvider.GetRequiredService<DataContext>();

        // Arrange
        CreateItemDto dto = new()
        {
            Name = "",
            UomId = Guid.NewGuid(),
            Quantity = 10m,
            Locations = ["Shelf A", "Shelf B"]
        };
        
        // Act
        Result<GetItemDto> actual = await service.CreateItemAsync(dto);

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
    
    [Fact]
    public async Task CreateItem_ShouldUploadImageAndCreateItem()
    {
        using IServiceScope scope = CreateScope();

        IInventoryService service = scope.ServiceProvider.GetRequiredService<IInventoryService>();
        
        DataContext dataContext = scope.ServiceProvider.GetRequiredService<DataContext>();
        
        IImageStorage imageStorage = scope.ServiceProvider.GetRequiredService<IImageStorage>();

        // Arrange
        UnitOfMeasure uomSeed = new()
        {
            Id = Guid.NewGuid(),
            Name = "Test UOM",
            Code = "u"
        };

        dataContext.UnitOfMeasures.Add(uomSeed);
        await dataContext.SaveChangesAsync();
        
        string testImagePath = Path.Combine(
            AppContext.BaseDirectory,
            "TestData",
            "test-image.jpg");
        
        await using FileStream testImage = File.OpenRead(testImagePath);
        
        CreateItemDto dto = new()
        {
            Name = "Test Item",
            UomId = uomSeed.Id,
            Quantity = 10m,
            LowQuantityAlertThreshold = 2m,
            Locations = ["Shelf A", "Shelf B"]
        };
        
        // Act
        Result<GetItemDto> actual = await service.CreateItemAsync(dto, testImage);

        // Assert
        Assert.True(actual.IsSuccess);
        Assert.NotNull(actual.Value);
        Assert.Empty(actual.Errors);
        Assert.Null(actual.Exception);
        Assert.Null(actual.FailureType);

        Assert.NotNull(actual.Value.Image);
        Assert.NotEmpty(actual.Value.Image);

        Item? created = await dataContext.Items
            .AsNoTracking()
            .Include(i => i.Uom)
            .SingleOrDefaultAsync(i => i.Id == actual.Value.Id);

        Assert.NotNull(created);

        Assert.NotNull(created.Image);
        Assert.NotEmpty(created.Image);
        Assert.Equal(actual.Value.Id, created.Id);
        Assert.Equal(actual.Value.Image, created.Image);
        
        string physicalPath = imageStorage.GetPhysicalPath(created.Image);

        Assert.True(File.Exists(physicalPath));
    }
    
    // TODO
    // [Fact]
    // public async Task CreateItem_ShouldFailWhenImageIsInvalid()
    // {
    //     
    // }
    //
    // [Fact]
    // public async Task CreateItem_ShouldNotPersistWhenImageIsInvalid()
    // {
    //     
    // }
    //
    // [Fact]
    // public async Task CreateItem_ShouldNotUploadImageWhenItemCreationFails()
    // {
    //     
    // }
}