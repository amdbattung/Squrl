using FluentValidation;
using Squrl.App.Features.SalesOrderDetails.DTOs;
using Squrl.App.Services.Settings;

namespace Squrl.App.Features.SalesOrderDetails.Validators;

public class CreateSoDetailValidator : AbstractValidator<CreateSoDetailDto>
{
    public CreateSoDetailValidator(ISettingsService settingsService)
    {
        RuleFor(x => x.SalesOrderId)
            .NotEmpty()
            .When((_, context) => !context.RootContextData.ContainsKey("SkipSalesOrderIdValidation"))
            .WithName("Sales Order")
            .WithMessage("{PropertyName} is required.");

        RuleFor(x => x.LineSequence)
            .NotNull()
            .WithName("Line Sequence")
            .WithMessage("{PropertyName} is required.")
            .GreaterThanOrEqualTo(1)
            .WithName("Line Sequence")
            .WithMessage("{PropertyName} must start at 1.");
        
        RuleFor(x => x.ItemId)
            .NotEmpty()
            .WithName("Item")
            .WithMessage("{PropertyName} is required.");
        
        RuleFor(x => x.Quantity)
            .NotNull()
            .WithMessage("{PropertyName} is required.")
            .GreaterThan(0m)
            .WithMessage("{PropertyName} must not be zero or negative.")
            .Must(x => !x.HasValue ||
                       x.Value == decimal.Round(x.Value, settingsService.Current.ItemQuantityPrecision))
            .WithMessage("{PropertyName} must have no more than " +
                         $"{settingsService.Current.ItemQuantityPrecision} " +
                         "decimal places.");
        
        RuleFor(x => x.UnitPrice)
            .NotNull()
            .WithName("Unit Price")
            .WithMessage("{PropertyName} is required.")
            .GreaterThanOrEqualTo(0m)
            .WithName("Unit Price")
            .WithMessage("{PropertyName} must not be negative.")
            .Must(x => !x.HasValue || x.Value == decimal.Round(x.Value, 2))
            .WithName("Unit Price")
            .WithMessage("{PropertyName} must have no more than 2 decimal places.");
    }
}