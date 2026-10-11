using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.DependencyInjection;
using NodaTime;
using Squrl.App.Common;
using Squrl.App.Data;
using Squrl.App.Enums;
using Squrl.App.Features.SalesOrders.DTOs;
using Squrl.App.Models;
using Squrl.App.Services.SalesOrder;

namespace Squrl.App.Tests.ServiceTests.SalesOrderServiceTests;

[Collection("IntegrationTests")]
public sealed class GetManySalesOrdersTests : IntegrationTest
{
    public GetManySalesOrdersTests(SqurlWebApplicationFactory factory) : base(factory)
    {
        
    }

    [Fact]
    public async Task GetManySalesOrders_ShouldReturnSalesOrders()
    {
        using IServiceScope scope = CreateScope();

        ISalesOrderService service = scope.ServiceProvider.GetRequiredService<ISalesOrderService>();
        
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
        
        List<SalesOrder> seed = Enumerable.Range(1, 15)
            .Select(_ => new SalesOrder
            {
                Id = Guid.NewGuid(),
                DateOrdered = Instant.FromUtc(2020, 1, 1, 0, 0)
            })
            .ToList();

        List<SalesOrderDetail> soDetailSeed = seed
            .SelectMany(order =>
                new[]
                {
                    new SalesOrderDetail
                    {
                        Id = Guid.NewGuid(),
                        SalesOrder = order,
                        LineSequence = 1,
                        Item = itemSeedA,
                        UnitPrice = 600m,
                        Quantity = 5m
                    },
                    new SalesOrderDetail
                    {
                        Id = Guid.NewGuid(),
                        SalesOrder = order,
                        LineSequence = 2,
                        Item = itemSeedB,
                        UnitPrice = 600m,
                        Quantity = 5m
                    }
                })
            .ToList();
        
        dataContext.Items.AddRange(itemSeedA, itemSeedB);
        dataContext.SalesOrders.AddRange(seed);
        dataContext.SalesOrderDetails.AddRange(soDetailSeed);
        await dataContext.SaveChangesAsync();
        
        // Act
        Result<GetManySalesOrdersDto> actual = await service.GetManySalesOrdersAsync();
        
        // Assert
        Assert.True(actual.IsSuccess);
        Assert.NotNull(actual.Value);
        Assert.NotNull(actual.Value.SalesOrders);
        Assert.NotEmpty(actual.Value.SalesOrders);
        Assert.Empty(actual.Errors);
        Assert.Null(actual.Exception);
        Assert.Null(actual.FailureType);
        
        Assert.Equal(1, actual.Value.PageNumber);
        Assert.Equal(10, actual.Value.PageSize);
        Assert.Equal(15, actual.Value.TotalCount);
        Assert.Equal(10, actual.Value.SalesOrders.Count);
    }

    [Fact]
    public async Task GetManySalesOrders_ShouldReturnEmptyListIfSalesOrdersDoNotExist()
    {
        using IServiceScope scope = CreateScope();

        ISalesOrderService service = scope.ServiceProvider.GetRequiredService<ISalesOrderService>();
        
        // Arrange
        
        // Act
        Result<GetManySalesOrdersDto> actual = await service.GetManySalesOrdersAsync();
        
        // Assert
        Assert.True(actual.IsSuccess);
        Assert.NotNull(actual.Value);
        Assert.NotNull(actual.Value.SalesOrders);
        Assert.Empty(actual.Value.SalesOrders);
        Assert.Empty(actual.Errors);
        Assert.Null(actual.Exception);
        Assert.Null(actual.FailureType);
        
        Assert.Equal(1, actual.Value.PageNumber);
        Assert.Equal(10, actual.Value.PageSize);
        Assert.Equal(0, actual.Value.TotalCount);
    }

