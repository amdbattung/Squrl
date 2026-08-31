using FluentValidation;
using Squrl.App.Extensions;
using Squrl.App.Features.Suppliers.DTOs;

namespace Squrl.App.Features.Suppliers.Validators;

public class CreateSupplierValidator : AbstractValidator<CreateSupplierDto>
{
    public CreateSupplierValidator()
    {
        RuleFor(x => x.Name)
            .NotEmpty()
            .WithMessage("{PropertyName} is required.");
        
        RuleFor(x => x.Description)
            .BeNullOrNonWhitespace()
            .WithMessage("{PropertyName} must not be only whitespace.")
            .BeAscii()
            .WithMessage("Invalid {PropertyName}.");
    }
}