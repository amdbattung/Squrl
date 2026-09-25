using Microsoft.Extensions.DependencyInjection;
using Squrl.App.Common;
using Squrl.App.Data;
using Squrl.App.Features.UnitOfMeasures.DTOs;
using Squrl.App.Models;
using Squrl.App.Services.UnitOfMeasure;

namespace Squrl.App.Tests.ServiceTests.UomServiceTests;

[Collection("Integration Tests")]
public class GetUomByIdTests : IntegrationTest
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
        
        Assert.Equal(seed.Id, actual.Value.Id);
        Assert.Equal(seed.Name, actual.Value.Name);
        Assert.Equal(seed.Code, actual.Value.Code);
        Assert.Equal(seed.Description, actual.Value.Description);
    }
}