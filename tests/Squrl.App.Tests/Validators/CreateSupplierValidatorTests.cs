using FluentValidation.Results;
using Squrl.App.Features.Suppliers.DTOs;
using Squrl.App.Features.Suppliers.Validators;

namespace Squrl.App.Tests.Validators;

public class CreateSupplierValidatorTests
{
    private readonly CreateSupplierValidator _validator;

    public CreateSupplierValidatorTests()
    {
        _validator = new CreateSupplierValidator();
    }
    
    [Fact]
    public void ValidSupplier_ShouldPass()
    {
        CreateSupplierDto supplier = CreateValidSupplier();

        ValidationResult? actual = _validator.Validate(supplier);

        Assert.True(actual.IsValid);
        Assert.Empty(actual.Errors);
    }
    
    [Fact]
    public void Name_ShouldBeRequired()
    {
        CreateSupplierDto supplier = CreateValidSupplier();
        supplier.Name = "";
        
        ValidationResult? actual = _validator.Validate(supplier);

        Assert.False(actual.IsValid);
        Assert.Contains(actual.Errors, error =>
            error.PropertyName == nameof(CreateSupplierDto.Name) &&
            error.ErrorMessage == "Name is required.");
    }
    
    [Fact]
    public void Description_ShouldAllowNull()
    {
        CreateSupplierDto supplier = CreateValidSupplier();
        supplier.Description = null;
        
        ValidationResult? actual = _validator.Validate(supplier);
        
        Assert.True(actual.IsValid);
        Assert.DoesNotContain(actual.Errors, error => 
            error.PropertyName == nameof(CreateSupplierDto.Description));
    }
    
    [Fact]
    public void Description_ShouldRejectWhitespace()
    {
        CreateSupplierDto supplier = CreateValidSupplier();
        supplier.Description = "     ";
        
        ValidationResult? actual = _validator.Validate(supplier);

        Assert.False(actual.IsValid);
        Assert.Contains(actual.Errors, error => 
            error.PropertyName == nameof(CreateSupplierDto.Description) &&
            error.ErrorMessage == "Description must not be only whitespace.");
    }
    
    [Fact]
    public void Description_ShouldRejectNonLatin1()
    {
        CreateSupplierDto supplier = CreateValidSupplier();
        supplier.Description = "Москва";

        ValidationResult? actual = _validator.Validate(supplier);

        Assert.False(actual.IsValid);
        Assert.Contains(actual.Errors, error =>
            error.PropertyName == nameof(CreateSupplierDto.Description) &&
            error.ErrorMessage == "Invalid Description.");
    }
    
    private static CreateSupplierDto CreateValidSupplier()
    {
        return new CreateSupplierDto
        {
            Name = "Test Supplier",
            Description = "A valid description."
        };
    }
}