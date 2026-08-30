using FluentValidation;
using Squrl.App.Extensions;
using Squrl.App.Features.PurchaseOrderDetails.DTOs;
using Squrl.App.Features.PurchaseOrders.DTOs;

namespace Squrl.App.Features.PurchaseOrders.Validators;

public class CreatePurchaseOrderValidator : AbstractValidator<CreatePurchaseOrderDto>
{
    public CreatePurchaseOrderValidator(IValidator<CreatePoDetailDto> poDetailValidator)
    {
        RuleFor(x => x.Status)
            .NotNull()
            .WithMessage("{PropertyName} is required.")
            .IsInEnum()
            .WithMessage("{PropertyName} is invalid.");
        
        RuleFor(x => x.PurchaseOrderDetails)
            .NotEmpty()
            .WithName("Purchase Order Details")
            .WithMessage("{PropertyName} is required.")
            .BeSequential()
            .WithMessage("Line sequences must start at 1 and be consecutive.");
        
        RuleForEach(x => x.PurchaseOrderDetails)
            .SetValidator(poDetailValidator);

        RuleFor(x => x.DateOrdered)
            .NotNull()
            .WithName("Date Ordered")
            .WithMessage("{PropertyName} is required.");
        
        RuleFor(x => x.DateRequired)
            .GreaterThanOrEqualTo(x => x.DateOrdered)
            .WithName("Date Required")
            .WithMessage("{PropertyName} must be after date ordered.");
        
        RuleFor(x => x.DateShipped)
            .GreaterThanOrEqualTo(x => x.DateOrdered)
            .WithName("Date Shipped")
            .WithMessage("{PropertyName} must be after date ordered.");
    }
}