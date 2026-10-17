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
public sealed class UpdatePoDetailTests : IntegrationTest
{
    public UpdatePoDetailTests(SqurlWebApplicationFactory factory) : base(factory)
    {
        
    }

    [Fact]
    public async Task UpdatePoDetail_ShouldUpdateAndPersistPoDetail()
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
            Name = "Test Item 2",
            Uom = new()
            {
                Id = Guid.NewGuid(),
                Name = "Test UOM 2",
                Code = "u 2"
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
            Item = new()
            {
                Id = Guid.NewGuid(),
                Name = "Test Item 1",
                Uom = new()
                {
                    Id = Guid.NewGuid(),
                    Name = "Test UOM 1",
                    Code = "u 1"
                },
                Quantity = 10m,
                Locations = ["Shelf A", "Shelf B"]
            },
            UnitPrice = 600m,
            Quantity = 5m
        };
        
        dataContext.PurchaseOrders.Add(purchaseOrderSeed);
        dataContext.PurchaseOrderDetails.Add(seed);
        await dataContext.SaveChangesAsync();
        
        UpdatePoDetailDto dto = new()
        {
            PurchaseOrderId = purchaseOrderSeed.Id,
            LineSequence = 1,
            ItemId = itemSeed.Id,
            UnitPrice = 700m,
            Quantity = 6m
        };
        
        // Act
        Result<GetPoDetailDto> actual = await service.UpdatePoDetailAsync(seed.Id, dto);
        
        // Assert
        Assert.True(actual.IsSuccess);
        Assert.NotNull(actual.Value);
        Assert.Empty(actual.Errors);
        Assert.Null(actual.Exception);
        Assert.Null(actual.FailureType);
        
        Assert.Equal(seed.Id, actual.Value.Id);
        Assert.Equal(dto.PurchaseOrderId, actual.Value.PurchaseOrderId);
        Assert.Equal(dto.LineSequence, actual.Value.LineSequence);
        Assert.Equal(dto.ItemId, actual.Value.ItemId);
        Assert.Equal(dto.UnitPrice, actual.Value.UnitPrice);
        Assert.Equal(dto.Quantity, actual.Value.Quantity);
        
        PurchaseOrderDetail? updated = await dataContext.PurchaseOrderDetails
            .AsNoTracking()
            .Include(p => p.PurchaseOrder)
            .Include(p => p.Item)
            .SingleOrDefaultAsync(s => s.Id == seed.Id);
        
        Assert.NotNull(updated);
        
        Assert.Equal(seed.Id, updated.Id);
        Assert.Equal(dto.PurchaseOrderId, updated.PurchaseOrder.Id);
        Assert.Equal(dto.LineSequence, updated.LineSequence);
        Assert.Equal(dto.ItemId, updated.Item.Id);
        Assert.Equal(dto.UnitPrice, updated.UnitPrice);
        Assert.Equal(dto.Quantity, updated.Quantity);
    }

    [Fact]
    public async Task UpdatePoDetail_ShouldFailIfIdIsEmpty()
    {
        using IServiceScope scope = CreateScope();

        IPurchaseOrderService service = scope.ServiceProvider.GetRequiredService<IPurchaseOrderService>();
        
        // Arrange
        UpdatePoDetailDto dto = new()
        {
            PurchaseOrderId = Guid.NewGuid(),
            LineSequence = 1,
            ItemId = Guid.NewGuid(),
            UnitPrice = 600m,
            Quantity = 5m
        };
        
        // Act
        Result<GetPoDetailDto> actual = await service.UpdatePoDetailAsync(Guid.Empty, dto);
        
        // Assert
        Assert.False(actual.IsSuccess);
        Assert.Null(actual.Value);
        Assert.Contains("Null or invalid ID.", actual.Errors);
        Assert.Null(actual.Exception);
        Assert.Equal(FailureType.Validation, actual.FailureType);
    }

    [Fact]
    public async Task UpdatePoDetail_ShouldFailIfPoDetailDoesNotExist()
    {
        using IServiceScope scope = CreateScope();

        IPurchaseOrderService service = scope.ServiceProvider.GetRequiredService<IPurchaseOrderService>();
        
        // Arrange
        UpdatePoDetailDto dto = new()
        {
            PurchaseOrderId = Guid.NewGuid(),
            LineSequence = 1,
            ItemId = Guid.NewGuid(),
            UnitPrice = 600m,
            Quantity = 5m
        };
        
        // Act
        Result<GetPoDetailDto> actual = await service.UpdatePoDetailAsync(Guid.NewGuid(), dto);
        
        // Assert
        Assert.False(actual.IsSuccess);
        Assert.Null(actual.Value);
        Assert.Contains("PO detail not found.", actual.Errors);
        Assert.Null(actual.Exception);
        Assert.Equal(FailureType.NotFound, actual.FailureType);
    }

    [Fact]
    public async Task UpdatePoDetail_ShouldFailWhenDtoIsInvalid()
    {
        using IServiceScope scope = CreateScope();

        IPurchaseOrderService service = scope.ServiceProvider.GetRequiredService<IPurchaseOrderService>();
        
        // Arrange
        UpdatePoDetailDto dto = new()
        {
            PurchaseOrderId = Guid.NewGuid(),
            LineSequence = 0,
            ItemId = Guid.NewGuid(),
            UnitPrice = 600m,
            Quantity = 5m
        };
        
        // Act
        Result<GetPoDetailDto> actual = await service.UpdatePoDetailAsync(Guid.NewGuid(), dto);
        
        // Assert
        Assert.False(actual.IsSuccess);
        Assert.Null(actual.Value);
        Assert.Contains("Line Sequence must start at 1.", actual.Errors);
        Assert.Null(actual.Exception);
        Assert.Equal(FailureType.Validation, actual.FailureType);
    }

    [Fact]
    public async Task UpdatePoDetail_ShouldNotPersistWhenValidationFails()
    {
        using IServiceScope scope = CreateScope();

        IPurchaseOrderService service = scope.ServiceProvider.GetRequiredService<IPurchaseOrderService>();
        
        DataContext dataContext = scope.ServiceProvider.GetRequiredService<DataContext>();
        
        // Arrange
        UpdatePoDetailDto dto = new()
        {
            PurchaseOrderId = Guid.NewGuid(),
            LineSequence = 0,
            ItemId = Guid.NewGuid(),
            UnitPrice = 600m,
            Quantity = 5m
        };
        
        // Act
        Result<GetPoDetailDto> actual = await service.UpdatePoDetailAsync(Guid.NewGuid(), dto);
        
        // Assert
        Assert.False(actual.IsSuccess);
        Assert.Null(actual.Value);
        Assert.NotEmpty(actual.Errors);
        Assert.Null(actual.Exception);
        Assert.Equal(FailureType.Validation, actual.FailureType);
        
        bool exists = await dataContext.PurchaseOrderDetails
            .AnyAsync(p => p.PurchaseOrder.Id == dto.PurchaseOrderId);

        Assert.False(exists);
    }
}