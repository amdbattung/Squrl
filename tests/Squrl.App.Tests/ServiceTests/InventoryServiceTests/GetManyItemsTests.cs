using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.DependencyInjection;
using NodaTime;
using Squrl.App.Common;
using Squrl.App.Data;
using Squrl.App.Enums;
using Squrl.App.Features.Items.DTOs;
using Squrl.App.Models;
using Squrl.App.Services.Inventory;

namespace Squrl.App.Tests.ServiceTests.InventoryServiceTests;

[Collection("IntegrationTests")]
public sealed class GetManyItemsTests : IntegrationTest
{
    public GetManyItemsTests(SqurlWebApplicationFactory factory) : base(factory)
    {
        
    }

    [Fact]
    public async Task GetManyItems_ShouldReturnItems()
    {
        using IServiceScope scope = CreateScope();

        IInventoryService service = scope.ServiceProvider.GetRequiredService<IInventoryService>();
        
        DataContext dataContext = scope.ServiceProvider.GetRequiredService<DataContext>();
        
        // Arrange
        UnitOfMeasure uomSeed = new()
        {
            Id = Guid.NewGuid(),
            Name = "Test UOM",
            Code = "u"
        };

        dataContext.UnitOfMeasures.Add(uomSeed);

        List<Item> seed = Enumerable.Range(1, 15)
            .Select(i => new Item
            {
                Id = Guid.NewGuid(),
                Name = $"Test Item {i}",
                Uom = uomSeed,
                Quantity = 10m,
                Locations = ["Shelf A", "Shelf B"]
            })
            .ToList();

        dataContext.Items.AddRange(seed);
        await dataContext.SaveChangesAsync();
        
        // Act
        Result<GetManyItemsDto> actual = await service.GetManyItemsAsync();
        
        // Assert
        Assert.True(actual.IsSuccess);
        Assert.NotNull(actual.Value);
        Assert.NotNull(actual.Value.Items);
        Assert.NotEmpty(actual.Value.Items);
        Assert.Empty(actual.Errors);
        Assert.Null(actual.Exception);
        Assert.Null(actual.FailureType);
        
        Assert.Equal(1, actual.Value.PageNumber);
        Assert.Equal(10, actual.Value.PageSize);
        Assert.Equal(15, actual.Value.TotalCount);
        Assert.Equal(10, actual.Value.Items.Count);
    }

    [Fact]
    public async Task GetManyItems_ShouldReturnEmptyListIfItemsDoNotExist()
    {
        using IServiceScope scope = CreateScope();

        IInventoryService service = scope.ServiceProvider.GetRequiredService<IInventoryService>();
        
        // Arrange
        
        // Act
        Result<GetManyItemsDto> actual = await service.GetManyItemsAsync();
        
        // Assert
        Assert.True(actual.IsSuccess);
        Assert.NotNull(actual.Value);
        Assert.NotNull(actual.Value.Items);
        Assert.Empty(actual.Value.Items);
        Assert.Empty(actual.Errors);
        Assert.Null(actual.Exception);
        Assert.Null(actual.FailureType);
        
        Assert.Equal(1, actual.Value.PageNumber);
        Assert.Equal(10, actual.Value.PageSize);
        Assert.Equal(0, actual.Value.TotalCount);
    }

    [Fact]
    public async Task GetManyItems_ShouldReturnPagedItems()
    {
        using IServiceScope scope = CreateScope();

        IInventoryService service = scope.ServiceProvider.GetRequiredService<IInventoryService>();
        
        DataContext dataContext = scope.ServiceProvider.GetRequiredService<DataContext>();
        
        // Arrange
        UnitOfMeasure uomSeed = new()
        {
            Id = Guid.NewGuid(),
            Name = "Test UOM",
            Code = "u"
        };

        dataContext.UnitOfMeasures.Add(uomSeed);
        
        List<Item> seed = Enumerable.Range(1, 15)
            .Select(i => new Item
            {
                Id = Guid.NewGuid(),
                Name = $"Test Item {i}",
                Uom = uomSeed,
                Quantity = 10m,
                Locations = ["Shelf A", "Shelf B"]
            })
            .ToList();

        dataContext.Items.AddRange(seed);
        await dataContext.SaveChangesAsync();
        
        // Act
        Result<GetManyItemsDto> actual = await service.GetManyItemsAsync(pageNumber: 3, pageSize: 6);
        
        // Assert
        Assert.True(actual.IsSuccess);
        Assert.NotNull(actual.Value);
        Assert.NotNull(actual.Value.Items);
        Assert.NotEmpty(actual.Value.Items);
        Assert.Empty(actual.Errors);
        Assert.Null(actual.Exception);
        Assert.Null(actual.FailureType);
        
        Assert.Equal(3, actual.Value.PageNumber);
        Assert.Equal(6, actual.Value.PageSize);
        Assert.Equal(15, actual.Value.TotalCount);
        Assert.Equal(3, actual.Value.Items.Count);
    }

