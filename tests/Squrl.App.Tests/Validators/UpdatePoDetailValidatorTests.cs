using FluentValidation.Results;
using Squrl.App.Features.PurchaseOrderDetails.DTOs;
using Squrl.App.Features.PurchaseOrderDetails.Validators;

namespace Squrl.App.Tests.Validators;

public class UpdatePoDetailValidatorTests
{
    private readonly UpdatePoDetailValidator _validator;

    public UpdatePoDetailValidatorTests()
    {
        _validator = new UpdatePoDetailValidator();
    }
    
    [Fact]
    public void ValidPoDetail_ShouldPass()
    {
        UpdatePoDetailDto poDetail = CreateValidPoDetail();

        ValidationResult? actual = _validator.Validate(poDetail);

        Assert.True(actual.IsValid);
        Assert.Empty(actual.Errors);
    }
    
    [Fact]
    public void PurchaseOrderId_ShouldBeRequired()
    {
        UpdatePoDetailDto poDetail = CreateValidPoDetail();
        poDetail.PurchaseOrderId = null;
        
        ValidationResult? actual = _validator.Validate(poDetail);

        Assert.False(actual.IsValid);
        Assert.Contains(actual.Errors, error =>
            error.PropertyName == nameof(UpdatePoDetailDto.PurchaseOrderId) &&
            error.ErrorMessage == "Purchase Order is required.");
    }
    
    [Fact]
    public void LineSequence_ShouldBeRequired()
    {
        UpdatePoDetailDto poDetail = CreateValidPoDetail();
        poDetail.LineSequence = null;

        ValidationResult? actual = _validator.Validate(poDetail);

        Assert.False(actual.IsValid);
        Assert.Contains(actual.Errors, error =>
            error.PropertyName == nameof(UpdatePoDetailDto.LineSequence) &&
            error.ErrorMessage == "Line Sequence is required.");
    }
    
    [Theory]
    [InlineData(-5)]
    [InlineData(-1)]
    public void LineSequence_ShouldRejectNegativeValues(int sequence)
    {
        UpdatePoDetailDto poDetail = CreateValidPoDetail();
        poDetail.LineSequence = sequence;

        ValidationResult? actual = _validator.Validate(poDetail);

        Assert.False(actual.IsValid);
        Assert.Contains(actual.Errors, error =>
            error.PropertyName == nameof(UpdatePoDetailDto.LineSequence) &&
            error.ErrorMessage == "Line Sequence must start at 1.");
    }
    
    [Fact]
    public void LineSequence_ShouldRejectZero()
    {
        UpdatePoDetailDto poDetail = CreateValidPoDetail();
        poDetail.LineSequence = 0;

        ValidationResult? actual = _validator.Validate(poDetail);

        Assert.False(actual.IsValid);
        Assert.Contains(actual.Errors, error =>
            error.PropertyName == nameof(UpdatePoDetailDto.LineSequence) &&
            error.ErrorMessage == "Line Sequence must start at 1.");
    }
    
    [Fact]
    public void LineSequence_ShouldAllowIntMaxValue()
    {
        UpdatePoDetailDto poDetail = CreateValidPoDetail();
        poDetail.LineSequence = int.MaxValue;

        ValidationResult? actual = _validator.Validate(poDetail);

        Assert.True(actual.IsValid);
        Assert.DoesNotContain(actual.Errors, error =>
            error.PropertyName == nameof(UpdatePoDetailDto.LineSequence));
    }
    
    [Fact]
    public void LineSequence_ShouldRejectIntMinValue()
    {
        UpdatePoDetailDto poDetail = CreateValidPoDetail();
        poDetail.LineSequence = int.MinValue;

        ValidationResult? actual = _validator.Validate(poDetail);

        Assert.False(actual.IsValid);
        Assert.Contains(actual.Errors, error =>
            error.PropertyName == nameof(UpdatePoDetailDto.LineSequence) &&
            error.ErrorMessage == "Line Sequence must start at 1.");
    }
    
    [Fact]
    public void ItemId_ShouldBeRequired()
    {
        UpdatePoDetailDto poDetail = CreateValidPoDetail();
        poDetail.ItemId = null;
        
        ValidationResult? actual = _validator.Validate(poDetail);

        Assert.False(actual.IsValid);
        Assert.Contains(actual.Errors, error =>
            error.PropertyName == nameof(UpdatePoDetailDto.ItemId) &&
            error.ErrorMessage == "Item is required.");
    }
    
    [Fact]
    public void UnitPrice_ShouldBeRequired()
    {
        UpdatePoDetailDto poDetail = CreateValidPoDetail();
        poDetail.UnitPrice = null;

        ValidationResult? actual = _validator.Validate(poDetail);

        Assert.False(actual.IsValid);
        Assert.Contains(actual.Errors, error =>
            error.PropertyName == nameof(UpdatePoDetailDto.UnitPrice) &&
            error.ErrorMessage == "Unit Price is required.");
    }
    
