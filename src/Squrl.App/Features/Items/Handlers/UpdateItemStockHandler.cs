using MediatR;
using Microsoft.EntityFrameworkCore;
using Squrl.App.Common;
using Squrl.App.Data;
using Squrl.App.Enums;
using Squrl.App.Features.Items.Commands;
using Squrl.App.Models;

namespace Squrl.App.Features.Items.Handlers;

public class UpdateItemStockHandler : IRequestHandler<UpdateItemStockCommand, Item?>
{
    private readonly DataContext _dataContext;

    public UpdateItemStockHandler(DataContext dataContext)
    {
        _dataContext = dataContext;
    }
    
    public async Task<Item?> Handle(UpdateItemStockCommand request, CancellationToken cancellationToken)
    {
        // Preferred handler for updating stocks.
        
        if (request.Operation == StockOperation.Set)
        {
            if (!request.AllowNegative && request.Value < 0m)
            {
                return null;
            }
        }
        else if (request.Value <= 0m)
        {
            return null;
        }
        
        int affected = request.Operation switch
        {
            StockOperation.Set => await _dataContext.Database.ExecuteSqlInterpolatedAsync(
                $"""
                 UPDATE Items
                 SET Quantity = {request.Value}
                 WHERE Id = {request.ItemId}
                 """,
                cancellationToken),
            
            StockOperation.Add => await _dataContext.Database.ExecuteSqlInterpolatedAsync(
                $"""
                 UPDATE Items
                 SET Quantity = Quantity + {request.Value}
                 WHERE Id = {request.ItemId}
                 """,
                cancellationToken),

            StockOperation.Subtract => request.AllowNegative
                ? await _dataContext.Database.ExecuteSqlInterpolatedAsync(
                    $"""
                     UPDATE Items
                     SET Quantity = Quantity - {request.Value}
                     WHERE Id = {request.ItemId}
                     """,
                    cancellationToken)

                : await _dataContext.Database.ExecuteSqlInterpolatedAsync(
                    $"""
                     UPDATE Items
                     SET Quantity = Quantity - {request.Value}
                     WHERE Id = {request.ItemId}
                       AND Quantity >= {request.Value}
                     """,
                    cancellationToken),

            StockOperation.Multiply => await _dataContext.Database.ExecuteSqlInterpolatedAsync(
                $"""
                UPDATE Items
                SET Quantity = Quantity * {request.Value}
                WHERE Id = {request.ItemId}
                """,
                cancellationToken),

            StockOperation.Divide when request.Value != 0 => await _dataContext.Database.ExecuteSqlInterpolatedAsync(
                $"""
                UPDATE Items
                SET Quantity = Quantity / {request.Value}
                WHERE Id = {request.ItemId}
                """,
                cancellationToken),

            _ => 0
        };

        if (affected <= 0)
        {
            return null;
        }

        return await _dataContext.Items
            .AsNoTracking()
            .Include(i => i.Uom)
            .FirstOrDefaultAsync(i => i.Id == request.ItemId, cancellationToken);
    }
}