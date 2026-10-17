using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.DependencyInjection;
using NodaTime;
using Squrl.App.Common;
using Squrl.App.Data;
using Squrl.App.Enums;
using Squrl.App.Features.PurchaseOrders.DTOs;
using Squrl.App.Models;
using Squrl.App.Services.PurchaseOrder;

namespace Squrl.App.Tests.ServiceTests.PurchaseOrderServiceTests;

[Collection("IntegrationTests")]
public sealed class ReturnPurchaseOrderTests : IntegrationTest
{
    public ReturnPurchaseOrderTests(SqurlWebApplicationFactory factory) : base(factory)
    {
        
    }

    [Fact]
    public async Task ReturnPurchaseOrder_ShouldUpdatePurchaseOrderStatusToReturned()
    {
        using IServiceScope scope = CreateScope();

        IPurchaseOrderService service = scope.ServiceProvider.GetRequiredService<IPurchaseOrderService>();
        
        DataContext dataContext = scope.ServiceProvider.GetRequiredService<DataContext>();
        
        // Arrange
        PurchaseOrder seed = new()
        {
            Id = Guid.NewGuid(),
            Status = PurchaseOrderStatus.Received,
            DateOrdered = Instant.FromUtc(2020, 1, 1, 0, 0)
        };
        
        PurchaseOrderDetail poDetailSeed = new()
        {
            Id = Guid.NewGuid(),
            PurchaseOrder = seed,
            LineSequence = 1,
            Item = new()
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
            },
            UnitPrice = 600m,
            Quantity = 5m
        };

        dataContext.PurchaseOrders.Add(seed);
        dataContext.PurchaseOrderDetails.Add(poDetailSeed);
        await dataContext.SaveChangesAsync();
        
        // Act
        Result<GetPurchaseOrderDto> actual = await service.ReturnPurchaseOrderAsync(seed.Id);
        
        // Assert
        Assert.True(actual.IsSuccess);
        Assert.NotNull(actual.Value);
        Assert.Empty(actual.Errors);
        Assert.Null(actual.Exception);
        Assert.Null(actual.FailureType);
        
        Assert.Equal(PurchaseOrderStatus.Returned, actual.Value.Status);
        
        Assert.Equal(seed.Id, actual.Value.Id);
        Assert.Equal(seed.Supplier?.Id, actual.Value.SupplierId);
        Assert.Equal(seed.InvoiceNumber, actual.Value.InvoiceNumber);
        Assert.Equal(seed.DateOrdered, actual.Value.DateOrdered);
        Assert.Equal(seed.DateRequired, actual.Value.DateRequired);
        Assert.Equal(seed.DateShipped, actual.Value.DateShipped);
        
        PurchaseOrder? updated = await dataContext.PurchaseOrders
            .AsNoTracking()
            .SingleOrDefaultAsync(p => p.Id == seed.Id);
        
        Assert.NotNull(updated);
        
