using FluentValidation;
using Squrl.App.Extensions;
using Squrl.App.Features.SalesOrders.DTOs;

namespace Squrl.App.Features.SalesOrders.Validators;

public class UpdateSalesOrderValidator : AbstractValidator<UpdateSalesOrderDto>
{
    public UpdateSalesOrderValidator()
    {
        RuleFor(x => x.Customer)
            .BeNullOrNonWhitespace()
            .WithMessage("{PropertyName} must not only be whitespace.")
            .BeAscii()
            .WithMessage("Invalid {PropertyName}.");
        
        RuleFor(x => x.InvoiceNumber)
            .BeNullOrNonWhitespace()
            .WithName("Invoice Number")
            .WithMessage("{PropertyName} must not only be whitespace.")
            .BeAlphanumeric()
            .WithName("Invoice Number")
            .WithMessage("Invalid {PropertyName}.");
        
        //TODO: Details
        
        RuleFor(x => x.DateOrdered)
            .NotNull()
            .WithName("Date Ordered")
            .WithMessage("{PropertyName} is required.");
    }
}