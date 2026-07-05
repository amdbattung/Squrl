using MediatR;
using Microsoft.EntityFrameworkCore;
using Squrl.App.Data;
using Squrl.App.Features.UnitOfMeasures.Commands;
using Squrl.App.Models;

namespace Squrl.App.Features.UnitOfMeasures.Handlers;

public class DeleteUomHandler : IRequestHandler<DeleteUomCommand, UnitOfMeasure?>
{
    private readonly DataContext _dataContext;

    public DeleteUomHandler(DataContext dataContext)
    {
        _dataContext = dataContext;
    }
    
    public async Task<UnitOfMeasure?> Handle(DeleteUomCommand request, CancellationToken cancellationToken)
    {
        UnitOfMeasure? existingUom = await _dataContext.UnitOfMeasures
            .FirstOrDefaultAsync(u => u.Id == request.Id, cancellationToken);
        
        if (existingUom == null)
        {
            return null;
        }
        
        _dataContext.UnitOfMeasures.Remove(existingUom);
        
        return await _dataContext.SaveChangesAsync(cancellationToken) > 0 ? existingUom : null;
    }
}