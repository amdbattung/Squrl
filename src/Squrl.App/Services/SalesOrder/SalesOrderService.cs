using FluentValidation;
using FluentValidation.Results;
using MediatR;
using NodaTime;
using Squrl.App.Common;
using Squrl.App.Enums;
using Squrl.App.Features.Items.DTOs;
using Squrl.App.Features.Items.Queries;
using Squrl.App.Features.SalesOrderDetails.Commands;
using Squrl.App.Features.SalesOrderDetails.DTOs;
using Squrl.App.Features.SalesOrderDetails.Queries;
using Squrl.App.Features.SalesOrders.Commands;
using Squrl.App.Features.SalesOrders.DTOs;
using Squrl.App.Features.SalesOrders.Mapping;
using Squrl.App.Features.SalesOrders.Queries;
using Squrl.App.Infrastructure.TransactionManager;
using Squrl.App.Models;
using Squrl.App.Services.Inventory;

namespace Squrl.App.Services.SalesOrder;

public partial class SalesOrderService : ISalesOrderService
{
    private readonly IMediator _mediator;
    private readonly ITransactionManager _transactionManager;
    private readonly IValidator<CreateSalesOrderDto> _createSalesOrderValidator;
    private readonly IValidator<UpdateSalesOrderDto> _updateSalesOrderValidator;
    private readonly IValidator<CreateSoDetailDto> _createSoDetailValidator;
    private readonly IValidator<UpdateSoDetailDto> _updateSoDetailValidator;
    private readonly IInventoryService _inventoryService;
    private readonly IClock _clock;

    public SalesOrderService(IMediator mediator,
        ITransactionManager transactionManager,
        IValidator<CreateSalesOrderDto> createSalesOrderValidator,
        IValidator<UpdateSalesOrderDto> updateSalesOrderValidator,
        IValidator<CreateSoDetailDto> createSoDetailValidator,
        IValidator<UpdateSoDetailDto> updateSoDetailValidator,
        IInventoryService inventoryService,
        IClock clock)
    {
        _mediator = mediator;
        _transactionManager = transactionManager;
        _createSalesOrderValidator = createSalesOrderValidator;
        _updateSalesOrderValidator = updateSalesOrderValidator;
        _createSoDetailValidator = createSoDetailValidator;
        _updateSoDetailValidator = updateSoDetailValidator;
        _inventoryService = inventoryService;
        _clock = clock;
    }
    
    public async Task<Result<GetManySalesOrdersDto>> GetManySalesOrdersAsync(string? query = null,
        string? customer = null,
        int? pageNumber = null,
        int? pageSize = null,
        SortDirection orderDirection = SortDirection.Ascending,
        CancellationToken cancellationToken = default)
    {
        try
        {
            pageNumber = pageNumber >= 1 ? pageNumber : 1;
            pageSize = pageSize is >= 1 and <= 50 ? pageSize : 50;
            
            var result = await _mediator.Send(new GetManySalesOrdersQuery(
                query,
                customer,
                pageNumber,
                pageSize,
                orderDirection), cancellationToken);
            
            GetManySalesOrdersDto payload = new GetManySalesOrdersDto(
                result.PageSize,
                result.PageNumber,
                result.TotalCount,
                result.Value.Select(SalesOrderMapper.ToDto).ToList());
            
            return Result<GetManySalesOrdersDto>.Ok(payload);
        }
        catch (OperationCanceledException)
        {
            throw;
        }
        catch (Exception e)
        {
            return Result<GetManySalesOrdersDto>.Fail("Failed to retrieve sales orders.")
                .WithException(e)
                .WithFailureType(FailureType.Exception);
        }
    }

