using Microsoft.EntityFrameworkCore;
using NodaTime;
using Squrl.App.Common;
using Squrl.App.Data;
using Squrl.App.Enums;
using Squrl.App.Features.PurchaseOrderDetails.DTOs;
using Squrl.App.Features.PurchaseOrders.DTOs;
using Squrl.App.Features.PurchaseOrders.Mapping;
using Squrl.App.Infrastructure.TransactionManager;
using Squrl.App.Models;
using Squrl.App.Services.Inventory;

namespace Squrl.App.Services.PurchaseOrder;

public partial class PurchaseOrderService : IPurchaseOrderService
{
    private readonly DataContext _dataContext;
    private readonly ITransactionManager _transactionManager;
    private readonly IInventoryService _inventoryService;
    private readonly IClock _clock;

    public PurchaseOrderService(DataContext dataContext,
        ITransactionManager transactionManager,
        IInventoryService inventoryService,
        IClock clock)
    {
        _dataContext = dataContext;
        _transactionManager = transactionManager;
        _inventoryService = inventoryService;
        _clock = clock;
    }

    public async Task<Result<GetManyPurchaseOrdersDto>> GetManyPurchaseOrdersAsync(
        string? search = null,
        Guid? supplierId = null,
        bool isPending = false,
        bool isReceived = false,
        bool isCanceled = false,
        bool hasFailed = false,
        bool isReturned = false,
        int pageNumber = 1,
        int pageSize = 10,
        SortDirection orderDirection = SortDirection.Ascending,
        CancellationToken cancellationToken = default)
    {
        IQueryable<Models.PurchaseOrder> query = _dataContext.PurchaseOrders
            .AsNoTracking()
            .Include(p => p.Supplier);
        
        if (!string.IsNullOrWhiteSpace(search))
        {
            string pattern = $"%{search}%";

            query = query.Where(p =>
                EF.Functions.Like(p.Id.ToString(), pattern) ||
                EF.Functions.Like(p.InvoiceNumber, pattern));
        }
        
        if (supplierId.HasValue)
        {
            query = query.Where(p => p.Supplier != null && p.Supplier.Id == supplierId);
        }

        List<PurchaseOrderStatus> statuses = new();

        if (isPending)
        {
            statuses.AddRange([
                PurchaseOrderStatus.Pending,
                PurchaseOrderStatus.ToOrder,
                PurchaseOrderStatus.Ordered,
                PurchaseOrderStatus.InTransit
            ]);
        }

        if (isReceived)
        {
            statuses.Add(PurchaseOrderStatus.Received);
        }

        if (isCanceled)
        {
            statuses.Add(PurchaseOrderStatus.Canceled);
        }

        if (hasFailed)
        {
            statuses.AddRange([
                PurchaseOrderStatus.Failed,
                PurchaseOrderStatus.Lost
            ]);
        }

        if (isReturned)
        {
            statuses.Add(PurchaseOrderStatus.Returned);
        }

        if (statuses.Count > 0)
        {
            query = query.Where(p => statuses.Contains(p.Status));
        }

        if (orderDirection == SortDirection.Descending)
        {
            query = query
                .OrderByDescending(p => EF.Property<Instant>(p, "DateCreated"))
                .ThenBy(p => p.Id);
        }
        else
        {
            query = query
                .OrderBy(p => EF.Property<Instant>(p, "DateCreated"))
                .ThenBy(p => p.Id);
        }
        
        int totalCount = await query.CountAsync(cancellationToken);

        IReadOnlyList<Models.PurchaseOrder> existingPurchaseOrders = await query
            .Skip((pageNumber - 1) * pageSize)
            .Take(pageSize)
            .ToListAsync(cancellationToken);
        
        GetManyPurchaseOrdersDto payload = new(
            pageNumber,
            pageSize,
            totalCount,
            existingPurchaseOrders.Select(PurchaseOrderMapper.ToDto).ToList());

        return Result<GetManyPurchaseOrdersDto>.Ok(payload);
    }

