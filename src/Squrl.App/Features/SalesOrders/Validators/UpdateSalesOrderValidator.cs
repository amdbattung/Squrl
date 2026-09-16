using FluentValidation;
using Squrl.App.Extensions;
using Squrl.App.Features.SalesOrderDetails.DTOs;
using Squrl.App.Features.SalesOrders.DTOs;

namespace Squrl.App.Features.SalesOrders.Validators;

public class UpdateSalesOrderValidator : AbstractValidator<UpdateSalesOrderDto>
{
    public UpdateSalesOrderValidator(IValidator<UpdateSoDetailDto> soDetailValidator)
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
        
        RuleFor(x => x.Details)
            .NotEmpty()
            .WithName("Sales Order Details")
            .WithMessage("{PropertyName} is required.")
            .BeSequential()
            .WithMessage("Line sequences must start at 1 and be consecutive.");
        
        RuleForEach(x => x.Details)
            .SetValidator(soDetailValidator);
        
        RuleFor(x => x.DateOrdered)
            .NotNull()
            .WithName("Date Ordered")
            .WithMessage("{PropertyName} is required.");
    }
}