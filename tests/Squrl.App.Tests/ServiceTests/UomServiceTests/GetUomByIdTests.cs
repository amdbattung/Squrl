using Microsoft.Extensions.DependencyInjection;
using Squrl.App.Common;
using Squrl.App.Data;
using Squrl.App.Enums;
using Squrl.App.Features.UnitOfMeasures.DTOs;
using Squrl.App.Models;
using Squrl.App.Services.UnitOfMeasure;

namespace Squrl.App.Tests.ServiceTests.UomServiceTests;

[Collection("IntegrationTests")]
public sealed class GetUomByIdTests : IntegrationTest
{
    public GetUomByIdTests(SqurlWebApplicationFactory factory) : base(factory)
    {
        
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
        Assert.Empty(actual.Errors);
        Assert.Null(actual.Exception);
        Assert.Null(actual.FailureType);
        
        Assert.Equal(seed.Id, actual.Value.Id);
        Assert.Equal(seed.Name, actual.Value.Name);
        Assert.Equal(seed.Code, actual.Value.Code);
        Assert.Equal(seed.Description, actual.Value.Description);
    }

    [Fact]
    public async Task GetUomById_ShouldFailIfIdIsEmpty()
    {
        using IServiceScope scope = CreateScope();

        IUomService service = scope.ServiceProvider.GetRequiredService<IUomService>();
        
        // Arrange
        
        // Act
        Result<GetUomDto> actual = await service.GetUomByIdAsync(Guid.Empty);
        
        // Assert
        Assert.False(actual.IsSuccess);
        Assert.Null(actual.Value);
        Assert.Contains("Null or invalid ID.", actual.Errors);
        Assert.Null(actual.Exception);
        Assert.Equal(FailureType.Validation, actual.FailureType);
    }
    
    [Fact]
    public async Task GetUomById_ShouldFailIfUomDoesNotExist()
    {
        using IServiceScope scope = CreateScope();

        IUomService service = scope.ServiceProvider.GetRequiredService<IUomService>();
        
        // Arrange
        
        // Act
        Result<GetUomDto> actual = await service.GetUomByIdAsync(Guid.NewGuid());
        
        // Assert
        Assert.False(actual.IsSuccess);
        Assert.Null(actual.Value);
        Assert.Contains("UOM not found.", actual.Errors);
        Assert.Null(actual.Exception);
        Assert.Equal(FailureType.NotFound, actual.FailureType);
    }
}