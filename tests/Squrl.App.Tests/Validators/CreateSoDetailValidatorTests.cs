using FluentValidation.Results;
using Squrl.App.Features.SalesOrderDetails.DTOs;
using Squrl.App.Features.SalesOrderDetails.Validators;

namespace Squrl.App.Tests.Validators;

public class CreateSoDetailValidatorTests
{
    private readonly CreateSoDetailValidator _validator;

    public CreateSoDetailValidatorTests()
    {
        _validator = new CreateSoDetailValidator();
    }
    
    [Fact]
    public void ValidSoDetail_ShouldPass()
    {
        CreateSoDetailDto soDetail = CreateValidSoDetail();

        ValidationResult? actual = _validator.Validate(soDetail);

        Assert.True(actual.IsValid);
        Assert.Empty(actual.Errors);
    }
    
    [Fact]
    public void SalesOrderId_ShouldBeRequired()
    {
        CreateSoDetailDto soDetail = CreateValidSoDetail();
        soDetail.SalesOrderId = null;
        
        ValidationResult? actual = _validator.Validate(soDetail);

        Assert.False(actual.IsValid);
        Assert.Contains(actual.Errors, error =>
            error.PropertyName == nameof(CreateSoDetailDto.SalesOrderId) &&
            error.ErrorMessage == "Sales Order is required.");
    }
    
    [Fact]
    public void LineSequence_ShouldBeRequired()
    {
        CreateSoDetailDto soDetail = CreateValidSoDetail();
        soDetail.LineSequence = null;

        ValidationResult? actual = _validator.Validate(soDetail);

        Assert.False(actual.IsValid);
        Assert.Contains(actual.Errors, error =>
            error.PropertyName == nameof(CreateSoDetailDto.LineSequence) &&
            error.ErrorMessage == "Line Sequence is required.");
    }
    
    [Theory]
    [InlineData(-5)]
    [InlineData(-1)]
    public void LineSequence_ShouldRejectNegativeValues(int sequence)
    {
        CreateSoDetailDto soDetail = CreateValidSoDetail();
        soDetail.LineSequence = sequence;

        ValidationResult? actual = _validator.Validate(soDetail);

        Assert.False(actual.IsValid);
        Assert.Contains(actual.Errors, error =>
            error.PropertyName == nameof(CreateSoDetailDto.LineSequence) &&
            error.ErrorMessage == "Line Sequence must start at 1.");
    }
    
    [Fact]
    public void LineSequence_ShouldRejectZero()
    {
        CreateSoDetailDto soDetail = CreateValidSoDetail();
        soDetail.LineSequence = 0;

        ValidationResult? actual = _validator.Validate(soDetail);

        Assert.False(actual.IsValid);
        Assert.Contains(actual.Errors, error =>
            error.PropertyName == nameof(CreateSoDetailDto.LineSequence) &&
            error.ErrorMessage == "Line Sequence must start at 1.");
    }
    
    [Fact]
    public void LineSequence_ShouldAllowIntMaxValue()
    {
        CreateSoDetailDto soDetail = CreateValidSoDetail();
        soDetail.LineSequence = int.MaxValue;

        ValidationResult? actual = _validator.Validate(soDetail);

        Assert.True(actual.IsValid);
        Assert.DoesNotContain(actual.Errors, error =>
            error.PropertyName == nameof(CreateSoDetailDto.LineSequence));
    }
    
    [Fact]
    public void LineSequence_ShouldRejectIntMinValue()
    {
        CreateSoDetailDto soDetail = CreateValidSoDetail();
        soDetail.LineSequence = int.MinValue;

        ValidationResult? actual = _validator.Validate(soDetail);

        Assert.False(actual.IsValid);
        Assert.Contains(actual.Errors, error =>
            error.PropertyName == nameof(CreateSoDetailDto.LineSequence) &&
            error.ErrorMessage == "Line Sequence must start at 1.");
    }
    
    [Fact]
    public void ItemId_ShouldBeRequired()
    {
        CreateSoDetailDto soDetail = CreateValidSoDetail();
        soDetail.ItemId = null;
        
        ValidationResult? actual = _validator.Validate(soDetail);

        Assert.False(actual.IsValid);
        Assert.Contains(actual.Errors, error =>
            error.PropertyName == nameof(CreateSoDetailDto.ItemId) &&
            error.ErrorMessage == "Item is required.");
    }
    
    [Fact]
    public void UnitPrice_ShouldBeRequired()
    {
        CreateSoDetailDto soDetail = CreateValidSoDetail();
        soDetail.UnitPrice = null;

        ValidationResult? actual = _validator.Validate(soDetail);

        Assert.False(actual.IsValid);
        Assert.Contains(actual.Errors, error =>
            error.PropertyName == nameof(CreateSoDetailDto.UnitPrice) &&
            error.ErrorMessage == "Unit Price is required.");
    }
    