        Assert.Equal(seed.Id, updated.Id);
        Assert.Equal(PurchaseOrderStatus.Returned, updated.Status);
    }

    [Fact]
    public async Task ReturnPurchaseOrder_ShouldRemoveStocksFromItems()
    {
        using IServiceScope scope = CreateScope();

        IPurchaseOrderService service = scope.ServiceProvider.GetRequiredService<IPurchaseOrderService>();
        
        DataContext dataContext = scope.ServiceProvider.GetRequiredService<DataContext>();
        
        // Arrange
        Item itemSeedA = new()
        {
            Id = Guid.NewGuid(),
            Name = "Test Item A",
            Uom = new()
            {
                Id = Guid.NewGuid(),
                Name = "Test UOM A",
                Code = "uA"
            },
            Quantity = 5m,
            Locations = ["Shelf A", "Shelf B"]
        };
        
        Item itemSeedB = new()
        {
            Id = Guid.NewGuid(),
            Name = "Test Item B",
            Uom = new()
            {
                Id = Guid.NewGuid(),
                Name = "Test UOM B",
                Code = "uB"
            },
            Quantity = 15m,
            Locations = ["Shelf A", "Shelf B"]
        };
        
        PurchaseOrder seed = new()
        {
            Id = Guid.NewGuid(),
            Status = PurchaseOrderStatus.Received,
            DateOrdered = Instant.FromUtc(2020, 1, 1, 0, 0)
        };

        List<PurchaseOrderDetail> poDetailSeed = [
            new()
            {
                Id = Guid.NewGuid(),
                PurchaseOrder = seed,
                LineSequence = 1,
                Item = itemSeedA,
                UnitPrice = 600m,
                Quantity = 5m
            },
            new()
            {
                Id = Guid.NewGuid(),
                PurchaseOrder = seed,
                LineSequence = 2,
                Item = itemSeedB,
                UnitPrice = 600m,
                Quantity = 10.5m
            }
        ];

        dataContext.Items.AddRange(itemSeedA, itemSeedB);
        dataContext.PurchaseOrders.AddRange(seed);
        dataContext.PurchaseOrderDetails.AddRange(poDetailSeed);
        await dataContext.SaveChangesAsync();
        
        // Act
        Result<GetPurchaseOrderDto> actual = await service.ReturnPurchaseOrderAsync(seed.Id);
        
        // Assert
        Assert.True(actual.IsSuccess);
        Assert.NotNull(actual.Value);
        Assert.Empty(actual.Errors);
        Assert.Null(actual.Exception);
        Assert.Null(actual.FailureType);
        
        Assert.Equal(PurchaseOrderStatus.Returned, actual.Value.Status);
        
        List<PurchaseOrderDetail> poDetails = await dataContext.PurchaseOrderDetails
            .AsNoTracking()
            .Include(p => p.Item)
            .Where(p => p.PurchaseOrder.Id == seed.Id)
            .ToListAsync();
        
        Assert.NotNull(poDetails);
        Assert.NotEmpty(poDetails);
        
        Dictionary<Guid, decimal> quantityByItemId = poDetails
            .ToDictionary(
                p => p.Item.Id,
                p => p.Item.Quantity
            );
        
        Assert.Equal(0m, quantityByItemId[itemSeedA.Id]);
        Assert.Equal(4.5m, quantityByItemId[itemSeedB.Id]);
    }

    [Fact]
    public async Task ReturnPurchaseOrder_ShouldFailIfSalesOrderIdIsEmpty()
    {
        using IServiceScope scope = CreateScope();

        IPurchaseOrderService service = scope.ServiceProvider.GetRequiredService<IPurchaseOrderService>();
        
        // Arrange
        
        // Act
        Result<GetPurchaseOrderDto> actual = await service.ReturnPurchaseOrderAsync(Guid.Empty);
        
        // Assert
        Assert.False(actual.IsSuccess);
        Assert.Null(actual.Value);
        // TODO: Change Error Message
        Assert.Contains("Purchase order not found.", actual.Errors);
        Assert.Null(actual.Exception);
        Assert.Equal(FailureType.NotFound, actual.FailureType);
    }

    [Fact]
    public async Task ReturnPurchaseOrder_ShouldFailIfSalesOrderDoesNotExist()
    {
        using IServiceScope scope = CreateScope();

        IPurchaseOrderService service = scope.ServiceProvider.GetRequiredService<IPurchaseOrderService>();
        
        // Arrange
        
        // Act
        Result<GetPurchaseOrderDto> actual = await service.ReturnPurchaseOrderAsync(Guid.NewGuid());
        
        // Assert
        Assert.False(actual.IsSuccess);
        Assert.Null(actual.Value);
        Assert.Contains("Purchase order not found.", actual.Errors);
        Assert.Null(actual.Exception);
        Assert.Equal(FailureType.NotFound, actual.FailureType);
    }

    [Fact]
    public async Task ReturnPurchaseOrder_ShouldRejectNotReceivedSalesOrder()
    {
        using IServiceScope scope = CreateScope();

        IPurchaseOrderService service = scope.ServiceProvider.GetRequiredService<IPurchaseOrderService>();
        
        DataContext dataContext = scope.ServiceProvider.GetRequiredService<DataContext>();
        
        // Arrange
        PurchaseOrder seed = new()
        {
            Id = Guid.NewGuid(),
            Status = PurchaseOrderStatus.Returned,
            DateOrdered = Instant.FromUtc(2020, 1, 1, 0, 0)
        };

        dataContext.PurchaseOrders.Add(seed);
        await dataContext.SaveChangesAsync();
        
        // Act
        Result<GetPurchaseOrderDto> actual = await service.ReturnPurchaseOrderAsync(seed.Id);
        
        // Assert
        Assert.False(actual.IsSuccess);
        Assert.Null(actual.Value);
        Assert.Contains("Purchase order cannot be returned.", actual.Errors);
        Assert.Null(actual.Exception);
        Assert.Equal(FailureType.BusinessLogic, actual.FailureType);
    }
}