using FluentValidation.Results;
using Squrl.App.Features.PurchaseOrderDetails.DTOs;
using Squrl.App.Features.PurchaseOrderDetails.Validators;

namespace Squrl.App.Tests.Validators;

public class CreatePoDetailValidatorTests
{
    private readonly CreatePoDetailValidator _validator;

    public CreatePoDetailValidatorTests()
    {
        _validator = new CreatePoDetailValidator();
    }
    
    [Fact]
    public void ValidPoDetail_ShouldPass()
    {
        CreatePoDetailDto poDetail = CreateValidPoDetail();

        ValidationResult? actual = _validator.Validate(poDetail);

        Assert.True(actual.IsValid);
        Assert.Empty(actual.Errors);
    }
    
    [Fact]
    public void PurchaseOrderId_ShouldBeRequired()
    {
        CreatePoDetailDto poDetail = CreateValidPoDetail();
        poDetail.PurchaseOrderId = null;
        
        ValidationResult? actual = _validator.Validate(poDetail);

        Assert.False(actual.IsValid);
        Assert.Contains(actual.Errors, error =>
            error.PropertyName == nameof(CreatePoDetailDto.PurchaseOrderId) &&
            error.ErrorMessage == "Purchase Order is required.");
    }
    
    [Fact]
    public void LineSequence_ShouldBeRequired()
    {
        CreatePoDetailDto poDetail = CreateValidPoDetail();
        poDetail.LineSequence = null;

        ValidationResult? actual = _validator.Validate(poDetail);

        Assert.False(actual.IsValid);
        Assert.Contains(actual.Errors, error =>
            error.PropertyName == nameof(CreatePoDetailDto.LineSequence) &&
            error.ErrorMessage == "Line Sequence is required.");
    }
    
    [Theory]
    [InlineData(-5)]
    [InlineData(-1)]
    public void LineSequence_ShouldRejectNegativeValues(int sequence)
    {
        CreatePoDetailDto poDetail = CreateValidPoDetail();
        poDetail.LineSequence = sequence;

        ValidationResult? actual = _validator.Validate(poDetail);

        Assert.False(actual.IsValid);
        Assert.Contains(actual.Errors, error =>
            error.PropertyName == nameof(CreatePoDetailDto.LineSequence) &&
            error.ErrorMessage == "Line Sequence must start at 1.");
    }
    
    [Fact]
    public void LineSequence_ShouldRejectZero()
    {
        CreatePoDetailDto poDetail = CreateValidPoDetail();
        poDetail.LineSequence = 0;

        ValidationResult? actual = _validator.Validate(poDetail);

        Assert.False(actual.IsValid);
        Assert.Contains(actual.Errors, error =>
            error.PropertyName == nameof(CreatePoDetailDto.LineSequence) &&
            error.ErrorMessage == "Line Sequence must start at 1.");
    }
    
    [Fact]
    public void LineSequence_ShouldAllowIntMaxValue()
    {
        CreatePoDetailDto poDetail = CreateValidPoDetail();
        poDetail.LineSequence = int.MaxValue;

        ValidationResult? actual = _validator.Validate(poDetail);

        Assert.True(actual.IsValid);
        Assert.DoesNotContain(actual.Errors, error =>
            error.PropertyName == nameof(CreatePoDetailDto.LineSequence));
    }
    
    [Fact]
    public void LineSequence_ShouldRejectIntMinValue()
    {
        CreatePoDetailDto poDetail = CreateValidPoDetail();
        poDetail.LineSequence = int.MinValue;

        ValidationResult? actual = _validator.Validate(poDetail);

        Assert.False(actual.IsValid);
        Assert.Contains(actual.Errors, error =>
            error.PropertyName == nameof(CreatePoDetailDto.LineSequence) &&
            error.ErrorMessage == "Line Sequence must start at 1.");
    }
    
    [Fact]
    public void ItemId_ShouldBeRequired()
    {
        CreatePoDetailDto poDetail = CreateValidPoDetail();
        poDetail.ItemId = null;
        
        ValidationResult? actual = _validator.Validate(poDetail);

        Assert.False(actual.IsValid);
        Assert.Contains(actual.Errors, error =>
            error.PropertyName == nameof(CreatePoDetailDto.ItemId) &&
            error.ErrorMessage == "Item is required.");
    }
    
    [Fact]
    public void UnitPrice_ShouldBeRequired()
    {
        CreatePoDetailDto poDetail = CreateValidPoDetail();
        poDetail.UnitPrice = null;

        ValidationResult? actual = _validator.Validate(poDetail);

        Assert.False(actual.IsValid);
        Assert.Contains(actual.Errors, error =>
            error.PropertyName == nameof(CreatePoDetailDto.UnitPrice) &&
            error.ErrorMessage == "Unit Price is required.");
    }
    
