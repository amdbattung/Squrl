using MediatR;
using Microsoft.EntityFrameworkCore;
using Squrl.App.Data;
using Squrl.App.Features.Suppliers.Commands;
using Squrl.App.Features.Suppliers.DTOs;
using Squrl.App.Features.UnitOfMeasures.DTOs;
using Squrl.App.Models;

namespace Squrl.App.Features.Suppliers.Handlers;

public class UpdateSupplierHandler : IRequestHandler<UpdateSupplierCommand, Supplier?>
{
    private readonly DataContext _dataContext;

    public UpdateSupplierHandler(DataContext dataContext)
    {
        _dataContext = dataContext;
    }

    public async Task<Supplier?> Handle(UpdateSupplierCommand request, CancellationToken cancellationToken)
    {
        Supplier? existingSupplier = await _dataContext.Suppliers.FirstOrDefaultAsync(s => s.Id == request.Id, cancellationToken);
        
        if (existingSupplier == null)
        {
            return null;
        }
        
        UpdateSupplierDto requestSupplier = request.Supplier;
        
        existingSupplier.Name = requestSupplier.Name ?? existingSupplier.Name;
        existingSupplier.Description = requestSupplier.Description ?? existingSupplier.Description;
        
        await _dataContext.SaveChangesAsync(cancellationToken);
        return existingSupplier;
    }
}