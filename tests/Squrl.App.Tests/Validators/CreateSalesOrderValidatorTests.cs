using FluentValidation.Results;
using NodaTime;
using Squrl.App.Features.SalesOrderDetails.DTOs;
using Squrl.App.Features.SalesOrderDetails.Validators;
using Squrl.App.Features.SalesOrders.DTOs;
using Squrl.App.Features.SalesOrders.Validators;

namespace Squrl.App.Tests.Validators;

public class CreateSalesOrderValidatorTests
{
    private readonly CreateSalesOrderValidator _validator;

    public CreateSalesOrderValidatorTests()
    {
        _validator = new CreateSalesOrderValidator(new CreateSoDetailValidator());
    }
    
    [Fact]
    public void ValidSalesOrder_ShouldPass()
    {
        CreateSalesOrderDto salesOrder = CreateValidSalesOrder();

        ValidationResult? actual = _validator.Validate(salesOrder);

        Assert.True(actual.IsValid);
        Assert.Empty(actual.Errors);
    }
    
    [Fact]
    public void Customer_ShouldAllowNull()
    {
        CreateSalesOrderDto salesOrder = CreateValidSalesOrder();
        salesOrder.Customer = null;
        
        ValidationResult? actual = _validator.Validate(salesOrder);
        
        Assert.True(actual.IsValid);
        Assert.DoesNotContain(actual.Errors, error => 
            error.PropertyName == nameof(CreateSalesOrderDto.Customer));
    }
    
    [Fact]
    public void Customer_ShouldRejectWhitespace()
    {
        CreateSalesOrderDto salesOrder = CreateValidSalesOrder();
        salesOrder.Customer = "     ";
        
        ValidationResult? actual = _validator.Validate(salesOrder);

        Assert.False(actual.IsValid);
        Assert.Contains(actual.Errors, error => 
            error.PropertyName == nameof(CreateSalesOrderDto.Customer) &&
            error.ErrorMessage == "Customer must not be only whitespace.");
    }
    
    [Fact]
    public void Customer_ShouldRejectNonAscii()
    {
        CreateSalesOrderDto salesOrder = CreateValidSalesOrder();
        salesOrder.Customer = "Café";

        ValidationResult? actual = _validator.Validate(salesOrder);

        Assert.False(actual.IsValid);
        Assert.Contains(actual.Errors, error =>
            error.PropertyName == nameof(CreateSalesOrderDto.Customer) &&
            error.ErrorMessage == "Invalid Customer.");
    }
    
    [Fact]
    public void Details_ShouldBeRequired()
    {
        CreateSalesOrderDto salesOrder = CreateValidSalesOrder();
        salesOrder.Details = null;

        ValidationResult? actual = _validator.Validate(salesOrder);

        Assert.False(actual.IsValid);
        Assert.Contains(actual.Errors, error =>
            error.PropertyName == nameof(CreateSalesOrderDto.Details) &&
            error.ErrorMessage == "Sales Order Details is required.");
    }
    
    [Fact]
    public void Details_ShouldRejectEmpty()
    {
        CreateSalesOrderDto salesOrder = CreateValidSalesOrder();
        salesOrder.Details = [];

        ValidationResult? actual = _validator.Validate(salesOrder);

        Assert.False(actual.IsValid);
        Assert.Contains(actual.Errors, error =>
            error.PropertyName == nameof(CreateSalesOrderDto.Details) &&
            error.ErrorMessage == "Sales Order Details is required.");
    }
    
    [Fact]
    public void Details_ShouldRejectNonSequential()
    {
        CreateSalesOrderDto salesOrder = CreateValidSalesOrder();
        salesOrder.Details?[0].LineSequence = 1;
        salesOrder.Details?[1].LineSequence = 3;

        ValidationResult? actual = _validator.Validate(salesOrder);

        Assert.False(actual.IsValid);
        Assert.Contains(actual.Errors, error =>
            error.PropertyName == nameof(CreateSalesOrderDto.Details) &&
            error.ErrorMessage == "Line sequences must start at 1 and be consecutive.");
    }
    
    [Fact]
    public void DateOrdered_ShouldBeRequired()
    {
        CreateSalesOrderDto salesOrder = CreateValidSalesOrder();
        salesOrder.DateOrdered = null;
        
        ValidationResult? actual = _validator.Validate(salesOrder);

        Assert.False(actual.IsValid);
        Assert.Contains(actual.Errors, error =>
            error.PropertyName == nameof(CreateSalesOrderDto.DateOrdered) &&
            error.ErrorMessage == "Date Ordered is required.");
    }
    
    private static CreateSalesOrderDto CreateValidSalesOrder()
    {
        return new CreateSalesOrderDto
        {
            Customer = "Test Customer",
            InvoiceNumber = "5000001",
            Details = [
                new CreateSoDetailDto
                {
                    SalesOrderId = Guid.Empty,
                    LineSequence = 1,
                    ItemId = Guid.NewGuid(),
                    UnitPrice = 600m,
                    Quantity = 10m
                },
                new CreateSoDetailDto
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