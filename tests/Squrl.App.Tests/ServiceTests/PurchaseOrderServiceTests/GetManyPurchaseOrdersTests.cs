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
public class GetManyPurchaseOrdersTests : IntegrationTest
{
    public GetManyPurchaseOrdersTests(SqurlWebApplicationFactory factory) : base(factory)
    {
        
    }

    [Fact]
    public async Task GetManyPurchaseOrders_ShouldReturnPurchaseOrders()
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
            Quantity = 100m,
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
            Quantity = 100m,
            Locations = ["Shelf A", "Shelf B"]
        };
        
        List<PurchaseOrder> seed = Enumerable.Range(1, 15)
            .Select(_ => new PurchaseOrder
            {
                Id = Guid.NewGuid(),
                Status = PurchaseOrderStatus.Pending,
                DateOrdered = Instant.FromUtc(2020, 1, 1, 0, 0)
            })
            .ToList();

        List<PurchaseOrderDetail> poDetailSeed = seed
            .SelectMany(order =>
                new[]
                {
                    new PurchaseOrderDetail
                    {
                        Id = Guid.NewGuid(),
                        PurchaseOrder = order,
                        LineSequence = 1,
                        Item = itemSeedA,
                        UnitPrice = 600m,
                        Quantity = 5m
                    },
                    new PurchaseOrderDetail
                    {
                        Id = Guid.NewGuid(),
                        PurchaseOrder = order,
                        LineSequence = 2,
                        Item = itemSeedB,
                        UnitPrice = 600m,
                        Quantity = 5m
                    }
                })
            .ToList();
        
        dataContext.Items.AddRange(itemSeedA, itemSeedB);
        dataContext.PurchaseOrders.AddRange(seed);
        dataContext.PurchaseOrderDetails.AddRange(poDetailSeed);
        await dataContext.SaveChangesAsync();
        
        // Act
        Result<GetManyPurchaseOrdersDto> actual = await service.GetManyPurchaseOrdersAsync();
        
        // Assert
        Assert.True(actual.IsSuccess);
        Assert.NotNull(actual.Value);
        Assert.NotNull(actual.Value.PurchaseOrders);
        Assert.NotEmpty(actual.Value.PurchaseOrders);
        Assert.Empty(actual.Errors);
        Assert.Null(actual.Exception);
        Assert.Null(actual.FailureType);
        
        Assert.Equal(1, actual.Value.PageNumber);
        Assert.Equal(10, actual.Value.PageSize);
        Assert.Equal(15, actual.Value.TotalCount);
        Assert.Equal(10, actual.Value.PurchaseOrders.Count);
    }

    [Fact]
    public async Task GetManyPurchaseOrders_ShouldReturnEmptyListIfPurchaseOrdersDoNotExist()
    {
        using IServiceScope scope = CreateScope();

        IPurchaseOrderService service = scope.ServiceProvider.GetRequiredService<IPurchaseOrderService>();
        
        // Arrange
        
        // Act
        Result<GetManyPurchaseOrdersDto> actual = await service.GetManyPurchaseOrdersAsync();
        
        // Assert
        Assert.True(actual.IsSuccess);
        Assert.NotNull(actual.Value);
        Assert.NotNull(actual.Value.PurchaseOrders);
        Assert.Empty(actual.Value.PurchaseOrders);
        Assert.Empty(actual.Errors);
        Assert.Null(actual.Exception);
        Assert.Null(actual.FailureType);
        
        Assert.Equal(1, actual.Value.PageNumber);
        Assert.Equal(10, actual.Value.PageSize);
        Assert.Equal(0, actual.Value.TotalCount);
    }

    [Fact]
    public async Task GetManyPurchaseOrders_ShouldReturnPagedPurchaseOrders()
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
            Quantity = 100m,
            Locations = ["Shelf A", "Shelf B"]
        };
        
        List<PurchaseOrder> seed = Enumerable.Range(1, 15)
            .Select(_ => new PurchaseOrder
            {
                Id = Guid.NewGuid(),
                Status = PurchaseOrderStatus.Pending,
                DateOrdered = Instant.FromUtc(2020, 1, 1, 0, 0)
            })
            .ToList();

        List<PurchaseOrderDetail> poDetailSeed = seed
            .SelectMany(order =>
                new[]
                {
                    new PurchaseOrderDetail
                    {
                        Id = Guid.NewGuid(),
                        PurchaseOrder = order,
                        LineSequence = 1,
                        Item = itemSeed,
                        UnitPrice = 600m,
                        Quantity = 5m
                    }
                })
            .ToList();
        
        dataContext.Items.Add(itemSeed);
        dataContext.PurchaseOrders.AddRange(seed);
        dataContext.PurchaseOrderDetails.AddRange(poDetailSeed);
        await dataContext.SaveChangesAsync();
        
        // Act
        Result<GetManyPurchaseOrdersDto> actual = await service.GetManyPurchaseOrdersAsync(pageNumber: 3, pageSize: 6);
        
        // Assert
        Assert.True(actual.IsSuccess);
        Assert.NotNull(actual.Value);
        Assert.NotNull(actual.Value.PurchaseOrders);
        Assert.NotEmpty(actual.Value.PurchaseOrders);
        Assert.Empty(actual.Errors);
        Assert.Null(actual.Exception);
        Assert.Null(actual.FailureType);
        
        Assert.Equal(3, actual.Value.PageNumber);
        Assert.Equal(6, actual.Value.PageSize);
        Assert.Equal(15, actual.Value.TotalCount);
        Assert.Equal(3, actual.Value.PurchaseOrders.Count);
    }

    [Fact]
    public async Task GetManyPurchaseOrders_ShouldReturnFilteredPurchaseOrdersBySupplier()
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
            Quantity = 100m,
            Locations = ["Shelf A", "Shelf B"]
        };
        
        List<PurchaseOrder> seed = Enumerable.Range(1, 15)
            .Select(i => new PurchaseOrder
            {
                Id = Guid.NewGuid(),
                Supplier = new()
                {
                    Id = Guid.NewGuid(),
                    Name = $"Test Supplier {i}",
                },
                Status = PurchaseOrderStatus.Pending,
                DateOrdered = Instant.FromUtc(2020, 1, 1, 0, 0)
            })
            .ToList();

        List<PurchaseOrderDetail> poDetailSeed = seed
            .SelectMany(order =>
                new[]
                {
                    new PurchaseOrderDetail
                    {
                        Id = Guid.NewGuid(),
                        PurchaseOrder = order,
                        LineSequence = 1,
                        Item = itemSeed,
                        UnitPrice = 600m,
                        Quantity = 5m
                    }
                })
            .ToList();
        
        dataContext.Items.Add(itemSeed);
        dataContext.PurchaseOrders.AddRange(seed);
        dataContext.PurchaseOrderDetails.AddRange(poDetailSeed);
        await dataContext.SaveChangesAsync();
        
        // Act
        Result<GetManyPurchaseOrdersDto> actual = await service
            .GetManyPurchaseOrdersAsync(supplierId: seed[0].Supplier?.Id);
        
        // Assert
        Assert.True(actual.IsSuccess);
        Assert.NotNull(actual.Value);
        Assert.NotNull(actual.Value.PurchaseOrders);
        Assert.NotEmpty(actual.Value.PurchaseOrders);
        Assert.Empty(actual.Errors);
        Assert.Null(actual.Exception);
        Assert.Null(actual.FailureType);
        
        Assert.Equal(1, actual.Value.PageNumber);
        Assert.Equal(10, actual.Value.PageSize);
        Assert.Equal(1, actual.Value.TotalCount);
        Assert.Single(actual.Value.PurchaseOrders);
    }
    
    [Fact]
    public async Task GetManyPurchaseOrders_ShouldReturnFilteredPurchaseOrdersByStatusPending()
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
            Quantity = 100m,
            Locations = ["Shelf A", "Shelf B"]
        };
        
        PurchaseOrderStatus[] statuses =
        [
            PurchaseOrderStatus.Pending, PurchaseOrderStatus.ToOrder, PurchaseOrderStatus.Ordered,
            PurchaseOrderStatus.InTransit, PurchaseOrderStatus.Received, PurchaseOrderStatus.canceled,
            PurchaseOrderStatus.Lost, PurchaseOrderStatus.Failed, PurchaseOrderStatus.Returned,
            PurchaseOrderStatus.Pending, PurchaseOrderStatus.ToOrder, PurchaseOrderStatus.Ordered,
            PurchaseOrderStatus.InTransit, PurchaseOrderStatus.Received, PurchaseOrderStatus.canceled
        ];
        
        List<PurchaseOrder> seed = Enumerable.Range(1, 15)
            .Select(i => new PurchaseOrder
            {
                Id = Guid.NewGuid(),
                Status = statuses[i - 1],
                DateOrdered = Instant.FromUtc(2020, 1, 1, 0, 0)
            })
            .ToList();

        List<PurchaseOrderDetail> poDetailSeed = seed
            .SelectMany(order =>
                new[]
                {
                    new PurchaseOrderDetail
                    {
                        Id = Guid.NewGuid(),
                        PurchaseOrder = order,
                        LineSequence = 1,
                        Item = itemSeed,
                        UnitPrice = 600m,
                        Quantity = 5m
                    }
                })
            .ToList();
        
        dataContext.Items.Add(itemSeed);
        dataContext.PurchaseOrders.AddRange(seed);
        dataContext.PurchaseOrderDetails.AddRange(poDetailSeed);
        await dataContext.SaveChangesAsync();
        
        // Act
        Result<GetManyPurchaseOrdersDto> actual = await service
            .GetManyPurchaseOrdersAsync(isPending: true);
        
        // Assert
        Assert.True(actual.IsSuccess);
        Assert.NotNull(actual.Value);
        Assert.NotNull(actual.Value.PurchaseOrders);
        Assert.NotEmpty(actual.Value.PurchaseOrders);
        Assert.Empty(actual.Errors);
        Assert.Null(actual.Exception);
        Assert.Null(actual.FailureType);
        
        Assert.Equal(1, actual.Value.PageNumber);
        Assert.Equal(10, actual.Value.PageSize);
        Assert.Equal(8, actual.Value.TotalCount);
        Assert.Equal(8, actual.Value.PurchaseOrders.Count);
    }
    
    [Fact]
    public async Task GetManyPurchaseOrders_ShouldReturnFilteredPurchaseOrdersByStatusReceived()
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
            Quantity = 100m,
            Locations = ["Shelf A", "Shelf B"]
        };
        
        PurchaseOrderStatus[] statuses =
        [
            PurchaseOrderStatus.Pending, PurchaseOrderStatus.ToOrder, PurchaseOrderStatus.Ordered,
            PurchaseOrderStatus.InTransit, PurchaseOrderStatus.Received, PurchaseOrderStatus.canceled,
            PurchaseOrderStatus.Lost, PurchaseOrderStatus.Failed, PurchaseOrderStatus.Returned,
            PurchaseOrderStatus.Pending, PurchaseOrderStatus.ToOrder, PurchaseOrderStatus.Ordered,
            PurchaseOrderStatus.InTransit, PurchaseOrderStatus.Received, PurchaseOrderStatus.canceled
        ];
        
        List<PurchaseOrder> seed = Enumerable.Range(1, 15)
            .Select(i => new PurchaseOrder
            {
                Id = Guid.NewGuid(),
                Status = statuses[i - 1],
                DateOrdered = Instant.FromUtc(2020, 1, 1, 0, 0)
            })
            .ToList();

        List<PurchaseOrderDetail> poDetailSeed = seed
            .SelectMany(order =>
                new[]
                {
                    new PurchaseOrderDetail
                    {
                        Id = Guid.NewGuid(),
                        PurchaseOrder = order,
                        LineSequence = 1,
                        Item = itemSeed,
                        UnitPrice = 600m,
                        Quantity = 5m
                    }
                })
            .ToList();
        
        dataContext.Items.Add(itemSeed);
        dataContext.PurchaseOrders.AddRange(seed);
        dataContext.PurchaseOrderDetails.AddRange(poDetailSeed);
        await dataContext.SaveChangesAsync();
        
        // Act
        Result<GetManyPurchaseOrdersDto> actual = await service
            .GetManyPurchaseOrdersAsync(isReceived: true);
        
        // Assert
        Assert.True(actual.IsSuccess);
        Assert.NotNull(actual.Value);
        Assert.NotNull(actual.Value.PurchaseOrders);
        Assert.NotEmpty(actual.Value.PurchaseOrders);
        Assert.Empty(actual.Errors);
        Assert.Null(actual.Exception);
        Assert.Null(actual.FailureType);
        
        Assert.Equal(1, actual.Value.PageNumber);
        Assert.Equal(10, actual.Value.PageSize);
        Assert.Equal(2, actual.Value.TotalCount);
        Assert.Equal(2, actual.Value.PurchaseOrders.Count);
    }
    
    [Fact]
    public async Task GetManyPurchaseOrders_ShouldReturnFilteredPurchaseOrdersByStatuscanceled()
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
            Quantity = 100m,
            Locations = ["Shelf A", "Shelf B"]
        };
        
        PurchaseOrderStatus[] statuses =
        [
            PurchaseOrderStatus.Pending, PurchaseOrderStatus.ToOrder, PurchaseOrderStatus.Ordered,
            PurchaseOrderStatus.InTransit, PurchaseOrderStatus.Received, PurchaseOrderStatus.canceled,
            PurchaseOrderStatus.Lost, PurchaseOrderStatus.Failed, PurchaseOrderStatus.Returned,
            PurchaseOrderStatus.Pending, PurchaseOrderStatus.ToOrder, PurchaseOrderStatus.Ordered,
            PurchaseOrderStatus.InTransit, PurchaseOrderStatus.Received, PurchaseOrderStatus.canceled
        ];
        
        List<PurchaseOrder> seed = Enumerable.Range(1, 15)
            .Select(i => new PurchaseOrder
            {
                Id = Guid.NewGuid(),
                Status = statuses[i - 1],
                DateOrdered = Instant.FromUtc(2020, 1, 1, 0, 0)
            })
            .ToList();

        List<PurchaseOrderDetail> poDetailSeed = seed
            .SelectMany(order =>
                new[]
                {
                    new PurchaseOrderDetail
                    {
                        Id = Guid.NewGuid(),
                        PurchaseOrder = order,
                        LineSequence = 1,
                        Item = itemSeed,
                        UnitPrice = 600m,
                        Quantity = 5m
                    }
                })
            .ToList();
        
        dataContext.Items.Add(itemSeed);
        dataContext.PurchaseOrders.AddRange(seed);
        dataContext.PurchaseOrderDetails.AddRange(poDetailSeed);
        await dataContext.SaveChangesAsync();
        
        // Act
        Result<GetManyPurchaseOrdersDto> actual = await service
            .GetManyPurchaseOrdersAsync(iscanceled: true);
        
        // Assert
        Assert.True(actual.IsSuccess);
        Assert.NotNull(actual.Value);
        Assert.NotNull(actual.Value.PurchaseOrders);
        Assert.NotEmpty(actual.Value.PurchaseOrders);
        Assert.Empty(actual.Errors);
        Assert.Null(actual.Exception);
        Assert.Null(actual.FailureType);
        
        Assert.Equal(1, actual.Value.PageNumber);
        Assert.Equal(10, actual.Value.PageSize);
        Assert.Equal(2, actual.Value.TotalCount);
        Assert.Equal(2, actual.Value.PurchaseOrders.Count);
    }
    
    [Fact]
    public async Task GetManyPurchaseOrders_ShouldReturnFilteredPurchaseOrdersByStatusFailed()
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
            Quantity = 100m,
            Locations = ["Shelf A", "Shelf B"]
        };
        
        PurchaseOrderStatus[] statuses =
        [
            PurchaseOrderStatus.Pending, PurchaseOrderStatus.ToOrder, PurchaseOrderStatus.Ordered,
            PurchaseOrderStatus.InTransit, PurchaseOrderStatus.Received, PurchaseOrderStatus.canceled,
            PurchaseOrderStatus.Lost, PurchaseOrderStatus.Failed, PurchaseOrderStatus.Returned,
            PurchaseOrderStatus.Pending, PurchaseOrderStatus.ToOrder, PurchaseOrderStatus.Ordered,
            PurchaseOrderStatus.InTransit, PurchaseOrderStatus.Received, PurchaseOrderStatus.canceled
        ];
        
        List<PurchaseOrder> seed = Enumerable.Range(1, 15)
            .Select(i => new PurchaseOrder
            {
                Id = Guid.NewGuid(),
                Status = statuses[i - 1],
                DateOrdered = Instant.FromUtc(2020, 1, 1, 0, 0)
            })
            .ToList();

        List<PurchaseOrderDetail> poDetailSeed = seed
            .SelectMany(order =>
                new[]
                {
                    new PurchaseOrderDetail
                    {
                        Id = Guid.NewGuid(),
                        PurchaseOrder = order,
                        LineSequence = 1,
                        Item = itemSeed,
                        UnitPrice = 600m,
                        Quantity = 5m
                    }
                })
            .ToList();
        
        dataContext.Items.Add(itemSeed);
        dataContext.PurchaseOrders.AddRange(seed);
        dataContext.PurchaseOrderDetails.AddRange(poDetailSeed);
        await dataContext.SaveChangesAsync();
        
        // Act
        Result<GetManyPurchaseOrdersDto> actual = await service
            .GetManyPurchaseOrdersAsync(hasFailed: true);
        
        // Assert
        Assert.True(actual.IsSuccess);
        Assert.NotNull(actual.Value);
        Assert.NotNull(actual.Value.PurchaseOrders);
        Assert.NotEmpty(actual.Value.PurchaseOrders);
        Assert.Empty(actual.Errors);
        Assert.Null(actual.Exception);
        Assert.Null(actual.FailureType);
        
        Assert.Equal(1, actual.Value.PageNumber);
        Assert.Equal(10, actual.Value.PageSize);
        Assert.Equal(2, actual.Value.TotalCount);
        Assert.Equal(2, actual.Value.PurchaseOrders.Count);
    }
    
    [Fact]
    public async Task GetManyPurchaseOrders_ShouldReturnFilteredPurchaseOrdersByStatusReturned()
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
            Quantity = 100m,
            Locations = ["Shelf A", "Shelf B"]
        };
        
        PurchaseOrderStatus[] statuses =
        [
            PurchaseOrderStatus.Pending, PurchaseOrderStatus.ToOrder, PurchaseOrderStatus.Ordered,
            PurchaseOrderStatus.InTransit, PurchaseOrderStatus.Received, PurchaseOrderStatus.canceled,
            PurchaseOrderStatus.Lost, PurchaseOrderStatus.Failed, PurchaseOrderStatus.Returned,
            PurchaseOrderStatus.Pending, PurchaseOrderStatus.ToOrder, PurchaseOrderStatus.Ordered,
            PurchaseOrderStatus.InTransit, PurchaseOrderStatus.Received, PurchaseOrderStatus.canceled
        ];
        
        List<PurchaseOrder> seed = Enumerable.Range(1, 15)
            .Select(i => new PurchaseOrder
            {
                Id = Guid.NewGuid(),
                Status = statuses[i - 1],
                DateOrdered = Instant.FromUtc(2020, 1, 1, 0, 0)
            })
            .ToList();

        List<PurchaseOrderDetail> poDetailSeed = seed
            .SelectMany(order =>
                new[]
                {
                    new PurchaseOrderDetail
                    {
                        Id = Guid.NewGuid(),
                        PurchaseOrder = order,
                        LineSequence = 1,
                        Item = itemSeed,
                        UnitPrice = 600m,
                        Quantity = 5m
                    }
                })
            .ToList();
        
        dataContext.Items.Add(itemSeed);
        dataContext.PurchaseOrders.AddRange(seed);
        dataContext.PurchaseOrderDetails.AddRange(poDetailSeed);
        await dataContext.SaveChangesAsync();
        
        // Act
        Result<GetManyPurchaseOrdersDto> actual = await service
            .GetManyPurchaseOrdersAsync(isReturned: true);
        
        // Assert
        Assert.True(actual.IsSuccess);
        Assert.NotNull(actual.Value);
        Assert.NotNull(actual.Value.PurchaseOrders);
        Assert.NotEmpty(actual.Value.PurchaseOrders);
        Assert.Empty(actual.Errors);
        Assert.Null(actual.Exception);
        Assert.Null(actual.FailureType);
        
        Assert.Equal(1, actual.Value.PageNumber);
        Assert.Equal(10, actual.Value.PageSize);
        Assert.Equal(1, actual.Value.TotalCount);
        Assert.Single(actual.Value.PurchaseOrders);
    }
    
    [Fact]
    public async Task GetManyPurchaseOrders_ShouldReturnFilteredPurchaseOrdersByMultipleFilters()
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
            Quantity = 100m,
            Locations = ["Shelf A", "Shelf B"]
        };
        
        PurchaseOrderStatus[] statuses =
        [
            PurchaseOrderStatus.Pending, PurchaseOrderStatus.ToOrder, PurchaseOrderStatus.Ordered,
            PurchaseOrderStatus.InTransit, PurchaseOrderStatus.Received, PurchaseOrderStatus.canceled,
            PurchaseOrderStatus.Lost, PurchaseOrderStatus.Failed, PurchaseOrderStatus.Returned,
            PurchaseOrderStatus.Pending, PurchaseOrderStatus.ToOrder, PurchaseOrderStatus.Ordered,
            PurchaseOrderStatus.InTransit, PurchaseOrderStatus.Received, PurchaseOrderStatus.canceled
        ];
        
        List<PurchaseOrder> seed = Enumerable.Range(1, 15)
            .Select(i => new PurchaseOrder
            {
                Id = Guid.NewGuid(),
                Status = statuses[i - 1],
                DateOrdered = Instant.FromUtc(2020, 1, 1, 0, 0)
            })
            .ToList();

        List<PurchaseOrderDetail> poDetailSeed = seed
            .SelectMany(order =>
                new[]
                {
                    new PurchaseOrderDetail
                    {
                        Id = Guid.NewGuid(),
                        PurchaseOrder = order,
                        LineSequence = 1,
                        Item = itemSeed,
                        UnitPrice = 600m,
                        Quantity = 5m
                    }
                })
            .ToList();
        
        dataContext.Items.Add(itemSeed);
        dataContext.PurchaseOrders.AddRange(seed);
        dataContext.PurchaseOrderDetails.AddRange(poDetailSeed);
        await dataContext.SaveChangesAsync();
        
        // Act
        Result<GetManyPurchaseOrdersDto> actual = await service
            .GetManyPurchaseOrdersAsync(iscanceled: true, hasFailed: true, isReturned: true);
        
        // Assert
        Assert.True(actual.IsSuccess);
        Assert.NotNull(actual.Value);
        Assert.NotNull(actual.Value.PurchaseOrders);
        Assert.NotEmpty(actual.Value.PurchaseOrders);
        Assert.Empty(actual.Errors);
        Assert.Null(actual.Exception);
        Assert.Null(actual.FailureType);
        
        Assert.Equal(1, actual.Value.PageNumber);
        Assert.Equal(10, actual.Value.PageSize);
        Assert.Equal(5, actual.Value.TotalCount);
        Assert.Equal(5, actual.Value.PurchaseOrders.Count);
    }

    [Fact]
    public async Task GetManyPurchaseOrders_ShouldReturnPurchaseOrdersInAscendingOrder()
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
            Quantity = 100m,
            Locations = ["Shelf A", "Shelf B"]
        };
        
        List<PurchaseOrder> seed = Enumerable.Range(1, 10)
            .Select(_ => new PurchaseOrder
            {
                Id = Guid.NewGuid(),
                Status = PurchaseOrderStatus.Pending,
                DateOrdered = Instant.FromUtc(2020, 1, 1, 0, 0)
            })
            .ToList();

        List<PurchaseOrderDetail> poDetailSeed = seed
            .SelectMany(order =>
                new[]
                {
                    new PurchaseOrderDetail
                    {
                        Id = Guid.NewGuid(),
                        PurchaseOrder = order,
                        LineSequence = 1,
                        Item = itemSeed,
                        UnitPrice = 600m,
                        Quantity = 5m
                    }
                })
            .ToList();
        
        dataContext.Items.Add(itemSeed);
        dataContext.PurchaseOrders.AddRange(seed);
        dataContext.PurchaseOrderDetails.AddRange(poDetailSeed);
        
        Instant now = SystemClock.Instance.GetCurrentInstant();

        for (int i = 0; i < seed.Count; i++)
        {
            dataContext.Entry(seed[i])
                .Property<Instant?>("DateCreated")
                .CurrentValue = now.Plus(Duration.FromSeconds(i));
        }
        
        await dataContext.SaveChangesAsync();
        
        List<PurchaseOrder> expected = await dataContext.PurchaseOrders
            .OrderBy(p => EF.Property<Instant>(p, "DateCreated"))
            .ThenBy(p => p.Id)
            .ToListAsync();
        
        // Act
        // Default order direction is ascending,
        // parameter is not required.
        Result<GetManyPurchaseOrdersDto> actual = await service
            .GetManyPurchaseOrdersAsync(orderDirection: SortDirection.Ascending);
        
        // Assert
        Assert.True(actual.IsSuccess);
        Assert.NotNull(actual.Value);
        Assert.NotNull(actual.Value.PurchaseOrders);
        Assert.NotEmpty(actual.Value.PurchaseOrders);
        Assert.Empty(actual.Errors);
        Assert.Null(actual.Exception);
        Assert.Null(actual.FailureType);
        
        Assert.Equal(
            expected.Select(s => s.Id),
            actual.Value.PurchaseOrders.Select(s => s.Id ?? Guid.Empty));
        
        Assert.Equal(1, actual.Value.PageNumber);
        Assert.Equal(10, actual.Value.PageSize);
        Assert.Equal(10, actual.Value.TotalCount);
        Assert.Equal(10, actual.Value.PurchaseOrders.Count);
    }

    [Fact]
    public async Task GetManyPurchaseOrders_ShouldReturnPurchaseOrdersInDescendingOrder()
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
            Quantity = 100m,
            Locations = ["Shelf A", "Shelf B"]
        };
        
        List<PurchaseOrder> seed = Enumerable.Range(1, 10)
            .Select(_ => new PurchaseOrder
            {
                Id = Guid.NewGuid(),
                Status = PurchaseOrderStatus.Pending,
                DateOrdered = Instant.FromUtc(2020, 1, 1, 0, 0)
            })
            .ToList();

        List<PurchaseOrderDetail> poDetailSeed = seed
            .SelectMany(order =>
                new[]
                {
                    new PurchaseOrderDetail
                    {
                        Id = Guid.NewGuid(),
                        PurchaseOrder = order,
                        LineSequence = 1,
                        Item = itemSeed,
                        UnitPrice = 600m,
                        Quantity = 5m
                    }
                })
            .ToList();
        
        dataContext.Items.Add(itemSeed);
        dataContext.PurchaseOrders.AddRange(seed);
        dataContext.PurchaseOrderDetails.AddRange(poDetailSeed);
        
        Instant now = SystemClock.Instance.GetCurrentInstant();

        for (int i = 0; i < seed.Count; i++)
        {
            dataContext.Entry(seed[i])
                .Property<Instant?>("DateCreated")
                .CurrentValue = now.Plus(Duration.FromSeconds(i));
        }
        
        await dataContext.SaveChangesAsync();
        
        List<PurchaseOrder> expected = await dataContext.PurchaseOrders
            .OrderByDescending(p => EF.Property<Instant>(p, "DateCreated"))
            .ThenBy(p => p.Id)
            .ToListAsync();
        
        // Act
        Result<GetManyPurchaseOrdersDto> actual = await service
            .GetManyPurchaseOrdersAsync(orderDirection: SortDirection.Descending);
        
        // Assert
        Assert.True(actual.IsSuccess);
        Assert.NotNull(actual.Value);
        Assert.NotNull(actual.Value.PurchaseOrders);
        Assert.NotEmpty(actual.Value.PurchaseOrders);
        Assert.Empty(actual.Errors);
        Assert.Null(actual.Exception);
        Assert.Null(actual.FailureType);
        
        Assert.Equal(
            expected.Select(s => s.Id),
            actual.Value.PurchaseOrders.Select(s => s.Id ?? Guid.Empty));
        
        Assert.Equal(1, actual.Value.PageNumber);
        Assert.Equal(10, actual.Value.PageSize);
        Assert.Equal(10, actual.Value.TotalCount);
        Assert.Equal(10, actual.Value.PurchaseOrders.Count);
    }
}