using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.DependencyInjection;
using Squrl.App.Common;
using Squrl.App.Data;
using Squrl.App.Features.UnitOfMeasures.DTOs;
using Squrl.App.Models;
using Squrl.App.Services.UnitOfMeasure;

namespace Squrl.App.Tests.ServiceTests;

public sealed class UomServiceTests : IntegrationTest, IClassFixture<SqurlWebApplicationFactory>
{
    public UomServiceTests(SqurlWebApplicationFactory factory) : base(factory)
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
                Code = $"u {i}",
                Description = $"A valid description. {i}"
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
        
        Assert.Equal(1, actual.Value.PageNumber);
        Assert.Equal(10, actual.Value.PageSize);
        Assert.Equal(15, actual.Value.TotalCount);
        Assert.Equal(10, actual.Value.Uoms.Count);
    }

    [Fact]
    public async Task CreateUom_ShouldCreateUom()
    {
        using IServiceScope scope = CreateScope();

        IUomService service = scope.ServiceProvider.GetRequiredService<IUomService>();
        
        DataContext dataContext = scope.ServiceProvider.GetRequiredService<DataContext>();

        // Arrange
        CreateUomDto dto = new CreateUomDto
        {
            Name = "Test UOM",
            Code = "u",
            Description = "A valid description."
        };
        
        // Act
        Result<GetUomDto> actual = await service.CreateUomAsync(dto);

        // Assert
        Assert.True(actual.IsSuccess);
        Assert.NotNull(actual.Value);

        Assert.Equal(dto.Name, actual.Value.Name);
        Assert.Equal(dto.Code, actual.Value.Code);
        Assert.Equal(dto.Description, actual.Value.Description);

        UnitOfMeasure? created = await dataContext.UnitOfMeasures
            .SingleOrDefaultAsync(x => x.Id == actual.Value.Id);

        Assert.NotNull(created);

        Assert.Equal(dto.Name, created.Name);
        Assert.Equal(dto.Code, created.Code);
        Assert.Equal(dto.Description, created.Description);
    }

    [Fact]
    public async Task GetUomById_ShouldReturnUom()
    {
        using IServiceScope scope = CreateScope();

        IUomService service = scope.ServiceProvider.GetRequiredService<IUomService>();
        
        DataContext dataContext = scope.ServiceProvider.GetRequiredService<DataContext>();
        
        // Arrange
        UnitOfMeasure seed = new()
        {
            Id = Guid.NewGuid(),
            Name = "Test UOM",
            Code = "u",
            Description = "A valid description."
        };

        dataContext.UnitOfMeasures.Add(seed);
        await dataContext.SaveChangesAsync();
        
        // Act
        Result<GetUomDto> actual = await service.GetUomByIdAsync(seed.Id);
        
        // Assert
        Assert.True(actual.IsSuccess);
        Assert.NotNull(actual.Value);
        
        Assert.Equal(seed.Id, actual.Value.Id);
        Assert.Equal(seed.Name, actual.Value.Name);
        Assert.Equal(seed.Code, actual.Value.Code);
        Assert.Equal(seed.Description, actual.Value.Description);
    }
    
    [Fact]
    public async Task UpdateUom_ShouldUpdateAndPersistUom()
    {
        using IServiceScope scope = CreateScope();

        IUomService service = scope.ServiceProvider.GetRequiredService<IUomService>();
        
        DataContext dataContext = scope.ServiceProvider.GetRequiredService<DataContext>();
        
        // Arrange
        UnitOfMeasure seed = new()
        {
            Id = Guid.NewGuid(),
            Name = "Test UOM",
            Code = "u",
            Description = "A valid description."
        };

        dataContext.UnitOfMeasures.Add(seed);
        await dataContext.SaveChangesAsync();
        
        UpdateUomDto dto = new()
        {
            Name = "Updated Test UOM",
            Code = "up",
            Description = "An updated description"
        };
        
        // Act
        Result<GetUomDto> actual = await service.UpdateUomAsync(seed.Id, dto);
        
        // Assert
        Assert.True(actual.IsSuccess);
        Assert.NotNull(actual.Value);
        
        Assert.Equal(seed.Id, actual.Value.Id);
        Assert.Equal(dto.Name, actual.Value.Name);
        Assert.Equal(dto.Code, actual.Value.Code);
        Assert.Equal(dto.Description, actual.Value.Description);
        
        UnitOfMeasure? updated = await dataContext.UnitOfMeasures
            .SingleOrDefaultAsync(x => x.Id == seed.Id);
        
        Assert.NotNull(updated);
        
        Assert.Equal(seed.Id, updated.Id);
        Assert.Equal(dto.Name, updated.Name);
        Assert.Equal(dto.Code, updated.Code);
        Assert.Equal(dto.Description, updated.Description);
    }
    
    [Fact]
    public async Task DeleteUom_ShouldDeleteAndReturnUom()
    {
        using IServiceScope scope = CreateScope();

        IUomService service = scope.ServiceProvider.GetRequiredService<IUomService>();
        
        DataContext dataContext = scope.ServiceProvider.GetRequiredService<DataContext>();
        
        // Arrange
        UnitOfMeasure seed = new()
        {
            Id = Guid.NewGuid(),
            Name = "Test UOM",
            Code = "u",
            Description = "A valid description."
        };

        dataContext.UnitOfMeasures.Add(seed);
        await dataContext.SaveChangesAsync();
        
        // Act
        Result<GetUomDto> actual = await service.DeleteUomAsync(seed.Id);
        
        // Assert
        Assert.True(actual.IsSuccess);
        Assert.NotNull(actual.Value);
        
        Assert.Equal(seed.Id, actual.Value.Id);
        Assert.Equal(seed.Name, actual.Value.Name);
        Assert.Equal(seed.Code, actual.Value.Code);
        Assert.Equal(seed.Description, actual.Value.Description);
        
        UnitOfMeasure? deleted = await dataContext.UnitOfMeasures
            .SingleOrDefaultAsync(x => x.Id == seed.Id);
        
        Assert.Null(deleted);
    }
}