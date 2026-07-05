using MediatR;
using Squrl.App.Data;
using Squrl.App.Features.UnitOfMeasures.Commands;
using Squrl.App.Models;

namespace Squrl.App.Features.UnitOfMeasures.Handlers;

public class CreateUomHandler : IRequestHandler<CreateUomCommand, UnitOfMeasure?>
{
    private readonly DataContext _dataContext;
    
    public CreateUomHandler(DataContext dataContext)
    {
        _dataContext = dataContext;
    }

    public async Task<UnitOfMeasure?> Handle(CreateUomCommand request, CancellationToken cancellationToken)
    {
        UnitOfMeasure newUom = new()
        {
            Id = Guid.NewGuid(),
            Name = request.Uom.Name ?? "",
            Code = request.Uom.Code ?? "",
            Description = request.Uom.Description,
        };
        
        await _dataContext.UnitOfMeasures.AddAsync(newUom, cancellationToken);
        
        return await _dataContext.SaveChangesAsync(cancellationToken) > 0 ? newUom : null;
    }
}