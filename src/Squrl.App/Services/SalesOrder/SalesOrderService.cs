using Microsoft.EntityFrameworkCore;
using NodaTime;
using Squrl.App.Common;
using Squrl.App.Data;
using Squrl.App.Enums;
using Squrl.App.Features.Items.DTOs;
using Squrl.App.Features.SalesOrderDetails.DTOs;
using Squrl.App.Features.SalesOrders.DTOs;
using Squrl.App.Features.SalesOrders.Mapping;
using Squrl.App.Infrastructure.TransactionManager;
using Squrl.App.Models;
using Squrl.App.Services.Inventory;

namespace Squrl.App.Services.SalesOrder;

public partial class SalesOrderService : ISalesOrderService
{
    private readonly DataContext _dataContext;
    private readonly ITransactionManager _transactionManager;
    private readonly IInventoryService _inventoryService;
    private readonly IClock _clock;

    public SalesOrderService(DataContext dataContext,
        ITransactionManager transactionManager,
        IInventoryService inventoryService,
        IClock clock)
    {
        _dataContext = dataContext;
        _transactionManager = transactionManager;
        _inventoryService = inventoryService;
        _clock = clock;
    }
    
    public async Task<Result<GetManySalesOrdersDto>> GetManySalesOrdersAsync(
        string? search = null,
        string? customer = null,
        int pageNumber = 1,
        int pageSize = 10,
        SortDirection orderDirection = SortDirection.Ascending,
        CancellationToken cancellationToken = default)
    {
        IQueryable<Models.SalesOrder> query = _dataContext.SalesOrders
            .AsNoTracking();
        
        if (!string.IsNullOrWhiteSpace(search))
        {
            string pattern = $"%{search}%";

            query = query.Where(s =>
                EF.Functions.Like(s.Id.ToString(), pattern) ||
                EF.Functions.Like(s.InvoiceNumber, pattern) ||
                EF.Functions.Like(s.Customer, pattern));
        }
        
        if (!string.IsNullOrWhiteSpace(customer))
        {
            query = query.Where(s => s.Customer == customer);
        }
        
        if (orderDirection == SortDirection.Descending)
        {
            query = query
                .OrderByDescending(s => EF.Property<Instant>(s, "DateCreated"))
                .ThenBy(s => s.Id);
        }
        else
        {
            query = query
                .OrderBy(s => EF.Property<Instant>(s, "DateCreated"))
                .ThenBy(s => s.Id);
        }
        
        int totalCount = await query.CountAsync(cancellationToken);

        IReadOnlyList<Models.SalesOrder> existingSalesOrders = await query
            .Skip((pageNumber - 1) * pageSize)
            .Take(pageSize)
            .ToListAsync(cancellationToken);
        
        GetManySalesOrdersDto payload = new(
            pageNumber,
            pageSize,
            totalCount,
            existingSalesOrders.Select(SalesOrderMapper.ToDto).ToList());

        return Result<GetManySalesOrdersDto>.Ok(payload);
    }

