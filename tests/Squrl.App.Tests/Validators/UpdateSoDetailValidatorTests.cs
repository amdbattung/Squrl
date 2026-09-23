using FluentValidation.Results;
using Squrl.App.Features.SalesOrderDetails.DTOs;
using Squrl.App.Features.SalesOrderDetails.Validators;

namespace Squrl.App.Tests.Validators;

public class UpdateSoDetailValidatorTests
{
    private readonly UpdateSoDetailValidator _validator;

    public UpdateSoDetailValidatorTests()
    {
        _validator = new UpdateSoDetailValidator();
    }
    
    [Fact]
    public void ValidSoDetail_ShouldPass()
    {
        UpdateSoDetailDto soDetail = CreateValidSoDetail();

        ValidationResult? actual = _validator.Validate(soDetail);

        Assert.True(actual.IsValid);
        Assert.Empty(actual.Errors);
    }
    
    [Fact]
    public void SalesOrderId_ShouldBeRequired()
    {
        UpdateSoDetailDto soDetail = CreateValidSoDetail();
        soDetail.SalesOrderId = null;
        
        ValidationResult? actual = _validator.Validate(soDetail);

        Assert.False(actual.IsValid);
        Assert.Contains(actual.Errors, error =>
            error.PropertyName == nameof(UpdateSoDetailDto.SalesOrderId) &&
            error.ErrorMessage == "Sales Order is required.");
    }
    
    [Fact]
    public void LineSequence_ShouldBeRequired()
    {
        UpdateSoDetailDto soDetail = CreateValidSoDetail();
        soDetail.LineSequence = null;

        ValidationResult? actual = _validator.Validate(soDetail);

        Assert.False(actual.IsValid);
        Assert.Contains(actual.Errors, error =>
            error.PropertyName == nameof(UpdateSoDetailDto.LineSequence) &&
            error.ErrorMessage == "Line Sequence is required.");
    }
    
    [Theory]
    [InlineData(-5)]
    [InlineData(-1)]
    public void LineSequence_ShouldRejectNegativeValues(int sequence)
    {
        UpdateSoDetailDto soDetail = CreateValidSoDetail();
        soDetail.LineSequence = sequence;

        ValidationResult? actual = _validator.Validate(soDetail);

        Assert.False(actual.IsValid);
        Assert.Contains(actual.Errors, error =>
            error.PropertyName == nameof(UpdateSoDetailDto.LineSequence) &&
            error.ErrorMessage == "Line Sequence must start at 1.");
    }
    
    [Fact]
    public void LineSequence_ShouldRejectZero()
    {
        UpdateSoDetailDto soDetail = CreateValidSoDetail();
        soDetail.LineSequence = 0;

        ValidationResult? actual = _validator.Validate(soDetail);

        Assert.False(actual.IsValid);
        Assert.Contains(actual.Errors, error =>
            error.PropertyName == nameof(UpdateSoDetailDto.LineSequence) &&
            error.ErrorMessage == "Line Sequence must start at 1.");
    }
    
    [Fact]
    public void LineSequence_ShouldAllowIntMaxValue()
    {
        UpdateSoDetailDto soDetail = CreateValidSoDetail();
        soDetail.LineSequence = int.MaxValue;

        ValidationResult? actual = _validator.Validate(soDetail);

        Assert.True(actual.IsValid);
        Assert.DoesNotContain(actual.Errors, error =>
            error.PropertyName == nameof(UpdateSoDetailDto.LineSequence));
    }
    
    [Fact]
    public void LineSequence_ShouldRejectIntMinValue()
    {
        UpdateSoDetailDto soDetail = CreateValidSoDetail();
        soDetail.LineSequence = int.MinValue;

        ValidationResult? actual = _validator.Validate(soDetail);

        Assert.False(actual.IsValid);
        Assert.Contains(actual.Errors, error =>
            error.PropertyName == nameof(UpdateSoDetailDto.LineSequence) &&
            error.ErrorMessage == "Line Sequence must start at 1.");
    }
    
    [Fact]
    public void ItemId_ShouldBeRequired()
    {
        UpdateSoDetailDto soDetail = CreateValidSoDetail();
        soDetail.ItemId = null;
        
        ValidationResult? actual = _validator.Validate(soDetail);

        Assert.False(actual.IsValid);
        Assert.Contains(actual.Errors, error =>
            error.PropertyName == nameof(UpdateSoDetailDto.ItemId) &&
            error.ErrorMessage == "Item is required.");
    }
    
    [Fact]
    public void UnitPrice_ShouldBeRequired()
    {
        UpdateSoDetailDto soDetail = CreateValidSoDetail();
        soDetail.UnitPrice = null;

        ValidationResult? actual = _validator.Validate(soDetail);

        Assert.False(actual.IsValid);
        Assert.Contains(actual.Errors, error =>
            error.PropertyName == nameof(UpdateSoDetailDto.UnitPrice) &&
            error.ErrorMessage == "Unit Price is required.");
    }
    
