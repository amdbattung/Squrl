using FluentValidation;
using Squrl.App.Extensions;
using Squrl.App.Features.UnitOfMeasures.DTOs;

namespace Squrl.App.Features.UnitOfMeasures.Validators;

public class UpdateUomValidator : AbstractValidator<UpdateUomDto>
{
    public UpdateUomValidator()
    {
        RuleFor(x => x.Name)
            .NotEmpty()
            .WithMessage("{PropertyName} is required.");
        
        RuleFor(x => x.Code)
            .NotEmpty()
            .WithMessage("{PropertyName} is required.");
        
        RuleFor(x => x.Description)
            .BeNullOrNonWhitespace()
            .WithMessage("{PropertyName} must not be only whitespace.")
            .BeAscii()
            .WithMessage("Invalid {PropertyName}.");
    }
}