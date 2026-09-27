using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.DependencyInjection;
using NodaTime;
using Squrl.App.Common;
using Squrl.App.Data;
using Squrl.App.Enums;
using Squrl.App.Features.Suppliers.DTOs;
using Squrl.App.Models;
using Squrl.App.Services.Supplier;

namespace Squrl.App.Tests.ServiceTests.SupplierServiceTests;

[Collection("IntegrationTests")]
public sealed class GetManySuppliersTests : IntegrationTest
{
    public GetManySuppliersTests(SqurlWebApplicationFactory factory) : base(factory)
    {
        
    }
    
    [Fact]
    public async Task GetManySuppliers_ShouldReturnSuppliers()
    {
        using IServiceScope scope = CreateScope();

        ISupplierService service = scope.ServiceProvider.GetRequiredService<ISupplierService>();
        
        DataContext dataContext = scope.ServiceProvider.GetRequiredService<DataContext>();
        
        // Arrange
        List<Supplier> seed = Enumerable.Range(1, 15)
            .Select(i => new Supplier
            {
                Id = Guid.NewGuid(),
                Name = $"Test Supplier {i}"
            })
            .ToList();

        dataContext.Suppliers.AddRange(seed);
        await dataContext.SaveChangesAsync();
        
        // Act
        Result<GetManySuppliersDto> actual = await service.GetManySuppliersAsync();
        
        // Assert
        Assert.True(actual.IsSuccess);
        Assert.NotNull(actual.Value);
        Assert.NotNull(actual.Value.Suppliers);
        Assert.NotEmpty(actual.Value.Suppliers);
        Assert.Empty(actual.Errors);
        Assert.Null(actual.Exception);
        Assert.Null(actual.FailureType);
        
        Assert.Equal(1, actual.Value.PageNumber);
        Assert.Equal(10, actual.Value.PageSize);
        Assert.Equal(15, actual.Value.TotalCount);
        Assert.Equal(10, actual.Value.Suppliers.Count);
    }

    [Fact]
    public async Task GetManySuppliers_ShouldReturnEmptyListIfSuppliersDoNotExist()
    {
        using IServiceScope scope = CreateScope();

        ISupplierService service = scope.ServiceProvider.GetRequiredService<ISupplierService>();
        
        // Arrange
        
        // Act
        Result<GetManySuppliersDto> actual = await service.GetManySuppliersAsync();
        
        // Assert
        Assert.True(actual.IsSuccess);
        Assert.NotNull(actual.Value);
        Assert.NotNull(actual.Value.Suppliers);
        Assert.Empty(actual.Value.Suppliers);
        Assert.Empty(actual.Errors);
        Assert.Null(actual.Exception);
        Assert.Null(actual.FailureType);
        
        Assert.Equal(1, actual.Value.PageNumber);
        Assert.Equal(10, actual.Value.PageSize);
        Assert.Equal(0, actual.Value.TotalCount);
    }

    [Fact]
    public async Task GetManySuppliers_ShouldReturnPagedSuppliers()
    {
        using IServiceScope scope = CreateScope();

        ISupplierService service = scope.ServiceProvider.GetRequiredService<ISupplierService>();
        
        DataContext dataContext = scope.ServiceProvider.GetRequiredService<DataContext>();
        
        // Arrange
        List<Supplier> seed = Enumerable.Range(1, 15)
            .Select(i => new Supplier
            {
                Id = Guid.NewGuid(),
                Name = $"Test Supplier {i}"
            })
            .ToList();

        dataContext.Suppliers.AddRange(seed);
        await dataContext.SaveChangesAsync();
        
        // Act
        Result<GetManySuppliersDto> actual = await service.GetManySuppliersAsync(pageNumber: 3, pageSize: 6);
        
        // Assert
        Assert.True(actual.IsSuccess);
        Assert.NotNull(actual.Value);
        Assert.NotNull(actual.Value.Suppliers);
        Assert.NotEmpty(actual.Value.Suppliers);
        Assert.Empty(actual.Errors);
        Assert.Null(actual.Exception);
        Assert.Null(actual.FailureType);
        
        Assert.Equal(3, actual.Value.PageNumber);
        Assert.Equal(6, actual.Value.PageSize);
        Assert.Equal(15, actual.Value.TotalCount);
        Assert.Equal(3, actual.Value.Suppliers.Count);
    }
    
