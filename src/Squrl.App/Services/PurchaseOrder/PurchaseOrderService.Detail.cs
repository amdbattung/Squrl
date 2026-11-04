using Microsoft.EntityFrameworkCore;
using NodaTime;
using Squrl.App.Common;
using Squrl.App.Enums;
using Squrl.App.Features.PurchaseOrderDetails.DTOs;
using Squrl.App.Features.PurchaseOrderDetails.Mapping;
using Squrl.App.Models;

namespace Squrl.App.Services.PurchaseOrder;

public partial class PurchaseOrderService
{
    public async Task<Result<GetManyPoDetailsDto>> GetPoDetailsAsync(
        Guid purchaseOrderId,
        SortDirection orderDirection = SortDirection.Ascending,
        CancellationToken cancellationToken = default)
    {
        IQueryable<PurchaseOrderDetail> query = _dataContext.PurchaseOrderDetails
            .AsNoTracking()
            .Include(p => p.Item);
        
        query = query.Where(p => p.PurchaseOrder.Id == purchaseOrderId);
        
        if (orderDirection == SortDirection.Descending)
        {
            query = query
                .OrderByDescending(p => p.LineSequence)
                .ThenByDescending(p => EF.Property<Instant>(p, "DateCreated"))
                .ThenBy(p => p.Id);
        }
        else
        {
            query = query
                .OrderBy(p => p.LineSequence)
                .ThenBy(p => EF.Property<Instant>(p, "DateCreated"))
                .ThenBy(p => p.Id);
        }
        
        IReadOnlyList<PurchaseOrderDetail> existingPoDetails = await query
            .ToListAsync(cancellationToken);
        
        GetManyPoDetailsDto payload = new(
            1,
            existingPoDetails.Count,
            existingPoDetails.Count,
            existingPoDetails.Select(PoDetailMapper.ToDto).ToList());

        return Result<GetManyPoDetailsDto>.Ok(payload);
    }

    public async Task<Result<GetPoDetailDto>> AddPoDetailAsync(Guid purchaseOrderId, CreatePoDetailDto poDetail, CancellationToken cancellationToken = default)
    {
        Func<CancellationToken, Task<Result<GetPoDetailDto>>> action = async ct =>
        {
            Models.PurchaseOrder? purchaseOrder = await _dataContext.PurchaseOrders
                .AsNoTracking()
                .FirstOrDefaultAsync(p => p.Id == purchaseOrderId, ct);

            if (purchaseOrder == null)
            {
                return Result<GetPoDetailDto>
                    .Fail("Purchase order not found.")
                    .WithFailureType(FailureType.NotFound);
            }
        
            Item? item = await _dataContext.Items
                .AsNoTracking()
                .FirstOrDefaultAsync(i => i.Id == poDetail.ItemId, ct);

            if (item == null)
            {
                return Result<GetPoDetailDto>
                    .Fail("Selected item not found.")
                    .WithFailureType(FailureType.NotFound);
            }
        
            int nextLineSequence = await _dataContext.PurchaseOrderDetails
                .AsNoTracking()
                .Where(p => p.PurchaseOrder.Id == purchaseOrderId)
                .CountAsync(ct) + 1;

            if (poDetail.LineSequence != nextLineSequence)
            {
                return Result<GetPoDetailDto>
                    .Fail("Line sequence must be consecutive.")
                    .WithFailureType(FailureType.Validation);
            }
        
            PurchaseOrderDetail newPoDetail = new()
            {
                Id = Guid.NewGuid(),
                PurchaseOrder = purchaseOrder,
                LineSequence = poDetail.LineSequence ?? 0,
                Item = item,
                UnitPrice = poDetail.UnitPrice ?? 0m,
                Quantity = poDetail.Quantity ?? 0m
            };
        
            await _dataContext.PurchaseOrderDetails.AddAsync(newPoDetail, ct);

            return Result<GetPoDetailDto>.Ok(PoDetailMapper.ToDto(newPoDetail));
        };
        
        return await _transactionManager.ExecuteAsync(action, cancellationToken);
    }

    public async Task<Result<GetPoDetailDto>> GetPoDetailByIdAsync(Guid id, CancellationToken cancellationToken = default)
    {
        PurchaseOrderDetail? existingPoDetail = await _dataContext.PurchaseOrderDetails
            .AsNoTracking()
            .Include(p => p.PurchaseOrder)
            .Include(p => p.Item)
            .Where(p => p.Id == id)
            .FirstOrDefaultAsync(cancellationToken);

        if (existingPoDetail == null)
        {
            return Result<GetPoDetailDto>.Fail("PO detail not found.")
                .WithFailureType(FailureType.NotFound);
        }
        
        return Result<GetPoDetailDto>.Ok(PoDetailMapper.ToDto(existingPoDetail));
    }

