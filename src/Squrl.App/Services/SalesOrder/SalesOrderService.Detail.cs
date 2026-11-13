using Microsoft.EntityFrameworkCore;
using NodaTime;
using Squrl.App.Common;
using Squrl.App.Enums;
using Squrl.App.Features.SalesOrderDetails.DTOs;
using Squrl.App.Features.SalesOrderDetails.Mapping;
using Squrl.App.Models;

namespace Squrl.App.Services.SalesOrder;

public partial class SalesOrderService
{
    public async Task<Result<GetManySoDetailsDto>> GetSoDetailsAsync(
        Guid salesOrderId,
        SortDirection orderDirection = SortDirection.Ascending,
        CancellationToken cancellationToken = default)
    {
        IQueryable<SalesOrderDetail> query = _dataContext.SalesOrderDetails
            .AsNoTracking()
            .Include(s => s.SalesOrder)
            .Include(s => s.Item);
        
        query = query.Where(s => s.SalesOrder.Id == salesOrderId);
        
        if (orderDirection == SortDirection.Descending)
        {
            query = query
                .OrderByDescending(s => s.LineSequence)
                .ThenByDescending(s => EF.Property<Instant>(s, "DateCreated"))
                .ThenBy(s => s.Id);
        }
        else
        {
            query = query
                .OrderBy(s => s.LineSequence)
                .ThenBy(s => EF.Property<Instant>(s, "DateCreated"))
                .ThenBy(s => s.Id);
        }
        
        IReadOnlyList<SalesOrderDetail> existingSoDetails = await query
            .ToListAsync(cancellationToken);
        
        GetManySoDetailsDto payload = new(
            1,
            existingSoDetails.Count,
            existingSoDetails.Count,
            existingSoDetails.Select(SoDetailMapper.ToDto).ToList());

        return Result<GetManySoDetailsDto>.Ok(payload);
    }

    public async Task<Result<GetSoDetailDto>> AddSoDetailAsync(Guid salesOrderId, CreateSoDetailDto soDetail, CancellationToken cancellationToken = default)
    {
        Func<CancellationToken, Task<Result<GetSoDetailDto>>> action = async ct =>
        {
            Models.SalesOrder? salesOrder = await _dataContext.SalesOrders
                .FirstOrDefaultAsync(s => s.Id == salesOrderId, ct);

            if (salesOrder == null)
            {
                return Result<GetSoDetailDto>
                    .Fail("Sales order not found.")
                    .WithFailureType(FailureType.NotFound);
            }
        
            Item? item = await _dataContext.Items
                .FirstOrDefaultAsync(i => i.Id == soDetail.ItemId, ct);

            if (item == null)
            {
                return Result<GetSoDetailDto>
                    .Fail("Selected item not found.")
                    .WithFailureType(FailureType.NotFound);
            }
        
            int nextLineSequence = await _dataContext.SalesOrderDetails
                .AsNoTracking()
                .Where(s => s.SalesOrder.Id == salesOrderId)
                .CountAsync(ct) + 1;

            if (soDetail.LineSequence != nextLineSequence)
            {
                return Result<GetSoDetailDto>
                    .Fail("Line sequence must be consecutive.")
                    .WithFailureType(FailureType.Validation);
            }
        
            SalesOrderDetail newSoDetail = new()
            {
                Id = Guid.NewGuid(),
                SalesOrder = salesOrder,
                LineSequence = soDetail.LineSequence ?? 0,
                Item = item,
                Quantity = soDetail.Quantity ?? 0m,
                UnitPrice = soDetail.UnitPrice ?? 0m
            };
        
            await _dataContext.SalesOrderDetails.AddAsync(newSoDetail, ct);

            return Result<GetSoDetailDto>.Ok(SoDetailMapper.ToDto(newSoDetail));
        };
        
        return await _transactionManager.ExecuteAsync(action, cancellationToken);
    }

    public async Task<Result<GetSoDetailDto>> GetSoDetailByIdAsync(Guid id, CancellationToken cancellationToken = default)
    {
        SalesOrderDetail? existingSoDetail = await _dataContext.SalesOrderDetails
            .AsNoTracking()
            .Include(s => s.SalesOrder)
            .Include(s => s.Item)
            .Where(s => s.Id == id)
            .FirstOrDefaultAsync(cancellationToken);

        if (existingSoDetail == null)
        {
            return Result<GetSoDetailDto>.Fail("SO detail not found.")
                .WithFailureType(FailureType.NotFound);
        }
        
        return Result<GetSoDetailDto>.Ok(SoDetailMapper.ToDto(existingSoDetail));
    }