    [Fact]
    public async Task GetManySuppliers_ShouldReturnQueriedSuppliers()
    {
        using IServiceScope scope = CreateScope();

        ISupplierService service = scope.ServiceProvider.GetRequiredService<ISupplierService>();
        
        DataContext dataContext = scope.ServiceProvider.GetRequiredService<DataContext>();
        
        // Arrange
        List<Supplier> seed = Enumerable.Range(1, 15)
            .Select(i => new Supplier
            {
                Id = Guid.NewGuid(),
                Name = $"Test Supplier {i}",
            })
            .ToList();

        dataContext.Suppliers.AddRange(seed);
        await dataContext.SaveChangesAsync();
        
        // Act
        // Returns: Test Supplier 1, Test Supplier 10, Test Supplier 11 ... Test Supplier 15
        Result<GetManySuppliersDto> actual = await service.GetManySuppliersAsync(query: "Test Supplier 1");
        
        // Assert
        Assert.True(actual.IsSuccess);
        Assert.NotNull(actual.Value);
        Assert.NotNull(actual.Value.Suppliers);
        Assert.NotEmpty(actual.Value.Suppliers);
        Assert.Empty(actual.Errors);
        Assert.Null(actual.Exception);
        Assert.Null(actual.FailureType);
        
        Assert.Equal(1, actual.Value.PageNumber);
        Assert.Equal(10, actual.Value.PageSize);
        Assert.Equal(7, actual.Value.TotalCount);
        Assert.Equal(7, actual.Value.Suppliers.Count);
    }
    
    [Fact]
    public async Task GetManySuppliers_ShouldReturnSuppliersInAscendingOrder()
    {
        using IServiceScope scope = CreateScope();

        ISupplierService service = scope.ServiceProvider.GetRequiredService<ISupplierService>();
        
        DataContext dataContext = scope.ServiceProvider.GetRequiredService<DataContext>();
        
        // Arrange
        List<Supplier> seed = Enumerable.Range(1, 10)
            .Select(i => new Supplier
            {
                Id = Guid.NewGuid(),
                Name = $"Test Supplier {i}"
            })
            .ToList();

        dataContext.Suppliers.AddRange(seed);

        Instant now = SystemClock.Instance.GetCurrentInstant();

        for (int i = 0; i < seed.Count; i++)
        {
            dataContext.Entry(seed[i])
                .Property<Instant?>("DateCreated")
                .CurrentValue = now.Plus(Duration.FromSeconds(i));
        }

        await dataContext.SaveChangesAsync();

        List<Supplier> expected = await dataContext.Suppliers
            .OrderBy(s => EF.Property<Instant>(s, "DateCreated"))
            .ThenBy(s => s.Id)
            .ToListAsync();
        
        // Act
        // Default order direction is ascending,
        // parameter is not required.
        Result<GetManySuppliersDto> actual = await service.GetManySuppliersAsync(orderDirection: SortDirection.Ascending);
        
        // Assert
        Assert.True(actual.IsSuccess);
        Assert.NotNull(actual.Value);
        Assert.NotNull(actual.Value.Suppliers);
        Assert.NotEmpty(actual.Value.Suppliers);
        Assert.Empty(actual.Errors);
        Assert.Null(actual.Exception);
        Assert.Null(actual.FailureType);
        
        Assert.Equal(
            expected.Select(s => s.Id),
            actual.Value.Suppliers.Select(s => s.Id ?? Guid.Empty));
        
        Assert.Equal(1, actual.Value.PageNumber);
        Assert.Equal(10, actual.Value.PageSize);
        Assert.Equal(10, actual.Value.TotalCount);
        Assert.Equal(10, actual.Value.Suppliers.Count);
    }

    [Fact]
    public async Task GetManySuppliers_ShouldReturnSuppliersInDescendingOrder()
    {
        using IServiceScope scope = CreateScope();

        ISupplierService service = scope.ServiceProvider.GetRequiredService<ISupplierService>();
        
        DataContext dataContext = scope.ServiceProvider.GetRequiredService<DataContext>();
        
        // Arrange
        List<Supplier> seed = Enumerable.Range(1, 10)
            .Select(i => new Supplier
            {
                Id = Guid.NewGuid(),
                Name = $"Test Supplier {i}"
            })
            .ToList();

        dataContext.Suppliers.AddRange(seed);

        Instant now = SystemClock.Instance.GetCurrentInstant();

        for (int i = 0; i < seed.Count; i++)
        {
            dataContext.Entry(seed[i])
                .Property<Instant?>("DateCreated")
                .CurrentValue = now.Plus(Duration.FromSeconds(i));
        }

        await dataContext.SaveChangesAsync();

        List<Supplier> expected = await dataContext.Suppliers
            .OrderByDescending(s => EF.Property<Instant>(s, "DateCreated"))
            .ThenBy(s => s.Id)
            .ToListAsync();
        
        // Act
        Result<GetManySuppliersDto> actual = await service.GetManySuppliersAsync(orderDirection: SortDirection.Descending);
        
        // Assert
        Assert.True(actual.IsSuccess);
        Assert.NotNull(actual.Value);
        Assert.NotNull(actual.Value.Suppliers);
        Assert.NotEmpty(actual.Value.Suppliers);
        Assert.Empty(actual.Errors);
        Assert.Null(actual.Exception);
        Assert.Null(actual.FailureType);
        
        Assert.Equal(
            expected.Select(s => s.Id),
            actual.Value.Suppliers.Select(s => s.Id ?? Guid.Empty));
        
        Assert.Equal(1, actual.Value.PageNumber);
        Assert.Equal(10, actual.Value.PageSize);
        Assert.Equal(10, actual.Value.TotalCount);
        Assert.Equal(10, actual.Value.Suppliers.Count);
    }
}