    [Theory]
    [InlineData(-5)]
    [InlineData(-0.01)]
    public void UnitPrice_ShouldRejectNegativeValues(decimal price)
    {
        CreatePoDetailDto poDetail = CreateValidPoDetail();
        poDetail.UnitPrice = price;

        ValidationResult? actual = _validator.Validate(poDetail);

        Assert.False(actual.IsValid);
        Assert.Contains(actual.Errors, error =>
            error.PropertyName == nameof(CreatePoDetailDto.UnitPrice) &&
            error.ErrorMessage == "Unit Price must not be negative.");
    }
    
    [Fact]
    public void RetailPrice_ShouldAllowZero()
    {
        CreatePoDetailDto poDetail = CreateValidPoDetail();
        poDetail.UnitPrice = 0m;

        ValidationResult? actual = _validator.Validate(poDetail);

        Assert.True(actual.IsValid);
        Assert.DoesNotContain(actual.Errors, error =>
            error.PropertyName == nameof(CreatePoDetailDto.UnitPrice));
    }
    
    [Fact]
    public void UnitPrice_ShouldAllowDecimalMaxValue()
    {
        CreatePoDetailDto poDetail = CreateValidPoDetail();
        poDetail.UnitPrice = decimal.MaxValue;

        ValidationResult? actual = _validator.Validate(poDetail);

        Assert.True(actual.IsValid);
        Assert.DoesNotContain(actual.Errors, error =>
            error.PropertyName == nameof(CreatePoDetailDto.UnitPrice));
    }
    
    [Fact]
    public void UnitPrice_ShouldRejectDecimalMinValue()
    {
        CreatePoDetailDto poDetail = CreateValidPoDetail();
        poDetail.UnitPrice = decimal.MinValue;

        ValidationResult? actual = _validator.Validate(poDetail);

        Assert.False(actual.IsValid);
        Assert.Contains(actual.Errors, error =>
            error.PropertyName == nameof(CreatePoDetailDto.UnitPrice) &&
            error.ErrorMessage == "Unit Price must not be negative.");
    }
    
    [Fact]
    public void Quantity_ShouldBeRequired()
    {
        CreatePoDetailDto poDetail = CreateValidPoDetail();
        poDetail.Quantity = null;

        ValidationResult? actual = _validator.Validate(poDetail);

        Assert.False(actual.IsValid);
        Assert.Contains(actual.Errors, error =>
            error.PropertyName == nameof(CreatePoDetailDto.Quantity) &&
            error.ErrorMessage == "Quantity is required.");
    }
    
    [Theory]
    [InlineData(-5)]
    [InlineData(-0.01)]
    public void Quantity_ShouldRejectNegativeValues(decimal quantity)
    {
        CreatePoDetailDto poDetail = CreateValidPoDetail();
        poDetail.Quantity = quantity;

        ValidationResult? actual = _validator.Validate(poDetail);

        Assert.False(actual.IsValid);
        Assert.Contains(actual.Errors, error =>
            error.PropertyName == nameof(CreatePoDetailDto.Quantity) &&
            error.ErrorMessage == "Quantity must not be zero or negative.");
    }
    
    [Fact]
    public void Quantity_ShouldRejectZero()
    {
        CreatePoDetailDto poDetail = CreateValidPoDetail();
        poDetail.Quantity = 0m;

        ValidationResult? actual = _validator.Validate(poDetail);

        Assert.False(actual.IsValid);
        Assert.Contains(actual.Errors, error =>
            error.PropertyName == nameof(CreatePoDetailDto.Quantity) &&
            error.ErrorMessage == "Quantity must not be zero or negative.");
    }
    
    [Fact]
    public void Quantity_ShouldAllowDecimalMaxValue()
    {
        CreatePoDetailDto poDetail = CreateValidPoDetail();
        poDetail.Quantity = decimal.MaxValue;

        ValidationResult? actual = _validator.Validate(poDetail);

        Assert.True(actual.IsValid);
        Assert.DoesNotContain(actual.Errors, error =>
            error.PropertyName == nameof(CreatePoDetailDto.Quantity));
    }
    
    [Fact]
    public void Quantity_ShouldRejectDecimalMinValue()
    {
        CreatePoDetailDto poDetail = CreateValidPoDetail();
        poDetail.Quantity = decimal.MinValue;

        ValidationResult? actual = _validator.Validate(poDetail);

        Assert.False(actual.IsValid);
        Assert.Contains(actual.Errors, error =>
            error.PropertyName == nameof(CreatePoDetailDto.Quantity) &&
            error.ErrorMessage == "Quantity must not be zero or negative.");
    }
    
    private static CreatePoDetailDto CreateValidPoDetail()
    {
        return new CreatePoDetailDto
        {
            PurchaseOrderId = Guid.NewGuid(),
            LineSequence = 1,
            ItemId = Guid.NewGuid(),
            UnitPrice = 600m,
            Quantity = 10.05m
        };
    }
}