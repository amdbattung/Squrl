using FluentValidation;
using FluentValidation.Results;
using MediatR;
using NodaTime;
using Squrl.App.Common;
using Squrl.App.Enums;
using Squrl.App.Features.PurchaseOrderDetails.Commands;
using Squrl.App.Features.PurchaseOrderDetails.DTOs;
using Squrl.App.Features.PurchaseOrderDetails.Queries;
using Squrl.App.Features.PurchaseOrders.Commands;
using Squrl.App.Features.PurchaseOrders.DTOs;
using Squrl.App.Features.PurchaseOrders.Mapping;
using Squrl.App.Features.PurchaseOrders.Queries;
using Squrl.App.Models;
using Squrl.App.Services.Inventory;
using Squrl.App.Services.TransactionManager;

namespace Squrl.App.Services.PurchaseOrder;

public partial class PurchaseOrderService : IPurchaseOrderService
{
    private readonly IMediator _mediator;
    private readonly ITransactionManager _transactionManager;
    private readonly IValidator<CreatePurchaseOrderDto> _createPurchaseOrderValidator;
    private readonly IValidator<UpdatePurchaseOrderDto> _updatePurchaseOrderValidator;
    private readonly IValidator<CreatePoDetailDto> _createPoDetailValidator;
    private readonly IValidator<UpdatePoDetailDto> _updatePoDetailValidator;
    private readonly IInventoryService _inventoryService;
    private readonly IClock _clock;

    public PurchaseOrderService(IMediator mediator,
        ITransactionManager transactionManager,
        IValidator<CreatePurchaseOrderDto> createPurchaseOrderValidator,
        IValidator<UpdatePurchaseOrderDto> updatePurchaseOrderValidator,
        IValidator<CreatePoDetailDto> createPoDetailValidator,
        IValidator<UpdatePoDetailDto> updatePoDetailValidator,
        IInventoryService inventoryService,
        IClock clock)
    {
        _mediator = mediator;
        _transactionManager = transactionManager;
        _createPurchaseOrderValidator = createPurchaseOrderValidator;
        _updatePurchaseOrderValidator = updatePurchaseOrderValidator;
        _createPoDetailValidator = createPoDetailValidator;
        _updatePoDetailValidator = updatePoDetailValidator;
        _inventoryService = inventoryService;
        _clock = clock;
    }

    public async Task<Result<GetManyPurchaseOrdersDto>> GetManyPurchaseOrdersAsync(string? query = null,
        Guid? supplierId = null,
        bool isPending = false,
        bool isReceived = false,
        bool isCancelled = false,
        bool hasFailed = false,
        bool isReturned = false,
        SortDirection orderDirection = SortDirection.Ascending,
        int? pageNumber = null,
        int? pageSize = null,
        CancellationToken cancellationToken = default)
    {
        try
        {
            pageNumber = pageNumber >= 1 ? pageNumber : null;
            pageSize = pageSize is >= 1 and <= 50 ? pageSize : null;
            
            var result = await _mediator.Send(new GetManyPurchaseOrdersQuery(query,
                supplierId,
                isPending,
                isReceived,
                isCancelled,
                hasFailed,
                isReturned,
                orderDirection,
                pageNumber,
                pageSize), cancellationToken);
            
            GetManyPurchaseOrdersDto payload = new GetManyPurchaseOrdersDto(
                result.PageSize,
                result.PageNumber,
                result.TotalCount,
                result.Value.Select(PurchaseOrderMapper.ToDto).ToList());
            
            return Result<GetManyPurchaseOrdersDto>.Ok(payload);
        }
        catch (OperationCanceledException)
        {
            throw;
        }
        catch (Exception e)
        {
            return Result<GetManyPurchaseOrdersDto>.Fail("Failed to retrieve purchase orders.")
                .WithException(e)
                .WithFailureType(FailureType.Exception);
        }
    }