    public async Task<Result<GetSalesOrderDto>> CreateSalesOrderAsync(CreateSalesOrderDto salesOrder, CancellationToken cancellationToken = default)
    {
        try
        {
            ValidationResult soValidationResult = await _createSalesOrderValidator.ValidateAsync(salesOrder, cancellationToken);
            
            if (!soValidationResult.IsValid)
            {
                return Result<GetSalesOrderDto>.Fail(soValidationResult.Errors
                        .Select(e => e.ErrorMessage)
                        .ToArray())
                    .WithFailureType(FailureType.Validation);
            }
            
            var failure = Result<GetSalesOrderDto>.Fail("Failed to return sales order.")
                .WithFailureType(FailureType.BusinessLogic);
            
            Models.SalesOrder? result = await _transactionManager.ExecuteAsync(async ct =>
            {
                Models.SalesOrder? salesOrderResult = await _mediator.Send(
                    new CreateSalesOrderCommand(salesOrder), ct);
                
                if (salesOrderResult is null)
                {
                    return null;
                }

                foreach (CreateSoDetailDto soDetail in salesOrder.Details ?? [])
                {
                    soDetail.SalesOrderId = salesOrderResult.Id;
                    SalesOrderDetail? soDetailResult = await _mediator
                        .Send(new CreateSoDetailCommand(soDetail), ct);
                    
                    if (soDetailResult is null)
                    {
                        return null;
                    }
                    
                    Item? item = await _mediator
                        .Send(new GetItemByIdQuery(soDetailResult.Item.Id), ct);

                    if (item is null)
                    {
                        return null;
                    }

                    if (item.Quantity < soDetail.Quantity)
                    {
                        failure.WithMessage($"Not enough quantity to remove from item {item.Name}.");
                        return null;
                    }
                    
                    Result<GetItemDto> removeStocksResult = await _inventoryService
                        .RemoveStocksAsync(soDetailResult.Item.Id, soDetailResult.Quantity, ct);

                    if (!removeStocksResult.IsSuccess)
                    {
                        return null;
                    }
                }

                return salesOrderResult;
            }, cancellationToken);
            
            if (result == null)
            {
                return failure;
            }
            
            return Result<GetSalesOrderDto>.Ok(SalesOrderMapper.ToDto(result));
        }
        catch (OperationCanceledException)
        {
            throw;
        }
        catch (Exception e)
        {
            return Result<GetSalesOrderDto>.Fail("Failed to create sales order.")
                .WithException(e)
                .WithFailureType(FailureType.Exception);
        }
    }

    public async Task<Result<GetSalesOrderDto>> GetSalesOrderByIdAsync(Guid id, CancellationToken cancellationToken = default)
    {
        try
        {
            if (id == Guid.Empty)
            {
                return Result<GetSalesOrderDto>.Fail("Null or invalid ID.")
                    .WithFailureType(FailureType.Validation);
            }
            
            Models.SalesOrder? result = await _mediator.Send(new GetSalesOrderByIdQuery(id), cancellationToken);
            
            if (result == null)
            {
                return Result<GetSalesOrderDto>.Fail("Sales order not found.")
                    .WithFailureType(FailureType.NotFound);
            }
            
            return Result<GetSalesOrderDto>.Ok(SalesOrderMapper.ToDto(result));
        }
        catch (OperationCanceledException)
        {
            throw;
        }
        catch (Exception e)
        {
            return Result<GetSalesOrderDto>.Fail("Failed to retrieve sales order.")
                .WithException(e)
                .WithFailureType(FailureType.Exception);
        }
    }

