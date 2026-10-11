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
public sealed class DeleteSalesOrderTests : IntegrationTest
{
    public DeleteSalesOrderTests(SqurlWebApplicationFactory factory) : base(factory)
    {
        
    }

    [Fact]
    public async Task DeleteSalesOrder_ShouldDeleteAndReturnSalesOrder()
    {
        using IServiceScope scope = CreateScope();

        ISalesOrderService service = scope.ServiceProvider.GetRequiredService<ISalesOrderService>();
        
        DataContext dataContext = scope.ServiceProvider.GetRequiredService<DataContext>();
        
        // Arrange
        SalesOrder seed = new()
        {
            Id = Guid.NewGuid(),
            DateOrdered = Instant.FromUtc(2020, 1, 1, 0, 0)
        };

        SalesOrderDetail soDetailSeed = new()
        {
            Id = Guid.NewGuid(),
            SalesOrder = seed,
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
                Quantity = 100m,
                Locations = ["Shelf A", "Shelf B"]
            },
            UnitPrice = 600m,
            Quantity = 5m
        };

        dataContext.SalesOrders.Add(seed);
        dataContext.SalesOrderDetails.Add(soDetailSeed);
        await dataContext.SaveChangesAsync();
        
        // Act
        Result<GetSalesOrderDto> actual = await service.DeleteSalesOrderAsync(seed.Id);
        
        // Assert
        Assert.True(actual.IsSuccess);
        Assert.NotNull(actual.Value);
        Assert.Empty(actual.Errors);
        Assert.Null(actual.Exception);
        Assert.Null(actual.FailureType);
        
        Assert.Equal(seed.Id, actual.Value.Id);
        Assert.Equal(seed.Customer, actual.Value.Customer);
        Assert.Equal(seed.InvoiceNumber, actual.Value.InvoiceNumber);
        Assert.Equal(seed.DateOrdered, actual.Value.DateOrdered);
        
        bool exists = await dataContext.SalesOrders
            .AnyAsync(s => s.Id == seed.Id);
        
        Assert.False(exists);
    }

    [Fact]
    public async Task DeleteSalesOrder_ShouldFailIfIdIsEmpty()
    {
        using IServiceScope scope = CreateScope();

        ISalesOrderService service = scope.ServiceProvider.GetRequiredService<ISalesOrderService>();
        
        // Arrange
        
        // Act
        Result<GetSalesOrderDto> actual = await service.DeleteSalesOrderAsync(Guid.Empty);
        
        // Assert
        Assert.False(actual.IsSuccess);
        Assert.Null(actual.Value);
        Assert.Contains("Null or invalid ID.", actual.Errors);
        Assert.Null(actual.Exception);
        Assert.Equal(FailureType.Validation, actual.FailureType);
    }

    [Fact]
    public async Task DeleteSalesOrder_ShouldFailIfSalesOrderDoesNotExist()
    {
        using IServiceScope scope = CreateScope();

        ISalesOrderService service = scope.ServiceProvider.GetRequiredService<ISalesOrderService>();
        
        // Arrange
        
        // Act
        Result<GetSalesOrderDto> actual = await service.DeleteSalesOrderAsync(Guid.NewGuid());
        
        // Assert
        Assert.False(actual.IsSuccess);
        Assert.Null(actual.Value);
        Assert.Contains("Sales order not found.", actual.Errors);
        Assert.Null(actual.Exception);
        Assert.Equal(FailureType.NotFound, actual.FailureType);
    }
}