    [Theory]
    [InlineData(-5)]
    [InlineData(-0.01)]
    public void UnitPrice_ShouldRejectNegativeValues(decimal price)
    {
        UpdateSoDetailDto soDetail = CreateValidSoDetail();
        soDetail.UnitPrice = price;

        ValidationResult? actual = _validator.Validate(soDetail);

        Assert.False(actual.IsValid);
        Assert.Contains(actual.Errors, error =>
            error.PropertyName == nameof(UpdateSoDetailDto.UnitPrice) &&
            error.ErrorMessage == "Unit Price must not be negative.");
    }
    
    [Fact]
    public void RetailPrice_ShouldAllowZero()
    {
        UpdateSoDetailDto soDetail = CreateValidSoDetail();
        soDetail.UnitPrice = 0m;

        ValidationResult? actual = _validator.Validate(soDetail);

        Assert.True(actual.IsValid);
        Assert.DoesNotContain(actual.Errors, error =>
            error.PropertyName == nameof(UpdateSoDetailDto.UnitPrice));
    }
    
    [Fact]
    public void UnitPrice_ShouldAllowDecimalMaxValue()
    {
        UpdateSoDetailDto soDetail = CreateValidSoDetail();
        soDetail.UnitPrice = decimal.MaxValue;

        ValidationResult? actual = _validator.Validate(soDetail);

        Assert.True(actual.IsValid);
        Assert.DoesNotContain(actual.Errors, error =>
            error.PropertyName == nameof(UpdateSoDetailDto.UnitPrice));
    }
    
    [Fact]
    public void UnitPrice_ShouldRejectDecimalMinValue()
    {
        UpdateSoDetailDto soDetail = CreateValidSoDetail();
        soDetail.UnitPrice = decimal.MinValue;

        ValidationResult? actual = _validator.Validate(soDetail);

        Assert.False(actual.IsValid);
        Assert.Contains(actual.Errors, error =>
            error.PropertyName == nameof(UpdateSoDetailDto.UnitPrice) &&
            error.ErrorMessage == "Unit Price must not be negative.");
    }
    
    [Fact]
    public void Quantity_ShouldBeRequired()
    {
        UpdateSoDetailDto soDetail = CreateValidSoDetail();
        soDetail.Quantity = null;

        ValidationResult? actual = _validator.Validate(soDetail);

        Assert.False(actual.IsValid);
        Assert.Contains(actual.Errors, error =>
            error.PropertyName == nameof(UpdateSoDetailDto.Quantity) &&
            error.ErrorMessage == "Quantity is required.");
    }
    
    [Theory]
    [InlineData(-5)]
    [InlineData(-0.01)]
    public void Quantity_ShouldRejectNegativeValues(decimal quantity)
    {
        UpdateSoDetailDto soDetail = CreateValidSoDetail();
        soDetail.Quantity = quantity;

        ValidationResult? actual = _validator.Validate(soDetail);

        Assert.False(actual.IsValid);
        Assert.Contains(actual.Errors, error =>
            error.PropertyName == nameof(UpdateSoDetailDto.Quantity) &&
            error.ErrorMessage == "Quantity must not be zero or negative.");
    }
    
    [Fact]
    public void Quantity_ShouldRejectZero()
    {
        UpdateSoDetailDto soDetail = CreateValidSoDetail();
        soDetail.Quantity = 0m;

        ValidationResult? actual = _validator.Validate(soDetail);

        Assert.False(actual.IsValid);
        Assert.Contains(actual.Errors, error =>
            error.PropertyName == nameof(UpdateSoDetailDto.Quantity) &&
            error.ErrorMessage == "Quantity must not be zero or negative.");
    }
    
    [Fact]
    public void Quantity_ShouldAllowDecimalMaxValue()
    {
        UpdateSoDetailDto soDetail = CreateValidSoDetail();
        soDetail.Quantity = decimal.MaxValue;

        ValidationResult? actual = _validator.Validate(soDetail);

        Assert.True(actual.IsValid);
        Assert.DoesNotContain(actual.Errors, error =>
            error.PropertyName == nameof(UpdateSoDetailDto.Quantity));
    }
    
    [Fact]
    public void Quantity_ShouldRejectDecimalMinValue()
    {
        UpdateSoDetailDto soDetail = CreateValidSoDetail();
        soDetail.Quantity = decimal.MinValue;

        ValidationResult? actual = _validator.Validate(soDetail);

        Assert.False(actual.IsValid);
        Assert.Contains(actual.Errors, error =>
            error.PropertyName == nameof(UpdateSoDetailDto.Quantity) &&
            error.ErrorMessage == "Quantity must not be zero or negative.");
    }
    
    private static UpdateSoDetailDto CreateValidSoDetail()
    {
        return new UpdateSoDetailDto
        {
            SalesOrderId = Guid.NewGuid(),
            LineSequence = 1,
            ItemId = Guid.NewGuid(),
            UnitPrice = 600m,
            Quantity = 10.05m
        };
    }
}