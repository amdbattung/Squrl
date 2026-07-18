using FluentValidation;
using Squrl.App.Features.PurchaseOrderDetails.DTOs;

namespace Squrl.App.Features.PurchaseOrderDetails.Validators;

public class CreatePoDetailValidator : AbstractValidator<CreatePoDetailDto>
{
    public CreatePoDetailValidator()
    {
        RuleFor(x => x.PurchaseOrderId)
            .NotEmpty()
            .WithName("Purchase Order")
            .WithMessage("{PropertyName} is required.");
        
        RuleFor(x => x.ItemId)
            .NotEmpty()
            .WithName("Item")
            .WithMessage("{PropertyName} is required.");
        
        RuleFor(x => x.Quantity)
            .NotNull()
            .WithMessage("{PropertyName} is required.")
            .GreaterThan(0m)
            .WithMessage("{PropertyName} must not be zero or negative.");
    }
}