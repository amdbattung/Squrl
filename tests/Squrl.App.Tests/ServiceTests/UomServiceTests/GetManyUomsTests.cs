using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.DependencyInjection;
using NodaTime;
using Squrl.App.Common;
using Squrl.App.Data;
using Squrl.App.Enums;
using Squrl.App.Features.UnitOfMeasures.DTOs;
using Squrl.App.Models;
using Squrl.App.Services.UnitOfMeasure;

namespace Squrl.App.Tests.ServiceTests.UomServiceTests;

[Collection("IntegrationTests")]
public sealed class GetManyUomsTests : IntegrationTest
{
    public GetManyUomsTests(SqurlWebApplicationFactory factory) : base(factory)
    {
        
    }
    
    [Fact]
    public async Task GetManyUoms_ShouldReturnUoms()
    {
        using IServiceScope scope = CreateScope();

        IUomService service = scope.ServiceProvider.GetRequiredService<IUomService>();
        
        DataContext dataContext = scope.ServiceProvider.GetRequiredService<DataContext>();
        
        // Arrange
        List<UnitOfMeasure> seed = Enumerable.Range(1, 15)
            .Select(i => new UnitOfMeasure
            {
                Id = Guid.NewGuid(),
                Name = $"Test UOM {i}",
                Code = $"u {i}"
            })
            .ToList();

        dataContext.UnitOfMeasures.AddRange(seed);
        await dataContext.SaveChangesAsync();
        
        // Act
        Result<GetManyUomsDto> actual = await service.GetManyUomsAsync();
        
        // Assert
        Assert.True(actual.IsSuccess);
        Assert.NotNull(actual.Value);
        Assert.NotNull(actual.Value.Uoms);
        Assert.NotEmpty(actual.Value.Uoms);
        Assert.Empty(actual.Errors);
        Assert.Null(actual.Exception);
        Assert.Null(actual.FailureType);
        
        Assert.Equal(1, actual.Value.PageNumber);
        Assert.Equal(10, actual.Value.PageSize);
        Assert.Equal(15, actual.Value.TotalCount);
        Assert.Equal(10, actual.Value.Uoms.Count);
    }

    [Fact]
    public async Task GetManyUoms_ShouldReturnEmptyListIfUomsDoNotExist()
    {
        using IServiceScope scope = CreateScope();

        IUomService service = scope.ServiceProvider.GetRequiredService<IUomService>();
        
        // Arrange
        
        // Act
        Result<GetManyUomsDto> actual = await service.GetManyUomsAsync();
        
        // Assert
        Assert.True(actual.IsSuccess);
        Assert.NotNull(actual.Value);
        Assert.NotNull(actual.Value.Uoms);
        Assert.Empty(actual.Value.Uoms);
        Assert.Empty(actual.Errors);
        Assert.Null(actual.Exception);
        Assert.Null(actual.FailureType);
        
        Assert.Equal(1, actual.Value.PageNumber);
        Assert.Equal(10, actual.Value.PageSize);
        Assert.Equal(0, actual.Value.TotalCount);
    }

    [Fact]
    public async Task GetManyUoms_ShouldReturnPagedUoms()
    {
        using IServiceScope scope = CreateScope();

        IUomService service = scope.ServiceProvider.GetRequiredService<IUomService>();
        
        DataContext dataContext = scope.ServiceProvider.GetRequiredService<DataContext>();
        
        // Arrange
        List<UnitOfMeasure> seed = Enumerable.Range(1, 15)
            .Select(i => new UnitOfMeasure
            {
                Id = Guid.NewGuid(),
                Name = $"Test UOM {i}",
                Code = $"u {i}"
            })
            .ToList();

        dataContext.UnitOfMeasures.AddRange(seed);
        await dataContext.SaveChangesAsync();
        
        // Act
        Result<GetManyUomsDto> actual = await service.GetManyUomsAsync(pageNumber: 3, pageSize: 6);
        
        // Assert
        Assert.True(actual.IsSuccess);
        Assert.NotNull(actual.Value);
        Assert.NotNull(actual.Value.Uoms);
        Assert.NotEmpty(actual.Value.Uoms);
        Assert.Empty(actual.Errors);
        Assert.Null(actual.Exception);
        Assert.Null(actual.FailureType);
        
        Assert.Equal(3, actual.Value.PageNumber);
        Assert.Equal(6, actual.Value.PageSize);
        Assert.Equal(15, actual.Value.TotalCount);
        Assert.Equal(3, actual.Value.Uoms.Count);
    }
    
