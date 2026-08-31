using MediatR;
using Microsoft.EntityFrameworkCore;
using Squrl.App.Data;
using Squrl.App.Features.UnitOfMeasures.Commands;
using Squrl.App.Features.UnitOfMeasures.DTOs;
using Squrl.App.Models;

namespace Squrl.App.Features.UnitOfMeasures.Handlers;

public class UpdateUomHandler : IRequestHandler<UpdateUomCommand, UnitOfMeasure?>
{
    private readonly DataContext _dataContext;

    public UpdateUomHandler(DataContext dataContext)
    {
        _dataContext = dataContext;
    }
    
    public async Task<UnitOfMeasure?> Handle(UpdateUomCommand request, CancellationToken cancellationToken)
    {
        UnitOfMeasure? existingUom = await _dataContext.UnitOfMeasures.FirstOrDefaultAsync(u => u.Id == request.Id, cancellationToken);
        
        if (existingUom == null)
        {
            return null;
        }
        
        UpdateUomDto requestUom = request.Uom;
        
        existingUom.Name = requestUom.Name ?? existingUom.Name;
        existingUom.Code = requestUom.Code ?? existingUom.Code;
        existingUom.Description = requestUom.Name ?? existingUom.Description;
        
        await _dataContext.SaveChangesAsync(cancellationToken);
        return existingUom;
    }
}