    public async Task<Result<GetSalesOrderDto>> UpdateSalesOrderAsync(Guid id, UpdateSalesOrderDto salesOrder, CancellationToken cancellationToken = default)
    {
        try
        {
            if (id == Guid.Empty)
            {
                return Result<GetSalesOrderDto>.Fail("Null or invalid ID.")
                    .WithFailureType(FailureType.Validation);
            }
            
            ValidationResult soValidationResult = await _updateSalesOrderValidator.ValidateAsync(salesOrder, cancellationToken);
            
            if (!soValidationResult.IsValid)
            {
                return Result<GetSalesOrderDto>.Fail(soValidationResult.Errors
                        .Select(e => e.ErrorMessage)
                        .ToArray())
                    .WithFailureType(FailureType.Validation);
            }
            
            Models.SalesOrder? result = await _transactionManager.ExecuteAsync(async ct =>
            {
                IReadOnlyList<SalesOrderDetail> existingSoDetails = (await _mediator
                        .Send(new GetManySoDetailsQuery(SalesOrderId: id, PageSize: null), ct))
                    .Value;
                
                Models.SalesOrder? salesOrderResult = await _mediator
                    .Send(new UpdateSalesOrderCommand(id, salesOrder), ct);
                
                if (salesOrderResult is null)
                {
                    return null;
                }
                
                foreach (UpdateSoDetailDto soDetail in salesOrder.Details ?? [])
                {
                    soDetail.SalesOrderId = null;
                    
                    SalesOrderDetail? existingSoDetail = existingSoDetails
                        .FirstOrDefault(s => s.LineSequence == soDetail.LineSequence);

                    SalesOrderDetail? soDetailResult;

                    if (existingSoDetail is not null)
                    {
                        soDetailResult = await _mediator
                            .Send(new UpdateSoDetailCommand(existingSoDetail.Id, soDetail), ct);
                    }
                    else
                    {
                        soDetailResult = await _mediator
                            .Send(new CreateSoDetailCommand(new CreateSoDetailDto
                            {
                                SalesOrderId = id,
                                LineSequence = soDetail.LineSequence,
                                ItemId = soDetail.ItemId,
                                UnitPrice = soDetail.UnitPrice,
                                Quantity = soDetail.Quantity
                            }), ct);
                    }
                    
                    if (soDetailResult is null)
                    {
                        return null;
                    }
                }

                foreach (SalesOrderDetail soDetail in existingSoDetails
                             .Where(e => (salesOrder.Details ?? [])
                                 .All(u => u.LineSequence != e.LineSequence)))
                {
                    SalesOrderDetail? soDetailResult = await _mediator
                        .Send(new DeleteSoDetailCommand(soDetail.Id), ct);
                    
                    if (soDetailResult is null)
                    {
                        return null;
                    }
                }

                return salesOrderResult;
            }, cancellationToken);
            
            if (result == null)
            {
                return Result<GetSalesOrderDto>.Fail("Failed to update sales order.")
                    .WithFailureType(FailureType.NotFound);
            }
            
            return Result<GetSalesOrderDto>.Ok(SalesOrderMapper.ToDto(result));
        }
        catch (OperationCanceledException)
        {
            throw;
        }
        catch (Exception e)
        {
            return Result<GetSalesOrderDto>.Fail("Failed to update sales order.")
                .WithException(e)
                .WithFailureType(FailureType.Exception);
        }
    }

    public async Task<Result<GetSalesOrderDto>> DeleteSalesOrderAsync(Guid id, CancellationToken cancellationToken = default)
    {
        try
        {
            if (id == Guid.Empty)
            {
                return Result<GetSalesOrderDto>.Fail("Null or invalid ID.")
                    .WithFailureType(FailureType.Validation);
            }
            
            Models.SalesOrder? result = await _transactionManager.ExecuteAsync(async ct =>
            {
                var soDetails = (await _mediator
                    .Send(new GetManySoDetailsQuery(SalesOrderId: id, PageSize: null), ct)).Value;
                
                foreach (SalesOrderDetail soDetail in soDetails)
                {
                    Result<GetItemDto> addStocksResult = await _inventoryService
                        .AddStocksAsync(soDetail.Item.Id, soDetail.Quantity, ct);

                    if (!addStocksResult.IsSuccess)
                    {
                        return null;
                    }
                }
                
                Models.SalesOrder? deleteSalesOrderResult = await _mediator
                    .Send(new DeleteSalesOrderCommand(id), ct);

                if (deleteSalesOrderResult is null)
                {
                    return null;
                }

                return deleteSalesOrderResult;
            }, cancellationToken);
            
            if (result == null)
            {
                return Result<GetSalesOrderDto>.Fail("Sales order not found.")
                    .WithFailureType(FailureType.NotFound);
            }
            
            return Result<GetSalesOrderDto>.Ok(SalesOrderMapper.ToDto(result));
        }
        catch (OperationCanceledException)
        {
            throw;
        }
        catch (Exception e)
        {
            return Result<GetSalesOrderDto>.Fail("Failed to delete sales order.")
                .WithException(e)
                .WithFailureType(FailureType.Exception);
        }
    }
}