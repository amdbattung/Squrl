using MediatR;
using Microsoft.EntityFrameworkCore;
using Squrl.App.Data;
using Squrl.App.Features.Suppliers.Queries;
using Squrl.App.Models;

namespace Squrl.App.Features.Suppliers.Handlers;

public class GetSupplierByIdHandler : IRequestHandler<GetSupplierByIdQuery, Supplier?>
{
    private readonly DataContext _dataContext;

    public GetSupplierByIdHandler(DataContext dataContext)
    {
        _dataContext = dataContext;
    }

    public async Task<Supplier?> Handle(GetSupplierByIdQuery request, CancellationToken cancellationToken)
    {
        Supplier? existingSupplier = await _dataContext.Suppliers
            .AsNoTracking()
            .Where(s => s.Id == request.Id)
            .FirstOrDefaultAsync(cancellationToken);

        return existingSupplier;
    }
}