    [Theory]
    [InlineData(-5)]
    [InlineData(-0.01)]
    public void UnitPrice_ShouldRejectNegativeValues(decimal price)
    {
        UpdatePoDetailDto poDetail = CreateValidPoDetail();
        poDetail.UnitPrice = price;

        ValidationResult? actual = _validator.Validate(poDetail);

        Assert.False(actual.IsValid);
        Assert.Contains(actual.Errors, error =>
            error.PropertyName == nameof(UpdatePoDetailDto.UnitPrice) &&
            error.ErrorMessage == "Unit Price must not be negative.");
    }
    
    [Fact]
    public void RetailPrice_ShouldAllowZero()
    {
        UpdatePoDetailDto poDetail = CreateValidPoDetail();
        poDetail.UnitPrice = 0m;

        ValidationResult? actual = _validator.Validate(poDetail);

        Assert.True(actual.IsValid);
        Assert.DoesNotContain(actual.Errors, error =>
            error.PropertyName == nameof(UpdatePoDetailDto.UnitPrice));
    }
    
    [Fact]
    public void UnitPrice_ShouldAllowDecimalMaxValue()
    {
        UpdatePoDetailDto poDetail = CreateValidPoDetail();
        poDetail.UnitPrice = decimal.MaxValue;

        ValidationResult? actual = _validator.Validate(poDetail);

        Assert.True(actual.IsValid);
        Assert.DoesNotContain(actual.Errors, error =>
            error.PropertyName == nameof(UpdatePoDetailDto.UnitPrice));
    }
    
    [Fact]
    public void UnitPrice_ShouldRejectDecimalMinValue()
    {
        UpdatePoDetailDto poDetail = CreateValidPoDetail();
        poDetail.UnitPrice = decimal.MinValue;

        ValidationResult? actual = _validator.Validate(poDetail);

        Assert.False(actual.IsValid);
        Assert.Contains(actual.Errors, error =>
            error.PropertyName == nameof(UpdatePoDetailDto.UnitPrice) &&
            error.ErrorMessage == "Unit Price must not be negative.");
    }
    
    [Fact]
    public void Quantity_ShouldBeRequired()
    {
        UpdatePoDetailDto poDetail = CreateValidPoDetail();
        poDetail.Quantity = null;

        ValidationResult? actual = _validator.Validate(poDetail);

        Assert.False(actual.IsValid);
        Assert.Contains(actual.Errors, error =>
            error.PropertyName == nameof(UpdatePoDetailDto.Quantity) &&
            error.ErrorMessage == "Quantity is required.");
    }
    
    [Theory]
    [InlineData(-5)]
    [InlineData(-0.01)]
    public void Quantity_ShouldRejectNegativeValues(decimal quantity)
    {
        UpdatePoDetailDto poDetail = CreateValidPoDetail();
        poDetail.Quantity = quantity;

        ValidationResult? actual = _validator.Validate(poDetail);

        Assert.False(actual.IsValid);
        Assert.Contains(actual.Errors, error =>
            error.PropertyName == nameof(UpdatePoDetailDto.Quantity) &&
            error.ErrorMessage == "Quantity must not be zero or negative.");
    }
    
    [Fact]
    public void Quantity_ShouldRejectZero()
    {
        UpdatePoDetailDto poDetail = CreateValidPoDetail();
        poDetail.Quantity = 0m;

        ValidationResult? actual = _validator.Validate(poDetail);

        Assert.False(actual.IsValid);
        Assert.Contains(actual.Errors, error =>
            error.PropertyName == nameof(UpdatePoDetailDto.Quantity) &&
            error.ErrorMessage == "Quantity must not be zero or negative.");
    }
    
    [Fact]
    public void Quantity_ShouldAllowDecimalMaxValue()
    {
        UpdatePoDetailDto poDetail = CreateValidPoDetail();
        poDetail.Quantity = decimal.MaxValue;

        ValidationResult? actual = _validator.Validate(poDetail);

        Assert.True(actual.IsValid);
        Assert.DoesNotContain(actual.Errors, error =>
            error.PropertyName == nameof(UpdatePoDetailDto.Quantity));
    }
    
    [Fact]
    public void Quantity_ShouldRejectDecimalMinValue()
    {
        UpdatePoDetailDto poDetail = CreateValidPoDetail();
        poDetail.Quantity = decimal.MinValue;

        ValidationResult? actual = _validator.Validate(poDetail);

        Assert.False(actual.IsValid);
        Assert.Contains(actual.Errors, error =>
            error.PropertyName == nameof(UpdatePoDetailDto.Quantity) &&
            error.ErrorMessage == "Quantity must not be zero or negative.");
    }
    
    private static UpdatePoDetailDto CreateValidPoDetail()
    {
        return new UpdatePoDetailDto
        {
            PurchaseOrderId = Guid.NewGuid(),
            LineSequence = 1,
            ItemId = Guid.NewGuid(),
            UnitPrice = 600m,
            Quantity = 10.05m
        };
    }
}