    public async Task<Result<GetSalesOrderDto>> CreateSalesOrderAsync(CreateSalesOrderDto salesOrder, CancellationToken cancellationToken = default)
    {
        Func<CancellationToken, Task<Result<GetSalesOrderDto>>> action = async ct =>
        {
            Models.SalesOrder newSalesOrder = new()
            {
                Id = Guid.NewGuid(),
                Customer = salesOrder.Customer,
                InvoiceNumber = salesOrder.InvoiceNumber,
                DateOrdered = salesOrder.DateOrdered ?? _clock.GetCurrentInstant(),
            };

            await _dataContext.SalesOrders.AddAsync(newSalesOrder, ct);

            foreach (CreateSoDetailDto soDetail in salesOrder.Details ?? [])
            {
                Item? item = await _dataContext.Items
                    .AsNoTracking()
                    .FirstOrDefaultAsync(i => i.Id == soDetail.ItemId, ct);

                if (item == null)
                {
                    return Result<GetSalesOrderDto>
                        .Fail($"Selected item for SO detail {soDetail.LineSequence} not found.")
                        .WithFailureType(FailureType.NotFound);
                }

                SalesOrderDetail newSoDetail = new()
                {
                    Id = Guid.NewGuid(),
                    SalesOrder = newSalesOrder,
                    LineSequence = soDetail.LineSequence ?? 0,
                    Item = item,
                    UnitPrice = soDetail.UnitPrice ?? 0m,
                    Quantity = soDetail.Quantity ?? 0m
                };

                await _dataContext.SalesOrderDetails.AddAsync(newSoDetail, ct);
                
                if (item.Quantity < soDetail.Quantity)
                {
                    return Result<GetSalesOrderDto>
                        .Fail($"Failed to remove stocks in SO detail {soDetail.LineSequence}; " +
                              $"Not enough stocks to remove from item {item.Name}.")
                        .WithFailureType(FailureType.BusinessLogic);
                }
                
                Result<GetItemDto> removeStocksResult = await _inventoryService
                    .RemoveStocksAsync(soDetail.ItemId ?? Guid.Empty, soDetail.Quantity ?? 0, ct);
                
                if (!removeStocksResult.IsSuccess)
                {
                    return Result<GetSalesOrderDto>
                        .Fail($"Failed to remove stocks in SO detail {soDetail.LineSequence}; " +
                              $"{removeStocksResult.Message}.")
                        .WithException(removeStocksResult.Exception)
                        .WithFailureType(removeStocksResult.FailureType);
                }
            }

            return Result<GetSalesOrderDto>.Ok(SalesOrderMapper.ToDto(newSalesOrder));
        };
        
        return await _transactionManager.ExecuteAsync(action, cancellationToken);
    }

    public async Task<Result<GetSalesOrderDto>> GetSalesOrderByIdAsync(Guid id, CancellationToken cancellationToken = default)
    {
        Models.SalesOrder? existingSalesOrder = await _dataContext.SalesOrders
            .AsNoTracking()
            .Where(s => s.Id == id)
            .FirstOrDefaultAsync(cancellationToken);

        if (existingSalesOrder == null)
        {
            return Result<GetSalesOrderDto>.Fail("Sales order not found.")
                .WithFailureType(FailureType.NotFound);
        }

        return Result<GetSalesOrderDto>.Ok(SalesOrderMapper.ToDto(existingSalesOrder));
    }

