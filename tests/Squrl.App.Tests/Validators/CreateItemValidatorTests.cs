using FluentValidation.Results;
using Squrl.App.Features.Items.DTOs;
using Squrl.App.Features.Items.Validators;

namespace Squrl.App.Tests.Validators;

public class CreateItemValidatorTests
{
    private readonly CreateItemValidator _validator;
    
    public CreateItemValidatorTests()
    {
        _validator = new CreateItemValidator();
    }
    
    [Fact]
    public void ValidItem_ShouldPass()
    {
        CreateItemDto item = CreateValidItem();

        ValidationResult? actual = _validator.Validate(item);

        Assert.True(actual.IsValid);
        Assert.Empty(actual.Errors);
    }

    [Fact]
    public void Name_ShouldBeRequired()
    {
        CreateItemDto item = CreateValidItem();
        item.Name = "";
        
        ValidationResult? actual = _validator.Validate(item);

        Assert.False(actual.IsValid);
        Assert.Contains(actual.Errors, error =>
            error.PropertyName == nameof(CreateItemDto.Name) &&
            error.ErrorMessage == "Name is required.");
    }
    
    [Fact]
    public void UomId_ShouldBeRequired()
    {
        CreateItemDto item = CreateValidItem();
        item.UomId = null;
        
        ValidationResult? actual = _validator.Validate(item);

        Assert.False(actual.IsValid);
        Assert.Contains(actual.Errors, error =>
            error.PropertyName == nameof(CreateItemDto.UomId) &&
            error.ErrorMessage == "UOM is required.");
    }
    
    [Fact]
    public void Description_ShouldAllowNull()
    {
        CreateItemDto item = CreateValidItem();
        item.Description = null;
        
        ValidationResult? actual = _validator.Validate(item);
        
        Assert.True(actual.IsValid);
        Assert.DoesNotContain(actual.Errors, error => 
            error.PropertyName == nameof(CreateItemDto.Description));
    }
    
    [Fact]
    public void Description_ShouldRejectWhitespace()
    {
        CreateItemDto item = CreateValidItem();
        item.Description = "     ";
        
        ValidationResult? actual = _validator.Validate(item);

        Assert.False(actual.IsValid);
        Assert.Contains(actual.Errors, error => 
            error.PropertyName == nameof(CreateItemDto.Description) &&
            error.ErrorMessage == "Description must not be only whitespace.");
    }
    
    [Fact]
    public void Description_ShouldRejectNonLatin1()
    {
        CreateItemDto item = CreateValidItem();
        item.Description = "Москва";

        ValidationResult? actual = _validator.Validate(item);

        Assert.False(actual.IsValid);
        Assert.Contains(actual.Errors, error =>
            error.PropertyName == nameof(CreateItemDto.Description) &&
            error.ErrorMessage == "Invalid Description.");
    }
    
    [Fact]
    public void Image_ShouldAllowNull()
    {
        CreateItemDto item = CreateValidItem();
        item.Image = null;
        
        ValidationResult? actual = _validator.Validate(item);
        
        Assert.True(actual.IsValid);
        Assert.DoesNotContain(actual.Errors, error => 
            error.PropertyName == nameof(CreateItemDto.Image));
    }
    
    [Fact]
    public void Image_ShouldRejectWhitespace()
    {
        CreateItemDto item = CreateValidItem();
        item.Image = "     ";
        
        ValidationResult? actual = _validator.Validate(item);

        Assert.False(actual.IsValid);
        Assert.Contains(actual.Errors, error => 
            error.PropertyName == nameof(CreateItemDto.Image) &&
            error.ErrorMessage == "Image must not be only whitespace.");
    }
    
    [Fact]
    public void Image_ShouldRejectNonAscii()
    {
        CreateItemDto item = CreateValidItem();
        item.Image = "Café";

        ValidationResult? actual = _validator.Validate(item);

        Assert.False(actual.IsValid);
        Assert.Contains(actual.Errors, error =>
            error.PropertyName == nameof(CreateItemDto.Image) &&
            error.ErrorMessage == "Invalid Image.");
    }
    
