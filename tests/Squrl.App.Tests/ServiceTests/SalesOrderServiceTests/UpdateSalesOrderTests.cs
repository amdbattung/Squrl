using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.DependencyInjection;
using NodaTime;
using Squrl.App.Common;
using Squrl.App.Data;
using Squrl.App.Enums;
using Squrl.App.Features.SalesOrderDetails.DTOs;
using Squrl.App.Features.SalesOrders.DTOs;
using Squrl.App.Models;
using Squrl.App.Services.SalesOrder;

namespace Squrl.App.Tests.ServiceTests.SalesOrderServiceTests;

[Collection("IntegrationTests")]
public class UpdateSalesOrderTests : IntegrationTest
{
    public UpdateSalesOrderTests(SqurlWebApplicationFactory factory) : base(factory)
    {
        
    }

    [Fact]
    public async Task UpdateSalesOrder_ShouldUpdateAndPersistSalesOrder()
    {
        using IServiceScope scope = CreateScope();

        ISalesOrderService service = scope.ServiceProvider.GetRequiredService<ISalesOrderService>();
        
        DataContext dataContext = scope.ServiceProvider.GetRequiredService<DataContext>();
        
        // Arrange
        SalesOrder seed = new()
        {
            Id = Guid.NewGuid(),
            Customer = "Test Customer",
            InvoiceNumber = "5000001",
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

        List<SalesOrderDetail> soDetailSeed = [
            new()
            {
                Id = Guid.NewGuid(),
                SalesOrder = seed,
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
                SalesOrder = seed,
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

        dataContext.SalesOrders.Add(seed);
        dataContext.SalesOrderDetails.AddRange(soDetailSeed);
        await dataContext.SaveChangesAsync();
        
        UpdateSalesOrderDto dto = new()
        {
            Customer = "Updated Test Customer",
            InvoiceNumber = "6000001",
            Details = [
                new()
                {
                    SalesOrderId = Guid.Empty,
                    LineSequence = 1,
                    ItemId = itemSeed.Id,
                    UnitPrice = 700m,
                    Quantity = 6m
                },
                new()
                {
                    SalesOrderId = Guid.Empty,
                    LineSequence = 2,
                    ItemId = itemSeed.Id,
                    UnitPrice = 700m,
                    Quantity = 6m
                }
            ],
            DateOrdered = Instant.FromUtc(2020, 2, 1, 0, 0)
        };
        
        // Act
        Result<GetSalesOrderDto> actual = await service.UpdateSalesOrderAsync(seed.Id, dto);
        
        // Assert
        Assert.True(actual.IsSuccess);
        Assert.NotNull(actual.Value);
        Assert.Empty(actual.Errors);
        Assert.Null(actual.Exception);
        Assert.Null(actual.FailureType);
        
        Assert.Equal(seed.Id, actual.Value.Id);
        Assert.Equal(dto.Customer, actual.Value.Customer);
        Assert.Equal(dto.InvoiceNumber, actual.Value.InvoiceNumber);
        Assert.Equal(dto.DateOrdered, actual.Value.DateOrdered);
        
        SalesOrder? updated = await dataContext.SalesOrders
            .AsNoTracking()
            .SingleOrDefaultAsync(s => s.Id == seed.Id);
        
        Assert.NotNull(updated);
        
        Assert.Equal(seed.Id, updated.Id);
        Assert.Equal(dto.Customer, updated.Customer);
        Assert.Equal(dto.InvoiceNumber, updated.InvoiceNumber);
        Assert.Equal(dto.DateOrdered, updated.DateOrdered);
        
        IReadOnlyList<SalesOrderDetail> updatedSoDetails = await dataContext.SalesOrderDetails
            .AsNoTracking()
            .Include(s => s.SalesOrder)
            .Include(s => s.Item)
            .Where(s => s.SalesOrder.Id == seed.Id)
            .ToListAsync();
        
        Assert.NotNull(updatedSoDetails);
        Assert.NotEmpty(updatedSoDetails);
        
        foreach (SalesOrderDetail updatedSoDetail in updatedSoDetails)
        {
            UpdateSoDetailDto? dtoSoDetail = dto.Details
                .SingleOrDefault(s => s.LineSequence == updatedSoDetail.LineSequence);
            
            Assert.NotNull(dtoSoDetail);
            
            Assert.Equal(actual.Value.Id, updatedSoDetail.SalesOrder.Id);
            Assert.Equal(dtoSoDetail.LineSequence, updatedSoDetail.LineSequence);
            Assert.Equal(dtoSoDetail.ItemId, updatedSoDetail.Item.Id);
            Assert.Equal(dtoSoDetail.UnitPrice, updatedSoDetail.UnitPrice);
            Assert.Equal(dtoSoDetail.Quantity, updatedSoDetail.Quantity);
        }
    }

    [Fact]
    public async Task UpdateSalesOrder_ShouldFailIfIdIsEmpty()
    {
        using IServiceScope scope = CreateScope();

        ISalesOrderService service = scope.ServiceProvider.GetRequiredService<ISalesOrderService>();
        
        // Arrange
        UpdateSalesOrderDto dto = new()
        {
            Details = [
                new()
                {
                    SalesOrderId = Guid.Empty,
                    LineSequence = 1,
                    ItemId = Guid.NewGuid(),
                    UnitPrice = 600m,
                    Quantity = 5m
                }
            ],
            DateOrdered = Instant.FromUtc(2020, 1, 1, 0, 0)
        };
        
        // Act
        Result<GetSalesOrderDto> actual = await service.UpdateSalesOrderAsync(Guid.Empty, dto);
        
        // Assert
        Assert.False(actual.IsSuccess);
        Assert.Null(actual.Value);
        Assert.Contains("Null or invalid ID.", actual.Errors);
        Assert.Null(actual.Exception);
        Assert.Equal(FailureType.Validation, actual.FailureType);
    }

    [Fact]
    public async Task UpdateSalesOrder_ShouldFailIfSalesOrderDoesNotExist()
    {
        using IServiceScope scope = CreateScope();

        ISalesOrderService service = scope.ServiceProvider.GetRequiredService<ISalesOrderService>();
        
        // Arrange
        UpdateSalesOrderDto dto = new()
        {
            Details = [
                new()
                {
                    SalesOrderId = Guid.Empty,
                    LineSequence = 1,
                    ItemId = Guid.NewGuid(),
                    UnitPrice = 600m,
                    Quantity = 5m
                }
            ],
            DateOrdered = Instant.FromUtc(2020, 1, 1, 0, 0)
        };
        
        // Act
        Result<GetSalesOrderDto> actual = await service.UpdateSalesOrderAsync(Guid.NewGuid(), dto);
        
        // Assert
        Assert.False(actual.IsSuccess);
        Assert.Null(actual.Value);
        // TODO: Change Error Message
        Assert.Contains("Failed to update sales order.", actual.Errors);
        Assert.Null(actual.Exception);
        Assert.Equal(FailureType.NotFound, actual.FailureType);
    }

    [Fact]
    public async Task UpdateSalesOrder_ShouldFailWhenDtoIsInvalid()
    {
        using IServiceScope scope = CreateScope();

        ISalesOrderService service = scope.ServiceProvider.GetRequiredService<ISalesOrderService>();
        
        // Arrange
        UpdateSalesOrderDto dto = new()
        {
            Details = [],
            DateOrdered = Instant.FromUtc(2020, 1, 1, 0, 0)
        };
        
        // Act
        Result<GetSalesOrderDto> actual = await service.UpdateSalesOrderAsync(Guid.NewGuid(), dto);
        
        // Assert
        Assert.False(actual.IsSuccess);
        Assert.Null(actual.Value);
        Assert.Contains("Sales Order Details is required.", actual.Errors);
        Assert.Null(actual.Exception);
        Assert.Equal(FailureType.Validation, actual.FailureType);
    }

    [Fact]
    public async Task UpdateSalesOrder_ShouldNotPersistWhenValidationFails()
    {
        using IServiceScope scope = CreateScope();

        ISalesOrderService service = scope.ServiceProvider.GetRequiredService<ISalesOrderService>();
        
        DataContext dataContext = scope.ServiceProvider.GetRequiredService<DataContext>();
        
        // Arrange
        UpdateSalesOrderDto dto = new()
        {
            InvoiceNumber = "5000001",
            Details = [],
            DateOrdered = Instant.FromUtc(2020, 1, 1, 0, 0)
        };
        
        // Act
        Result<GetSalesOrderDto> actual = await service.UpdateSalesOrderAsync(Guid.NewGuid(), dto);
        
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