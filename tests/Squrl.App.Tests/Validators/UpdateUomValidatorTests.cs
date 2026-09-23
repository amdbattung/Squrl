using FluentValidation.Results;
using Squrl.App.Features.UnitOfMeasures.DTOs;
using Squrl.App.Features.UnitOfMeasures.Validators;

namespace Squrl.App.Tests.Validators;

public class UpdateUomValidatorTests
{
    private readonly UpdateUomValidator _validator;

    public UpdateUomValidatorTests()
    {
        _validator = new UpdateUomValidator();
    }
    
    [Fact]
    public void ValidUom_ShouldPass()
    {
        UpdateUomDto uom = CreateValidUom();

        ValidationResult? actual = _validator.Validate(uom);

        Assert.True(actual.IsValid);
        Assert.Empty(actual.Errors);
    }
    
    [Fact]
    public void Name_ShouldBeRequired()
    {
        UpdateUomDto uom = CreateValidUom();
        uom.Name = "";
        
        ValidationResult? actual = _validator.Validate(uom);

        Assert.False(actual.IsValid);
        Assert.Contains(actual.Errors, error =>
            error.PropertyName == nameof(UpdateUomDto.Name) &&
            error.ErrorMessage == "Name is required.");
    }
    
    [Fact]
    public void Code_ShouldBeRequired()
    {
        UpdateUomDto uom = CreateValidUom();
        uom.Code = "";
        
        ValidationResult? actual = _validator.Validate(uom);

        Assert.False(actual.IsValid);
        Assert.Contains(actual.Errors, error =>
            error.PropertyName == nameof(UpdateUomDto.Code) &&
            error.ErrorMessage == "Code is required.");
    }
    
    [Fact]
    public void Description_ShouldAllowNull()
    {
        UpdateUomDto uom = CreateValidUom();
        uom.Description = null;
        
        ValidationResult? actual = _validator.Validate(uom);
        
        Assert.True(actual.IsValid);
        Assert.DoesNotContain(actual.Errors, error => 
            error.PropertyName == nameof(UpdateUomDto.Description));
    }
    
    [Fact]
    public void Description_ShouldRejectWhitespace()
    {
        UpdateUomDto uom = CreateValidUom();
        uom.Description = "     ";
        
        ValidationResult? actual = _validator.Validate(uom);

        Assert.False(actual.IsValid);
        Assert.Contains(actual.Errors, error => 
            error.PropertyName == nameof(UpdateUomDto.Description) &&
            error.ErrorMessage == "Description must not be only whitespace.");
    }
    
    [Fact]
    public void Description_ShouldRejectNonLatin1()
    {
        UpdateUomDto uom = CreateValidUom();
        uom.Description = "Москва";

        ValidationResult? actual = _validator.Validate(uom);

        Assert.False(actual.IsValid);
        Assert.Contains(actual.Errors, error =>
            error.PropertyName == nameof(UpdateUomDto.Description) &&
            error.ErrorMessage == "Invalid Description.");
    }
    
    private static UpdateUomDto CreateValidUom()
    {
        return new UpdateUomDto
        {
            Name = "Test UOM",
            Code = "u",
            Description = "A valid description."
        };
    }
}