    [Fact]
    public async Task GetManyItems_ShouldReturnQueriedItems()
    {
        using IServiceScope scope = CreateScope();

        IInventoryService service = scope.ServiceProvider.GetRequiredService<IInventoryService>();
        
        DataContext dataContext = scope.ServiceProvider.GetRequiredService<DataContext>();
        
        // Arrange
        UnitOfMeasure uomSeed = new()
        {
            Id = Guid.NewGuid(),
            Name = "Test UOM",
            Code = "u"
        };

        dataContext.UnitOfMeasures.Add(uomSeed);
        
        List<Item> seed = Enumerable.Range(1, 15)
            .Select(i => new Item
            {
                Id = Guid.NewGuid(),
                Name = $"Test Item {i}",
                Uom = uomSeed,
                Quantity = 10m,
                Locations = ["Shelf A", "Shelf B"]
            })
            .ToList();

        dataContext.Items.AddRange(seed);
        await dataContext.SaveChangesAsync();
        
        // Act
        // Returns: Test Item 1, Test Item 10, Test Item 11 ... Test Item 15
        Result<GetManyItemsDto> actual = await service.GetManyItemsAsync(query: "Test Item 1");
        
        // Assert
        Assert.True(actual.IsSuccess);
        Assert.NotNull(actual.Value);
        Assert.NotNull(actual.Value.Items);
        Assert.NotEmpty(actual.Value.Items);
        Assert.Empty(actual.Errors);
        Assert.Null(actual.Exception);
        Assert.Null(actual.FailureType);
        
        Assert.Equal(1, actual.Value.PageNumber);
        Assert.Equal(10, actual.Value.PageSize);
        Assert.Equal(7, actual.Value.TotalCount);
        Assert.Equal(7, actual.Value.Items.Count);
    }

    [Fact]
    public async Task GetManyItems_ShouldReturnItemsInAscendingOrder()
    {
        using IServiceScope scope = CreateScope();

        IInventoryService service = scope.ServiceProvider.GetRequiredService<IInventoryService>();
        
        DataContext dataContext = scope.ServiceProvider.GetRequiredService<DataContext>();
        
        // Arrange
        UnitOfMeasure uomSeed = new()
        {
            Id = Guid.NewGuid(),
            Name = "Test UOM",
            Code = "u"
        };

        dataContext.UnitOfMeasures.Add(uomSeed);
        
        List<Item> seed = Enumerable.Range(1, 10)
            .Select(i => new Item
            {
                Id = Guid.NewGuid(),
                Name = $"Test Item {i}",
                Uom = uomSeed,
                Quantity = 10m,
                Locations = ["Shelf A", "Shelf B"]
            })
            .ToList();

        dataContext.Items.AddRange(seed);

        Instant now = SystemClock.Instance.GetCurrentInstant();

        for (int i = 0; i < seed.Count; i++)
        {
            dataContext.Entry(seed[i])
                .Property<Instant?>("DateCreated")
                .CurrentValue = now.Plus(Duration.FromSeconds(i));
        }

        await dataContext.SaveChangesAsync();

        List<Item> expected = await dataContext.Items
            .OrderBy(i => EF.Property<Instant>(i, "DateCreated"))
            .ThenBy(i => i.Id)
            .ToListAsync();
        
        // Act
        // Default order direction is ascending,
        // parameter is not required.
        Result<GetManyItemsDto> actual = await service.GetManyItemsAsync(orderDirection: SortDirection.Ascending);
        
        // Assert
        Assert.True(actual.IsSuccess);
        Assert.NotNull(actual.Value);
        Assert.NotNull(actual.Value.Items);
        Assert.NotEmpty(actual.Value.Items);
        Assert.Empty(actual.Errors);
        Assert.Null(actual.Exception);
        Assert.Null(actual.FailureType);
        
        Assert.Equal(
            expected.Select(i => i.Id),
            actual.Value.Items.Select(i => i.Id ?? Guid.Empty));
        
        Assert.Equal(1, actual.Value.PageNumber);
        Assert.Equal(10, actual.Value.PageSize);
        Assert.Equal(10, actual.Value.TotalCount);
        Assert.Equal(10, actual.Value.Items.Count);
    }

    [Fact]
    public async Task GetManyItems_ShouldReturnItemsInDescendingOrder()
    {
        using IServiceScope scope = CreateScope();

        IInventoryService service = scope.ServiceProvider.GetRequiredService<IInventoryService>();
        
        DataContext dataContext = scope.ServiceProvider.GetRequiredService<DataContext>();
        
        // Arrange
        UnitOfMeasure uomSeed = new()
        {
            Id = Guid.NewGuid(),
            Name = "Test UOM",
            Code = "u"
        };

        dataContext.UnitOfMeasures.Add(uomSeed);
        
        List<Item> seed = Enumerable.Range(1, 10)
            .Select(i => new Item
            {
                Id = Guid.NewGuid(),
                Name = $"Test Item {i}",
                Uom = uomSeed,
                Quantity = 10m,
                Locations = ["Shelf A", "Shelf B"]
            })
            .ToList();

        dataContext.Items.AddRange(seed);

        Instant now = SystemClock.Instance.GetCurrentInstant();

        for (int i = 0; i < seed.Count; i++)
        {
            dataContext.Entry(seed[i])
                .Property<Instant?>("DateCreated")
                .CurrentValue = now.Plus(Duration.FromSeconds(i));
        }

        await dataContext.SaveChangesAsync();

        List<Item> expected = await dataContext.Items
            .OrderByDescending(i => EF.Property<Instant>(i, "DateCreated"))
            .ThenBy(i => i.Id)
            .ToListAsync();
        
        // Act
        // Default order direction is ascending,
        // parameter is not required.
        Result<GetManyItemsDto> actual = await service.GetManyItemsAsync(orderDirection: SortDirection.Descending);
        
        // Assert
        Assert.True(actual.IsSuccess);
        Assert.NotNull(actual.Value);
        Assert.NotNull(actual.Value.Items);
        Assert.NotEmpty(actual.Value.Items);
        Assert.Empty(actual.Errors);
        Assert.Null(actual.Exception);
        Assert.Null(actual.FailureType);
        
        Assert.Equal(
            expected.Select(i => i.Id),
            actual.Value.Items.Select(i => i.Id ?? Guid.Empty));
        
        Assert.Equal(1, actual.Value.PageNumber);
        Assert.Equal(10, actual.Value.PageSize);
        Assert.Equal(10, actual.Value.TotalCount);
        Assert.Equal(10, actual.Value.Items.Count);
    }
}