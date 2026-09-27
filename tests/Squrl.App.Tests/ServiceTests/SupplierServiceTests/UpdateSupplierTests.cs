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
public sealed class UpdateSupplierTests : IntegrationTest
{
    public UpdateSupplierTests(SqurlWebApplicationFactory factory) : base(factory)
    {
        
    }
    
    [Fact]
    public async Task UpdateSupplier_ShouldUpdateAndPersistSupplier()
    {
        using IServiceScope scope = CreateScope();

        ISupplierService service = scope.ServiceProvider.GetRequiredService<ISupplierService>();
        
        DataContext dataContext = scope.ServiceProvider.GetRequiredService<DataContext>();
        
        // Arrange
        Supplier seed = new()
        {
            Id = Guid.NewGuid(),
            Name = "Test Supplier",
            Description = "A valid description."
        };

        dataContext.Suppliers.Add(seed);
        await dataContext.SaveChangesAsync();
        
        UpdateSupplierDto dto = new()
        {
            Name = "Updated Test Supplier",
            Description = "An updated description."
        };
        
        // Act
        Result<GetSupplierDto> actual = await service.UpdateSupplierAsync(seed.Id, dto);
        
        // Assert
        Assert.True(actual.IsSuccess);
        Assert.NotNull(actual.Value);
        Assert.Empty(actual.Errors);
        Assert.Null(actual.Exception);
        Assert.Null(actual.FailureType);
        
        Assert.Equal(seed.Id, actual.Value.Id);
        Assert.Equal(dto.Name, actual.Value.Name);
        Assert.Equal(dto.Description, actual.Value.Description);
        
        Supplier? updated = await dataContext.Suppliers
            .SingleOrDefaultAsync(s => s.Id == seed.Id);
        
        Assert.NotNull(updated);
        
        Assert.Equal(seed.Id, updated.Id);
        Assert.Equal(dto.Name, updated.Name);
        Assert.Equal(dto.Description, updated.Description);
    }

    [Fact]
    public async Task UpdateSupplier_ShouldFailIfIdIsEmpty()
    {
        using IServiceScope scope = CreateScope();

        ISupplierService service = scope.ServiceProvider.GetRequiredService<ISupplierService>();
        
        // Arrange
        UpdateSupplierDto dto = new()
        {
            Name = "Test Supplier"
        };
        
        // Act
        Result<GetSupplierDto> actual = await service.UpdateSupplierAsync(Guid.Empty, dto);
        
        // Assert
        Assert.False(actual.IsSuccess);
        Assert.Null(actual.Value);
        Assert.Contains("Null or invalid ID.", actual.Errors);
        Assert.Null(actual.Exception);
        Assert.Equal(FailureType.Validation, actual.FailureType);
    }

    [Fact]
    public async Task UpdateSupplier_ShouldFailIfSupplierDoesNotExist()
    {
        using IServiceScope scope = CreateScope();

        ISupplierService service = scope.ServiceProvider.GetRequiredService<ISupplierService>();
        
        // Arrange
        UpdateSupplierDto dto = new()
        {
            Name = "Test Supplier"
        };
        
        // Act
        Result<GetSupplierDto> actual = await service.UpdateSupplierAsync(Guid.NewGuid(), dto);
        
        // Assert
        Assert.False(actual.IsSuccess);
        Assert.Null(actual.Value);
        Assert.Contains("Supplier not found.", actual.Errors);
        Assert.Null(actual.Exception);
        Assert.Equal(FailureType.NotFound, actual.FailureType);
    }
    
    [Fact]
    public async Task UpdateSupplier_ShouldFailWhenDtoIsInvalid()
    {
        using IServiceScope scope = CreateScope();

        ISupplierService service = scope.ServiceProvider.GetRequiredService<ISupplierService>();
        
        // Arrange
        UpdateSupplierDto dto = new()
        {
            Name = ""
        };
        
        // Act
        Result<GetSupplierDto> actual = await service.UpdateSupplierAsync(Guid.NewGuid(), dto);
        
        // Assert
        Assert.False(actual.IsSuccess);
        Assert.Null(actual.Value);
        Assert.Contains("Name is required.", actual.Errors);
        Assert.Null(actual.Exception);
        Assert.Equal(FailureType.Validation, actual.FailureType);
    }
    
    [Fact]
    public async Task UpdateSupplier_ShouldNotPersistWhenValidationFails()
    {
        using IServiceScope scope = CreateScope();

        ISupplierService service = scope.ServiceProvider.GetRequiredService<ISupplierService>();
        
        DataContext dataContext = scope.ServiceProvider.GetRequiredService<DataContext>();
        
        // Arrange
        UpdateSupplierDto dto = new()
        {
            Name = "",
            Description = "A valid description."
        };
        
        // Act
        Result<GetSupplierDto> actual = await service.UpdateSupplierAsync(Guid.NewGuid(), dto);
        
        // Assert
        Assert.False(actual.IsSuccess);
        Assert.Null(actual.Value);
        Assert.NotEmpty(actual.Errors);
        Assert.Null(actual.Exception);
        Assert.Equal(FailureType.Validation, actual.FailureType);
        
        bool exists = await dataContext.Suppliers
            .AnyAsync(s => s.Description == dto.Description);

        Assert.False(exists);
    }
}