    [Theory]
    [InlineData(-5)]
    [InlineData(-0.01)]
    public void ListPrice_ShouldRejectNegativeValues(decimal price)
    {
        CreateItemDto item = CreateValidItem();
        item.ListPrice = price;

        ValidationResult? actual = _validator.Validate(item);

        Assert.False(actual.IsValid);
        Assert.Contains(actual.Errors, error =>
            error.PropertyName == nameof(CreateItemDto.ListPrice) &&
            error.ErrorMessage == "List Price must not be negative.");
    }
    
    [Fact]
    public void ListPrice_ShouldAllowNull()
    {
        CreateItemDto item = CreateValidItem();
        item.ListPrice = null;

        ValidationResult? actual = _validator.Validate(item);

        Assert.True(actual.IsValid);
        Assert.DoesNotContain(actual.Errors, error =>
            error.PropertyName == nameof(CreateItemDto.ListPrice));
    }
    
    [Fact]
    public void ListPrice_ShouldAllowZero()
    {
        CreateItemDto item = CreateValidItem();
        item.ListPrice = 0m;

        ValidationResult? actual = _validator.Validate(item);

        Assert.True(actual.IsValid);
        Assert.DoesNotContain(actual.Errors, error =>
            error.PropertyName == nameof(CreateItemDto.ListPrice));
    }
    
    [Fact]
    public void ListPrice_ShouldAllowDecimalMaxValue()
    {
        CreateItemDto item = CreateValidItem();
        item.ListPrice = decimal.MaxValue;

        ValidationResult? actual = _validator.Validate(item);

        Assert.True(actual.IsValid);
        Assert.DoesNotContain(actual.Errors, error =>
            error.PropertyName == nameof(CreateItemDto.ListPrice));
    }
    
    [Fact]
    public void ListPrice_ShouldRejectDecimalMinValue()
    {
        CreateItemDto item = CreateValidItem();
        item.ListPrice = decimal.MinValue;

        ValidationResult? actual = _validator.Validate(item);

        Assert.False(actual.IsValid);
        Assert.Contains(actual.Errors, error =>
            error.PropertyName == nameof(CreateItemDto.ListPrice) &&
            error.ErrorMessage == "List Price must not be negative.");
    }
    
    [Theory]
    [InlineData(-5)]
    [InlineData(-0.01)]
    public void RetailPrice_ShouldRejectNegativeValues(decimal price)
    {
        CreateItemDto item = CreateValidItem();
        item.RetailPrice = price;

        ValidationResult? actual = _validator.Validate(item);

        Assert.False(actual.IsValid);
        Assert.Contains(actual.Errors, error =>
            error.PropertyName == nameof(CreateItemDto.RetailPrice) &&
            error.ErrorMessage == "Retail Price must not be negative.");
    }
    
    [Fact]
    public void RetailPrice_ShouldAllowNull()
    {
        CreateItemDto item = CreateValidItem();
        item.RetailPrice = null;

        ValidationResult? actual = _validator.Validate(item);

        Assert.True(actual.IsValid);
        Assert.DoesNotContain(actual.Errors, error =>
            error.PropertyName == nameof(CreateItemDto.RetailPrice));
    }
    
    [Fact]
    public void RetailPrice_ShouldAllowZero()
    {
        CreateItemDto item = CreateValidItem();
        item.RetailPrice = 0m;

        ValidationResult? actual = _validator.Validate(item);

        Assert.True(actual.IsValid);
        Assert.DoesNotContain(actual.Errors, error =>
            error.PropertyName == nameof(CreateItemDto.RetailPrice));
    }
    
    [Fact]
    public void RetailPrice_ShouldAllowDecimalMaxValue()
    {
        CreateItemDto item = CreateValidItem();
        item.RetailPrice = decimal.MaxValue;

        ValidationResult? actual = _validator.Validate(item);

        Assert.True(actual.IsValid);
        Assert.DoesNotContain(actual.Errors, error =>
            error.PropertyName == nameof(CreateItemDto.RetailPrice));
    }
    
    [Fact]
    public void RetailPrice_ShouldRejectDecimalMinValue()
    {
        CreateItemDto item = CreateValidItem();
        item.RetailPrice = decimal.MinValue;

        ValidationResult? actual = _validator.Validate(item);

        Assert.False(actual.IsValid);
        Assert.Contains(actual.Errors, error =>
            error.PropertyName == nameof(CreateItemDto.RetailPrice) &&
            error.ErrorMessage == "Retail Price must not be negative.");
    }
    
