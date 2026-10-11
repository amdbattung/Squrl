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
public sealed class RemoveSoDetailTests : IntegrationTest
{
    public RemoveSoDetailTests(SqurlWebApplicationFactory factory) : base(factory)
    {
        
    }

    [Fact]
    public async Task DeleteSoDetail_ShouldDeleteAndReturnSoDetail()
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
        
        SalesOrderDetail seed = new()
        {
            Id = Guid.NewGuid(),
            SalesOrder = salesOrderSeed,
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

        dataContext.SalesOrders.Add(salesOrderSeed);
        dataContext.SalesOrderDetails.Add(seed);
        await dataContext.SaveChangesAsync();
        
        // Act
        Result<GetSoDetailDto> actual = await service.RemoveSoDetailAsync(seed.Id);
        
        // Assert
        Assert.True(actual.IsSuccess);
        Assert.NotNull(actual.Value);
        Assert.Empty(actual.Errors);
        Assert.Null(actual.Exception);
        Assert.Null(actual.FailureType);
        
        Assert.Equal(seed.Id, actual.Value.Id);
        Assert.Equal(seed.SalesOrder.Id, actual.Value.SalesOrderId);
        Assert.Equal(seed.LineSequence, actual.Value.LineSequence);
        Assert.Equal(seed.Item.Id, actual.Value.ItemId);
        Assert.Equal(seed.UnitPrice, actual.Value.UnitPrice);
        Assert.Equal(seed.Quantity, actual.Value.Quantity);
        
        bool exists = await dataContext.SalesOrderDetails
            .AnyAsync(s => s.Id == seed.Id);
        
        Assert.False(exists);
    }

    [Fact]
    public async Task DeleteSoDetail_ShouldFailIfIdIsEmpty()
    {
        using IServiceScope scope = CreateScope();

        ISalesOrderService service = scope.ServiceProvider.GetRequiredService<ISalesOrderService>();
        
        // Arrange
        
        // Act
        Result<GetSoDetailDto> actual = await service.RemoveSoDetailAsync(Guid.Empty);
        
        // Assert
        Assert.False(actual.IsSuccess);
        Assert.Null(actual.Value);
        Assert.Contains("Null or invalid ID.", actual.Errors);
        Assert.Null(actual.Exception);
        Assert.Equal(FailureType.Validation, actual.FailureType);
    }

    [Fact]
    public async Task DeleteSoDetail_ShouldFailIfSoDetailDoesNotExist()
    {
        using IServiceScope scope = CreateScope();

        ISalesOrderService service = scope.ServiceProvider.GetRequiredService<ISalesOrderService>();
        
        // Arrange
        
        // Act
        Result<GetSoDetailDto> actual = await service.RemoveSoDetailAsync(Guid.NewGuid());
        
        // Assert
        Assert.False(actual.IsSuccess);
        Assert.Null(actual.Value);
        Assert.Contains("SO detail not found.", actual.Errors);
        Assert.Null(actual.Exception);
        Assert.Equal(FailureType.NotFound, actual.FailureType);
    }
}