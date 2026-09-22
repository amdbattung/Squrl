using FluentValidation.Results;
using Squrl.App.Features.Items.DTOs;
using Squrl.App.Features.Items.Validators;

namespace Squrl.App.Tests.Validators;

public class UpdateItemValidatorTests
{
    private readonly UpdateItemValidator _validator;
    
    public UpdateItemValidatorTests()
    {
        _validator = new UpdateItemValidator();
    }
    
    [Fact]
    public void ValidItem_ShouldPass()
    {
        UpdateItemDto item = CreateValidItem();

        ValidationResult? actual = _validator.Validate(item);

        Assert.True(actual.IsValid);
        Assert.Empty(actual.Errors);
    }

    [Fact]
    public void Name_ShouldBeRequired()
    {
        UpdateItemDto item = CreateValidItem();
        item.Name = "";
        
        ValidationResult? actual = _validator.Validate(item);

        Assert.False(actual.IsValid);
        Assert.Contains(actual.Errors, error =>
            error.PropertyName == nameof(UpdateItemDto.Name) &&
            error.ErrorMessage == "Name is required.");
    }
    
    [Fact]
    public void UomId_ShouldBeRequired()
    {
        UpdateItemDto item = CreateValidItem();
        item.UomId = null;
        
        ValidationResult? actual = _validator.Validate(item);

        Assert.False(actual.IsValid);
        Assert.Contains(actual.Errors, error =>
            error.PropertyName == nameof(UpdateItemDto.UomId) &&
            error.ErrorMessage == "UOM is required.");
    }
    
    [Fact]
    public void Description_ShouldAllowNull()
    {
        UpdateItemDto item = CreateValidItem();
        item.Description = null;
        
        ValidationResult? actual = _validator.Validate(item);
        
        Assert.True(actual.IsValid);
        Assert.DoesNotContain(actual.Errors, error => 
            error.PropertyName == nameof(UpdateItemDto.Description));
    }
    
    [Fact]
    public void Description_ShouldRejectWhitespace()
    {
        UpdateItemDto item = CreateValidItem();
        item.Description = "     ";
        
        ValidationResult? actual = _validator.Validate(item);

        Assert.False(actual.IsValid);
        Assert.Contains(actual.Errors, error => 
            error.PropertyName == nameof(UpdateItemDto.Description) &&
            error.ErrorMessage == "Description must not be only whitespace.");
    }
    
    [Fact]
    public void Description_ShouldRejectNonLatin1()
    {
        UpdateItemDto item = CreateValidItem();
        item.Description = "Москва";

        ValidationResult? actual = _validator.Validate(item);

        Assert.False(actual.IsValid);
        Assert.Contains(actual.Errors, error =>
            error.PropertyName == nameof(UpdateItemDto.Description) &&
            error.ErrorMessage == "Invalid Description.");
    }
    
    [Fact]
    public void Image_ShouldAllowNull()
    {
        UpdateItemDto item = CreateValidItem();
        item.Image = null;
        
        ValidationResult? actual = _validator.Validate(item);
        
        Assert.True(actual.IsValid);
        Assert.DoesNotContain(actual.Errors, error => 
            error.PropertyName == nameof(UpdateItemDto.Image));
    }
    
    [Fact]
    public void Image_ShouldRejectWhitespace()
    {
        UpdateItemDto item = CreateValidItem();
        item.Image = "     ";
        
        ValidationResult? actual = _validator.Validate(item);

        Assert.False(actual.IsValid);
        Assert.Contains(actual.Errors, error => 
            error.PropertyName == nameof(UpdateItemDto.Image) &&
            error.ErrorMessage == "Image must not be only whitespace.");
    }
    
    [Fact]
    public void Image_ShouldRejectNonAscii()
    {
        UpdateItemDto item = CreateValidItem();
        item.Image = "Café";

        ValidationResult? actual = _validator.Validate(item);

        Assert.False(actual.IsValid);
        Assert.Contains(actual.Errors, error =>
            error.PropertyName == nameof(UpdateItemDto.Image) &&
            error.ErrorMessage == "Invalid Image.");
    }
    
    [Theory]
    [InlineData(-5)]
    [InlineData(-0.01)]
    public void ListPrice_ShouldRejectNegativeValues(decimal price)
    {
        UpdateItemDto item = CreateValidItem();
        item.ListPrice = price;

        ValidationResult? actual = _validator.Validate(item);

        Assert.False(actual.IsValid);
        Assert.Contains(actual.Errors, error =>
            error.PropertyName == nameof(UpdateItemDto.ListPrice) &&
            error.ErrorMessage == "List Price must not be negative.");
    }
    
