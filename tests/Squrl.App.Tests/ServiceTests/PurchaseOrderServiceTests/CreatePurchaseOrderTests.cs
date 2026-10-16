using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.DependencyInjection;
using NodaTime;
using Squrl.App.Common;
using Squrl.App.Data;
using Squrl.App.Enums;
using Squrl.App.Features.PurchaseOrderDetails.DTOs;
using Squrl.App.Features.PurchaseOrders.DTOs;
using Squrl.App.Models;
using Squrl.App.Services.PurchaseOrder;

namespace Squrl.App.Tests.ServiceTests.PurchaseOrderServiceTests;

[Collection("IntegrationTests")]
public sealed class CreatePurchaseOrderTests : IntegrationTest
{
    public CreatePurchaseOrderTests(SqurlWebApplicationFactory factory) : base(factory)
    {
        
    }

    [Fact]
    public async Task CreatePurchaseOrder_ShouldCreatePurchaseOrder()
    {
        using IServiceScope scope = CreateScope();

        IPurchaseOrderService service = scope.ServiceProvider.GetRequiredService<IPurchaseOrderService>();
        
        DataContext dataContext = scope.ServiceProvider.GetRequiredService<DataContext>();
        
        // Arrange
        Supplier supplierSeed = new()
        {
            Id = Guid.NewGuid(),
            Name = "Test Supplier",
        };
        
        dataContext.Suppliers.Add(supplierSeed);
        
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
        await dataContext.SaveChangesAsync();
        
        CreatePurchaseOrderDto dto = new()
        {
            SupplierId = supplierSeed.Id,
            InvoiceNumber = "5000001",
            Status = PurchaseOrderStatus.Pending,
            Details = [
                new()
                {
                    PurchaseOrderId = Guid.Empty,
                    LineSequence = 1,
                    ItemId = itemSeed.Id,
                    UnitPrice = 600m,
                    Quantity = 5m
                },
                new()
                {
                    PurchaseOrderId = Guid.Empty,
                    LineSequence = 2,
                    ItemId = itemSeed.Id,
                    UnitPrice = 600.05m,
                    Quantity = 5m
                }
            ],
            DateOrdered = Instant.FromUtc(2020, 1, 1, 0, 0),
            DateRequired = Instant.FromUtc(2020, 2, 1, 0, 0)
        };
        
        // Act
        Result<GetPurchaseOrderDto> actual = await service.CreatePurchaseOrderAsync(dto);
        
        // Assert
        Assert.True(actual.IsSuccess);
        Assert.NotNull(actual.Value);
        Assert.Empty(actual.Errors);
        Assert.Null(actual.Exception);
        Assert.Null(actual.FailureType);

        Assert.Equal(dto.SupplierId, actual.Value.SupplierId);
        Assert.Equal(dto.InvoiceNumber, actual.Value.InvoiceNumber);
        Assert.Equal(dto.Status, actual.Value.Status);
        Assert.Equal(dto.DateOrdered, actual.Value.DateOrdered);
        Assert.Equal(dto.DateRequired, actual.Value.DateRequired);
        Assert.Equal(dto.DateShipped, actual.Value.DateShipped);
        
        PurchaseOrder? created = await dataContext.PurchaseOrders
            .AsNoTracking()
            .Include(purchaseOrder => purchaseOrder.Supplier)
            .SingleOrDefaultAsync(p => p.Id == actual.Value.Id);
        
        Assert.NotNull(created);

        Assert.Equal(actual.Value.Id, created.Id);
        Assert.Equal(dto.SupplierId, created.Supplier?.Id);
        Assert.Equal(dto.InvoiceNumber, created.InvoiceNumber);
        Assert.Equal(dto.Status, created.Status);
        Assert.Equal(dto.DateOrdered, created.DateOrdered);
        Assert.Equal(dto.DateRequired, created.DateRequired);
        Assert.Equal(dto.DateShipped, created.DateShipped);
        
        IReadOnlyList<PurchaseOrderDetail> createdPoDetails = await dataContext.PurchaseOrderDetails
            .AsNoTracking()
            .Include(p => p.PurchaseOrder)
            .Include(p => p.Item)
            .Where(p => p.PurchaseOrder.Id == actual.Value.Id)
            .ToListAsync();;
        
        Assert.NotNull(createdPoDetails);
        Assert.NotEmpty(createdPoDetails);
        
        foreach (PurchaseOrderDetail createdPoDetail in createdPoDetails)
        {
            CreatePoDetailDto? dtoPoDetail = dto.Details
                .SingleOrDefault(p => p.LineSequence == createdPoDetail.LineSequence);
            
            Assert.NotNull(dtoPoDetail);
            
            Assert.Equal(actual.Value.Id, createdPoDetail.PurchaseOrder.Id);
            Assert.Equal(dtoPoDetail.LineSequence, createdPoDetail.LineSequence);
            Assert.Equal(dtoPoDetail.ItemId, createdPoDetail.Item.Id);
            Assert.Equal(dtoPoDetail.UnitPrice, createdPoDetail.UnitPrice);
            Assert.Equal(dtoPoDetail.Quantity, createdPoDetail.Quantity);
        }
    }

    [Fact]
    public async Task CreatePurchaseOrder_ShouldFailWhenDtoIsInvalid()
    {
        using IServiceScope scope = CreateScope();

        IPurchaseOrderService service = scope.ServiceProvider.GetRequiredService<IPurchaseOrderService>();

        // Arrange
        CreatePurchaseOrderDto dto = new()
        {
            SupplierId = Guid.NewGuid(),
            InvoiceNumber = "5000001",
            Status = PurchaseOrderStatus.Pending,
            Details = [],
            DateOrdered = Instant.FromUtc(2020, 1, 1, 0, 0),
            DateRequired = Instant.FromUtc(2020, 2, 1, 0, 0)
        };
        
        // Act
        Result<GetPurchaseOrderDto> actual = await service.CreatePurchaseOrderAsync(dto);

        // Assert
        Assert.False(actual.IsSuccess);
        Assert.Null(actual.Value);
        Assert.Contains("Purchase Order Details is required.", actual.Errors);
        Assert.Null(actual.Exception);
        Assert.Equal(FailureType.Validation, actual.FailureType);
    }

    [Fact]
    public async Task CreatePurchaseOrder_ShouldNotPersistWhenValidationFails()
    {
        using IServiceScope scope = CreateScope();

        IPurchaseOrderService service = scope.ServiceProvider.GetRequiredService<IPurchaseOrderService>();

        DataContext dataContext = scope.ServiceProvider.GetRequiredService<DataContext>();
        
        // Arrange
        CreatePurchaseOrderDto dto = new()
        {
            SupplierId = Guid.NewGuid(),
            InvoiceNumber = "5000001",
            Status = PurchaseOrderStatus.Pending,
            Details = [],
            DateOrdered = Instant.FromUtc(2020, 1, 1, 0, 0),
            DateRequired = Instant.FromUtc(2020, 2, 1, 0, 0)
        };
        
        // Act
        Result<GetPurchaseOrderDto> actual = await service.CreatePurchaseOrderAsync(dto);

        // Assert
        Assert.False(actual.IsSuccess);
        Assert.Null(actual.Value);
        Assert.NotEmpty(actual.Errors);
        Assert.Null(actual.Exception);
        Assert.Equal(FailureType.Validation, actual.FailureType);
        
        bool exists = await dataContext.PurchaseOrders
            .AnyAsync(p => p.InvoiceNumber == dto.InvoiceNumber);

        Assert.False(exists);
    }
}