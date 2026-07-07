using MediatR;
using Microsoft.EntityFrameworkCore;
using Squrl.App.Data;
using Squrl.App.Features.Suppliers.Commands;
using Squrl.App.Models;

namespace Squrl.App.Features.Suppliers.Handlers;

public class DeleteSupplierHandler : IRequestHandler<DeleteSupplierCommand, Supplier?>
{
    private readonly DataContext _dataContext;

    public DeleteSupplierHandler(DataContext dataContext)
    {
        _dataContext = dataContext;
    }

    public async Task<Supplier?> Handle(DeleteSupplierCommand request, CancellationToken cancellationToken)
    {
        Supplier? existingSupplier = await _dataContext.Suppliers
            .FirstOrDefaultAsync(s => s.Id == request.Id, cancellationToken);
        
        if (existingSupplier == null)
        {
            return null;
        }
        
        _dataContext.Suppliers.Remove(existingSupplier);
        
        return await _dataContext.SaveChangesAsync(cancellationToken) > 0 ? existingSupplier : null;
    }
}