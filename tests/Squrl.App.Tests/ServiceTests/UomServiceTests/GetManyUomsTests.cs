using Microsoft.Extensions.DependencyInjection;
using Squrl.App.Common;
using Squrl.App.Data;
using Squrl.App.Features.UnitOfMeasures.DTOs;
using Squrl.App.Models;
using Squrl.App.Services.UnitOfMeasure;

namespace Squrl.App.Tests.ServiceTests.UomServiceTests;

[Collection("Integration Tests")]
public class GetManyUomsTests : IntegrationTest
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
}