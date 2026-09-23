using FluentValidation.Results;
using NodaTime;
using Squrl.App.Enums;
using Squrl.App.Features.PurchaseOrderDetails.DTOs;
using Squrl.App.Features.PurchaseOrderDetails.Validators;
using Squrl.App.Features.PurchaseOrders.DTOs;
using Squrl.App.Features.PurchaseOrders.Validators;

namespace Squrl.App.Tests.Validators;

public class CreatePurchaseOrderValidatorTests
{
    private readonly CreatePurchaseOrderValidator _validator;

    public CreatePurchaseOrderValidatorTests()
    {
        _validator = new CreatePurchaseOrderValidator(new CreatePoDetailValidator());
    }
    
    [Fact]
    public void ValidPurchaseOrder_ShouldPass()
    {
        CreatePurchaseOrderDto purchaseOrder = CreateValidPurchaseOrder();

        ValidationResult? actual = _validator.Validate(purchaseOrder);

        Assert.True(actual.IsValid);
        Assert.Empty(actual.Errors);
    }
    
    [Fact]
    public void InvoiceNumber_ShouldAllowNull()
    {
        CreatePurchaseOrderDto purchaseOrder = CreateValidPurchaseOrder();
        purchaseOrder.InvoiceNumber = null;
        
        ValidationResult? actual = _validator.Validate(purchaseOrder);
        
        Assert.True(actual.IsValid);
        Assert.DoesNotContain(actual.Errors, error => 
            error.PropertyName == nameof(CreatePurchaseOrderDto.InvoiceNumber));
    }
    
    [Fact]
    public void InvoiceNumber_ShouldRejectWhitespace()
    {
        CreatePurchaseOrderDto purchaseOrder = CreateValidPurchaseOrder();
        purchaseOrder.InvoiceNumber = "     ";
        
        ValidationResult? actual = _validator.Validate(purchaseOrder);

        Assert.False(actual.IsValid);
        Assert.Contains(actual.Errors, error => 
            error.PropertyName == nameof(CreatePurchaseOrderDto.InvoiceNumber) &&
            error.ErrorMessage == "Invoice Number must not be only whitespace.");
    }
    
    [Fact]
    public void InvoiceNumber_ShouldRejectNonAlphanumeric()
    {
        CreatePurchaseOrderDto purchaseOrder = CreateValidPurchaseOrder();
        purchaseOrder.InvoiceNumber = "R&B";

        ValidationResult? actual = _validator.Validate(purchaseOrder);

        Assert.False(actual.IsValid);
        Assert.Contains(actual.Errors, error =>
            error.PropertyName == nameof(CreatePurchaseOrderDto.InvoiceNumber) &&
            error.ErrorMessage == "Invalid Invoice Number.");
    }
    
    [Fact]
    public void Status_ShouldBeRequired()
    {
        CreatePurchaseOrderDto purchaseOrder = CreateValidPurchaseOrder();
        purchaseOrder.Status = null;

        ValidationResult? actual = _validator.Validate(purchaseOrder);

        Assert.False(actual.IsValid);
        Assert.Contains(actual.Errors, error =>
            error.PropertyName == nameof(CreatePurchaseOrderDto.Status) &&
            error.ErrorMessage == "Status is required.");
    }

    [Fact]
    public void Status_ShouldRejectInvalidEnumValue()
    {
        CreatePurchaseOrderDto purchaseOrder = CreateValidPurchaseOrder();
        purchaseOrder.Status = (PurchaseOrderStatus)999;

        ValidationResult? actual = _validator.Validate(purchaseOrder);

        Assert.False(actual.IsValid);
        Assert.Contains(actual.Errors, error =>
            error.PropertyName == nameof(CreatePurchaseOrderDto.Status) &&
            error.ErrorMessage == "Status is invalid.");
    }
    
    [Fact]
    public void Details_ShouldBeRequired()
    {
        CreatePurchaseOrderDto purchaseOrder = CreateValidPurchaseOrder();
        purchaseOrder.Details = null;

        ValidationResult? actual = _validator.Validate(purchaseOrder);

        Assert.False(actual.IsValid);
        Assert.Contains(actual.Errors, error =>
            error.PropertyName == nameof(CreatePurchaseOrderDto.Details) &&
            error.ErrorMessage == "Purchase Order Details is required.");
    }
    
    [Fact]
    public void Details_ShouldRejectEmpty()
    {
        CreatePurchaseOrderDto purchaseOrder = CreateValidPurchaseOrder();
        purchaseOrder.Details = [];

        ValidationResult? actual = _validator.Validate(purchaseOrder);

        Assert.False(actual.IsValid);
        Assert.Contains(actual.Errors, error =>
            error.PropertyName == nameof(CreatePurchaseOrderDto.Details) &&
            error.ErrorMessage == "Purchase Order Details is required.");
    }
    