    [Theory]
    [InlineData(-5)]
    [InlineData(-0.01)]
    public void UnitPrice_ShouldRejectNegativeValues(decimal price)
    {
        CreateSoDetailDto soDetail = CreateValidSoDetail();
        soDetail.UnitPrice = price;

        ValidationResult? actual = _validator.Validate(soDetail);

        Assert.False(actual.IsValid);
        Assert.Contains(actual.Errors, error =>
            error.PropertyName == nameof(CreateSoDetailDto.UnitPrice) &&
            error.ErrorMessage == "Unit Price must not be negative.");
    }
    
    [Fact]
    public void RetailPrice_ShouldAllowZero()
    {
        CreateSoDetailDto soDetail = CreateValidSoDetail();
        soDetail.UnitPrice = 0m;

        ValidationResult? actual = _validator.Validate(soDetail);

        Assert.True(actual.IsValid);
        Assert.DoesNotContain(actual.Errors, error =>
            error.PropertyName == nameof(CreateSoDetailDto.UnitPrice));
    }
    
    [Fact]
    public void UnitPrice_ShouldAllowDecimalMaxValue()
    {
        CreateSoDetailDto soDetail = CreateValidSoDetail();
        soDetail.UnitPrice = decimal.MaxValue;

        ValidationResult? actual = _validator.Validate(soDetail);

        Assert.True(actual.IsValid);
        Assert.DoesNotContain(actual.Errors, error =>
            error.PropertyName == nameof(CreateSoDetailDto.UnitPrice));
    }
    
    [Fact]
    public void UnitPrice_ShouldRejectDecimalMinValue()
    {
        CreateSoDetailDto soDetail = CreateValidSoDetail();
        soDetail.UnitPrice = decimal.MinValue;

        ValidationResult? actual = _validator.Validate(soDetail);

        Assert.False(actual.IsValid);
        Assert.Contains(actual.Errors, error =>
            error.PropertyName == nameof(CreateSoDetailDto.UnitPrice) &&
            error.ErrorMessage == "Unit Price must not be negative.");
    }
    
    [Fact]
    public void Quantity_ShouldBeRequired()
    {
        CreateSoDetailDto soDetail = CreateValidSoDetail();
        soDetail.Quantity = null;

        ValidationResult? actual = _validator.Validate(soDetail);

        Assert.False(actual.IsValid);
        Assert.Contains(actual.Errors, error =>
            error.PropertyName == nameof(CreateSoDetailDto.Quantity) &&
            error.ErrorMessage == "Quantity is required.");
    }
    
    [Theory]
    [InlineData(-5)]
    [InlineData(-0.01)]
    public void Quantity_ShouldRejectNegativeValues(decimal quantity)
    {
        CreateSoDetailDto soDetail = CreateValidSoDetail();
        soDetail.Quantity = quantity;

        ValidationResult? actual = _validator.Validate(soDetail);

        Assert.False(actual.IsValid);
        Assert.Contains(actual.Errors, error =>
            error.PropertyName == nameof(CreateSoDetailDto.Quantity) &&
            error.ErrorMessage == "Quantity must not be zero or negative.");
    }
    
    [Fact]
    public void Quantity_ShouldRejectZero()
    {
        CreateSoDetailDto soDetail = CreateValidSoDetail();
        soDetail.Quantity = 0m;

        ValidationResult? actual = _validator.Validate(soDetail);

        Assert.False(actual.IsValid);
        Assert.Contains(actual.Errors, error =>
            error.PropertyName == nameof(CreateSoDetailDto.Quantity) &&
            error.ErrorMessage == "Quantity must not be zero or negative.");
    }
    
    [Fact]
    public void Quantity_ShouldAllowDecimalMaxValue()
    {
        CreateSoDetailDto soDetail = CreateValidSoDetail();
        soDetail.Quantity = decimal.MaxValue;

        ValidationResult? actual = _validator.Validate(soDetail);

        Assert.True(actual.IsValid);
        Assert.DoesNotContain(actual.Errors, error =>
            error.PropertyName == nameof(CreateSoDetailDto.Quantity));
    }
    
    [Fact]
    public void Quantity_ShouldRejectDecimalMinValue()
    {
        CreateSoDetailDto soDetail = CreateValidSoDetail();
        soDetail.Quantity = decimal.MinValue;

        ValidationResult? actual = _validator.Validate(soDetail);

        Assert.False(actual.IsValid);
        Assert.Contains(actual.Errors, error =>
            error.PropertyName == nameof(CreateSoDetailDto.Quantity) &&
            error.ErrorMessage == "Quantity must not be zero or negative.");
    }
    
    private static CreateSoDetailDto CreateValidSoDetail()
    {
        return new CreateSoDetailDto
        {
            SalesOrderId = Guid.NewGuid(),
            LineSequence = 1,
            ItemId = Guid.NewGuid(),
            UnitPrice = 600m,
            Quantity = 10.05m
        };
    }
}