    public async Task<Result<GetPurchaseOrderDto>> CreatePurchaseOrderAsync(CreatePurchaseOrderDto purchaseOrder, CancellationToken cancellationToken = default)
    {
        try
        {
            purchaseOrder.Status ??= PurchaseOrderStatus.Pending;
            
            ValidationResult poValidationResult = await _createPurchaseOrderValidator.ValidateAsync(purchaseOrder, cancellationToken);
            
            if (!poValidationResult.IsValid)
            {
                return Result<GetPurchaseOrderDto>.Fail(poValidationResult.Errors
                        .Select(e => e.ErrorMessage)
                        .ToArray())
                    .WithFailureType(FailureType.Validation);
            }

            List<CreatePoDetailDto>? poDetails = purchaseOrder.PurchaseOrderDetails;
            
            for (int index = 0; index < poDetails?.Count; index++)
            {
                CreatePoDetailDto poDetail = poDetails[index];

                poDetail.PurchaseOrderId = Guid.Empty;
                
                ValidationResult? poDetailsValidationResult = await _createPoDetailValidator
                    .ValidateAsync(poDetail, cancellationToken);

                if (!poDetailsValidationResult.IsValid)
                {
                    string[] errors = new[]
                        {
                            $"PO detail {index} is invalid"
                        }
                        .Concat(poDetailsValidationResult.Errors.Select(e => e.ErrorMessage))
                        .ToArray();

                    return Result<GetPurchaseOrderDto>.Fail(errors)
                        .WithFailureType(FailureType.Validation);
                }
            }
            
            Models.PurchaseOrder? result = await _transactionManager.ExecuteAsync(async ct =>
            {
                Models.PurchaseOrder? purchaseOrderResult = await _mediator.Send(
                    new CreatePurchaseOrderCommand(purchaseOrder), ct);
                
                if (purchaseOrderResult is null)
                {
                    return null;
                }

                foreach (CreatePoDetailDto poDetail in poDetails ?? [])
                {
                    poDetail.PurchaseOrderId = purchaseOrderResult.Id;
                    PurchaseOrderDetail? poDetailResult = await _mediator
                        .Send(new CreatePoDetailCommand(poDetail), ct);
                    
                    if (poDetailResult is null)
                    {
                        return null;
                    }
                }

                return purchaseOrderResult;
            }, cancellationToken);
            
            if (result == null)
            {
                return Result<GetPurchaseOrderDto>.Fail("Failed to create purchase order.")
                    .WithFailureType(FailureType.BusinessLogic);
            }
            
            return Result<GetPurchaseOrderDto>.Ok(PurchaseOrderMapper.ToDto(result));
        }
        catch (OperationCanceledException)
        {
            throw;
        }
        catch (Exception e)
        {
            return Result<GetPurchaseOrderDto>.Fail("Failed to create purchase order.")
                .WithException(e)
                .WithFailureType(FailureType.Exception);
        }
    }

    public async Task<Result<GetPurchaseOrderDto>> GetPurchaseOrderByIdAsync(Guid id, CancellationToken cancellationToken = default)
    {
        try
        {
            if (id == Guid.Empty)
            {
                return Result<GetPurchaseOrderDto>.Fail("Null or invalid ID.")
                    .WithFailureType(FailureType.Validation);
            }
            
            Models.PurchaseOrder? result = await _mediator.Send(new GetPurchaseOrderByIdQuery(id), cancellationToken);
            
            if (result == null)
            {
                return Result<GetPurchaseOrderDto>.Fail("Purchase order not found.")
                    .WithFailureType(FailureType.NotFound);
            }
            
            return Result<GetPurchaseOrderDto>.Ok(PurchaseOrderMapper.ToDto(result));
        }
        catch (OperationCanceledException)
        {
            throw;
        }
        catch (Exception e)
        {
            return Result<GetPurchaseOrderDto>.Fail("Failed to retrieve purchase order.")
                .WithException(e)
                .WithFailureType(FailureType.Exception);
        }
    }