    [Theory]
    [InlineData(-5)]
    [InlineData(-0.01)]
    public void CostPrice_ShouldRejectNegativeValues(decimal price)
    {
        CreateItemDto item = CreateValidItem();
        item.CostPrice = price;

        ValidationResult? actual = _validator.Validate(item);

        Assert.False(actual.IsValid);
        Assert.Contains(actual.Errors, error =>
            error.PropertyName == nameof(CreateItemDto.CostPrice) &&
            error.ErrorMessage == "Cost Price must not be negative.");
    }
    
    [Fact]
    public void CostPrice_ShouldAllowNull()
    {
        CreateItemDto item = CreateValidItem();
        item.CostPrice = null;

        ValidationResult? actual = _validator.Validate(item);

        Assert.True(actual.IsValid);
        Assert.DoesNotContain(actual.Errors, error =>
            error.PropertyName == nameof(CreateItemDto.CostPrice));
    }
    
    [Fact]
    public void CostPrice_ShouldAllowZero()
    {
        CreateItemDto item = CreateValidItem();
        item.CostPrice = 0m;

        ValidationResult? actual = _validator.Validate(item);

        Assert.True(actual.IsValid);
        Assert.DoesNotContain(actual.Errors, error =>
            error.PropertyName == nameof(CreateItemDto.CostPrice));
    }
    
    [Fact]
    public void CostPrice_ShouldAllowDecimalMaxValue()
    {
        CreateItemDto item = CreateValidItem();
        item.CostPrice = decimal.MaxValue;

        ValidationResult? actual = _validator.Validate(item);

        Assert.True(actual.IsValid);
        Assert.DoesNotContain(actual.Errors, error =>
            error.PropertyName == nameof(CreateItemDto.CostPrice));
    }
    
    [Fact]
    public void CostPrice_ShouldRejectDecimalMinValue()
    {
        CreateItemDto item = CreateValidItem();
        item.CostPrice = decimal.MinValue;

        ValidationResult? actual = _validator.Validate(item);

        Assert.False(actual.IsValid);
        Assert.Contains(actual.Errors, error =>
            error.PropertyName == nameof(CreateItemDto.CostPrice) &&
            error.ErrorMessage == "Cost Price must not be negative.");
    }
    
    [Fact]
    public void Quantity_ShouldBeRequired()
    {
        CreateItemDto item = CreateValidItem();
        item.Quantity = null;

        ValidationResult? actual = _validator.Validate(item);

        Assert.False(actual.IsValid);
        Assert.Contains(actual.Errors, error =>
            error.PropertyName == nameof(CreateItemDto.Quantity) &&
            error.ErrorMessage == "Quantity is required.");
    }
    
    [Theory]
    [InlineData(-5)]
    [InlineData(-0.01)]
    public void Quantity_ShouldRejectNegativeValues(decimal price)
    {
        CreateItemDto item = CreateValidItem();
        item.Quantity = price;

        ValidationResult? actual = _validator.Validate(item);

        Assert.False(actual.IsValid);
        Assert.Contains(actual.Errors, error =>
            error.PropertyName == nameof(CreateItemDto.Quantity) &&
            error.ErrorMessage == "Quantity must not be negative.");
    }
    
    [Fact]
    public void Quantity_ShouldAllowDecimalMaxValue()
    {
        CreateItemDto item = CreateValidItem();
        item.Quantity = decimal.MaxValue;

        ValidationResult? actual = _validator.Validate(item);

        Assert.True(actual.IsValid);
        Assert.DoesNotContain(actual.Errors, error =>
            error.PropertyName == nameof(CreateItemDto.Quantity));
    }
    
    [Fact]
    public void Quantity_ShouldRejectDecimalMinValue()
    {
        CreateItemDto item = CreateValidItem();
        item.Quantity = decimal.MinValue;

        ValidationResult? actual = _validator.Validate(item);

        Assert.False(actual.IsValid);
        Assert.Contains(actual.Errors, error =>
            error.PropertyName == nameof(CreateItemDto.Quantity) &&
            error.ErrorMessage == "Quantity must not be negative.");
    }
    
