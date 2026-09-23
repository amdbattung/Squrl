using FluentValidation.Results;
using Squrl.App.Features.Suppliers.DTOs;
using Squrl.App.Features.Suppliers.Validators;

namespace Squrl.App.Tests.Validators;

public class UpdateSupplierValidatorTests
{
    private readonly UpdateSupplierValidator _validator;

    public UpdateSupplierValidatorTests()
    {
        _validator = new UpdateSupplierValidator();
    }
    
    [Fact]
    public void ValidSupplier_ShouldPass()
    {
        UpdateSupplierDto supplier = CreateValidSupplier();

        ValidationResult? actual = _validator.Validate(supplier);

        Assert.True(actual.IsValid);
        Assert.Empty(actual.Errors);
    }
    
    [Fact]
    public void Name_ShouldBeRequired()
    {
        UpdateSupplierDto supplier = CreateValidSupplier();
        supplier.Name = "";
        
        ValidationResult? actual = _validator.Validate(supplier);

        Assert.False(actual.IsValid);
        Assert.Contains(actual.Errors, error =>
            error.PropertyName == nameof(UpdateSupplierDto.Name) &&
            error.ErrorMessage == "Name is required.");
    }
    
    [Fact]
    public void Description_ShouldAllowNull()
    {
        UpdateSupplierDto supplier = CreateValidSupplier();
        supplier.Description = null;
        
        ValidationResult? actual = _validator.Validate(supplier);
        
        Assert.True(actual.IsValid);
        Assert.DoesNotContain(actual.Errors, error => 
            error.PropertyName == nameof(UpdateSupplierDto.Description));
    }
    
    [Fact]
    public void Description_ShouldRejectWhitespace()
    {
        UpdateSupplierDto supplier = CreateValidSupplier();
        supplier.Description = "     ";
        
        ValidationResult? actual = _validator.Validate(supplier);

        Assert.False(actual.IsValid);
        Assert.Contains(actual.Errors, error => 
            error.PropertyName == nameof(UpdateSupplierDto.Description) &&
            error.ErrorMessage == "Description must not be only whitespace.");
    }
    
    [Fact]
    public void Description_ShouldRejectNonLatin1()
    {
        UpdateSupplierDto supplier = CreateValidSupplier();
        supplier.Description = "Москва";

        ValidationResult? actual = _validator.Validate(supplier);

        Assert.False(actual.IsValid);
        Assert.Contains(actual.Errors, error =>
            error.PropertyName == nameof(UpdateSupplierDto.Description) &&
            error.ErrorMessage == "Invalid Description.");
    }
    
    private static UpdateSupplierDto CreateValidSupplier()
    {
        return new UpdateSupplierDto
        {
            Name = "Test Supplier",
            Description = "A valid description."
        };
    }
}