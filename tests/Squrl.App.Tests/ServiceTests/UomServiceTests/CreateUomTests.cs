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
public sealed class CreateUomTests : IntegrationTest
{
    public CreateUomTests(SqurlWebApplicationFactory factory) : base(factory)
    {
        
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
        Assert.Empty(actual.Errors);
        Assert.Null(actual.Exception);
        Assert.Null(actual.FailureType);

        Assert.Equal(dto.Name, actual.Value.Name);
        Assert.Equal(dto.Code, actual.Value.Code);
        Assert.Equal(dto.Description, actual.Value.Description);

        UnitOfMeasure? created = await dataContext.UnitOfMeasures
            .SingleOrDefaultAsync(u => u.Id == actual.Value.Id);

        Assert.NotNull(created);

        Assert.Equal(actual.Value.Id, created.Id);
        Assert.Equal(dto.Name, created.Name);
        Assert.Equal(dto.Code, created.Code);
        Assert.Equal(dto.Description, created.Description);
    }

    [Fact]
    public async Task CreateUom_ShouldFailWhenDtoIsInvalid()
    {
        using IServiceScope scope = CreateScope();

        IUomService service = scope.ServiceProvider.GetRequiredService<IUomService>();

        // Arrange
        CreateUomDto dto = new CreateUomDto
        {
            Name = "",
            Code = "u"
        };
        
        // Act
        Result<GetUomDto> actual = await service.CreateUomAsync(dto);

        // Assert
        Assert.False(actual.IsSuccess);
        Assert.Null(actual.Value);
        Assert.Contains("Name is required.", actual.Errors);
        Assert.Null(actual.Exception);
        Assert.Equal(FailureType.Validation, actual.FailureType);
    }

    [Fact]
    public async Task CreateUom_ShouldNotPersistWhenValidationFails()
    {
        using IServiceScope scope = CreateScope();

        IUomService service = scope.ServiceProvider.GetRequiredService<IUomService>();
        
        DataContext dataContext = scope.ServiceProvider.GetRequiredService<DataContext>();

        // Arrange
        CreateUomDto dto = new CreateUomDto
        {
            Name = "",
            Code = "u"
        };
        
        // Act
        Result<GetUomDto> actual = await service.CreateUomAsync(dto);

        // Assert
        Assert.False(actual.IsSuccess);
        Assert.Null(actual.Value);

        bool exists = await dataContext.UnitOfMeasures
            .AnyAsync(u => u.Code == dto.Code);

        Assert.False(exists);
    }
}