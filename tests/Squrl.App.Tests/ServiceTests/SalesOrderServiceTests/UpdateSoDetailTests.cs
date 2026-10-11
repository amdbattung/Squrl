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
public sealed class UpdateSoDetailTests : IntegrationTest
{
    public UpdateSoDetailTests(SqurlWebApplicationFactory factory) : base(factory)
    {
        
    }

    [Fact]
    public async Task UpdateSoDetail_ShouldUpdateAndPersistSoDetail()
    {
        using IServiceScope scope = CreateScope();

        ISalesOrderService service = scope.ServiceProvider.GetRequiredService<ISalesOrderService>();
        
        DataContext dataContext = scope.ServiceProvider.GetRequiredService<DataContext>();
        
        // Arrange
        SalesOrder salesOrderSeed = new()
        {
            Id = Guid.NewGuid(),
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
        
        SalesOrderDetail seed = new()
        {
            Id = Guid.NewGuid(),
            SalesOrder = salesOrderSeed,
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

        dataContext.SalesOrders.Add(salesOrderSeed);
        dataContext.SalesOrderDetails.Add(seed);
        await dataContext.SaveChangesAsync();
        
        UpdateSoDetailDto dto = new()
        {
            SalesOrderId = salesOrderSeed.Id,
            LineSequence = 1,
            ItemId = itemSeed.Id,
            UnitPrice = 700m,
            Quantity = 6m
        };
        
        // Act
        Result<GetSoDetailDto> actual = await service.UpdateSoDetailAsync(seed.Id, dto);
        
        // Assert
        Assert.True(actual.IsSuccess);
        Assert.NotNull(actual.Value);
        Assert.Empty(actual.Errors);
        Assert.Null(actual.Exception);
        Assert.Null(actual.FailureType);
        
        Assert.Equal(seed.Id, actual.Value.Id);
        Assert.Equal(dto.SalesOrderId, actual.Value.SalesOrderId);
        Assert.Equal(dto.LineSequence, actual.Value.LineSequence);
        Assert.Equal(dto.ItemId, actual.Value.ItemId);
        Assert.Equal(dto.UnitPrice, actual.Value.UnitPrice);
        Assert.Equal(dto.Quantity, actual.Value.Quantity);
        
        SalesOrderDetail? updated = await dataContext.SalesOrderDetails
            .AsNoTracking()
            .Include(salesOrderDetail => salesOrderDetail.SalesOrder)
            .Include(salesOrderDetail => salesOrderDetail.Item)
            .SingleOrDefaultAsync(s => s.Id == seed.Id);
        
        Assert.NotNull(updated);
        
        Assert.Equal(seed.Id, updated.Id);
        Assert.Equal(dto.SalesOrderId, updated.SalesOrder.Id);
        Assert.Equal(dto.LineSequence, updated.LineSequence);
        Assert.Equal(dto.ItemId, updated.Item.Id);
        Assert.Equal(dto.UnitPrice, updated.UnitPrice);
        Assert.Equal(dto.Quantity, updated.Quantity);
    }

    [Fact]
    public async Task UpdateSoDetail_ShouldFailIfIdIsEmpty()
    {
        using IServiceScope scope = CreateScope();

        ISalesOrderService service = scope.ServiceProvider.GetRequiredService<ISalesOrderService>();
        
        // Arrange
        UpdateSoDetailDto dto = new()
        {
            SalesOrderId = Guid.NewGuid(),
            LineSequence = 1,
            ItemId = Guid.NewGuid(),
            UnitPrice = 600m,
            Quantity = 5m
        };
        
        // Act
        Result<GetSoDetailDto> actual = await service.UpdateSoDetailAsync(Guid.Empty, dto);
        
        // Assert
        Assert.False(actual.IsSuccess);
        Assert.Null(actual.Value);
        Assert.Contains("Null or invalid ID.", actual.Errors);
        Assert.Null(actual.Exception);
        Assert.Equal(FailureType.Validation, actual.FailureType);
    }

    [Fact]
    public async Task UpdateSoDetail_ShouldFailIfSoDetailDoesNotExist()
    {
        using IServiceScope scope = CreateScope();

        ISalesOrderService service = scope.ServiceProvider.GetRequiredService<ISalesOrderService>();
        
        // Arrange
        UpdateSoDetailDto dto = new()
        {
            SalesOrderId = Guid.NewGuid(),
            LineSequence = 1,
            ItemId = Guid.NewGuid(),
            UnitPrice = 600m,
            Quantity = 5m
        };
        
        // Act
        Result<GetSoDetailDto> actual = await service.UpdateSoDetailAsync(Guid.NewGuid(), dto);
        
        // Assert
        Assert.False(actual.IsSuccess);
        Assert.Null(actual.Value);
        Assert.Contains("SO detail not found.", actual.Errors);
        Assert.Null(actual.Exception);
        Assert.Equal(FailureType.NotFound, actual.FailureType);
    }

    [Fact]
    public async Task UpdateSoDetail_ShouldFailWhenDtoIsInvalid()
    {
        using IServiceScope scope = CreateScope();

        ISalesOrderService service = scope.ServiceProvider.GetRequiredService<ISalesOrderService>();
        
        // Arrange
        UpdateSoDetailDto dto = new()
        {
            SalesOrderId = Guid.NewGuid(),
            LineSequence = 0,
            ItemId = Guid.NewGuid(),
            UnitPrice = 600m,
            Quantity = 5m
        };
        
        // Act
        Result<GetSoDetailDto> actual = await service.UpdateSoDetailAsync(Guid.NewGuid(), dto);
        
        // Assert
        Assert.False(actual.IsSuccess);
        Assert.Null(actual.Value);
        Assert.Contains("Line Sequence must start at 1.", actual.Errors);
        Assert.Null(actual.Exception);
        Assert.Equal(FailureType.Validation, actual.FailureType);
    }

    [Fact]
    public async Task UpdateSoDetail_ShouldNotPersistWhenValidationFails()
    {
        using IServiceScope scope = CreateScope();

        ISalesOrderService service = scope.ServiceProvider.GetRequiredService<ISalesOrderService>();
        
        DataContext dataContext = scope.ServiceProvider.GetRequiredService<DataContext>();
        
        // Arrange
        UpdateSoDetailDto dto = new()
        {
            SalesOrderId = Guid.NewGuid(),
            LineSequence = 0,
            ItemId = Guid.NewGuid(),
            UnitPrice = 600m,
            Quantity = 5m
        };
        
        // Act
        Result<GetSoDetailDto> actual = await service.UpdateSoDetailAsync(Guid.NewGuid(), dto);
        
        // Assert
        Assert.False(actual.IsSuccess);
        Assert.Null(actual.Value);
        Assert.NotEmpty(actual.Errors);
        Assert.Null(actual.Exception);
        Assert.Equal(FailureType.Validation, actual.FailureType);
        
        bool exists = await dataContext.SalesOrderDetails
            .AnyAsync(s => s.SalesOrder.Id == dto.SalesOrderId);

        Assert.False(exists);
    }
}