    public async Task<Result<GetPurchaseOrderDto>> CreatePurchaseOrderAsync(CreatePurchaseOrderDto purchaseOrder, CancellationToken cancellationToken = default)
    {
        Func<CancellationToken, Task<Result<GetPurchaseOrderDto>>> action = async ct =>
        {
            Models.Supplier? supplier = await _dataContext.Suppliers
                .FirstOrDefaultAsync(s => s.Id == purchaseOrder.SupplierId, ct);

            if (supplier == null)
            {
                return Result<GetPurchaseOrderDto>
                    .Fail("Selected supplier not found.")
                    .WithFailureType(FailureType.NotFound);
            }

            Models.PurchaseOrder newPurchaseOrder = new()
            {
                Id = Guid.NewGuid(),
                Supplier = supplier,
                InvoiceNumber = purchaseOrder.InvoiceNumber,
                Status = purchaseOrder.Status ?? default,
                DateOrdered = purchaseOrder.DateOrdered ?? _clock.GetCurrentInstant(),
                DateRequired = purchaseOrder.DateRequired,
                DateShipped = purchaseOrder.DateShipped
            };

            await _dataContext.PurchaseOrders.AddAsync(newPurchaseOrder, ct);

            foreach (CreatePoDetailDto poDetail in purchaseOrder.Details ?? [])
            {
                Item? item = await _dataContext.Items
                    .FirstOrDefaultAsync(i => i.Id == poDetail.ItemId, ct);

                if (item == null)
                {
                    return Result<GetPurchaseOrderDto>
                        .Fail($"Selected item for PO detail {poDetail.LineSequence} not found.")
                        .WithFailureType(FailureType.NotFound);
                }

                PurchaseOrderDetail newPoDetail = new()
                {
                    Id = Guid.NewGuid(),
                    PurchaseOrder = newPurchaseOrder,
                    LineSequence = poDetail.LineSequence ?? 0,
                    Item = item,
                    Quantity = poDetail.Quantity ?? 0m,
                    UnitPrice = poDetail.UnitPrice ?? 0m
                };

                await _dataContext.PurchaseOrderDetails.AddAsync(newPoDetail, ct);
            }

            return Result<GetPurchaseOrderDto>.Ok(PurchaseOrderMapper.ToDto(newPurchaseOrder));
        };
        
        return await _transactionManager.ExecuteAsync(action, cancellationToken);
    }

    public async Task<Result<GetPurchaseOrderDto>> GetPurchaseOrderByIdAsync(Guid id, CancellationToken cancellationToken = default)
    {
        Models.PurchaseOrder? existingPurchaseOrder = await _dataContext.PurchaseOrders
            .AsNoTracking()
            .Include(p => p.Supplier)
            .Where(p => p.Id == id)
            .FirstOrDefaultAsync(cancellationToken);
        
        if (existingPurchaseOrder == null)
        {
            return Result<GetPurchaseOrderDto>.Fail("Purchase order not found.")
                .WithFailureType(FailureType.NotFound);
        }

        return Result<GetPurchaseOrderDto>.Ok(PurchaseOrderMapper.ToDto(existingPurchaseOrder));
    }