    [Fact]
    public void ListPrice_ShouldAllowNull()
    {
        UpdateItemDto item = CreateValidItem();
        item.ListPrice = null;

        ValidationResult? actual = _validator.Validate(item);

        Assert.True(actual.IsValid);
        Assert.DoesNotContain(actual.Errors, error =>
            error.PropertyName == nameof(UpdateItemDto.ListPrice));
    }
    
    [Fact]
    public void ListPrice_ShouldAllowZero()
    {
        UpdateItemDto item = CreateValidItem();
        item.ListPrice = 0m;

        ValidationResult? actual = _validator.Validate(item);

        Assert.True(actual.IsValid);
        Assert.DoesNotContain(actual.Errors, error =>
            error.PropertyName == nameof(UpdateItemDto.ListPrice));
    }
    
    [Fact]
    public void ListPrice_ShouldAllowDecimalMaxValue()
    {
        UpdateItemDto item = CreateValidItem();
        item.ListPrice = decimal.MaxValue;

        ValidationResult? actual = _validator.Validate(item);

        Assert.True(actual.IsValid);
        Assert.DoesNotContain(actual.Errors, error =>
            error.PropertyName == nameof(UpdateItemDto.ListPrice));
    }
    
    [Fact]
    public void ListPrice_ShouldRejectDecimalMinValue()
    {
        UpdateItemDto item = CreateValidItem();
        item.ListPrice = decimal.MinValue;

        ValidationResult? actual = _validator.Validate(item);

        Assert.False(actual.IsValid);
        Assert.Contains(actual.Errors, error =>
            error.PropertyName == nameof(UpdateItemDto.ListPrice) &&
            error.ErrorMessage == "List Price must not be negative.");
    }
    
    [Theory]
    [InlineData(-5)]
    [InlineData(-0.01)]
    public void RetailPrice_ShouldRejectNegativeValues(decimal price)
    {
        UpdateItemDto item = CreateValidItem();
        item.RetailPrice = price;

        ValidationResult? actual = _validator.Validate(item);

        Assert.False(actual.IsValid);
        Assert.Contains(actual.Errors, error =>
            error.PropertyName == nameof(UpdateItemDto.RetailPrice) &&
            error.ErrorMessage == "Retail Price must not be negative.");
    }
    
    [Fact]
    public void RetailPrice_ShouldAllowNull()
    {
        UpdateItemDto item = CreateValidItem();
        item.RetailPrice = null;

        ValidationResult? actual = _validator.Validate(item);

        Assert.True(actual.IsValid);
        Assert.DoesNotContain(actual.Errors, error =>
            error.PropertyName == nameof(UpdateItemDto.RetailPrice));
    }
    
    [Fact]
    public void RetailPrice_ShouldAllowZero()
    {
        UpdateItemDto item = CreateValidItem();
        item.RetailPrice = 0m;

        ValidationResult? actual = _validator.Validate(item);

        Assert.True(actual.IsValid);
        Assert.DoesNotContain(actual.Errors, error =>
            error.PropertyName == nameof(UpdateItemDto.RetailPrice));
    }
    
    [Fact]
    public void RetailPrice_ShouldAllowDecimalMaxValue()
    {
        UpdateItemDto item = CreateValidItem();
        item.RetailPrice = decimal.MaxValue;

        ValidationResult? actual = _validator.Validate(item);

        Assert.True(actual.IsValid);
        Assert.DoesNotContain(actual.Errors, error =>
            error.PropertyName == nameof(UpdateItemDto.RetailPrice));
    }
    
    [Fact]
    public void RetailPrice_ShouldRejectDecimalMinValue()
    {
        UpdateItemDto item = CreateValidItem();
        item.RetailPrice = decimal.MinValue;

        ValidationResult? actual = _validator.Validate(item);

        Assert.False(actual.IsValid);
        Assert.Contains(actual.Errors, error =>
            error.PropertyName == nameof(UpdateItemDto.RetailPrice) &&
            error.ErrorMessage == "Retail Price must not be negative.");
    }
    