    public async Task<Result<GetPoDetailDto>> UpdatePoDetailAsync(Guid id, UpdatePoDetailDto poDetail, CancellationToken cancellationToken = default)
    {
        Func<CancellationToken, Task<Result<GetPoDetailDto>>> action = async ct =>
        {
            PurchaseOrderDetail? existingPoDetail = await _dataContext.PurchaseOrderDetails
            .Include(p => p.PurchaseOrder)
            .Include(p => p.Item)
            .FirstOrDefaultAsync(p => p.Id == id, ct);
        
            if (existingPoDetail == null)
            {
                return Result<GetPoDetailDto>.Fail("PO detail not found.")
                    .WithFailureType(FailureType.NotFound);
            }
            
            Models.PurchaseOrder? purchaseOrder = null;

            if (poDetail.PurchaseOrderId != null
                && poDetail.PurchaseOrderId != existingPoDetail.PurchaseOrder.Id)
            {
                purchaseOrder = await _dataContext.PurchaseOrders
                    .AsNoTracking()
                    .FirstOrDefaultAsync(p => p.Id == poDetail.PurchaseOrderId, ct);
            
                if (purchaseOrder == null)
                {
                    return Result<GetPoDetailDto>.Fail("Selected purchase order not found.")
                        .WithFailureType(FailureType.NotFound);
                }
            }
            
            Item? item = null;

            if (poDetail.ItemId != null
                && poDetail.ItemId != existingPoDetail.Item.Id)
            {
                item = await _dataContext.Items
                    .AsNoTracking()
                    .FirstOrDefaultAsync(i => i.Id == poDetail.ItemId, ct);
            
                if (item == null)
                {
                    return Result<GetPoDetailDto>.Fail("Selected item not found.")
                        .WithFailureType(FailureType.NotFound);
                }
            }

            if (purchaseOrder == null)
            {
                if (poDetail.LineSequence != null || poDetail.LineSequence != existingPoDetail.LineSequence)
                {
                    return Result<GetPoDetailDto>.Fail("Line sequence must be consecutive.")
                        .WithFailureType(FailureType.Validation);
                }
            }
            else
            {
                int nextLineSequence = await _dataContext.PurchaseOrderDetails
                    .AsNoTracking()
                    .Where(p => p.PurchaseOrder.Id == poDetail.PurchaseOrderId)
                    .CountAsync(ct) + 1;

                if (poDetail.LineSequence != nextLineSequence)
                {
                    return Result<GetPoDetailDto>
                        .Fail("Line sequence must be consecutive.")
                        .WithFailureType(FailureType.Validation);
                }
            }
            
            existingPoDetail.PurchaseOrder = purchaseOrder ?? existingPoDetail.PurchaseOrder;
            existingPoDetail.Item = item ?? existingPoDetail.Item;
            existingPoDetail.UnitPrice = poDetail.UnitPrice ?? existingPoDetail.Quantity;
            existingPoDetail.Quantity = poDetail.Quantity ?? existingPoDetail.Quantity;

            return Result<GetPoDetailDto>.Ok(PoDetailMapper.ToDto(existingPoDetail));
        };
        
        return await _transactionManager.ExecuteAsync(action, cancellationToken);
    }

    public async Task<Result<GetPoDetailDto>> RemovePoDetailAsync(Guid id, CancellationToken cancellationToken = default)
    {
        Func<CancellationToken, Task<Result<GetPoDetailDto>>> action = async ct =>
        {
            PurchaseOrderDetail? existingPoDetail = await _dataContext.PurchaseOrderDetails
                .Include(p => p.PurchaseOrder)
                .Include(p => p.Item)
                .FirstOrDefaultAsync(p => p.Id == id, ct);
        
            if (existingPoDetail == null)
            {
                return Result<GetPoDetailDto>.Fail("PO detail not found.")
                    .WithFailureType(FailureType.NotFound);
            }
                   
            // Shift all subsequent line sequences down by 1.
            await _dataContext.PurchaseOrderDetails
                .Where(p => p.PurchaseOrder.Id == existingPoDetail.PurchaseOrder.Id
                            && p.LineSequence > existingPoDetail.LineSequence)
                .ExecuteUpdateAsync(setters => setters
                        .SetProperty(p => p.LineSequence, p => p.LineSequence - 1),
                    ct);
                
            _dataContext.PurchaseOrderDetails.Remove(existingPoDetail);
                
            return Result<GetPoDetailDto>.Ok(PoDetailMapper.ToDto(existingPoDetail));
        };
        
        return await _transactionManager.ExecuteAsync(action, cancellationToken);
    }
}