    [Fact]
    public async Task GetManySalesOrders_ShouldReturnPagedSalesOrders()
    {
        using IServiceScope scope = CreateScope();

        ISalesOrderService service = scope.ServiceProvider.GetRequiredService<ISalesOrderService>();
        
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
        
        List<SalesOrder> seed = Enumerable.Range(1, 15)
            .Select(_ => new SalesOrder
            {
                Id = Guid.NewGuid(),
                DateOrdered = Instant.FromUtc(2020, 1, 1, 0, 0)
            })
            .ToList();

        List<SalesOrderDetail> soDetailSeed = seed
            .SelectMany(order =>
                new[]
                {
                    new SalesOrderDetail
                    {
                        Id = Guid.NewGuid(),
                        SalesOrder = order,
                        LineSequence = 1,
                        Item = itemSeed,
                        UnitPrice = 600m,
                        Quantity = 5m
                    }
                })
            .ToList();
        
        dataContext.Items.Add(itemSeed);
        dataContext.SalesOrders.AddRange(seed);
        dataContext.SalesOrderDetails.AddRange(soDetailSeed);
        await dataContext.SaveChangesAsync();
        
        // Act
        Result<GetManySalesOrdersDto> actual = await service.GetManySalesOrdersAsync(pageNumber: 3, pageSize: 6);
        
        // Assert
        Assert.True(actual.IsSuccess);
        Assert.NotNull(actual.Value);
        Assert.NotNull(actual.Value.SalesOrders);
        Assert.NotEmpty(actual.Value.SalesOrders);
        Assert.Empty(actual.Errors);
        Assert.Null(actual.Exception);
        Assert.Null(actual.FailureType);
        
        Assert.Equal(3, actual.Value.PageNumber);
        Assert.Equal(6, actual.Value.PageSize);
        Assert.Equal(15, actual.Value.TotalCount);
        Assert.Equal(3, actual.Value.SalesOrders.Count);
    }

    [Fact]
    public async Task GetManySalesOrders_ShouldReturnFilteredSalesOrders()
    {
        using IServiceScope scope = CreateScope();

        ISalesOrderService service = scope.ServiceProvider.GetRequiredService<ISalesOrderService>();
        
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
        
        List<SalesOrder> seed = Enumerable.Range(1, 15)
            .Select(i => new SalesOrder
            {
                Customer = $"Test Customer {i}",
                Id = Guid.NewGuid(),
                DateOrdered = Instant.FromUtc(2020, 1, 1, 0, 0)
            })
            .ToList();

        List<SalesOrderDetail> soDetailSeed = seed
            .SelectMany(order =>
                new[]
                {
                    new SalesOrderDetail
                    {
                        Id = Guid.NewGuid(),
                        SalesOrder = order,
                        LineSequence = 1,
                        Item = itemSeed,
                        UnitPrice = 600m,
                        Quantity = 5m
                    }
                })
            .ToList();
        
        dataContext.Items.Add(itemSeed);
        dataContext.SalesOrders.AddRange(seed);
        dataContext.SalesOrderDetails.AddRange(soDetailSeed);
        await dataContext.SaveChangesAsync();
        
        // Act
        Result<GetManySalesOrdersDto> actual = await service.GetManySalesOrdersAsync(customer: "Test Customer 1");
        
        // Assert
        Assert.True(actual.IsSuccess);
        Assert.NotNull(actual.Value);
        Assert.NotNull(actual.Value.SalesOrders);
        Assert.NotEmpty(actual.Value.SalesOrders);
        Assert.Empty(actual.Errors);
        Assert.Null(actual.Exception);
        Assert.Null(actual.FailureType);
        
        Assert.Equal(1, actual.Value.PageNumber);
        Assert.Equal(10, actual.Value.PageSize);
        Assert.Equal(1, actual.Value.TotalCount);
        Assert.Single(actual.Value.SalesOrders);
    }

