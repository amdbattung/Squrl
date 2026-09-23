using FluentValidation.Results;
using NodaTime;
using Squrl.App.Features.SalesOrderDetails.DTOs;
using Squrl.App.Features.SalesOrderDetails.Validators;
using Squrl.App.Features.SalesOrders.DTOs;
using Squrl.App.Features.SalesOrders.Validators;

namespace Squrl.App.Tests.Validators;

public class UpdateSalesOrderValidatorTests
{
    private readonly UpdateSalesOrderValidator _validator;

    public UpdateSalesOrderValidatorTests()
    {
        _validator = new UpdateSalesOrderValidator(new UpdateSoDetailValidator());
    }
    
    [Fact]
    public void ValidSalesOrder_ShouldPass()
    {
        UpdateSalesOrderDto salesOrder = CreateValidSalesOrder();

        ValidationResult? actual = _validator.Validate(salesOrder);

        Assert.True(actual.IsValid);
        Assert.Empty(actual.Errors);
    }
    
    [Fact]
    public void Customer_ShouldAllowNull()
    {
        UpdateSalesOrderDto salesOrder = CreateValidSalesOrder();
        salesOrder.Customer = null;
        
        ValidationResult? actual = _validator.Validate(salesOrder);
        
        Assert.True(actual.IsValid);
        Assert.DoesNotContain(actual.Errors, error => 
            error.PropertyName == nameof(UpdateSalesOrderDto.Customer));
    }
    
    [Fact]
    public void Customer_ShouldRejectWhitespace()
    {
        UpdateSalesOrderDto salesOrder = CreateValidSalesOrder();
        salesOrder.Customer = "     ";
        
        ValidationResult? actual = _validator.Validate(salesOrder);

        Assert.False(actual.IsValid);
        Assert.Contains(actual.Errors, error => 
            error.PropertyName == nameof(UpdateSalesOrderDto.Customer) &&
            error.ErrorMessage == "Customer must not be only whitespace.");
    }
    
    [Fact]
    public void Customer_ShouldRejectNonAscii()
    {
        UpdateSalesOrderDto salesOrder = CreateValidSalesOrder();
        salesOrder.Customer = "Café";

        ValidationResult? actual = _validator.Validate(salesOrder);

        Assert.False(actual.IsValid);
        Assert.Contains(actual.Errors, error =>
            error.PropertyName == nameof(UpdateSalesOrderDto.Customer) &&
            error.ErrorMessage == "Invalid Customer.");
    }
    
    [Fact]
    public void Details_ShouldBeRequired()
    {
        UpdateSalesOrderDto salesOrder = CreateValidSalesOrder();
        salesOrder.Details = null;

        ValidationResult? actual = _validator.Validate(salesOrder);

        Assert.False(actual.IsValid);
        Assert.Contains(actual.Errors, error =>
            error.PropertyName == nameof(UpdateSalesOrderDto.Details) &&
            error.ErrorMessage == "Sales Order Details is required.");
    }
    
    [Fact]
    public void Details_ShouldRejectEmpty()
    {
        UpdateSalesOrderDto salesOrder = CreateValidSalesOrder();
        salesOrder.Details = [];

        ValidationResult? actual = _validator.Validate(salesOrder);

        Assert.False(actual.IsValid);
        Assert.Contains(actual.Errors, error =>
            error.PropertyName == nameof(UpdateSalesOrderDto.Details) &&
            error.ErrorMessage == "Sales Order Details is required.");
    }
    
    [Fact]
    public void Details_ShouldRejectNonSequential()
    {
        UpdateSalesOrderDto salesOrder = CreateValidSalesOrder();
        salesOrder.Details?[0].LineSequence = 1;
        salesOrder.Details?[1].LineSequence = 3;

        ValidationResult? actual = _validator.Validate(salesOrder);

        Assert.False(actual.IsValid);
        Assert.Contains(actual.Errors, error =>
            error.PropertyName == nameof(UpdateSalesOrderDto.Details) &&
            error.ErrorMessage == "Line sequences must start at 1 and be consecutive.");
    }
    
    [Fact]
    public void DateOrdered_ShouldBeRequired()
    {
        UpdateSalesOrderDto salesOrder = CreateValidSalesOrder();
        salesOrder.DateOrdered = null;
        
        ValidationResult? actual = _validator.Validate(salesOrder);

        Assert.False(actual.IsValid);
        Assert.Contains(actual.Errors, error =>
            error.PropertyName == nameof(UpdateSalesOrderDto.DateOrdered) &&
            error.ErrorMessage == "Date Ordered is required.");
    }
    
    private static UpdateSalesOrderDto CreateValidSalesOrder()
    {
        return new UpdateSalesOrderDto
        {
            Customer = "Test Customer",
            InvoiceNumber = "5000001",
            Details = [
                new UpdateSoDetailDto
                {
                    SalesOrderId = Guid.Empty,
                    LineSequence = 1,
                    ItemId = Guid.NewGuid(),
                    UnitPrice = 600m,
                    Quantity = 10m
                },
                new UpdateSoDetailDto
                {
                    SalesOrderId = Guid.Empty,
                    LineSequence = 2,
                    ItemId = Guid.NewGuid(),
                    UnitPrice = 600.05m,
                    Quantity = 10.05m
                }
            ],
            DateOrdered = Instant.FromUtc(2020, 1, 1, 0, 0)
        };
    }
}