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
public sealed class CreateSalesOrderTests : IntegrationTest
{
    public CreateSalesOrderTests(SqurlWebApplicationFactory factory) : base(factory)
    {
        
    }
    
    [Fact]
    public async Task CreateSalesOrder_ShouldCreateSalesOrder()
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
        await dataContext.SaveChangesAsync();
        
        CreateSalesOrderDto dto = new()
        {
            Customer = "Test Customer",
            InvoiceNumber = "5000001",
            Details = [
                new()
                {
                    SalesOrderId = Guid.Empty,
                    LineSequence = 1,
                    ItemId = itemSeed.Id,
                    UnitPrice = 600m,
                    Quantity = 5m
                },
                new()
                {
                    SalesOrderId = Guid.Empty,
                    LineSequence = 2,
                    ItemId = itemSeed.Id,
                    UnitPrice = 600.05m,
                    Quantity = 5m
                }
            ],
            DateOrdered = Instant.FromUtc(2020, 1, 1, 0, 0)
        };
        
        // Act
        Result<GetSalesOrderDto> actual = await service.CreateSalesOrderAsync(dto);

        // Assert
        Assert.True(actual.IsSuccess);
        Assert.NotNull(actual.Value);
        Assert.Empty(actual.Errors);
        Assert.Null(actual.Exception);
        Assert.Null(actual.FailureType);

        Assert.Equal(dto.Customer, actual.Value.Customer);
        Assert.Equal(dto.InvoiceNumber, actual.Value.InvoiceNumber);
        Assert.Equal(dto.DateOrdered, actual.Value.DateOrdered);

        SalesOrder? created = await dataContext.SalesOrders
            .AsNoTracking()
            .SingleOrDefaultAsync(s => s.Id == actual.Value.Id);

        Assert.NotNull(created);

        Assert.Equal(actual.Value.Id, created.Id);
        Assert.Equal(dto.Customer, created.Customer);
        Assert.Equal(dto.InvoiceNumber, created.InvoiceNumber);
        Assert.Equal(dto.DateOrdered, created.DateOrdered);

        IReadOnlyList<SalesOrderDetail> createdSoDetails = await dataContext.SalesOrderDetails
            .AsNoTracking()
            .Include(s => s.SalesOrder)
            .Include(s => s.Item)
            .Where(s => s.SalesOrder.Id == actual.Value.Id)
            .ToListAsync();;
        
        Assert.NotNull(createdSoDetails);
        Assert.NotEmpty(createdSoDetails);

        foreach (SalesOrderDetail createdSoDetail in createdSoDetails)
        {
            CreateSoDetailDto? dtoSoDetail = dto.Details
                .SingleOrDefault(s => s.LineSequence == createdSoDetail.LineSequence);
            
            Assert.NotNull(dtoSoDetail);
            
            Assert.Equal(actual.Value.Id, createdSoDetail.SalesOrder.Id);
            Assert.Equal(dtoSoDetail.LineSequence, createdSoDetail.LineSequence);
            Assert.Equal(dtoSoDetail.ItemId, createdSoDetail.Item.Id);
            Assert.Equal(dtoSoDetail.UnitPrice, createdSoDetail.UnitPrice);
            Assert.Equal(dtoSoDetail.Quantity, createdSoDetail.Quantity);
        }
    }

    [Fact]
    public async Task CreateSalesOrder_ShouldFailWhenDtoIsInvalid()
    {
        using IServiceScope scope = CreateScope();

        ISalesOrderService service = scope.ServiceProvider.GetRequiredService<ISalesOrderService>();

        // Arrange
        CreateSalesOrderDto dto = new()
        {
            Customer = "Test Customer",
            InvoiceNumber = "5000001",
            Details = [],
            DateOrdered = Instant.FromUtc(2020, 1, 1, 0, 0)
        };
        
        // Act
        Result<GetSalesOrderDto> actual = await service.CreateSalesOrderAsync(dto);

        // Assert
        Assert.False(actual.IsSuccess);
        Assert.Null(actual.Value);
        Assert.Contains("Sales Order Details is required.", actual.Errors);
        Assert.Null(actual.Exception);
        Assert.Equal(FailureType.Validation, actual.FailureType);
    }

    [Fact]
    public async Task CreateSalesOrder_ShouldNotPersistWhenValidationFails()
    {
        using IServiceScope scope = CreateScope();

        ISalesOrderService service = scope.ServiceProvider.GetRequiredService<ISalesOrderService>();

        DataContext dataContext = scope.ServiceProvider.GetRequiredService<DataContext>();
        
        // Arrange
        CreateSalesOrderDto dto = new()
        {
            Customer = "Test Customer",
            InvoiceNumber = "5000001",
            Details = [],
            DateOrdered = Instant.FromUtc(2020, 1, 1, 0, 0)
        };
        
        // Act
        Result<GetSalesOrderDto> actual = await service.CreateSalesOrderAsync(dto);

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

    // TODO
    // [Theory]
    // [InlineData(10.01)]
    // [InlineData(15)]
    // public async Task CreateSalesOrder_ShouldRemoveStocksFromItems(decimal quantity)
    // {
    //     
    // }
    
    [Theory]
    [InlineData(10.01)]
    [InlineData(15)]
    public async Task CreateSalesOrder_ShouldFailWhenNotEnoughStocksToRemove(decimal quantity)
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
        await dataContext.SaveChangesAsync();
        
        CreateSalesOrderDto dto = new()
        {
            Customer = "Test Customer",
            InvoiceNumber = "5000001",
            Details = [
                new CreateSoDetailDto
                {
                    SalesOrderId = Guid.Empty,
                    LineSequence = 1,
                    ItemId = itemSeed.Id,
                    UnitPrice = 600m,
                    Quantity = quantity
                }
            ],
            DateOrdered = Instant.FromUtc(2020, 1, 1, 0, 0)
        };
        
        // Act
        Result<GetSalesOrderDto> actual = await service.CreateSalesOrderAsync(dto);

        // Assert
        Assert.False(actual.IsSuccess);
        Assert.Null(actual.Value);
        // TODO: Change Error Message
        Assert.Contains("Failed to return sales order.", actual.Errors);
        Assert.Null(actual.Exception);
        Assert.Equal(FailureType.BusinessLogic, actual.FailureType);
    }
}