    [Fact]
    public async Task GetManySalesOrders_ShouldReturnSalesOrdersInAscendingOrder()
    {
        using IServiceScope scope = CreateScope();

        ISalesOrderService service = scope.ServiceProvider.GetRequiredService<ISalesOrderService>();
        
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
        
        List<SalesOrder> seed = Enumerable.Range(1, 10)
            .Select(_ => new SalesOrder
            {
                Id = Guid.NewGuid(),
                DateOrdered = Instant.FromUtc(2020, 1, 1, 0, 0)
            })
            .ToList();

        List<SalesOrderDetail> soDetailSeed = seed
            .SelectMany(order =>
                new[]
                {
                    new SalesOrderDetail
                    {
                        Id = Guid.NewGuid(),
                        SalesOrder = order,
                        LineSequence = 1,
                        Item = itemSeed,
                        UnitPrice = 600m,
                        Quantity = 5m
                    }
                })
            .ToList();
        
        dataContext.Items.Add(itemSeed);
        dataContext.SalesOrders.AddRange(seed);
        dataContext.SalesOrderDetails.AddRange(soDetailSeed);
        
        Instant now = SystemClock.Instance.GetCurrentInstant();

        for (int i = 0; i < seed.Count; i++)
        {
            dataContext.Entry(seed[i])
                .Property<Instant?>("DateCreated")
                .CurrentValue = now.Plus(Duration.FromSeconds(i));
        }
        
        await dataContext.SaveChangesAsync();
        
        List<SalesOrder> expected = await dataContext.SalesOrders
            .OrderBy(s => EF.Property<Instant>(s, "DateCreated"))
            .ThenBy(s => s.Id)
            .ToListAsync();
        
        // Act
        // Default order direction is ascending,
        // parameter is not required.
        Result<GetManySalesOrdersDto> actual = await service
            .GetManySalesOrdersAsync(orderDirection: SortDirection.Ascending);
        
        // Assert
        Assert.True(actual.IsSuccess);
        Assert.NotNull(actual.Value);
        Assert.NotNull(actual.Value.SalesOrders);
        Assert.NotEmpty(actual.Value.SalesOrders);
        Assert.Empty(actual.Errors);
        Assert.Null(actual.Exception);
        Assert.Null(actual.FailureType);
        
        Assert.Equal(
            expected.Select(s => s.Id),
            actual.Value.SalesOrders.Select(s => s.Id ?? Guid.Empty));
        
        Assert.Equal(1, actual.Value.PageNumber);
        Assert.Equal(10, actual.Value.PageSize);
        Assert.Equal(10, actual.Value.TotalCount);
        Assert.Equal(10, actual.Value.SalesOrders.Count);
    }

    [Fact]
    public async Task GetManySalesOrders_ShouldReturnSalesOrdersInDescendingOrder()
    {
        using IServiceScope scope = CreateScope();

        ISalesOrderService service = scope.ServiceProvider.GetRequiredService<ISalesOrderService>();
        
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
        
        List<SalesOrder> seed = Enumerable.Range(1, 10)
            .Select(_ => new SalesOrder
            {
                Id = Guid.NewGuid(),
                DateOrdered = Instant.FromUtc(2020, 1, 1, 0, 0)
            })
            .ToList();

        List<SalesOrderDetail> soDetailSeed = seed
            .SelectMany(order =>
                new[]
                {
                    new SalesOrderDetail
                    {
                        Id = Guid.NewGuid(),
                        SalesOrder = order,
                        LineSequence = 1,
                        Item = itemSeed,
                        UnitPrice = 600m,
                        Quantity = 5m
                    }
                })
            .ToList();
        
        dataContext.Items.Add(itemSeed);
        dataContext.SalesOrders.AddRange(seed);
        dataContext.SalesOrderDetails.AddRange(soDetailSeed);
        
        Instant now = SystemClock.Instance.GetCurrentInstant();

        for (int i = 0; i < seed.Count; i++)
        {
            dataContext.Entry(seed[i])
                .Property<Instant?>("DateCreated")
                .CurrentValue = now.Plus(Duration.FromSeconds(i));
        }
        
        await dataContext.SaveChangesAsync();
        
        List<SalesOrder> expected = await dataContext.SalesOrders
            .OrderByDescending(s => EF.Property<Instant>(s, "DateCreated"))
            .ThenBy(s => s.Id)
            .ToListAsync();
        
        // Act
        // Default order direction is ascending,
        // parameter is not required.
        Result<GetManySalesOrdersDto> actual = await service
            .GetManySalesOrdersAsync(orderDirection: SortDirection.Descending);
        
        // Assert
        Assert.True(actual.IsSuccess);
        Assert.NotNull(actual.Value);
        Assert.NotNull(actual.Value.SalesOrders);
        Assert.NotEmpty(actual.Value.SalesOrders);
        Assert.Empty(actual.Errors);
        Assert.Null(actual.Exception);
        Assert.Null(actual.FailureType);
        
        Assert.Equal(
            expected.Select(s => s.Id),
            actual.Value.SalesOrders.Select(s => s.Id ?? Guid.Empty));
        
        Assert.Equal(1, actual.Value.PageNumber);
        Assert.Equal(10, actual.Value.PageSize);
        Assert.Equal(10, actual.Value.TotalCount);
        Assert.Equal(10, actual.Value.SalesOrders.Count);
    }
}