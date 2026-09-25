using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.DependencyInjection;
using Squrl.App.Common;
using Squrl.App.Data;
using Squrl.App.Features.UnitOfMeasures.DTOs;
using Squrl.App.Models;
using Squrl.App.Services.UnitOfMeasure;

namespace Squrl.App.Tests.ServiceTests.UomServiceTests;

[Collection("Integration Tests")]
public class UpdateUomTests : IntegrationTest
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
}