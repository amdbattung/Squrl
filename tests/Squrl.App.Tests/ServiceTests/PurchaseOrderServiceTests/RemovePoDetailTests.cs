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
public sealed class RemovePoDetailTests : IntegrationTest
{
    public RemovePoDetailTests(SqurlWebApplicationFactory factory) : base(factory)
    {
        
    }

    [Fact]
    public async Task DeletePoDetail_ShouldDeleteAndReturnPoDetail()
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
        
        PurchaseOrderDetail seed = new()
        {
            Id = Guid.NewGuid(),
            PurchaseOrder = purchaseOrderSeed,
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

        dataContext.PurchaseOrders.Add(purchaseOrderSeed);
        dataContext.PurchaseOrderDetails.Add(seed);
        await dataContext.SaveChangesAsync();
        
        // Act
        Result<GetPoDetailDto> actual = await service.RemovePoDetailAsync(seed.Id);
        
        // Assert
        Assert.True(actual.IsSuccess);
        Assert.NotNull(actual.Value);
        Assert.Empty(actual.Errors);
        Assert.Null(actual.Exception);
        Assert.Null(actual.FailureType);
        
        Assert.Equal(seed.Id, actual.Value.Id);
        Assert.Equal(seed.PurchaseOrder.Id, actual.Value.PurchaseOrderId);
        Assert.Equal(seed.LineSequence, actual.Value.LineSequence);
        Assert.Equal(seed.Item.Id, actual.Value.ItemId);
        Assert.Equal(seed.UnitPrice, actual.Value.UnitPrice);
        Assert.Equal(seed.Quantity, actual.Value.Quantity);
        
        bool exists = await dataContext.SalesOrderDetails
            .AnyAsync(s => s.Id == seed.Id);
        
        Assert.False(exists);
    }

    [Fact]
    public async Task DeletePoDetail_ShouldFailIfIdIsEmpty()
    {
        using IServiceScope scope = CreateScope();

        IPurchaseOrderService service = scope.ServiceProvider.GetRequiredService<IPurchaseOrderService>();
        
        // Arrange
        
        // Act
        Result<GetPoDetailDto> actual = await service.RemovePoDetailAsync(Guid.Empty);
        
        // Assert
        Assert.False(actual.IsSuccess);
        Assert.Null(actual.Value);
        Assert.Contains("Null or invalid ID.", actual.Errors);
        Assert.Null(actual.Exception);
        Assert.Equal(FailureType.Validation, actual.FailureType);
    }

    [Fact]
    public async Task DeletePoDetail_ShouldFailIfPoDetailDoesNotExist()
    {
        using IServiceScope scope = CreateScope();

        IPurchaseOrderService service = scope.ServiceProvider.GetRequiredService<IPurchaseOrderService>();
        
        // Arrange
        
        // Act
        Result<GetPoDetailDto> actual = await service.RemovePoDetailAsync(Guid.NewGuid());
        
        // Assert
        Assert.False(actual.IsSuccess);
        Assert.Null(actual.Value);
        Assert.Contains("PO detail not found.", actual.Errors);
        Assert.Null(actual.Exception);
        Assert.Equal(FailureType.NotFound, actual.FailureType);
    }
}