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
public sealed class CreateSupplierTests : IntegrationTest
{
    public CreateSupplierTests(SqurlWebApplicationFactory factory) : base(factory)
    {
        
    }
    
    [Fact]
    public async Task CreateSupplier_ShouldCreateSupplier()
    {
        using IServiceScope scope = CreateScope();

        ISupplierService service = scope.ServiceProvider.GetRequiredService<ISupplierService>();
        
        DataContext dataContext = scope.ServiceProvider.GetRequiredService<DataContext>();

        // Arrange
        CreateSupplierDto dto = new CreateSupplierDto
        {
            Name = "Test Supplier",
            Description = "A valid description."
        };
        
        // Act
        Result<GetSupplierDto> actual = await service.CreateSupplierAsync(dto);

        // Assert
        Assert.True(actual.IsSuccess);
        Assert.NotNull(actual.Value);
        Assert.Empty(actual.Errors);
        Assert.Null(actual.Exception);
        Assert.Null(actual.FailureType);

        Assert.Equal(dto.Name, actual.Value.Name);
        Assert.Equal(dto.Description, actual.Value.Description);

        Supplier? created = await dataContext.Suppliers
            .SingleOrDefaultAsync(s => s.Id == actual.Value.Id);

        Assert.NotNull(created);

        Assert.Equal(actual.Value.Id, created.Id);
        Assert.Equal(dto.Name, created.Name);
        Assert.Equal(dto.Description, created.Description);
    }

    [Fact]
    public async Task CreateSupplier_ShouldFailWhenDtoIsInvalid()
    {
        using IServiceScope scope = CreateScope();

        ISupplierService service = scope.ServiceProvider.GetRequiredService<ISupplierService>();

        // Arrange
        CreateSupplierDto dto = new CreateSupplierDto
        {
            Name = "",
        };
        
        // Act
        Result<GetSupplierDto> actual = await service.CreateSupplierAsync(dto);

        // Assert
        Assert.False(actual.IsSuccess);
        Assert.Null(actual.Value);
        Assert.Contains("Name is required.", actual.Errors);
        Assert.Null(actual.Exception);
        Assert.Equal(FailureType.Validation, actual.FailureType);
    }

    [Fact]
    public async Task CreateSupplier_ShouldNotPersistWhenValidationFails()
    {
        using IServiceScope scope = CreateScope();

        ISupplierService service = scope.ServiceProvider.GetRequiredService<ISupplierService>();
        
        DataContext dataContext = scope.ServiceProvider.GetRequiredService<DataContext>();

        // Arrange
        CreateSupplierDto dto = new CreateSupplierDto
        {
            Name = "",
            Description = "A valid description."
        };
        
        // Act
        Result<GetSupplierDto> actual = await service.CreateSupplierAsync(dto);

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