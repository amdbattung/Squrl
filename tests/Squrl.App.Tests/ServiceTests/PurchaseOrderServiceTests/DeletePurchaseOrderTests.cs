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
public sealed class DeletePurchaseOrderTests : IntegrationTest
{
    public DeletePurchaseOrderTests(SqurlWebApplicationFactory factory) : base(factory)
    {
        
    }

    [Fact]
    public async Task DeletePurchaseOrder_ShouldDeleteAndReturnPurchaseOrder()
    {
        using IServiceScope scope = CreateScope();

        IPurchaseOrderService service = scope.ServiceProvider.GetRequiredService<IPurchaseOrderService>();
        
        DataContext dataContext = scope.ServiceProvider.GetRequiredService<DataContext>();
        
        // Arrange
        PurchaseOrder seed = new()
        {
            Id = Guid.NewGuid(),
            Status = PurchaseOrderStatus.Pending,
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
        Result<GetPurchaseOrderDto> actual = await service.DeletePurchaseOrderAsync(seed.Id);
        
        // Assert
        Assert.True(actual.IsSuccess);
        Assert.NotNull(actual.Value);
        Assert.Empty(actual.Errors);
        Assert.Null(actual.Exception);
        Assert.Null(actual.FailureType);
        
        Assert.Equal(seed.Id, actual.Value.Id);
        Assert.Equal(seed.Supplier?.Id, actual.Value.SupplierId);
        Assert.Equal(seed.InvoiceNumber, actual.Value.InvoiceNumber);
        Assert.Equal(seed.Status, actual.Value.Status);
        Assert.Equal(seed.DateOrdered, actual.Value.DateOrdered);
        Assert.Equal(seed.DateRequired, actual.Value.DateRequired);
        Assert.Equal(seed.DateShipped, actual.Value.DateShipped);
        
        bool exists = await dataContext.PurchaseOrders
            .AnyAsync(p => p.Id == seed.Id);
        
        Assert.False(exists);
    }

    [Fact]
    public async Task DeletePurchaseOrder_ShouldFailIfIdIsEmpty()
    {
        using IServiceScope scope = CreateScope();

        IPurchaseOrderService service = scope.ServiceProvider.GetRequiredService<IPurchaseOrderService>();
        
        // Arrange
        
        // Act
        Result<GetPurchaseOrderDto> actual = await service.DeletePurchaseOrderAsync(Guid.Empty);
        
        // Assert
        Assert.False(actual.IsSuccess);
        Assert.Null(actual.Value);
        Assert.Contains("Null or invalid ID.", actual.Errors);
        Assert.Null(actual.Exception);
        Assert.Equal(FailureType.Validation, actual.FailureType);
    }

    [Fact]
    public async Task DeletePurchaseOrder_ShouldFailIfPurchaseOrderDoesNotExist()
    {
        using IServiceScope scope = CreateScope();

        IPurchaseOrderService service = scope.ServiceProvider.GetRequiredService<IPurchaseOrderService>();
        
        // Arrange
        
        // Act
        Result<GetPurchaseOrderDto> actual = await service.DeletePurchaseOrderAsync(Guid.NewGuid());
        
        // Assert
        Assert.False(actual.IsSuccess);
        Assert.Null(actual.Value);
        Assert.Contains("Purchase order not found.", actual.Errors);
        Assert.Null(actual.Exception);
        Assert.Equal(FailureType.NotFound, actual.FailureType);
    }
}