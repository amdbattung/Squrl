using FluentValidation.Results;
using Squrl.App.Features.UnitOfMeasures.DTOs;
using Squrl.App.Features.UnitOfMeasures.Validators;

namespace Squrl.App.Tests.Validators;

public class CreateUomValidatorTests
{
    private readonly CreateUomValidator _validator;

    public CreateUomValidatorTests()
    {
        _validator = new CreateUomValidator();
    }
    
    [Fact]
    public void ValidUom_ShouldPass()
    {
        CreateUomDto uom = CreateValidUom();

        ValidationResult? actual = _validator.Validate(uom);

        Assert.True(actual.IsValid);
        Assert.Empty(actual.Errors);
    }
    
    [Fact]
    public void Name_ShouldBeRequired()
    {
        CreateUomDto uom = CreateValidUom();
        uom.Name = "";
        
        ValidationResult? actual = _validator.Validate(uom);

        Assert.False(actual.IsValid);
        Assert.Contains(actual.Errors, error =>
            error.PropertyName == nameof(CreateUomDto.Name) &&
            error.ErrorMessage == "Name is required.");
    }
    
    [Fact]
    public void Code_ShouldBeRequired()
    {
        CreateUomDto uom = CreateValidUom();
        uom.Code = "";
        
        ValidationResult? actual = _validator.Validate(uom);

        Assert.False(actual.IsValid);
        Assert.Contains(actual.Errors, error =>
            error.PropertyName == nameof(CreateUomDto.Code) &&
            error.ErrorMessage == "Code is required.");
    }
    
    [Fact]
    public void Description_ShouldAllowNull()
    {
        CreateUomDto uom = CreateValidUom();
        uom.Description = null;
        
        ValidationResult? actual = _validator.Validate(uom);
        
        Assert.True(actual.IsValid);
        Assert.DoesNotContain(actual.Errors, error => 
            error.PropertyName == nameof(CreateUomDto.Description));
    }
    
    [Fact]
    public void Description_ShouldRejectWhitespace()
    {
        CreateUomDto uom = CreateValidUom();
        uom.Description = "     ";
        
        ValidationResult? actual = _validator.Validate(uom);

        Assert.False(actual.IsValid);
        Assert.Contains(actual.Errors, error => 
            error.PropertyName == nameof(CreateUomDto.Description) &&
            error.ErrorMessage == "Description must not be only whitespace.");
    }
    
    [Fact]
    public void Description_ShouldRejectNonLatin1()
    {
        CreateUomDto uom = CreateValidUom();
        uom.Description = "Москва";

        ValidationResult? actual = _validator.Validate(uom);

        Assert.False(actual.IsValid);
        Assert.Contains(actual.Errors, error =>
            error.PropertyName == nameof(CreateUomDto.Description) &&
            error.ErrorMessage == "Invalid Description.");
    }
    
    private static CreateUomDto CreateValidUom()
    {
        return new CreateUomDto
        {
            Name = "Test UOM",
            Code = "u",
            Description = "A valid description."
        };
    }
}