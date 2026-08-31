using MediatR;
using Squrl.App.Data;
using Squrl.App.Features.Suppliers.Commands;
using Squrl.App.Models;

namespace Squrl.App.Features.Suppliers.Handlers;

public class CreateSupplierHandler : IRequestHandler<CreateSupplierCommand, Supplier?>
{
    private readonly DataContext _dataContext;

    public CreateSupplierHandler(DataContext dataContext)
    {
        _dataContext = dataContext;
    }

    public async Task<Supplier?> Handle(CreateSupplierCommand request, CancellationToken cancellationToken)
    {
        Supplier newSupplier = new()
        {
            Id = Guid.NewGuid(),
            Name = request.Supplier.Name ?? "",
            Description = request.Supplier.Description,
        };
        
        await _dataContext.Suppliers.AddAsync(newSupplier, cancellationToken);
        
        return await _dataContext.SaveChangesAsync(cancellationToken) > 0 ? newSupplier : null;
    }
}