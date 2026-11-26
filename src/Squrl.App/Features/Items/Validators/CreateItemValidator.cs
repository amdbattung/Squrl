using FluentValidation;
using Squrl.App.Extensions;
using Squrl.App.Features.Items.DTOs;
using Squrl.App.Services.Settings;

namespace Squrl.App.Features.Items.Validators;

public class CreateItemValidator : AbstractValidator<CreateItemDto>
{
    public CreateItemValidator(ISettingsService settingsService)
    {
        RuleFor(x => x.Name)
            .NotEmpty()
            .WithMessage("{PropertyName} is required.");
        
        RuleFor(x => x.UomId)
            .NotEmpty()
            .WithName("UOM")
            .WithMessage("{PropertyName} is required.");
        
        RuleFor(x => x.Description)
            .BeNullOrNonWhitespace()
            .WithMessage("{PropertyName} must not be only whitespace.")
            .BeLatin1()
            .WithMessage("Invalid {PropertyName}.");
        
        RuleFor(x => x.Image)
            .BeNullOrNonWhitespace()
            .WithMessage("{PropertyName} must not be only whitespace.")
            .BeAscii()
            .WithMessage("Invalid {PropertyName}.");
        
        RuleFor(x => x.ListPrice)
            .GreaterThanOrEqualTo(0m)
            .WithName("List Price")
            .WithMessage("{PropertyName} must not be negative.")
            .Must(x => !x.HasValue || x.Value == decimal.Round(x.Value, 2))
            .WithName("List Price")
            .WithMessage("{PropertyName} must have no more than 2 decimal places.");
        
        RuleFor(x => x.RetailPrice)
            .GreaterThanOrEqualTo(0m)
            .WithName("Retail Price")
            .WithMessage("{PropertyName} must not be negative.")
            .Must(x => !x.HasValue || x.Value == decimal.Round(x.Value, 2))
            .WithName("Retail Price")
            .WithMessage("{PropertyName} must have no more than 2 decimal places.");
        
        RuleFor(x => x.CostPrice)
            .GreaterThanOrEqualTo(0m)
            .WithName("Cost Price")
            .WithMessage("{PropertyName} must not be negative.")
            .Must(x => !x.HasValue || x.Value == decimal.Round(x.Value, 2))
            .WithName("Cost Price")
            .WithMessage("{PropertyName} must have no more than 2 decimal places.");
        
        RuleFor(x => x.Quantity)
            .NotNull()
            .WithMessage("{PropertyName} is required.")
            .GreaterThanOrEqualTo(0m)
            .WithMessage("{PropertyName} must not be negative.")
            .Must(x => !x.HasValue ||
                       x.Value == decimal.Round(x.Value, settingsService.Current.ItemQuantityPrecision))
            .WithMessage("{PropertyName} must have no more than " +
                         $"{settingsService.Current.ItemQuantityPrecision} " +
                         "decimal places.");
        
        RuleFor(x => x.LowQuantityAlertThreshold)
            .GreaterThanOrEqualTo(0m)
            .WithName("Low Stock Threshold")
            .WithMessage("{PropertyName} must not be negative.");
        
        RuleFor(x => x.Locations)
            .NotNull()
            .WithMessage("{PropertyName} is required.")
            .Must(x => x?.Distinct(StringComparer.OrdinalIgnoreCase).Count() == x?.Count)
            .WithMessage("{PropertyName} must not contain duplicates.");
        
        RuleForEach(x => x.Locations)
            .NotEmpty()
            .WithName("Location")
            .WithMessage("Invalid {PropertyName}.")
            .BeAscii()
            .WithMessage("Invalid {PropertyName}.");
    }
}