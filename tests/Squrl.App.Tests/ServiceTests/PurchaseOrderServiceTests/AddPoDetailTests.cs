using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.DependencyInjection;
using NodaTime;
using Squrl.App.Common;
using Squrl.App.Data;
using Squrl.App.Enums;
using Squrl.App.Features.PurchaseOrderDetails.DTOs;
using Squrl.App.Models;
using Squrl.App.Services.PurchaseOrder;

namespace Squrl.App.Tests.ServiceTests.PurchaseOrderServiceTests;

[Collection("IntegrationTests")]
public sealed class AddPoDetailTests : IntegrationTest
{
    public AddPoDetailTests(SqurlWebApplicationFactory factory) : base(factory)
    {
        
    }

    [Fact]
    public async Task AddPoDetail_ShouldCreatePoDetail()
    {
        using IServiceScope scope = CreateScope();

        IPurchaseOrderService service = scope.ServiceProvider.GetRequiredService<IPurchaseOrderService>();
        
        DataContext dataContext = scope.ServiceProvider.GetRequiredService<DataContext>();
        
        // Arrange
        PurchaseOrder purchaseOrderSeed = new()
        {
            Id = Guid.NewGuid(),
            Status = PurchaseOrderStatus.Pending,
            DateOrdered = Instant.FromUtc(2020, 1, 1, 0, 0)
        };
        
        Item itemSeed = new()
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
        
        dataContext.Items.Add(itemSeed);
        
        PurchaseOrderDetail seed = new()
        {
            Id = Guid.NewGuid(),
            PurchaseOrder = purchaseOrderSeed,
            LineSequence = 1,
            Item = itemSeed,
            UnitPrice = 600m,
            Quantity = 5m
        };
        
        dataContext.PurchaseOrders.Add(purchaseOrderSeed);
        dataContext.PurchaseOrderDetails.Add(seed);
        await dataContext.SaveChangesAsync();
        
        CreatePoDetailDto dto = new()
        {
            LineSequence = 2,
            ItemId = itemSeed.Id,
            UnitPrice = 600m,
            Quantity = 5m
        };
        
        // Act
        Result<GetPoDetailDto> actual = await service.AddPoDetailAsync(purchaseOrderSeed.Id, dto);
        
        // Assert
        Assert.True(actual.IsSuccess);
        Assert.NotNull(actual.Value);
        Assert.Empty(actual.Errors);
        Assert.Null(actual.Exception);
        Assert.Null(actual.FailureType);
        
        Assert.Equal(dto.LineSequence, actual.Value.LineSequence);
        Assert.Equal(dto.ItemId, actual.Value.ItemId);
        Assert.Equal(dto.UnitPrice, actual.Value.UnitPrice);
        Assert.Equal(dto.Quantity, actual.Value.Quantity);

        PurchaseOrderDetail? created = await dataContext.PurchaseOrderDetails
            .AsNoTracking()
            .Include(p => p.PurchaseOrder)
            .Include(p => p.Item)
            .SingleOrDefaultAsync(p => p.Id == actual.Value.Id);

        Assert.NotNull(created);

        Assert.Equal(actual.Value.Id, created.Id);
        Assert.Equal(dto.LineSequence, created.LineSequence);
        Assert.Equal(dto.ItemId, created.Item.Id);
        Assert.Equal(dto.UnitPrice, created.UnitPrice);
        Assert.Equal(dto.Quantity, created.Quantity);
    }

    [Fact]
    public async Task AddPoDetail_ShouldFailIfSalesOrderIdIsEmpty()
    {
        using IServiceScope scope = CreateScope();

        IPurchaseOrderService service = scope.ServiceProvider.GetRequiredService<IPurchaseOrderService>();
        
        // Arrange
        CreatePoDetailDto dto = new()
        {
            LineSequence = 1,
            ItemId = Guid.NewGuid(),
            UnitPrice = 600m,
            Quantity = 5m
        };
        
        // Act
        Result<GetPoDetailDto> actual = await service.AddPoDetailAsync(Guid.Empty, dto);
        
        // Assert
        Assert.False(actual.IsSuccess);
        Assert.Null(actual.Value);
        // TODO: Change Error Message
        Assert.Contains("Failed to create PO detail.", actual.Errors);
        Assert.Null(actual.Exception);
        Assert.Equal(FailureType.BusinessLogic, actual.FailureType);
    }

    [Fact]
    public async Task AddPoDetail_ShouldFailIfSalesOrderDoesNotExist()
    {
        using IServiceScope scope = CreateScope();

        IPurchaseOrderService service = scope.ServiceProvider.GetRequiredService<IPurchaseOrderService>();
        
        // Arrange
        CreatePoDetailDto dto = new()
        {
            LineSequence = 1,
            ItemId = Guid.NewGuid(),
            UnitPrice = 600m,
            Quantity = 5m
        };
        
        // Act
        Result<GetPoDetailDto> actual = await service.AddPoDetailAsync(Guid.NewGuid(), dto);
        
        // Assert
        Assert.False(actual.IsSuccess);
        Assert.Null(actual.Value);
        // TODO: Change Error Message
        Assert.Contains("Failed to create PO detail.", actual.Errors);
        Assert.Null(actual.Exception);
        Assert.Equal(FailureType.BusinessLogic, actual.FailureType);
    }

    [Fact]
    public async Task AddPoDetail_ShouldFailWhenDtoIsInvalid()
    {
        using IServiceScope scope = CreateScope();

        IPurchaseOrderService service = scope.ServiceProvider.GetRequiredService<IPurchaseOrderService>();

        // Arrange
        CreatePoDetailDto dto = new()
        {
            LineSequence = 0,
            ItemId = Guid.NewGuid(),
            UnitPrice = 600m,
            Quantity = 5m
        };
        
        // Act
        Result<GetPoDetailDto> actual = await service.AddPoDetailAsync(Guid.NewGuid(), dto);

        // Assert
        Assert.False(actual.IsSuccess);
        Assert.Null(actual.Value);
        Assert.Contains("Line Sequence must start at 1.", actual.Errors);
        Assert.Null(actual.Exception);
        Assert.Equal(FailureType.Validation, actual.FailureType);
    }

    [Fact]
    public async Task AddPoDetail_ShouldNotPersistWhenValidationFails()
    {
        using IServiceScope scope = CreateScope();

        IPurchaseOrderService service = scope.ServiceProvider.GetRequiredService<IPurchaseOrderService>();
        
        DataContext dataContext = scope.ServiceProvider.GetRequiredService<DataContext>();

        // Arrange
        CreatePoDetailDto dto = new()
        {
            LineSequence = 0,
            ItemId = Guid.NewGuid(),
            UnitPrice = 600m,
            Quantity = 5m
        };
        
        // Act
        Result<GetPoDetailDto> actual = await service.AddPoDetailAsync(Guid.NewGuid(), dto);

        // Assert
        Assert.False(actual.IsSuccess);
        Assert.Null(actual.Value);
        Assert.NotEmpty(actual.Errors);
        Assert.Null(actual.Exception);
        Assert.Equal(FailureType.Validation, actual.FailureType);

        bool exists = await dataContext.PurchaseOrderDetails
            .AnyAsync(p => p.Item.Id == dto.ItemId);

        Assert.False(exists);
    }
}