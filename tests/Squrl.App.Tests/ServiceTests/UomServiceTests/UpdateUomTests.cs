using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.DependencyInjection;
using Squrl.App.Common;
using Squrl.App.Data;
using Squrl.App.Enums;
using Squrl.App.Features.UnitOfMeasures.DTOs;
using Squrl.App.Models;
using Squrl.App.Services.UnitOfMeasure;

namespace Squrl.App.Tests.ServiceTests.UomServiceTests;

[Collection("IntegrationTests")]
public sealed class UpdateUomTests : IntegrationTest
{
    public UpdateUomTests(SqurlWebApplicationFactory factory) : base(factory)
    {
        
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
            Description = "An updated description."
        };
        
        // Act
        Result<GetUomDto> actual = await service.UpdateUomAsync(seed.Id, dto);
        
        // Assert
        Assert.True(actual.IsSuccess);
        Assert.NotNull(actual.Value);
        Assert.Empty(actual.Errors);
        Assert.Null(actual.Exception);
        Assert.Null(actual.FailureType);
        
        Assert.Equal(seed.Id, actual.Value.Id);
        Assert.Equal(dto.Name, actual.Value.Name);
        Assert.Equal(dto.Code, actual.Value.Code);
        Assert.Equal(dto.Description, actual.Value.Description);
        
        UnitOfMeasure? updated = await dataContext.UnitOfMeasures
            .AsNoTracking()
            .SingleOrDefaultAsync(u => u.Id == seed.Id);
        
        Assert.NotNull(updated);
        
        Assert.Equal(seed.Id, updated.Id);
        Assert.Equal(dto.Name, updated.Name);
        Assert.Equal(dto.Code, updated.Code);
        Assert.Equal(dto.Description, updated.Description);
    }

    [Fact]
    public async Task UpdateUom_ShouldFailIfIdIsEmpty()
    {
        using IServiceScope scope = CreateScope();

        IUomService service = scope.ServiceProvider.GetRequiredService<IUomService>();
        
        // Arrange
        UpdateUomDto dto = new()
        {
            Name = "Test UOM",
            Code = "u"
        };
        
        // Act
        Result<GetUomDto> actual = await service.UpdateUomAsync(Guid.Empty, dto);
        
        // Assert
        Assert.False(actual.IsSuccess);
        Assert.Null(actual.Value);
        Assert.Contains("Null or invalid ID.", actual.Errors);
        Assert.Null(actual.Exception);
        Assert.Equal(FailureType.Validation, actual.FailureType);
    }

    [Fact]
    public async Task UpdateUom_ShouldFailIfUomDoesNotExist()
    {
        using IServiceScope scope = CreateScope();

        IUomService service = scope.ServiceProvider.GetRequiredService<IUomService>();
        
        // Arrange
        UpdateUomDto dto = new()
        {
            Name = "Test UOM",
            Code = "u"
        };
        
        // Act
        Result<GetUomDto> actual = await service.UpdateUomAsync(Guid.NewGuid(), dto);
        
        // Assert
        Assert.False(actual.IsSuccess);
        Assert.Null(actual.Value);
        Assert.Contains("UOM not found.", actual.Errors);
        Assert.Null(actual.Exception);
        Assert.Equal(FailureType.NotFound, actual.FailureType);
    }
    
    [Fact]
    public async Task UpdateUom_ShouldFailWhenDtoIsInvalid()
    {
        using IServiceScope scope = CreateScope();

        IUomService service = scope.ServiceProvider.GetRequiredService<IUomService>();
        
        // Arrange
        UpdateUomDto dto = new()
        {
            Name = "",
            Code = "u",
        };
        
        // Act
        Result<GetUomDto> actual = await service.UpdateUomAsync(Guid.NewGuid(), dto);
        
        // Assert
        Assert.False(actual.IsSuccess);
        Assert.Null(actual.Value);
        Assert.Contains("Name is required.", actual.Errors);
        Assert.Null(actual.Exception);
        Assert.Equal(FailureType.Validation, actual.FailureType);
    }
    
    [Fact]
    public async Task UpdateUom_ShouldNotPersistWhenValidationFails()
    {
        using IServiceScope scope = CreateScope();

        IUomService service = scope.ServiceProvider.GetRequiredService<IUomService>();
        
        DataContext dataContext = scope.ServiceProvider.GetRequiredService<DataContext>();
        
        // Arrange
        UpdateUomDto dto = new()
        {
            Name = "",
            Code = "u"
        };
        
        // Act
        Result<GetUomDto> actual = await service.UpdateUomAsync(Guid.NewGuid(), dto);
        
        // Assert
        Assert.False(actual.IsSuccess);
        Assert.Null(actual.Value);
        Assert.NotEmpty(actual.Errors);
        Assert.Null(actual.Exception);
        Assert.Equal(FailureType.Validation, actual.FailureType);
        
        bool exists = await dataContext.UnitOfMeasures
            .AnyAsync(u => u.Code == dto.Code);

        Assert.False(exists);
    }
}