    [Fact]
    public void LowQuantityAlertThreshold_ShouldAllowNull()
    {
        CreateItemDto item = CreateValidItem();
        item.LowQuantityAlertThreshold = null;

        ValidationResult? actual = _validator.Validate(item);

        Assert.True(actual.IsValid);
        Assert.DoesNotContain(actual.Errors, error =>
            error.PropertyName == nameof(CreateItemDto.LowQuantityAlertThreshold));
    }
    
    [Theory]
    [InlineData(-5)]
    [InlineData(-0.01)]
    public void LowQuantityAlertThreshold_ShouldRejectNegativeValues(decimal price)
    {
        CreateItemDto item = CreateValidItem();
        item.LowQuantityAlertThreshold = price;

        ValidationResult? actual = _validator.Validate(item);

        Assert.False(actual.IsValid);
        Assert.Contains(actual.Errors, error =>
            error.PropertyName == nameof(CreateItemDto.LowQuantityAlertThreshold) &&
            error.ErrorMessage == "Low Stock Threshold must not be negative.");
    }
    
    [Fact]
    public void LowQuantityAlertThreshold_ShouldAllowDecimalMaxValue()
    {
        CreateItemDto item = CreateValidItem();
        item.LowQuantityAlertThreshold = decimal.MaxValue;

        ValidationResult? actual = _validator.Validate(item);

        Assert.True(actual.IsValid);
        Assert.DoesNotContain(actual.Errors, error =>
            error.PropertyName == nameof(CreateItemDto.LowQuantityAlertThreshold));
    }
    
    [Fact]
    public void LowQuantityAlertThreshold_ShouldRejectDecimalMinValue()
    {
        CreateItemDto item = CreateValidItem();
        item.LowQuantityAlertThreshold = decimal.MinValue;

        ValidationResult? actual = _validator.Validate(item);

        Assert.False(actual.IsValid);
        Assert.Contains(actual.Errors, error =>
            error.PropertyName == nameof(CreateItemDto.LowQuantityAlertThreshold) &&
            error.ErrorMessage == "Low Stock Threshold must not be negative.");
    }
    
    [Fact]
    public void Locations_ShouldBeRequired()
    {
        CreateItemDto item = CreateValidItem();
        item.Locations = null;

        ValidationResult? actual = _validator.Validate(item);

        Assert.False(actual.IsValid);
        Assert.Contains(actual.Errors, error =>
            error.PropertyName == nameof(CreateItemDto.Locations) &&
            error.ErrorMessage == "Locations is required.");
    }

    [Fact]
    public void Locations_ShouldRejectDuplicateValues()
    {
        CreateItemDto item = CreateValidItem();
        item.Locations = ["Shelf A", "Shelf A"];

        ValidationResult? actual = _validator.Validate(item);

        Assert.False(actual.IsValid);
        Assert.Contains(actual.Errors, error =>
            error.PropertyName == nameof(CreateItemDto.Locations) &&
            error.ErrorMessage == "Locations must not contain duplicates.");
    }

    [Fact]
    public void Locations_ShouldRejectCaseInsensitiveDuplicates()
    {
        CreateItemDto item = CreateValidItem();
        item.Locations = ["Shelf A", "shelf a"];

        ValidationResult? actual = _validator.Validate(item);

        Assert.False(actual.IsValid);
        Assert.Contains(actual.Errors, error =>
            error.PropertyName == nameof(CreateItemDto.Locations) &&
            error.ErrorMessage == "Locations must not contain duplicates.");
    }
    
    [Fact]
    public void Location_ShouldBeRequired()
    {
        CreateItemDto item = CreateValidItem();
        item.Locations = [""];

        ValidationResult? actual = _validator.Validate(item);

        Assert.Contains(actual.Errors, error =>
            error.PropertyName == "Locations[0]" &&
            error.ErrorMessage == "Invalid Location.");
    }

    [Fact]
    public void Location_should_reject_non_ascii()
    {
        CreateItemDto item = CreateValidItem();
        item.Locations = ["Café"];

        ValidationResult? actual = _validator.Validate(item);

        Assert.Contains(actual.Errors, error =>
            error.PropertyName == "Locations[0]" &&
            error.ErrorMessage == "Invalid Location.");
    }
    
    private static CreateItemDto CreateValidItem()
    {
        return new CreateItemDto
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