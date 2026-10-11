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
public sealed class AddSoDetailTests : IntegrationTest
{
    public AddSoDetailTests(SqurlWebApplicationFactory factory) : base(factory)
    {
        
    }

    [Fact]
    public async Task AddSoDetail_ShouldCreateSoDetail()
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
        
        SalesOrderDetail seed = new()
        {
            Id = Guid.NewGuid(),
            SalesOrder = salesOrderSeed,
            LineSequence = 1,
            Item = itemSeed,
            UnitPrice = 600m,
            Quantity = 5m
        };

        dataContext.SalesOrders.Add(salesOrderSeed);
        dataContext.SalesOrderDetails.Add(seed);
        await dataContext.SaveChangesAsync();
        
        CreateSoDetailDto dto = new()
        {
            LineSequence = 2,
            ItemId = itemSeed.Id,
            UnitPrice = 600m,
            Quantity = 5m
        };
        
        // Act
        Result<GetSoDetailDto> actual = await service.AddSoDetailAsync(salesOrderSeed.Id, dto);

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

        SalesOrderDetail? created = await dataContext.SalesOrderDetails
            .AsNoTracking()
            .Include(salesOrderDetail => salesOrderDetail.Item)
            .SingleOrDefaultAsync(s => s.Id == actual.Value.Id);

        Assert.NotNull(created);

        Assert.Equal(actual.Value.Id, created.Id);
        Assert.Equal(dto.LineSequence, created.LineSequence);
        Assert.Equal(dto.ItemId, created.Item.Id);
        Assert.Equal(dto.UnitPrice, created.UnitPrice);
        Assert.Equal(dto.Quantity, created.Quantity);
    }

    [Fact]
    public async Task AddSoDetail_ShouldFailIfSalesOrderIdIsEmpty()
    {
        using IServiceScope scope = CreateScope();

        ISalesOrderService service = scope.ServiceProvider.GetRequiredService<ISalesOrderService>();
        
        // Arrange
        CreateSoDetailDto dto = new()
        {
            LineSequence = 1,
            ItemId = Guid.NewGuid(),
            UnitPrice = 600m,
            Quantity = 5m
        };
        
        // Act
        Result<GetSoDetailDto> actual = await service.AddSoDetailAsync(Guid.Empty, dto);
        
        // Assert
        Assert.False(actual.IsSuccess);
        Assert.Null(actual.Value);
        // TODO: Change Error Message
        Assert.Contains("Failed to create SO detail.", actual.Errors);
        Assert.Null(actual.Exception);
        Assert.Equal(FailureType.BusinessLogic, actual.FailureType);
    }

    [Fact]
    public async Task AddSoDetail_ShouldFailIfSalesOrderDoesNotExist()
    {
        using IServiceScope scope = CreateScope();

        ISalesOrderService service = scope.ServiceProvider.GetRequiredService<ISalesOrderService>();
        
        // Arrange
        CreateSoDetailDto dto = new()
        {
            LineSequence = 1,
            ItemId = Guid.NewGuid(),
            UnitPrice = 600m,
            Quantity = 5m
        };
        
        // Act
        Result<GetSoDetailDto> actual = await service.AddSoDetailAsync(Guid.NewGuid(), dto);
        
        // Assert
        Assert.False(actual.IsSuccess);
        Assert.Null(actual.Value);
        // TODO: Change Error Message
        Assert.Contains("Failed to create SO detail.", actual.Errors);
        Assert.Null(actual.Exception);
        Assert.Equal(FailureType.BusinessLogic, actual.FailureType);
    }

    [Fact]
    public async Task AddSoDetail_ShouldFailWhenDtoIsInvalid()
    {
        using IServiceScope scope = CreateScope();

        ISalesOrderService service = scope.ServiceProvider.GetRequiredService<ISalesOrderService>();

        // Arrange
        CreateSoDetailDto dto = new()
        {
            LineSequence = 0,
            ItemId = Guid.NewGuid(),
            UnitPrice = 600m,
            Quantity = 5m
        };
        
        // Act
        Result<GetSoDetailDto> actual = await service.AddSoDetailAsync(Guid.NewGuid(), dto);

        // Assert
        Assert.False(actual.IsSuccess);
        Assert.Null(actual.Value);
        Assert.Contains("Line Sequence must start at 1.", actual.Errors);
        Assert.Null(actual.Exception);
        Assert.Equal(FailureType.Validation, actual.FailureType);
    }

    [Fact]
    public async Task AddSoDetail_ShouldNotPersistWhenValidationFails()
    {
        using IServiceScope scope = CreateScope();

        ISalesOrderService service = scope.ServiceProvider.GetRequiredService<ISalesOrderService>();
        
        DataContext dataContext = scope.ServiceProvider.GetRequiredService<DataContext>();

        // Arrange
        CreateSoDetailDto dto = new()
        {
            LineSequence = 0,
            ItemId = Guid.NewGuid(),
            UnitPrice = 600m,
            Quantity = 5m
        };
        
        // Act
        Result<GetSoDetailDto> actual = await service.AddSoDetailAsync(Guid.NewGuid(), dto);

        // Assert
        Assert.False(actual.IsSuccess);
        Assert.Null(actual.Value);
        Assert.NotEmpty(actual.Errors);
        Assert.Null(actual.Exception);
        Assert.Equal(FailureType.Validation, actual.FailureType);

        bool exists = await dataContext.SalesOrderDetails
            .AnyAsync(s => s.Item.Id == dto.ItemId);

        Assert.False(exists);
    }
}