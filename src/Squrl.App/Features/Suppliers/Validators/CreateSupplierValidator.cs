using FluentValidation;
using Squrl.App.Features.Suppliers.DTOs;

namespace Squrl.App.Features.Suppliers.Validators;

public class CreateSupplierValidator : AbstractValidator<CreateSupplierDto>
{
    public CreateSupplierValidator()
    {
        
    }
}