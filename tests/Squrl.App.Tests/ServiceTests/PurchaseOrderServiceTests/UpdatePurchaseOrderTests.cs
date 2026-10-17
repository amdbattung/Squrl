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
public sealed class UpdatePurchaseOrderTests : IntegrationTest
{
    public UpdatePurchaseOrderTests(SqurlWebApplicationFactory factory) : base(factory)
    {
        
    }

    [Fact]
    public async Task UpdatePurchaseOrder_ShouldUpdateAndPersistPurchaseOrder()
    {
        using IServiceScope scope = CreateScope();

        IPurchaseOrderService service = scope.ServiceProvider.GetRequiredService<IPurchaseOrderService>();
        
        DataContext dataContext = scope.ServiceProvider.GetRequiredService<DataContext>();
        
        // Arrange
        PurchaseOrder seed = new()
        {
            Id = Guid.NewGuid(),
            Supplier = new()
            {
                Id = Guid.NewGuid(),
                Name = "Test Supplier 1"
            },
            InvoiceNumber = "5000001",
            Status = PurchaseOrderStatus.Received,
            DateOrdered = Instant.FromUtc(2020, 1, 1, 0, 0),
            DateRequired = Instant.FromUtc(2020, 2, 1, 0, 0),
            DateShipped = Instant.FromUtc(2020, 2, 1, 0, 0)
        };

        Supplier supplierSeed = new()
        {
            Id = Guid.NewGuid(),
            Name = "Test Supplier 2"
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
        
        List<PurchaseOrderDetail> poDetailSeed = [
            new()
            {
                Id = Guid.NewGuid(),
                PurchaseOrder = seed,
                LineSequence = 1,
                Item = new()
                {
                    Id = Guid.NewGuid(),
                    Name = "Test Item A",
                    Uom = new()
                    {
                        Id = Guid.NewGuid(),
                        Name = "Test UOM A",
                        Code = "uA"
                    },
                    Quantity = 10m,
                    Locations = ["Shelf A", "Shelf B"]
                },
                UnitPrice = 600m,
                Quantity = 5m
            },
            new()
            {
                Id = Guid.NewGuid(),
                PurchaseOrder = seed,
                LineSequence = 2,
                Item = new()
                {
                    Id = Guid.NewGuid(),
                    Name = "Test Item B",
                    Uom = new()
                    {
                        Id = Guid.NewGuid(),
                        Name = "Test UOM B",
                        Code = "uB"
                    },
                    Quantity = 10m,
                    Locations = ["Shelf A", "Shelf B"]
                },
                UnitPrice = 600m,
                Quantity = 5m
            }
        ];
        
        dataContext.PurchaseOrders.Add(seed);
        dataContext.PurchaseOrderDetails.AddRange(poDetailSeed);
        await dataContext.SaveChangesAsync();
        
        UpdatePurchaseOrderDto dto = new()
        {
            SupplierId = supplierSeed.Id,
            InvoiceNumber = "6000001",
            Status =  PurchaseOrderStatus.Returned,
            Details = [
                new()
                {
                    PurchaseOrderId = Guid.Empty,
                    LineSequence = 1,
                    ItemId = itemSeed.Id,
                    UnitPrice = 700m,
                    Quantity = 6m
                },
                new()
                {
                    PurchaseOrderId = Guid.Empty,
                    LineSequence = 2,
                    ItemId = itemSeed.Id,
                    UnitPrice = 700m,
                    Quantity = 6m
                }
            ],
            DateOrdered = Instant.FromUtc(2020, 2, 1, 0, 0),
            DateRequired = Instant.FromUtc(2020, 3, 1, 0, 0),
            DateShipped = Instant.FromUtc(2020, 3, 1, 0, 0)
        };
        
        // Act
        Result<GetPurchaseOrderDto> actual = await service.UpdatePurchaseOrderAsync(seed.Id, dto);
        
        // Assert
        Assert.True(actual.IsSuccess);
        Assert.NotNull(actual.Value);
        Assert.Empty(actual.Errors);
        Assert.Null(actual.Exception);
        Assert.Null(actual.FailureType);
        
        Assert.Equal(seed.Id, actual.Value.Id);
        Assert.Equal(dto.SupplierId, actual.Value.SupplierId);
        Assert.Equal(dto.InvoiceNumber, actual.Value.InvoiceNumber);
        Assert.Equal(dto.Status, actual.Value.Status);
        Assert.Equal(dto.DateOrdered, actual.Value.DateOrdered);
        Assert.Equal(dto.DateRequired, actual.Value.DateRequired);
        Assert.Equal(dto.DateShipped, actual.Value.DateShipped);
        
        PurchaseOrder? updated = await dataContext.PurchaseOrders
            .AsNoTracking()
            .Include(purchaseOrder => purchaseOrder.Supplier)
            .SingleOrDefaultAsync(p => p.Id == seed.Id);
        
        Assert.NotNull(updated);
        
        Assert.Equal(seed.Id, updated.Id);
        Assert.Equal(dto.SupplierId, updated.Supplier?.Id);
        Assert.Equal(dto.InvoiceNumber, updated.InvoiceNumber);
        Assert.Equal(dto.Status, updated.Status);
        Assert.Equal(dto.DateOrdered, updated.DateOrdered);
        Assert.Equal(dto.DateRequired, updated.DateRequired);
        Assert.Equal(dto.DateShipped, updated.DateShipped);
        
        IReadOnlyList<PurchaseOrderDetail> updatedPoDetails = await dataContext.PurchaseOrderDetails
            .AsNoTracking()
            .Include(p => p.PurchaseOrder)
            .Include(p => p.Item)
            .Where(s => s.PurchaseOrder.Id == seed.Id)
            .ToListAsync();
        
        Assert.NotNull(updatedPoDetails);
        Assert.NotEmpty(updatedPoDetails);
        
        foreach (PurchaseOrderDetail updatedPoDetail in updatedPoDetails)
        {
            UpdatePoDetailDto? dtoPoDetail = dto.Details
                .SingleOrDefault(p => p.LineSequence == updatedPoDetail.LineSequence);
            
            Assert.NotNull(dtoPoDetail);
            
            Assert.Equal(actual.Value.Id, updatedPoDetail.PurchaseOrder.Id);
            Assert.Equal(dtoPoDetail.LineSequence, updatedPoDetail.LineSequence);
            Assert.Equal(dtoPoDetail.ItemId, updatedPoDetail.Item.Id);
            Assert.Equal(dtoPoDetail.UnitPrice, updatedPoDetail.UnitPrice);
            Assert.Equal(dtoPoDetail.Quantity, updatedPoDetail.Quantity);
        }
    }

    [Fact]
    public async Task UpdatePurchaseOrder_ShouldFailIfIdIsEmpty()
    {
        using IServiceScope scope = CreateScope();

        IPurchaseOrderService service = scope.ServiceProvider.GetRequiredService<IPurchaseOrderService>();
        
        // Arrange
        UpdatePurchaseOrderDto dto = new()
        {
            Status = PurchaseOrderStatus.Pending,
            Details = [
                new()
                {
                    PurchaseOrderId = Guid.Empty,
                    LineSequence = 1,
                    ItemId = Guid.NewGuid(),
                    UnitPrice = 600m,
                    Quantity = 5m
                }
            ],
            DateOrdered = Instant.FromUtc(2020, 1, 1, 0, 0)
        };
        
        // Act
        Result<GetPurchaseOrderDto> actual = await service.UpdatePurchaseOrderAsync(Guid.Empty, dto);
        
        // Assert
        Assert.False(actual.IsSuccess);
        Assert.Null(actual.Value);
        Assert.Contains("Null or invalid ID.", actual.Errors);
        Assert.Null(actual.Exception);
        Assert.Equal(FailureType.Validation, actual.FailureType);
    }

    [Fact]
    public async Task UpdatePurchaseOrder_ShouldFailIfPurchaseOrderDoesNotExist()
    {
        using IServiceScope scope = CreateScope();

        IPurchaseOrderService service = scope.ServiceProvider.GetRequiredService<IPurchaseOrderService>();
        
        // Arrange
        UpdatePurchaseOrderDto dto = new()
        {
            Status = PurchaseOrderStatus.Pending,
            Details = [
                new()
                {
                    PurchaseOrderId = Guid.Empty,
                    LineSequence = 1,
                    ItemId = Guid.NewGuid(),
                    UnitPrice = 600m,
                    Quantity = 5m
                }
            ],
            DateOrdered = Instant.FromUtc(2020, 1, 1, 0, 0)
        };
        
        // Act
        Result<GetPurchaseOrderDto> actual = await service.UpdatePurchaseOrderAsync(Guid.NewGuid(), dto);
        
        // Assert
        Assert.False(actual.IsSuccess);
        Assert.Null(actual.Value);
        // TODO: Change Error Message
        Assert.Contains("Failed to update purchase order.", actual.Errors);
        Assert.Null(actual.Exception);
        Assert.Equal(FailureType.NotFound, actual.FailureType);
    }

    [Fact]
    public async Task UpdatePurchaseOrder_ShouldFailWhenDtoIsInvalid()
    {
        using IServiceScope scope = CreateScope();

        IPurchaseOrderService service = scope.ServiceProvider.GetRequiredService<IPurchaseOrderService>();
        
        // Arrange
        UpdatePurchaseOrderDto dto = new()
        {
            Status = PurchaseOrderStatus.Pending,
            Details = [],
            DateOrdered = Instant.FromUtc(2020, 1, 1, 0, 0)
        };
        
        // Act
        Result<GetPurchaseOrderDto> actual = await service.UpdatePurchaseOrderAsync(Guid.NewGuid(), dto);
        
        // Assert
        Assert.False(actual.IsSuccess);
        Assert.Null(actual.Value);
        Assert.Contains("Purchase Order Details is required.", actual.Errors);
        Assert.Null(actual.Exception);
        Assert.Equal(FailureType.Validation, actual.FailureType);
    }

    [Fact]
    public async Task UpdatePurchaseOrder_ShouldNotPersistWhenValidationFails()
    {
        using IServiceScope scope = CreateScope();

        IPurchaseOrderService service = scope.ServiceProvider.GetRequiredService<IPurchaseOrderService>();
        
        DataContext dataContext = scope.ServiceProvider.GetRequiredService<DataContext>();
        
        // Arrange
        UpdatePurchaseOrderDto dto = new()
        {
            Status = PurchaseOrderStatus.Pending,
            Details = [],
            DateOrdered = Instant.FromUtc(2020, 1, 1, 0, 0)
        };
        
        // Act
        Result<GetPurchaseOrderDto> actual = await service.UpdatePurchaseOrderAsync(Guid.NewGuid(), dto);
        
        // Assert
        Assert.False(actual.IsSuccess);
        Assert.Null(actual.Value);
        Assert.NotEmpty(actual.Errors);
        Assert.Null(actual.Exception);
        Assert.Equal(FailureType.Validation, actual.FailureType);
        
        bool exists = await dataContext.SalesOrders
            .AnyAsync(s => s.InvoiceNumber == dto.InvoiceNumber);

        Assert.False(exists);
    }
}