    [Theory]
    [InlineData(-5)]
    [InlineData(-0.01)]
    public void CostPrice_ShouldRejectNegativeValues(decimal price)
    {
        UpdateItemDto item = CreateValidItem();
        item.CostPrice = price;

        ValidationResult? actual = _validator.Validate(item);

        Assert.False(actual.IsValid);
        Assert.Contains(actual.Errors, error =>
            error.PropertyName == nameof(UpdateItemDto.CostPrice) &&
            error.ErrorMessage == "Cost Price must not be negative.");
    }
    
    [Fact]
    public void CostPrice_ShouldAllowNull()
    {
        UpdateItemDto item = CreateValidItem();
        item.CostPrice = null;

        ValidationResult? actual = _validator.Validate(item);

        Assert.True(actual.IsValid);
        Assert.DoesNotContain(actual.Errors, error =>
            error.PropertyName == nameof(UpdateItemDto.CostPrice));
    }
    
    [Fact]
    public void CostPrice_ShouldAllowZero()
    {
        UpdateItemDto item = CreateValidItem();
        item.CostPrice = 0m;

        ValidationResult? actual = _validator.Validate(item);

        Assert.True(actual.IsValid);
        Assert.DoesNotContain(actual.Errors, error =>
            error.PropertyName == nameof(UpdateItemDto.CostPrice));
    }
    
    [Fact]
    public void CostPrice_ShouldAllowDecimalMaxValue()
    {
        UpdateItemDto item = CreateValidItem();
        item.CostPrice = decimal.MaxValue;

        ValidationResult? actual = _validator.Validate(item);

        Assert.True(actual.IsValid);
        Assert.DoesNotContain(actual.Errors, error =>
            error.PropertyName == nameof(UpdateItemDto.CostPrice));
    }
    
    [Fact]
    public void CostPrice_ShouldRejectDecimalMinValue()
    {
        UpdateItemDto item = CreateValidItem();
        item.CostPrice = decimal.MinValue;

        ValidationResult? actual = _validator.Validate(item);

        Assert.False(actual.IsValid);
        Assert.Contains(actual.Errors, error =>
            error.PropertyName == nameof(UpdateItemDto.CostPrice) &&
            error.ErrorMessage == "Cost Price must not be negative.");
    }
    
    [Fact]
    public void Quantity_ShouldBeRequired()
    {
        UpdateItemDto item = CreateValidItem();
        item.Quantity = null;

        ValidationResult? actual = _validator.Validate(item);

        Assert.False(actual.IsValid);
        Assert.Contains(actual.Errors, error =>
            error.PropertyName == nameof(UpdateItemDto.Quantity) &&
            error.ErrorMessage == "Quantity is required.");
    }
    
    [Theory]
    [InlineData(-5)]
    [InlineData(-0.01)]
    public void Quantity_ShouldRejectNegativeValues(decimal price)
    {
        UpdateItemDto item = CreateValidItem();
        item.Quantity = price;

        ValidationResult? actual = _validator.Validate(item);

        Assert.False(actual.IsValid);
        Assert.Contains(actual.Errors, error =>
            error.PropertyName == nameof(UpdateItemDto.Quantity) &&
            error.ErrorMessage == "Quantity must not be negative.");
    }
    
    [Fact]
    public void Quantity_ShouldAllowDecimalMaxValue()
    {
        UpdateItemDto item = CreateValidItem();
        item.Quantity = decimal.MaxValue;

        ValidationResult? actual = _validator.Validate(item);

        Assert.True(actual.IsValid);
        Assert.DoesNotContain(actual.Errors, error =>
            error.PropertyName == nameof(UpdateItemDto.Quantity));
    }
    
    [Fact]
    public void Quantity_ShouldRejectDecimalMinValue()
    {
        UpdateItemDto item = CreateValidItem();
        item.Quantity = decimal.MinValue;

        ValidationResult? actual = _validator.Validate(item);

        Assert.False(actual.IsValid);
        Assert.Contains(actual.Errors, error =>
            error.PropertyName == nameof(UpdateItemDto.Quantity) &&
            error.ErrorMessage == "Quantity must not be negative.");
    }
    
    [Fact]
    public void LowQuantityAlertThreshold_ShouldAllowNull()
    {
        UpdateItemDto item = CreateValidItem();
        item.LowQuantityAlertThreshold = null;

        ValidationResult? actual = _validator.Validate(item);

        Assert.True(actual.IsValid);
        Assert.DoesNotContain(actual.Errors, error =>
            error.PropertyName == nameof(UpdateItemDto.LowQuantityAlertThreshold));
    }
    