    public async Task<Result<GetPurchaseOrderDto>> UpdatePurchaseOrderAsync(Guid id,
        UpdatePurchaseOrderDto purchaseOrder,
        CancellationToken cancellationToken = default)
    {
        try
        {
            if (id == Guid.Empty)
            {
                return Result<GetPurchaseOrderDto>.Fail("Null or invalid ID.")
                    .WithFailureType(FailureType.Validation);
            }
            
            ValidationResult poValidationResult = await _updatePurchaseOrderValidator.ValidateAsync(purchaseOrder, cancellationToken);
            
            if (!poValidationResult.IsValid)
            {
                return Result<GetPurchaseOrderDto>.Fail(poValidationResult.Errors
                        .Select(e => e.ErrorMessage)
                        .ToArray())
                    .WithFailureType(FailureType.Validation);
            }
            
            List<UpdatePoDetailDto>? updatePoDetails = purchaseOrder.PurchaseOrderDetails;
            
            for (int index = 0; index < updatePoDetails?.Count; index++)
            {
                UpdatePoDetailDto poDetail = updatePoDetails[index];

                poDetail.PurchaseOrderId = Guid.Empty;
                
                ValidationResult? poDetailsValidationResult = await _updatePoDetailValidator
                    .ValidateAsync(poDetail, cancellationToken);

                if (!poDetailsValidationResult.IsValid)
                {
                    string[] errors = new[]
                        {
                            $"PO detail {index} is invalid"
                        }
                        .Concat(poDetailsValidationResult.Errors.Select(e => e.ErrorMessage))
                        .ToArray();

                    return Result<GetPurchaseOrderDto>.Fail(errors)
                        .WithFailureType(FailureType.Validation);
                }
            }
            
            IReadOnlyList<PurchaseOrderDetail> existingPoDetails = (await _mediator
                    .Send(new GetManyPoDetailsQuery(PurchaseOrderId: id, PageSize: 500), cancellationToken))
                .Value;
            
            Models.PurchaseOrder? result = await _transactionManager.ExecuteAsync(async ct =>
            {
                Models.PurchaseOrder? purchaseOrderResult = await _mediator
                    .Send(new UpdatePurchaseOrderCommand(id, purchaseOrder), ct);
                
                if (purchaseOrderResult is null)
                {
                    Console.WriteLine("FILED HERE 1");
                    return null;
                }
                
                foreach (UpdatePoDetailDto poDetail in updatePoDetails ?? [])
                {
                    PurchaseOrderDetail? existingPoDetail = existingPoDetails
                        .FirstOrDefault(p => p.LineSequence == poDetail.LineSequence);

                    PurchaseOrderDetail? poDetailResult;

                    if (existingPoDetail is not null)
                    {
                        poDetailResult = await _mediator
                            .Send(new UpdatePoDetailCommand(id, poDetail), ct);
                    }
                    else
                    {
                        poDetailResult = await _mediator
                            .Send(new CreatePoDetailCommand(new CreatePoDetailDto
                            {
                                PurchaseOrderId = id,
                                LineSequence = poDetail.LineSequence,
                                ItemId = poDetail.ItemId,
                                Quantity = poDetail.Quantity
                            }), ct);
                    }
                    
                    if (poDetailResult is null)
                    {
                        Console.WriteLine("FILED HERE 2");
                        return null;
                    }
                }

                foreach (PurchaseOrderDetail poDetail in existingPoDetails
                             .Where(e => (updatePoDetails ?? [])
                                 .All(u => u.LineSequence != e.LineSequence)))
                {
                    var poDetailResult = await _mediator
                        .Send(new DeletePoDetailCommand(poDetail.Id), ct);
                    
                    if (poDetailResult is null)
                    {
                        Console.WriteLine("FILED HERE 3");
                        return null;
                    }
                }

                return purchaseOrderResult;
            }, cancellationToken);
            
            if (result == null)
            {
                return Result<GetPurchaseOrderDto>.Fail("Failed to update purchase order.")
                    .WithFailureType(FailureType.NotFound);
            }
            
            return Result<GetPurchaseOrderDto>.Ok(PurchaseOrderMapper.ToDto(result));
        }
        catch (OperationCanceledException)
        {
            throw;
        }
        catch (Exception e)
        {
            return Result<GetPurchaseOrderDto>.Fail("Failed to update purchase order.")
                .WithException(e)
                .WithFailureType(FailureType.Exception);
        }
    }

    public async Task<Result<GetPurchaseOrderDto>> DeletePurchaseOrderAsync(Guid id, CancellationToken cancellationToken = default)
    {
        try
        {
            if (id == Guid.Empty)
            {
                return Result<GetPurchaseOrderDto>.Fail("Null or invalid ID.")
                    .WithFailureType(FailureType.Validation);
            }

            Models.PurchaseOrder? result = await _mediator.Send(new DeletePurchaseOrderCommand(id), cancellationToken);
            
            if (result == null)
            {
                return Result<GetPurchaseOrderDto>.Fail("Purchase order not found.")
                    .WithFailureType(FailureType.NotFound);
            }
            
            return Result<GetPurchaseOrderDto>.Ok(PurchaseOrderMapper.ToDto(result));
        }
        catch (OperationCanceledException)
        {
            throw;
        }
        catch (Exception e)
        {
            return Result<GetPurchaseOrderDto>.Fail("Failed to delete purchase order.")
                .WithException(e)
                .WithFailureType(FailureType.Exception);
        }
    }
}