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
public class GetPoDetailsTests : IntegrationTest
{
    public GetPoDetailsTests(SqurlWebApplicationFactory factory) : base(factory)
    {
        
    }

    [Fact]
    public async Task GetPoDetails_ShouldReturnPoDetails()
    {
        using IServiceScope scope = CreateScope();

        IPurchaseOrderService service = scope.ServiceProvider.GetRequiredService<IPurchaseOrderService>();
        
        DataContext dataContext = scope.ServiceProvider.GetRequiredService<DataContext>();
        
        // Arrange
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
        
        PurchaseOrder purchaseOrderSeed = new()
        {
            Id = Guid.NewGuid(),
            Status = PurchaseOrderStatus.Pending,
            DateOrdered = Instant.FromUtc(2020, 1, 1, 0, 0)
        };
        
        List<PurchaseOrderDetail> seed = Enumerable.Range(1, 15)
            .Select(i => new PurchaseOrderDetail
            {
                Id = Guid.NewGuid(),
                PurchaseOrder = purchaseOrderSeed,
                LineSequence = i,
                Item = itemSeed,
                UnitPrice = 600m,
                Quantity = 5m
            })
            .ToList();
        
        dataContext.PurchaseOrders.Add(purchaseOrderSeed);
        dataContext.PurchaseOrderDetails.AddRange(seed);
        await dataContext.SaveChangesAsync();
        
        // Act
        Result<GetManyPoDetailsDto> actual = await service.GetPoDetailsAsync(purchaseOrderSeed.Id);
        
        // Assert
        Assert.True(actual.IsSuccess);
        Assert.NotNull(actual.Value);
        Assert.NotNull(actual.Value.PoDetails);
        Assert.NotEmpty(actual.Value.PoDetails);
        Assert.Empty(actual.Errors);
        Assert.Null(actual.Exception);
        Assert.Null(actual.FailureType);
        
        Assert.Equal(1, actual.Value.PageNumber);
        Assert.Equal(15, actual.Value.PageSize);
        Assert.Equal(15, actual.Value.TotalCount);
        Assert.Equal(15, actual.Value.PoDetails.Count);
    }

    [Fact]
    public async Task GetPoDetails_ShouldFailIfSalesOrderIdIsEmpty()
    {
        using IServiceScope scope = CreateScope();

        IPurchaseOrderService service = scope.ServiceProvider.GetRequiredService<IPurchaseOrderService>();
        
        // Arrange
        
        // Act
        Result<GetPoDetailDto> actual = await service.GetPoDetailByIdAsync(Guid.Empty);
        
        // Assert
        Assert.False(actual.IsSuccess);
        Assert.Null(actual.Value);
        Assert.Contains("Null or invalid ID.", actual.Errors);
        Assert.Null(actual.Exception);
        Assert.Equal(FailureType.Validation, actual.FailureType);
    }

    [Fact]
    public async Task GetPoDetails_ShouldFailIfSalesOrderDoesNotExist()
    {
        using IServiceScope scope = CreateScope();

        IPurchaseOrderService service = scope.ServiceProvider.GetRequiredService<IPurchaseOrderService>();
        
        // Arrange
        
        // Act
        Result<GetPoDetailDto> actual = await service.GetPoDetailByIdAsync(Guid.NewGuid());
        
        // Assert
        Assert.False(actual.IsSuccess);
        Assert.Null(actual.Value);
        // TODO: Change Error Message
        Assert.Contains("PO detail not found.", actual.Errors);
        Assert.Null(actual.Exception);
        Assert.Equal(FailureType.NotFound, actual.FailureType);
    }