    public async Task<Result<GetSoDetailDto>> UpdateSoDetailAsync(Guid id, UpdateSoDetailDto soDetail, CancellationToken cancellationToken = default)
    {
        Func<CancellationToken, Task<Result<GetSoDetailDto>>> action = async ct =>
        {
            SalesOrderDetail? existingSoDetail = await _dataContext.SalesOrderDetails
            .Include(s => s.SalesOrder)
            .Include(s => s.Item)
            .FirstOrDefaultAsync(s => s.Id == id, ct);
        
            if (existingSoDetail == null)
            {
                return Result<GetSoDetailDto>.Fail("SO detail not found.")
                    .WithFailureType(FailureType.NotFound);
            }
            
            Models.SalesOrder? salesOrder = null;

            if (soDetail.SalesOrderId != null
                && soDetail.SalesOrderId != existingSoDetail.SalesOrder.Id)
            {
                salesOrder = await _dataContext.SalesOrders
                    .FirstOrDefaultAsync(s => s.Id == soDetail.SalesOrderId, ct);
            
                if (salesOrder == null)
                {
                    return Result<GetSoDetailDto>.Fail("Selected sales order not found.")
                        .WithFailureType(FailureType.NotFound);
                }
            }
            
            Item? item = null;

            if (soDetail.ItemId != null
                && soDetail.ItemId != existingSoDetail.Item.Id)
            {
                item = await _dataContext.Items
                    .FirstOrDefaultAsync(i => i.Id == soDetail.ItemId, ct);
            
                if (item == null)
                {
                    return Result<GetSoDetailDto>.Fail("Selected item not found.")
                        .WithFailureType(FailureType.NotFound);
                }
            }
            
            if (salesOrder == null)
            {
                if (soDetail.LineSequence != null && soDetail.LineSequence != existingSoDetail.LineSequence)
                {
                    return Result<GetSoDetailDto>.Fail("Line sequence must be consecutive.")
                        .WithFailureType(FailureType.Validation);
                }
            }
            else
            {
                int nextLineSequence = await _dataContext.SalesOrderDetails
                    .AsNoTracking()
                    .Where(s => s.SalesOrder.Id == soDetail.SalesOrderId)
                    .CountAsync(ct) + 1;

                if (soDetail.LineSequence != nextLineSequence)
                {
                    return Result<GetSoDetailDto>
                        .Fail("Line sequence must be consecutive.")
                        .WithFailureType(FailureType.Validation);
                }
            }
            
            existingSoDetail.SalesOrder = salesOrder ?? existingSoDetail.SalesOrder;
            existingSoDetail.Item = item ?? existingSoDetail.Item;
            existingSoDetail.Quantity = soDetail.Quantity ?? existingSoDetail.Quantity;
            existingSoDetail.UnitPrice = soDetail.UnitPrice ?? existingSoDetail.UnitPrice;

            return Result<GetSoDetailDto>.Ok(SoDetailMapper.ToDto(existingSoDetail));
        };
        
        return await _transactionManager.ExecuteAsync(action, cancellationToken);
    }

    public async Task<Result<GetSoDetailDto>> RemoveSoDetailAsync(Guid id, CancellationToken cancellationToken = default)
    {
        Func<CancellationToken, Task<Result<GetSoDetailDto>>> action = async ct =>
        {
            SalesOrderDetail? existingSoDetail = await _dataContext.SalesOrderDetails
                .Include(s => s.SalesOrder)
                .Include(s => s.Item)
                .FirstOrDefaultAsync(s => s.Id == id, ct);
        
            if (existingSoDetail == null)
            {
                return Result<GetSoDetailDto>.Fail("SO detail not found.")
                    .WithFailureType(FailureType.NotFound);
            }
                   
            // Shift all subsequent line sequences down by 1.
            await _dataContext.SalesOrderDetails
                .Where(s => s.SalesOrder.Id == existingSoDetail.SalesOrder.Id
                            && s.LineSequence > existingSoDetail.LineSequence)
                .ExecuteUpdateAsync(setters => setters
                        .SetProperty(s => s.LineSequence, s => s.LineSequence - 1),
                    ct);
                
            _dataContext.SalesOrderDetails.Remove(existingSoDetail);
                
            return Result<GetSoDetailDto>.Ok(SoDetailMapper.ToDto(existingSoDetail));
        };
        
        return await _transactionManager.ExecuteAsync(action, cancellationToken);
    }
}