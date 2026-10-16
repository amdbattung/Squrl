using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.DependencyInjection;
using NodaTime;
using Squrl.App.Common;
using Squrl.App.Data;
using Squrl.App.Enums;
using Squrl.App.Features.SalesOrderDetails.DTOs;
using Squrl.App.Models;
using Squrl.App.Services.SalesOrder;

namespace Squrl.App.Tests.ServiceTests.SalesOrderServiceTests;

[Collection("IntegrationTests")]
public sealed class GetSoDetailsTests : IntegrationTest
{
    public GetSoDetailsTests(SqurlWebApplicationFactory factory) : base(factory)
    {
        
    }

    [Fact]
    public async Task GetSoDetails_ShouldReturnSoDetails()
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
            Quantity = 10m,
            Locations = ["Shelf A", "Shelf B"]
        };
        
        dataContext.Items.Add(itemSeed);
        
        SalesOrder salesOrderSeed = new()
        {
            Id = Guid.NewGuid(),
            DateOrdered = Instant.FromUtc(2020, 1, 1, 0, 0)
        };
        
        List<SalesOrderDetail> seed = Enumerable.Range(1, 15)
            .Select(i => new SalesOrderDetail
            {
                Id = Guid.NewGuid(),
                SalesOrder = salesOrderSeed,
                LineSequence = i,
                Item = itemSeed,
                UnitPrice = 600m,
                Quantity = 5m
            })
            .ToList();

        dataContext.SalesOrders.Add(salesOrderSeed);
        dataContext.SalesOrderDetails.AddRange(seed);
        await dataContext.SaveChangesAsync();
        
        // Act
        Result<GetManySoDetailsDto> actual = await service.GetSoDetailsAsync(salesOrderSeed.Id);
        
        // Assert
        Assert.True(actual.IsSuccess);
        Assert.NotNull(actual.Value);
        Assert.NotNull(actual.Value.SoDetails);
        Assert.NotEmpty(actual.Value.SoDetails);
        Assert.Empty(actual.Errors);
        Assert.Null(actual.Exception);
        Assert.Null(actual.FailureType);
        
        Assert.Equal(1, actual.Value.PageNumber);
        Assert.Equal(15, actual.Value.PageSize);
        Assert.Equal(15, actual.Value.TotalCount);
        Assert.Equal(15, actual.Value.SoDetails.Count);
    }

    [Fact]
    public async Task GetSoDetails_ShouldFailIfSalesOrderIdIsEmpty()
    {
        using IServiceScope scope = CreateScope();

        ISalesOrderService service = scope.ServiceProvider.GetRequiredService<ISalesOrderService>();
        
        // Arrange
        
        // Act
        Result<GetSoDetailDto> actual = await service.GetSoDetailByIdAsync(Guid.Empty);
        
        // Assert
        Assert.False(actual.IsSuccess);
        Assert.Null(actual.Value);
        Assert.Contains("Null or invalid ID.", actual.Errors);
        Assert.Null(actual.Exception);
        Assert.Equal(FailureType.Validation, actual.FailureType);
    }

    [Fact]
    public async Task GetSoDetails_ShouldFailIfSalesOrderDoesNotExist()
    {
        using IServiceScope scope = CreateScope();

        ISalesOrderService service = scope.ServiceProvider.GetRequiredService<ISalesOrderService>();
        
        // Arrange
        
        // Act
        Result<GetSoDetailDto> actual = await service.GetSoDetailByIdAsync(Guid.NewGuid());
        
        // Assert
        Assert.False(actual.IsSuccess);
        Assert.Null(actual.Value);
        // TODO: Change Error Message
        Assert.Contains("SO detail not found.", actual.Errors);
        Assert.Null(actual.Exception);
        Assert.Equal(FailureType.NotFound, actual.FailureType);
    }

    [Fact]
    public async Task GetSoDetails_ShouldReturnSoDetailsInAscendingOrder()
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
            Quantity = 10m,
            Locations = ["Shelf A", "Shelf B"]
        };
        
        dataContext.Items.Add(itemSeed);
        
        SalesOrder salesOrderSeed = new()
        {
            Id = Guid.NewGuid(),
            DateOrdered = Instant.FromUtc(2020, 1, 1, 0, 0)
        };
        
        List<SalesOrderDetail> seed = Enumerable.Range(1, 15)
            .Select(i => new SalesOrderDetail
            {
                Id = Guid.NewGuid(),
                SalesOrder = salesOrderSeed,
                LineSequence = i,
                Item = itemSeed,
                UnitPrice = 600m,
                Quantity = 5m
            })
            .ToList();

        dataContext.SalesOrders.Add(salesOrderSeed);
        dataContext.SalesOrderDetails.AddRange(seed);
        await dataContext.SaveChangesAsync();

        List<SalesOrderDetail> expected = await dataContext.SalesOrderDetails
            .OrderBy(s => s.LineSequence)
            .ThenBy(s => EF.Property<Instant>(s, "DateCreated"))
            .ThenBy(s => s.Id)
            .ToListAsync();
        
        // Act
        // Default order direction is ascending,
        // parameter is not required.
        Result<GetManySoDetailsDto> actual = await service
            .GetSoDetailsAsync(
                salesOrderId: salesOrderSeed.Id,
                orderDirection: SortDirection.Ascending);
        
        // Assert
        Assert.True(actual.IsSuccess);
        Assert.NotNull(actual.Value);
        Assert.NotNull(actual.Value.SoDetails);
        Assert.NotEmpty(actual.Value.SoDetails);
        Assert.Empty(actual.Errors);
        Assert.Null(actual.Exception);
        Assert.Null(actual.FailureType);
        
        Assert.Equal(
            expected.Select(s => s.Id),
            actual.Value.SoDetails.Select(s => s.Id ?? Guid.Empty));
        
        Assert.Equal(1, actual.Value.PageNumber);
        Assert.Equal(15, actual.Value.PageSize);
        Assert.Equal(15, actual.Value.TotalCount);
        Assert.Equal(15, actual.Value.SoDetails.Count);
    }

    [Fact]
    public async Task GetSoDetails_ShouldReturnSoDetailsInDescendingOrder()
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
            Quantity = 10m,
            Locations = ["Shelf A", "Shelf B"]
        };
        
        dataContext.Items.Add(itemSeed);
        
        SalesOrder salesOrderSeed = new()
        {
            Id = Guid.NewGuid(),
            DateOrdered = Instant.FromUtc(2020, 1, 1, 0, 0)
        };
        
        List<SalesOrderDetail> seed = Enumerable.Range(1, 15)
            .Select(i => new SalesOrderDetail
            {
                Id = Guid.NewGuid(),
                SalesOrder = salesOrderSeed,
                LineSequence = i,
                Item = itemSeed,
                UnitPrice = 600m,
                Quantity = 5m
            })
            .ToList();

        dataContext.SalesOrders.Add(salesOrderSeed);
        dataContext.SalesOrderDetails.AddRange(seed);
        await dataContext.SaveChangesAsync();

        List<SalesOrderDetail> expected = await dataContext.SalesOrderDetails
            .OrderByDescending(s => s.LineSequence)
            .ThenByDescending(s => EF.Property<Instant>(s, "DateCreated"))
            .ThenBy(s => s.Id)
            .ToListAsync();
        
        // Act
        Result<GetManySoDetailsDto> actual = await service
            .GetSoDetailsAsync(
                salesOrderId: salesOrderSeed.Id,
                orderDirection: SortDirection.Descending);
        
        // Assert
        Assert.True(actual.IsSuccess);
        Assert.NotNull(actual.Value);
        Assert.NotNull(actual.Value.SoDetails);
        Assert.NotEmpty(actual.Value.SoDetails);
        Assert.Empty(actual.Errors);
        Assert.Null(actual.Exception);
        Assert.Null(actual.FailureType);
        
        Assert.Equal(
            expected.Select(s => s.Id),
            actual.Value.SoDetails.Select(s => s.Id ?? Guid.Empty));
        
        Assert.Equal(1, actual.Value.PageNumber);
        Assert.Equal(15, actual.Value.PageSize);
        Assert.Equal(15, actual.Value.TotalCount);
        Assert.Equal(15, actual.Value.SoDetails.Count);
    }
}