    public async Task<Result<GetPurchaseOrderDto>> UpdatePurchaseOrderAsync(
        Guid id,
        UpdatePurchaseOrderDto purchaseOrder,
        CancellationToken cancellationToken = default)
    {
        Func<CancellationToken, Task<Result<GetPurchaseOrderDto>>> action = async ct =>
            {
                Models.PurchaseOrder? existingPurchaseOrder = await _dataContext.PurchaseOrders
                    .Include(p => p.Supplier)
                    .FirstOrDefaultAsync(p => p.Id == id, ct);
        
                if (existingPurchaseOrder == null)
                {
                    return Result<GetPurchaseOrderDto>
                        .Fail("Purchase order not found.")
                        .WithFailureType(FailureType.NotFound);
                }
            
                Models.Supplier? supplier = null;
    
                if (purchaseOrder.SupplierId != null
                    && purchaseOrder.SupplierId != existingPurchaseOrder.Supplier?.Id)
                {
                    supplier = await _dataContext.Suppliers
                        .FirstOrDefaultAsync(s => s.Id == purchaseOrder.SupplierId, ct);
        
                    if (supplier == null)
                    {
                        return Result<GetPurchaseOrderDto>.Fail("Selected supplier not found.")
                            .WithFailureType(FailureType.NotFound);
                    }
                }
                
                existingPurchaseOrder.Supplier = supplier;
                existingPurchaseOrder.InvoiceNumber = purchaseOrder.InvoiceNumber ?? existingPurchaseOrder.InvoiceNumber;
                existingPurchaseOrder.Status = purchaseOrder.Status ?? existingPurchaseOrder.Status;
                existingPurchaseOrder.DateOrdered = purchaseOrder.DateOrdered ?? existingPurchaseOrder.DateOrdered;
                existingPurchaseOrder.DateRequired = purchaseOrder.DateRequired ?? existingPurchaseOrder.DateRequired;
                existingPurchaseOrder.DateShipped = purchaseOrder.DateShipped ?? existingPurchaseOrder.DateShipped;
                
                IReadOnlyList<PurchaseOrderDetail> existingPoDetails = await _dataContext.PurchaseOrderDetails
                    .Include(p => p.Item)
                    .Where(p => p.PurchaseOrder.Id == id)
                    .OrderBy(p => p.LineSequence)
                    .ToListAsync(ct);
                
                foreach (UpdatePoDetailDto poDetail in purchaseOrder.Details ?? [])
                {
                    PurchaseOrderDetail? existingPoDetail = existingPoDetails
                        .FirstOrDefault(p => p.LineSequence == poDetail.LineSequence);
                    
                    if (existingPoDetail is not null)
                    {
                        Item? item = null;
                        
                        if (poDetail.ItemId != null
                            && poDetail.ItemId != existingPoDetail.Item.Id)
                        {
                            item = await _dataContext.Items
                                .FirstOrDefaultAsync(i => i.Id == poDetail.ItemId, ct);
                            
                            if (item == null)
                            {
                                return Result<GetPurchaseOrderDto>
                                    .Fail($"Selected item for PO detail {poDetail.LineSequence} not found.")
                                    .WithFailureType(FailureType.NotFound);
                            }
                        }
                        
                        existingPoDetail.Item = item ?? existingPoDetail.Item;
                        existingPoDetail.Quantity = poDetail.Quantity ?? existingPoDetail.Quantity;
                        existingPoDetail.UnitPrice = poDetail.UnitPrice ?? existingPoDetail.UnitPrice;
                    }
                    else
                    {
                        Item? item = await _dataContext.Items
                            .FirstOrDefaultAsync(i => i.Id == poDetail.ItemId, ct);
                        
                        if (item == null)
                        {
                            return Result<GetPurchaseOrderDto>
                                .Fail($"Selected item for PO detail {poDetail.LineSequence} not found.")
                                .WithFailureType(FailureType.NotFound);
                        }
                        
                        PurchaseOrderDetail newPoDetail = new()
                        {
                            Id = Guid.NewGuid(),
                            PurchaseOrder = existingPurchaseOrder,
                            LineSequence = poDetail.LineSequence ?? 0,
                            Item = item,
                            Quantity = poDetail.Quantity ?? 0m,
                            UnitPrice = poDetail.UnitPrice ?? 0m
                        };
                        
                        await _dataContext.PurchaseOrderDetails.AddAsync(newPoDetail, ct);
                    }
                }
                
                // Delete existing PO details that are not in the request.
                foreach (PurchaseOrderDetail poDetail in existingPoDetails
                             .Where(e => (purchaseOrder.Details ?? [])
                                 .All(u => u.LineSequence != e.LineSequence)))
                {
                    _dataContext.PurchaseOrderDetails.Remove(poDetail);
                }
                
                return Result<GetPurchaseOrderDto>.Ok(PurchaseOrderMapper.ToDto(existingPurchaseOrder));
            };
    
        return await _transactionManager.ExecuteAsync(action, cancellationToken);
    }

    public async Task<Result<GetPurchaseOrderDto>> DeletePurchaseOrderAsync(Guid id, CancellationToken cancellationToken = default)
    {
        Func<CancellationToken, Task<Result<GetPurchaseOrderDto>>> action = async ct =>
        {
            Models.PurchaseOrder? existingPurchaseOrder = await _dataContext.PurchaseOrders
                .Include(p => p.Supplier)
                .FirstOrDefaultAsync(p => p.Id == id, ct);

            if (existingPurchaseOrder == null)
            {
                return Result<GetPurchaseOrderDto>.Fail("Purchase order not found.")
                    .WithFailureType(FailureType.NotFound);
            }

            _dataContext.PurchaseOrders.Remove(existingPurchaseOrder);

            return Result<GetPurchaseOrderDto>.Ok(PurchaseOrderMapper.ToDto(existingPurchaseOrder));
        };
        
        return await _transactionManager.ExecuteAsync(action, cancellationToken);
    }
}