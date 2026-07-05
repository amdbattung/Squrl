using FluentValidation;
using Squrl.App.Extensions;
using Squrl.App.Features.Items.DTOs;

namespace Squrl.App.Features.Items.Validators;

public class UpdateItemValidator : AbstractValidator<UpdateItemDto>
{
    public UpdateItemValidator()
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
            .WithMessage("{PropertyName} must not only be whitespace.")
            .BeAscii()
            .WithMessage("Invalid {PropertyName}.");
        
        RuleFor(x => x.Quantity)
            .NotNull()
            .WithMessage("{PropertyName} is required.")
            .GreaterThanOrEqualTo(0m)
            .WithMessage("{PropertyName} must not be negative.");
        
        RuleFor(x => x.LowQuantityAlertThreshold)
            .GreaterThanOrEqualTo(0m)
            .WithName("Low Stock Threshold")
            .WithMessage("{PropertyName} must not be negative.");
        
        RuleFor(x => x.Locations)
            .NotNull()
            .WithMessage("{PropertyName} is required.");
        
        RuleForEach(x => x.Locations)
            .NotEmpty()
            .WithName("Location")
            .WithMessage("Invalid {PropertyName}.")
            .BeAscii()
            .WithMessage("Invalid {PropertyName}.");
    }
}