    [Fact]
    public async Task GetPoDetails_ShouldReturnPoDetailsInAscendingOrder()
    {
        using IServiceScope scope = CreateScope();

        IPurchaseOrderService service = scope.ServiceProvider.GetRequiredService<IPurchaseOrderService>();
        
        DataContext dataContext = scope.ServiceProvider.GetRequiredService<DataContext>();
        
        // Arrange
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
        
        PurchaseOrder purchaseOrderSeed = new()
        {
            Id = Guid.NewGuid(),
            Status = PurchaseOrderStatus.Pending,
            DateOrdered = Instant.FromUtc(2020, 1, 1, 0, 0)
        };
        
        List<PurchaseOrderDetail> seed = Enumerable.Range(1, 15)
            .Select(i => new PurchaseOrderDetail
            {
                Id = Guid.NewGuid(),
                PurchaseOrder = purchaseOrderSeed,
                LineSequence = i,
                Item = itemSeed,
                UnitPrice = 600m,
                Quantity = 5m
            })
            .ToList();
        
        dataContext.PurchaseOrders.Add(purchaseOrderSeed);
        dataContext.PurchaseOrderDetails.AddRange(seed);
        await dataContext.SaveChangesAsync();
        
        List<PurchaseOrderDetail> expected = await dataContext.PurchaseOrderDetails
            .OrderBy(p => p.LineSequence)
            .ThenBy(p => EF.Property<Instant>(p, "DateCreated"))
            .ThenBy(p => p.Id)
            .ToListAsync();
        
        // Act
        // Default order direction is ascending,
        // parameter is not required.
        Result<GetManyPoDetailsDto> actual = await service
            .GetPoDetailsAsync(
                purchaseOrderId: purchaseOrderSeed.Id,
                orderDirection: SortDirection.Ascending);
        
        // Assert
        Assert.True(actual.IsSuccess);
        Assert.NotNull(actual.Value);
        Assert.NotNull(actual.Value.PoDetails);
        Assert.NotEmpty(actual.Value.PoDetails);
        Assert.Empty(actual.Errors);
        Assert.Null(actual.Exception);
        Assert.Null(actual.FailureType);
        
        Assert.Equal(
            expected.Select(p => p.Id),
            actual.Value.PoDetails.Select(p => p.Id ?? Guid.Empty));
        
        Assert.Equal(1, actual.Value.PageNumber);
        Assert.Equal(15, actual.Value.PageSize);
        Assert.Equal(15, actual.Value.TotalCount);
        Assert.Equal(15, actual.Value.PoDetails.Count);
    }

    [Fact]
    public async Task GetPoDetails_ShouldReturnPoDetailsInDescendingOrder()
    {
        using IServiceScope scope = CreateScope();

        IPurchaseOrderService service = scope.ServiceProvider.GetRequiredService<IPurchaseOrderService>();
        
        DataContext dataContext = scope.ServiceProvider.GetRequiredService<DataContext>();
        
        // Arrange
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
        
        PurchaseOrder purchaseOrderSeed = new()
        {
            Id = Guid.NewGuid(),
            Status = PurchaseOrderStatus.Pending,
            DateOrdered = Instant.FromUtc(2020, 1, 1, 0, 0)
        };
        
        List<PurchaseOrderDetail> seed = Enumerable.Range(1, 15)
            .Select(i => new PurchaseOrderDetail
            {
                Id = Guid.NewGuid(),
                PurchaseOrder = purchaseOrderSeed,
                LineSequence = i,
                Item = itemSeed,
                UnitPrice = 600m,
                Quantity = 5m
            })
            .ToList();
        
        dataContext.PurchaseOrders.Add(purchaseOrderSeed);
        dataContext.PurchaseOrderDetails.AddRange(seed);
        await dataContext.SaveChangesAsync();
        
        List<PurchaseOrderDetail> expected = await dataContext.PurchaseOrderDetails
            .OrderByDescending(p => p.LineSequence)
            .ThenByDescending(p => EF.Property<Instant>(p, "DateCreated"))
            .ThenBy(p => p.Id)
            .ToListAsync();
        
        // Act
        Result<GetManyPoDetailsDto> actual = await service
            .GetPoDetailsAsync(
                purchaseOrderId: purchaseOrderSeed.Id,
                orderDirection: SortDirection.Descending);
        
        // Assert
        Assert.True(actual.IsSuccess);
        Assert.NotNull(actual.Value);
        Assert.NotNull(actual.Value.PoDetails);
        Assert.NotEmpty(actual.Value.PoDetails);
        Assert.Empty(actual.Errors);
        Assert.Null(actual.Exception);
        Assert.Null(actual.FailureType);
        
        Assert.Equal(
            expected.Select(p => p.Id),
            actual.Value.PoDetails.Select(p => p.Id ?? Guid.Empty));
        
        Assert.Equal(1, actual.Value.PageNumber);
        Assert.Equal(15, actual.Value.PageSize);
        Assert.Equal(15, actual.Value.TotalCount);
        Assert.Equal(15, actual.Value.PoDetails.Count);
    }
}