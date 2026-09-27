using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.DependencyInjection;
using Squrl.App.Common;
using Squrl.App.Data;
using Squrl.App.Enums;
using Squrl.App.Features.Suppliers.DTOs;
using Squrl.App.Models;
using Squrl.App.Services.Supplier;

namespace Squrl.App.Tests.ServiceTests.SupplierServiceTests;

[Collection("IntegrationTests")]
public sealed class DeleteSupplierTests : IntegrationTest
{
    public DeleteSupplierTests(SqurlWebApplicationFactory factory) : base(factory)
    {
        
    }
    
    [Fact]
    public async Task DeleteSupplier_ShouldDeleteAndReturnSupplier()
    {
        using IServiceScope scope = CreateScope();

        ISupplierService service = scope.ServiceProvider.GetRequiredService<ISupplierService>();
        
        DataContext dataContext = scope.ServiceProvider.GetRequiredService<DataContext>();
        
        // Arrange
        Supplier seed = new()
        {
            Id = Guid.NewGuid(),
            Name = "Test Supplier"
        };

        dataContext.Suppliers.Add(seed);
        await dataContext.SaveChangesAsync();
        
        // Act
        Result<GetSupplierDto> actual = await service.DeleteSupplierAsync(seed.Id);
        
        // Assert
        Assert.True(actual.IsSuccess);
        Assert.NotNull(actual.Value);
        Assert.Empty(actual.Errors);
        Assert.Null(actual.Exception);
        Assert.Null(actual.FailureType);
        
        Assert.Equal(seed.Id, actual.Value.Id);
        Assert.Equal(seed.Name, actual.Value.Name);
        Assert.Equal(seed.Description, actual.Value.Description);
        
        bool exists = await dataContext.Suppliers
            .AnyAsync(s => s.Id == seed.Id);
        
        Assert.False(exists);
    }
    
    [Fact]
    public async Task DeleteSupplier_ShouldFailIfIdIsEmpty()
    {
        using IServiceScope scope = CreateScope();

        ISupplierService service = scope.ServiceProvider.GetRequiredService<ISupplierService>();
        
        // Arrange
        
        // Act
        Result<GetSupplierDto> actual = await service.DeleteSupplierAsync(Guid.Empty);
        
        // Assert
        Assert.False(actual.IsSuccess);
        Assert.Null(actual.Value);
        Assert.Contains("Null or invalid ID.", actual.Errors);
        Assert.Null(actual.Exception);
        Assert.Equal(FailureType.Validation, actual.FailureType);
    }

    [Fact]
    public async Task DeleteSupplier_ShouldFailIfSupplierDoesNotExist()
    {
        using IServiceScope scope = CreateScope();

        ISupplierService service = scope.ServiceProvider.GetRequiredService<ISupplierService>();
        
        // Arrange
        
        // Act
        Result<GetSupplierDto> actual = await service.DeleteSupplierAsync(Guid.NewGuid());
        
        // Assert
        Assert.False(actual.IsSuccess);
        Assert.Null(actual.Value);
        Assert.Contains("Supplier not found.", actual.Errors);
        Assert.Null(actual.Exception);
        Assert.Equal(FailureType.NotFound, actual.FailureType);
    }
}