    [Fact]
    public async Task GetManyUoms_ShouldReturnQueriedUoms()
    {
        using IServiceScope scope = CreateScope();

        IUomService service = scope.ServiceProvider.GetRequiredService<IUomService>();
        
        DataContext dataContext = scope.ServiceProvider.GetRequiredService<DataContext>();
        
        // Arrange
        List<UnitOfMeasure> seed = Enumerable.Range(1, 15)
            .Select(i => new UnitOfMeasure
            {
                Id = Guid.NewGuid(),
                Name = $"Test UOM {i}",
                Code = $"u {i}"
            })
            .ToList();

        dataContext.UnitOfMeasures.AddRange(seed);
        await dataContext.SaveChangesAsync();
        
        // Act
        // Returns: Test UOM 1, Test UOM 10, Test UOM 11 ... Test UOM 15
        Result<GetManyUomsDto> actual = await service.GetManyUomsAsync(query: "Test UOM 1");
        
        // Assert
        Assert.True(actual.IsSuccess);
        Assert.NotNull(actual.Value);
        Assert.NotNull(actual.Value.Uoms);
        Assert.NotEmpty(actual.Value.Uoms);
        Assert.Empty(actual.Errors);
        Assert.Null(actual.Exception);
        Assert.Null(actual.FailureType);
        
        Assert.Equal(1, actual.Value.PageNumber);
        Assert.Equal(10, actual.Value.PageSize);
        Assert.Equal(7, actual.Value.TotalCount);
        Assert.Equal(7, actual.Value.Uoms.Count);
    }
    
    [Fact]
    public async Task GetManyUoms_ShouldReturnUomsInAscendingOrder()
    {
        using IServiceScope scope = CreateScope();

        IUomService service = scope.ServiceProvider.GetRequiredService<IUomService>();
        
        DataContext dataContext = scope.ServiceProvider.GetRequiredService<DataContext>();
        
        // Arrange
        List<UnitOfMeasure> seed = Enumerable.Range(1, 10)
            .Select(i => new UnitOfMeasure
            {
                Id = Guid.NewGuid(),
                Name = $"Test UOM {i}",
                Code = $"u {i}"
            })
            .ToList();

        dataContext.UnitOfMeasures.AddRange(seed);

        Instant now = SystemClock.Instance.GetCurrentInstant();

        for (int i = 0; i < seed.Count; i++)
        {
            dataContext.Entry(seed[i])
                .Property<Instant?>("DateCreated")
                .CurrentValue = now.Plus(Duration.FromSeconds(i));
        }

        await dataContext.SaveChangesAsync();

        List<UnitOfMeasure> expected = await dataContext.UnitOfMeasures
            .OrderBy(u => EF.Property<Instant>(u, "DateCreated"))
            .ThenBy(u => u.Id)
            .ToListAsync();
        
        // Act
        // Default order direction is ascending,
        // parameter is not required.
        Result<GetManyUomsDto> actual = await service.GetManyUomsAsync(orderDirection: SortDirection.Ascending);
        
        // Assert
        Assert.True(actual.IsSuccess);
        Assert.NotNull(actual.Value);
        Assert.NotNull(actual.Value.Uoms);
        Assert.NotEmpty(actual.Value.Uoms);
        Assert.Empty(actual.Errors);
        Assert.Null(actual.Exception);
        Assert.Null(actual.FailureType);
        
        Assert.Equal(
            expected.Select(u => u.Id),
            actual.Value.Uoms.Select(u => u.Id ?? Guid.Empty));
        
        Assert.Equal(1, actual.Value.PageNumber);
        Assert.Equal(10, actual.Value.PageSize);
        Assert.Equal(10, actual.Value.TotalCount);
        Assert.Equal(10, actual.Value.Uoms.Count);
    }

    [Fact]
    public async Task GetManyUoms_ShouldReturnUomsInDescendingOrder()
    {
        using IServiceScope scope = CreateScope();

        IUomService service = scope.ServiceProvider.GetRequiredService<IUomService>();
        
        DataContext dataContext = scope.ServiceProvider.GetRequiredService<DataContext>();
        
        // Arrange
        List<UnitOfMeasure> seed = Enumerable.Range(1, 10)
            .Select(i => new UnitOfMeasure
            {
                Id = Guid.NewGuid(),
                Name = $"Test UOM {i}",
                Code = $"u {i}"
            })
            .ToList();

        dataContext.UnitOfMeasures.AddRange(seed);

        Instant now = SystemClock.Instance.GetCurrentInstant();

        for (int i = 0; i < seed.Count; i++)
        {
            dataContext.Entry(seed[i])
                .Property<Instant?>("DateCreated")
                .CurrentValue = now.Plus(Duration.FromSeconds(i));
        }

        await dataContext.SaveChangesAsync();

        List<UnitOfMeasure> expected = await dataContext.UnitOfMeasures
            .OrderByDescending(u => EF.Property<Instant>(u, "DateCreated"))
            .ThenBy(u => u.Id)
            .ToListAsync();
        
        // Act
        Result<GetManyUomsDto> actual = await service.GetManyUomsAsync(orderDirection: SortDirection.Descending);
        
        // Assert
        Assert.True(actual.IsSuccess);
        Assert.NotNull(actual.Value);
        Assert.NotNull(actual.Value.Uoms);
        Assert.NotEmpty(actual.Value.Uoms);
        Assert.Empty(actual.Errors);
        Assert.Null(actual.Exception);
        Assert.Null(actual.FailureType);
        
        Assert.Equal(
            expected.Select(u => u.Id),
            actual.Value.Uoms.Select(u => u.Id ?? Guid.Empty));
        
        Assert.Equal(1, actual.Value.PageNumber);
        Assert.Equal(10, actual.Value.PageSize);
        Assert.Equal(10, actual.Value.TotalCount);
        Assert.Equal(10, actual.Value.Uoms.Count);
    }
}