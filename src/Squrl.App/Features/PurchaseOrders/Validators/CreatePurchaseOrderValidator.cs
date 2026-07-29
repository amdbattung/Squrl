using FluentValidation;
using Squrl.App.Features.PurchaseOrders.DTOs;

namespace Squrl.App.Features.PurchaseOrders.Validators;

public class CreatePurchaseOrderValidator : AbstractValidator<CreatePurchaseOrderDto>
{
    public CreatePurchaseOrderValidator()
    {
        RuleFor(x => x.Status)
            .NotNull()
            .WithMessage("{PropertyName} is required.")
            .IsInEnum()
            .WithMessage("{PropertyName} is invalid.");

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