    [Fact]
    public void Details_ShouldRejectNonSequential()
    {
        CreatePurchaseOrderDto purchaseOrder = CreateValidPurchaseOrder();
        purchaseOrder.Details?[0].LineSequence = 1;
        purchaseOrder.Details?[1].LineSequence = 3;

        ValidationResult? actual = _validator.Validate(purchaseOrder);

        Assert.False(actual.IsValid);
        Assert.Contains(actual.Errors, error =>
            error.PropertyName == nameof(CreatePurchaseOrderDto.Details) &&
            error.ErrorMessage == "Line sequences must start at 1 and be consecutive.");
    }
    
    [Fact]
    public void DateOrdered_ShouldBeRequired()
    {
        CreatePurchaseOrderDto purchaseOrder = CreateValidPurchaseOrder();
        purchaseOrder.DateOrdered = null;
        
        ValidationResult? actual = _validator.Validate(purchaseOrder);

        Assert.False(actual.IsValid);
        Assert.Contains(actual.Errors, error =>
            error.PropertyName == nameof(CreatePurchaseOrderDto.DateOrdered) &&
            error.ErrorMessage == "Date Ordered is required.");
    }
    
    [Fact]
    public void DateRequired_ShouldAllowNull()
    {
        CreatePurchaseOrderDto purchaseOrder = CreateValidPurchaseOrder();
        purchaseOrder.DateRequired = null;
        
        ValidationResult? actual = _validator.Validate(purchaseOrder);
        
        Assert.True(actual.IsValid);
        Assert.DoesNotContain(actual.Errors, error => 
            error.PropertyName == nameof(CreatePurchaseOrderDto.DateRequired));
    }
    
    [Fact]
    public void DateRequired_ShouldNotBeBeforeDateOrdered()
    {
        CreatePurchaseOrderDto purchaseOrder = CreateValidPurchaseOrder();
        purchaseOrder.DateRequired = Instant.FromUtc(2019, 12, 31, 23, 59, 59);
        
        ValidationResult? actual = _validator.Validate(purchaseOrder);
        
        Assert.False(actual.IsValid);
        Assert.Contains(actual.Errors, error =>
            error.PropertyName == nameof(CreatePurchaseOrderDto.DateRequired) &&
            error.ErrorMessage == "Date Required must be after order date.");
    }
    
    [Fact]
    public void DateShipped_ShouldAllowNull()
    {
        CreatePurchaseOrderDto purchaseOrder = CreateValidPurchaseOrder();
        purchaseOrder.DateShipped = null;
        
        ValidationResult? actual = _validator.Validate(purchaseOrder);
        
        Assert.True(actual.IsValid);
        Assert.DoesNotContain(actual.Errors, error => 
            error.PropertyName == nameof(CreatePurchaseOrderDto.DateShipped));
    }
    
    [Fact]
    public void DateShipped_ShouldNotBeBeforeDateOrdered()
    {
        CreatePurchaseOrderDto purchaseOrder = CreateValidPurchaseOrder();
        purchaseOrder.DateShipped = Instant.FromUtc(2019, 12, 31, 23, 59, 59);
        
        ValidationResult? actual = _validator.Validate(purchaseOrder);
        
        Assert.False(actual.IsValid);
        Assert.Contains(actual.Errors, error =>
            error.PropertyName == nameof(CreatePurchaseOrderDto.DateShipped) &&
            error.ErrorMessage == "Date Shipped must be after order date.");
    }
    
    private static CreatePurchaseOrderDto CreateValidPurchaseOrder()
    {
        return new CreatePurchaseOrderDto
        {
            SupplierId = Guid.NewGuid(),
            InvoiceNumber = "5000001",
            Status = PurchaseOrderStatus.Pending,
            Details = [
                new CreatePoDetailDto
                {
                    PurchaseOrderId = Guid.Empty,
                    LineSequence = 1,
                    ItemId = Guid.NewGuid(),
                    UnitPrice = 600m,
                    Quantity = 10m
                },
                new CreatePoDetailDto
                {
                    PurchaseOrderId = Guid.Empty,
                    LineSequence = 2,
                    ItemId = Guid.NewGuid(),
                    UnitPrice = 600.05m,
                    Quantity = 10.05m
                }
            ],
            DateOrdered = Instant.FromUtc(2020, 1, 1, 0, 0),
            DateRequired = Instant.FromUtc(2020, 1, 1, 0, 0),
            DateShipped = Instant.FromUtc(2020, 1, 1, 0, 0)
        };
    }
}