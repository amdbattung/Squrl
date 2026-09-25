using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.DependencyInjection;
using Squrl.App.Common;
using Squrl.App.Data;
using Squrl.App.Features.UnitOfMeasures.DTOs;
using Squrl.App.Models;
using Squrl.App.Services.UnitOfMeasure;

namespace Squrl.App.Tests.ServiceTests.UomServiceTests;

[Collection("Integration Tests")]
public class DeleteUomTests : IntegrationTest
{
    public DeleteUomTests(SqurlWebApplicationFactory factory) : base(factory)
    {
        
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