    public async Task<Result<GetSalesOrderDto>> UpdateSalesOrderAsync(Guid id, UpdateSalesOrderDto salesOrder, CancellationToken cancellationToken = default)
    {
        Func<CancellationToken, Task<Result<GetSalesOrderDto>>> action = async ct =>
        {
            Models.SalesOrder? existingSalesOrder = await _dataContext.SalesOrders
                .FirstOrDefaultAsync(p => p.Id == id, ct);
            
            if (existingSalesOrder == null)
            {
                return Result<GetSalesOrderDto>
                    .Fail("Sales order not found.")
                    .WithFailureType(FailureType.NotFound);
            }
            
            existingSalesOrder.Customer = salesOrder.Customer;
            existingSalesOrder.InvoiceNumber = salesOrder.InvoiceNumber ?? existingSalesOrder.InvoiceNumber;
            existingSalesOrder.DateOrdered = salesOrder.DateOrdered ?? existingSalesOrder.DateOrdered;
            
            IReadOnlyList<SalesOrderDetail> existingSoDetails = await _dataContext.SalesOrderDetails
                .Include(s => s.Item)
                .Where(s => s.SalesOrder.Id == id)
                .OrderBy(s => s.LineSequence)
                .ToListAsync(ct);

            foreach (UpdateSoDetailDto soDetail in salesOrder.Details ?? [])
            {
                SalesOrderDetail? existingSoDetail = existingSoDetails
                    .FirstOrDefault(p => p.LineSequence == soDetail.LineSequence);

                if (existingSoDetail is not null)
                {
                    Item? item = null;

                    if (soDetail.ItemId != null
                        && soDetail.ItemId != existingSoDetail.Item.Id)
                    {
                        item = await _dataContext.Items
                            .AsNoTracking()
                            .FirstOrDefaultAsync(i => i.Id == soDetail.ItemId, ct);
                        
                        if (item == null)
                        {
                            return Result<GetSalesOrderDto>
                                .Fail($"Selected item for SO detail {soDetail.LineSequence} not found.")
                                .WithFailureType(FailureType.NotFound);
                        }
                    }
                    
                    existingSoDetail.Item = item ?? existingSoDetail.Item;
                    existingSoDetail.UnitPrice = soDetail.UnitPrice ?? existingSoDetail.UnitPrice;
                    existingSoDetail.Quantity = soDetail.Quantity ?? existingSoDetail.Quantity;
                }
                else
                {
                    Item? item = await _dataContext.Items
                        .AsNoTracking()
                        .FirstOrDefaultAsync(i => i.Id == soDetail.ItemId, ct);
                        
                    if (item == null)
                    {
                        return Result<GetSalesOrderDto>
                            .Fail($"Selected item for SO detail {soDetail.LineSequence} not found.")
                            .WithFailureType(FailureType.NotFound);
                    }
                    
                    SalesOrderDetail newSoDetail = new()
                    {
                        Id = Guid.NewGuid(),
                        SalesOrder = existingSalesOrder,
                        LineSequence = soDetail.LineSequence ?? 0,
                        Item = item,
                        UnitPrice = soDetail.UnitPrice ?? 0m,
                        Quantity = soDetail.Quantity ?? 0m
                    };
                    
                    await _dataContext.SalesOrderDetails.AddAsync(newSoDetail, ct);
                }
            }
            
            // Delete existing PO details that are not in the request.
            foreach (SalesOrderDetail soDetail in existingSoDetails
                         .Where(e => (salesOrder.Details ?? [])
                             .All(u => u.LineSequence != e.LineSequence)))
            {
                _dataContext.SalesOrderDetails.Remove(soDetail);
            }
            
            return Result<GetSalesOrderDto>.Ok(SalesOrderMapper.ToDto(existingSalesOrder));
        };
        
        return await _transactionManager.ExecuteAsync(action, cancellationToken);
    }

    public async Task<Result<GetSalesOrderDto>> DeleteSalesOrderAsync(Guid id, CancellationToken cancellationToken = default)
    {
        Func<CancellationToken, Task<Result<GetSalesOrderDto>>> action = async ct =>
        {
            Models.SalesOrder? existingSalesOrder = await _dataContext.SalesOrders
                .FirstOrDefaultAsync(s => s.Id == id, ct);
            
            if (existingSalesOrder == null)
            {
                return Result<GetSalesOrderDto>.Fail("Sales order not found.")
                    .WithFailureType(FailureType.NotFound);
            }
            
            IReadOnlyList<SalesOrderDetail> existingSoDetails = await _dataContext.SalesOrderDetails
                .AsNoTracking()
                .Include(s => s.Item)
                .Where(s => s.SalesOrder.Id == id)
                .OrderBy(s => s.LineSequence)
                .ToListAsync(ct);
            
            foreach (SalesOrderDetail existingSoDetail in existingSoDetails)
            {
                Result<GetItemDto> addStocksResult = await _inventoryService
                    .AddStocksAsync(existingSoDetail.Item.Id, existingSoDetail.Quantity, ct);

                if (!addStocksResult.IsSuccess)
                {
                    return Result<GetSalesOrderDto>
                        .Fail($"Failed to add stocks in PO detail {existingSoDetail.LineSequence}; " +
                              $"{addStocksResult.Message}.")
                        .WithException(addStocksResult.Exception)
                        .WithFailureType(addStocksResult.FailureType);
                }
            }

            _dataContext.SalesOrders.Remove(existingSalesOrder);

            return Result<GetSalesOrderDto>.Ok(SalesOrderMapper.ToDto(existingSalesOrder));
        };
        
        return await _transactionManager.ExecuteAsync(action, cancellationToken);
    }
}