    [Theory]
    [InlineData(-5)]
    [InlineData(-0.01)]
    public void LowQuantityAlertThreshold_ShouldRejectNegativeValues(decimal price)
    {
        UpdateItemDto item = CreateValidItem();
        item.LowQuantityAlertThreshold = price;

        ValidationResult? actual = _validator.Validate(item);

        Assert.False(actual.IsValid);
        Assert.Contains(actual.Errors, error =>
            error.PropertyName == nameof(UpdateItemDto.LowQuantityAlertThreshold) &&
            error.ErrorMessage == "Low Stock Threshold must not be negative.");
    }
    
    [Fact]
    public void LowQuantityAlertThreshold_ShouldAllowDecimalMaxValue()
    {
        UpdateItemDto item = CreateValidItem();
        item.LowQuantityAlertThreshold = decimal.MaxValue;

        ValidationResult? actual = _validator.Validate(item);

        Assert.True(actual.IsValid);
        Assert.DoesNotContain(actual.Errors, error =>
            error.PropertyName == nameof(UpdateItemDto.LowQuantityAlertThreshold));
    }
    
    [Fact]
    public void LowQuantityAlertThreshold_ShouldRejectDecimalMinValue()
    {
        UpdateItemDto item = CreateValidItem();
        item.LowQuantityAlertThreshold = decimal.MinValue;

        ValidationResult? actual = _validator.Validate(item);

        Assert.False(actual.IsValid);
        Assert.Contains(actual.Errors, error =>
            error.PropertyName == nameof(UpdateItemDto.LowQuantityAlertThreshold) &&
            error.ErrorMessage == "Low Stock Threshold must not be negative.");
    }
    
    [Fact]
    public void Locations_ShouldBeRequired()
    {
        UpdateItemDto item = CreateValidItem();
        item.Locations = null;

        ValidationResult? actual = _validator.Validate(item);

        Assert.False(actual.IsValid);
        Assert.Contains(actual.Errors, error =>
            error.PropertyName == nameof(UpdateItemDto.Locations) &&
            error.ErrorMessage == "Locations is required.");
    }

    [Fact]
    public void Locations_ShouldRejectDuplicateValues()
    {
        UpdateItemDto item = CreateValidItem();
        item.Locations = ["Shelf A", "Shelf A"];

        ValidationResult? actual = _validator.Validate(item);

        Assert.False(actual.IsValid);
        Assert.Contains(actual.Errors, error =>
            error.PropertyName == nameof(UpdateItemDto.Locations) &&
            error.ErrorMessage == "Locations must not contain duplicates.");
    }

    [Fact]
    public void Locations_ShouldRejectCaseInsensitiveDuplicates()
    {
        UpdateItemDto item = CreateValidItem();
        item.Locations = ["Shelf A", "shelf a"];

        ValidationResult? actual = _validator.Validate(item);

        Assert.False(actual.IsValid);
        Assert.Contains(actual.Errors, error =>
            error.PropertyName == nameof(UpdateItemDto.Locations) &&
            error.ErrorMessage == "Locations must not contain duplicates.");
    }
    
    [Fact]
    public void Location_ShouldBeRequired()
    {
        UpdateItemDto item = CreateValidItem();
        item.Locations = [""];

        ValidationResult? actual = _validator.Validate(item);

        Assert.Contains(actual.Errors, error =>
            error.PropertyName == "Locations[0]" &&
            error.ErrorMessage == "Invalid Location.");
    }

    [Fact]
    public void Location_should_reject_non_ascii()
    {
        UpdateItemDto item = CreateValidItem();
        item.Locations = ["Café"];

        ValidationResult? actual = _validator.Validate(item);

        Assert.Contains(actual.Errors, error =>
            error.PropertyName == "Locations[0]" &&
            error.ErrorMessage == "Invalid Location.");
    }
    
    private static UpdateItemDto CreateValidItem()
    {
        return new UpdateItemDto
        {
            Name = "Test Item",
            UomId = Guid.NewGuid(),
            Description = "A valid description.",
            Image = "be693c7e.webp",
            ListPrice = 1000m,
            RetailPrice = 1500.10m,
            CostPrice = 600m,
            Quantity = 10m,
            LowQuantityAlertThreshold = 2m,
            Locations = ["Shelf A", "Shelf B"]
        };
    }
}