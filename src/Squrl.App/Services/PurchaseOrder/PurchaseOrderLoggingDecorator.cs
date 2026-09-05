using Squrl.App.Common;
using Squrl.App.Enums;
using Squrl.App.Features.PurchaseOrderDetails.DTOs;
using Squrl.App.Features.PurchaseOrders.DTOs;

namespace Squrl.App.Services.PurchaseOrder;

public class PurchaseOrderLoggingDecorator : IPurchaseOrderService
{
    private readonly IPurchaseOrderService _purchaseOrderService;
    private readonly ILogger<PurchaseOrderLoggingDecorator> _logger;

    public PurchaseOrderLoggingDecorator(IPurchaseOrderService purchaseOrderService,
        ILogger<PurchaseOrderLoggingDecorator> logger)
    {
        _purchaseOrderService = purchaseOrderService;
        _logger = logger;
    }
    
    public async Task<Result<GetManyPurchaseOrdersDto>> GetManyPurchaseOrdersAsync(
        string? query = null,
        Guid? supplierId = null,
        bool isPending = false,
        bool isReceived = false,
        bool isCancelled = false,
        bool hasFailed = false,
        bool isReturned = false,
        int? pageNumber = null,
        int? pageSize = null,
        SortDirection orderDirection = SortDirection.Ascending,
        CancellationToken cancellationToken = default)
    {
        Result<GetManyPurchaseOrdersDto> result = await _purchaseOrderService.GetManyPurchaseOrdersAsync(
            query,
            supplierId,
            isPending,
            isReceived,
            isCancelled,
            hasFailed,
            isReturned,
            pageNumber,
            pageSize,
            orderDirection,
            cancellationToken);

        if (result.IsSuccess)
        {
            _logger.LogInformation($"Get Many Purchase Orders Request: {result.Value?.PurchaseOrders?.Count} purchase orders successfully fetched.");
        }
        else if (result.FailureType == FailureType.Exception)
        {
            _logger.LogError(result.Exception, "Get Many Purchase Orders Request: unhandled exception.");
            return Result<GetManyPurchaseOrdersDto>.Fail(result.Message ?? "Failed to retrieve purchase orders.")
                .WithFailureType(FailureType.Unexpected);
        }
        else
        {
            _logger.LogError(result.Exception, "Get Many Purchase Orders Request: failed to fetch purchase orders.");
            return Result<GetManyPurchaseOrdersDto>.Fail(result.Message ?? "Failed to retrieve purchase orders.")
                .WithFailureType(FailureType.Unexpected);
        }
        
        return result;
    }

    public async Task<Result<GetPurchaseOrderDto>> CreatePurchaseOrderAsync(CreatePurchaseOrderDto purchaseOrder, CancellationToken cancellationToken = default)
    {
        Result<GetPurchaseOrderDto> result = await _purchaseOrderService.CreatePurchaseOrderAsync(purchaseOrder, cancellationToken);

        if (result.IsSuccess)
        {
            _logger.LogInformation($"Create Purchase Order Request: purchase order {result.Value?.Id} successfully created.");
        }
        else switch (result.FailureType)
        {
            case FailureType.Validation:
                _logger.LogInformation($"Create Purchase Order Request: failed to create purchase order {result.Value?.Id}, invalid purchase order.");
                break;
            
            case FailureType.BusinessLogic:
                _logger.LogError(result.Exception, "Create Purchase Order Request: failed to create purchase order.");
                break;
            
            case FailureType.Exception:
                _logger.LogError(result.Exception, "Create Purchase Order Request: unhandled exception.");
                return Result<GetPurchaseOrderDto>.Fail(result.Message ?? "Failed to create purchase order.")
                    .WithFailureType(FailureType.Unexpected);
            
            default:
                _logger.LogError(result.Exception, "Create Purchase Order Request: failed to create purchase order.");
                return Result<GetPurchaseOrderDto>.Fail(result.Message ?? "Failed to create purchase order.")
                    .WithFailureType(FailureType.Unexpected);
        }
    
        return result;
    }

    public async Task<Result<GetPurchaseOrderDto>> GetPurchaseOrderByIdAsync(Guid id, CancellationToken cancellationToken = default)
    {
        Result<GetPurchaseOrderDto> result = await _purchaseOrderService.GetPurchaseOrderByIdAsync(id, cancellationToken);

        if (result.IsSuccess)
        {
            _logger.LogInformation($"Get Purchase Order By ID Request: purchase order {result.Value?.Id} successfully fetched.");
        }
        else switch (result.FailureType)
        {
            case FailureType.Validation:
                _logger.LogInformation("Get Purchase Order By ID Request: failed to fetch, invalid request.");
                break;
            
            case FailureType.NotFound:
                _logger.LogInformation("Get Purchase Order By ID Request: failed to fetch, purchase order not found.");
                break;
            
            case FailureType.Exception:
                _logger.LogError(result.Exception, "Get Purchase Order By ID Request: unhandled exception.");
                return Result<GetPurchaseOrderDto>.Fail(result.Message ?? "Failed to retrieve purchase order.")
                    .WithFailureType(FailureType.Unexpected);
            
            default:
                _logger.LogError(result.Exception, "Get Purchase Order By ID Request: failed to fetch purchase order.");
                return Result<GetPurchaseOrderDto>.Fail(result.Message ?? "Failed to retrieve purchase order.")
                    .WithFailureType(FailureType.Unexpected);
        }
        
        return result;
    }

    public async Task<Result<GetPurchaseOrderDto>> UpdatePurchaseOrderAsync(Guid id, UpdatePurchaseOrderDto purchaseOrder,
        CancellationToken cancellationToken = default)
    {
        Result<GetPurchaseOrderDto> result = await _purchaseOrderService.UpdatePurchaseOrderAsync(id, purchaseOrder, cancellationToken);

        if (result.IsSuccess)
        {
            _logger.LogInformation($"Update Purchase Order Request: purchase order {result.Value?.Id} successfully updated.");
        }
        else switch (result.FailureType)
        {
            case FailureType.Validation:
                _logger.LogInformation("Update Purchase Order Request: failed to update, invalid request.");
                break;
            
            case FailureType.NotFound:
                _logger.LogInformation("Update Purchase Order Request: failed to update, purchase order not found.");
                break;
            
            case FailureType.Exception:
                _logger.LogError(result.Exception, "Update Purchase Order Request: unhandled exception.");
                return Result<GetPurchaseOrderDto>.Fail(result.Message ?? "Failed to update purchase order.")
                    .WithFailureType(FailureType.Unexpected);
            
            default:
                _logger.LogError(result.Exception, "Update Purchase Order Request: failed to update purchase order.");
                return Result<GetPurchaseOrderDto>.Fail(result.Message ?? "Failed to update purchase order.")
                    .WithFailureType(FailureType.Unexpected);
        }
        
        return result;
    }

    public async Task<Result<GetPurchaseOrderDto>> DeletePurchaseOrderAsync(Guid id, CancellationToken cancellationToken = default)
    {
        Result<GetPurchaseOrderDto> result = await _purchaseOrderService.DeletePurchaseOrderAsync(id, cancellationToken);

        if (result.IsSuccess)
        {
            _logger.LogInformation($"Delete Purchase Order Request: purchase order {result.Value?.Id} successfully deleted.");
        }
        else switch (result.FailureType)
        {
            case FailureType.Validation:
                _logger.LogInformation("Delete Purchase Order Request: failed to delete, invalid request.");
                break;
            
            case FailureType.NotFound:
                _logger.LogInformation("Delete Purchase Order Request: failed to update, purchase order not found.");
                break;
            
            case FailureType.Exception:
                _logger.LogError(result.Exception, "Delete Purchase Order Request: unhandled exception.");
                return Result<GetPurchaseOrderDto>.Fail(result.Message ?? "Failed to delete purchase order.")
                    .WithFailureType(FailureType.Unexpected);
            
            default:
                _logger.LogError(result.Exception, "Delete Purchase Order Request: failed to update purchase order.");
                return Result<GetPurchaseOrderDto>.Fail(result.Message ?? "Failed to delete purchase order.")
                    .WithFailureType(FailureType.Unexpected);
        }
        
        return result;
    }

    public async Task<Result<GetManyPoDetailsDto>> GetPoDetailsAsync(Guid purchaseOrderId, SortDirection orderDirection = SortDirection.Ascending,
        CancellationToken cancellationToken = default)
    {
        Result<GetManyPoDetailsDto> result = await _purchaseOrderService.GetPoDetailsAsync(
            purchaseOrderId,
            orderDirection,
            cancellationToken);

        if (result.IsSuccess)
        {
            _logger.LogInformation($"Get Many PO Details Request: {result.Value?.PoDetails?.Count} PO details successfully fetched.");
        }
        else if (result.FailureType == FailureType.Exception)
        {
            _logger.LogError(result.Exception, "Get Many PO Details Request: unhandled exception.");
            return Result<GetManyPoDetailsDto>.Fail(result.Message ?? "Failed to retrieve PO details.")
                .WithFailureType(FailureType.Unexpected);
        }
        else
        {
            _logger.LogError(result.Exception, "Get Many PO Details Request: failed to fetch PO details.");
            return Result<GetManyPoDetailsDto>.Fail(result.Message ?? "Failed to retrieve PO details.")
                .WithFailureType(FailureType.Unexpected);
        }
        
        return result;
    }

    public async Task<Result<GetPoDetailDto>> AddPoDetailAsync(Guid purchaseOrderId, CreatePoDetailDto poDetail, CancellationToken cancellationToken = default)
    {
        Result<GetPoDetailDto> result = await _purchaseOrderService.AddPoDetailAsync(purchaseOrderId, poDetail, cancellationToken);

        if (result.IsSuccess)
        {
            _logger.LogInformation($"Create PO Detail Request: PO detail {result.Value?.Id} successfully created.");
        }
        else switch (result.FailureType)
        {
            case FailureType.Validation:
                _logger.LogInformation($"Create PO Detail Request: failed to create PO detail {result.Value?.Id}, invalid PO detail.");
                break;
            
            case FailureType.BusinessLogic:
                _logger.LogError(result.Exception, "Create PO Detail Request: failed to create PO detail.");
                break;
            
            case FailureType.Exception:
                _logger.LogError(result.Exception, "Create PO Detail Request: unhandled exception.");
                return Result<GetPoDetailDto>.Fail(result.Message ?? "Failed to create PO detail.")
                    .WithFailureType(FailureType.Unexpected);
            
            default:
                _logger.LogError(result.Exception, "Create PO Detail Request: failed to create PO detail.");
                return Result<GetPoDetailDto>.Fail(result.Message ?? "Failed to create PO detail.")
                    .WithFailureType(FailureType.Unexpected);
        }
    
        return result;
    }

    public async Task<Result<GetPoDetailDto>> GetPoDetailByIdAsync(Guid id, CancellationToken cancellationToken = default)
    {
        Result<GetPoDetailDto> result = await _purchaseOrderService.GetPoDetailByIdAsync(id, cancellationToken);

        if (result.IsSuccess)
        {
            _logger.LogInformation($"Get PO Detail By ID Request: PO detail {result.Value?.Id} successfully fetched.");
        }
        else switch (result.FailureType)
        {
            case FailureType.Validation:
                _logger.LogInformation("Get PO Detail By ID Request: failed to fetch, invalid request.");
                break;
            
            case FailureType.NotFound:
                _logger.LogInformation("Get PO Detail By ID Request: failed to fetch, PO detail not found.");
                break;
            
            case FailureType.Exception:
                _logger.LogError(result.Exception, "Get PO Detail By ID Request: unhandled exception.");
                return Result<GetPoDetailDto>.Fail(result.Message ?? "Failed to retrieve PO detail.")
                    .WithFailureType(FailureType.Unexpected);
            
            default:
                _logger.LogError(result.Exception, "Get PO Detail By ID Request: failed to fetch PO detail.");
                return Result<GetPoDetailDto>.Fail(result.Message ?? "Failed to retrieve PO detail.")
                    .WithFailureType(FailureType.Unexpected);
        }
        
        return result;
    }

    public async Task<Result<GetPoDetailDto>> UpdatePoDetailAsync(Guid id, UpdatePoDetailDto poDetail, CancellationToken cancellationToken = default)
    {
        Result<GetPoDetailDto> result = await _purchaseOrderService.UpdatePoDetailAsync(id, poDetail, cancellationToken);

        if (result.IsSuccess)
        {
            _logger.LogInformation($"Update PO Detail Request: PO detail {result.Value?.Id} successfully updated.");
        }
        else switch (result.FailureType)
        {
            case FailureType.Validation:
                _logger.LogInformation("Update PO Detail Request: failed to update, invalid request.");
                break;
            
            case FailureType.NotFound:
                _logger.LogInformation("Update PO Detail Request: failed to update, PO detail not found.");
                break;
            
            case FailureType.Exception:
                _logger.LogError(result.Exception, "Update PO Detail Request: unhandled exception.");
                return Result<GetPoDetailDto>.Fail(result.Message ?? "Failed to update PO detail.")
                    .WithFailureType(FailureType.Unexpected);
            
            default:
                _logger.LogError(result.Exception, "Update PO Detail Request: failed to update PO detail.");
                return Result<GetPoDetailDto>.Fail(result.Message ?? "Failed to update PO detail.")
                    .WithFailureType(FailureType.Unexpected);
        }
        
        return result;
    }

    public async Task<Result<GetPoDetailDto>> RemovePoDetailAsync(Guid id, CancellationToken cancellationToken = default)
    {
        Result<GetPoDetailDto> result = await _purchaseOrderService.RemovePoDetailAsync(id, cancellationToken);

        if (result.IsSuccess)
        {
            _logger.LogInformation($"Delete PO Detail Request: PO detail {result.Value?.Id} successfully deleted.");
        }
        else switch (result.FailureType)
        {
            case FailureType.Validation:
                _logger.LogInformation("Delete PO Detail Request: failed to delete, invalid request.");
                break;
            
            case FailureType.NotFound:
                _logger.LogInformation("Delete PO Detail Request: failed to update, PO detail not found.");
                break;
            
            case FailureType.Exception:
                _logger.LogError(result.Exception, "Delete PO Detail Request: unhandled exception.");
                return Result<GetPoDetailDto>.Fail(result.Message ?? "Failed to delete PO detail.")
                    .WithFailureType(FailureType.Unexpected);
            
            default:
                _logger.LogError(result.Exception, "Delete PO Detail Request: failed to update PO detail.");
                return Result<GetPoDetailDto>.Fail(result.Message ?? "Failed to delete PO detail.")
                    .WithFailureType(FailureType.Unexpected);
        }
        
        return result;
    }

    public async Task<Result<GetPurchaseOrderDto>> ReceivePurchaseOrderAsync(Guid id, CancellationToken cancellationToken = default)
    {
        Result<GetPurchaseOrderDto> result = await _purchaseOrderService.ReceivePurchaseOrderAsync(id, cancellationToken);

        if (result.IsSuccess)
        {
            _logger.LogInformation($"Receive Purchase Order Request: purchase order {result.Value?.Id} successfully received.");
        }
        else switch (result.FailureType)
        {
            case FailureType.NotFound:
                _logger.LogInformation("Receive Purchase Order Request: failed to receive, purchase order not found.");
                break;
            
            case FailureType.BusinessLogic:
                _logger.LogInformation("Receive Purchase Order Request: failed to receive purchase order.");
                break;
            
            case FailureType.Exception:
                _logger.LogError(result.Exception, "Receive Purchase Order Request: unhandled exception.");
                return Result<GetPurchaseOrderDto>.Fail(result.Message ?? "Failed to receive purchase order.")
                    .WithFailureType(FailureType.Unexpected);
            
            default:
                _logger.LogError(result.Exception, "Receive Purchase Order Request: failed to receive purchase order.");
                return Result<GetPurchaseOrderDto>.Fail(result.Message ?? "Failed to receive purchase order.")
                    .WithFailureType(FailureType.Unexpected);
        }
        
        return result;
    }

    public async Task<Result<GetPurchaseOrderDto>> CancelPurchaseOrderAsync(Guid id, CancellationToken cancellationToken = default)
    {
        Result<GetPurchaseOrderDto> result = await _purchaseOrderService.CancelPurchaseOrderAsync(id, cancellationToken);

        if (result.IsSuccess)
        {
            _logger.LogInformation($"Cancel Purchase Order Request: purchase order {result.Value?.Id} successfully cancelled.");
        }
        else switch (result.FailureType)
        {
            case FailureType.NotFound:
                _logger.LogInformation("Cancel Purchase Order Request: failed to cancel, purchase order not found.");
                break;
            
            case FailureType.BusinessLogic:
                _logger.LogInformation("Cancel Purchase Order Request: failed to cancel purchase order.");
                break;
            
            case FailureType.Exception:
                _logger.LogError(result.Exception, "Cancel Purchase Order Request: unhandled exception.");
                return Result<GetPurchaseOrderDto>.Fail(result.Message ?? "Failed to cancel purchase order.")
                    .WithFailureType(FailureType.Unexpected);
            
            default:
                _logger.LogError(result.Exception, "Cancel Purchase Order Request: failed to cancel purchase order.");
                return Result<GetPurchaseOrderDto>.Fail(result.Message ?? "Failed to cancel purchase order.")
                    .WithFailureType(FailureType.Unexpected);
        }
        
        return result;
    }

    public async Task<Result<GetPurchaseOrderDto>> ReturnPurchaseOrderAsync(Guid id, CancellationToken cancellationToken = default)
    {
        Result<GetPurchaseOrderDto> result = await _purchaseOrderService.ReturnPurchaseOrderAsync(id, cancellationToken);

        if (result.IsSuccess)
        {
            _logger.LogInformation($"Return Purchase Order Request: purchase order {result.Value?.Id} successfully returned.");
        }
        else switch (result.FailureType)
        {
            case FailureType.NotFound:
                _logger.LogInformation("Return Purchase Order Request: failed to return, purchase order not found.");
                break;
            
            case FailureType.BusinessLogic:
                _logger.LogInformation("Return Purchase Order Request: failed to return purchase order.");
                break;
            
            case FailureType.Exception:
                _logger.LogError(result.Exception, "Return Purchase Order Request: unhandled exception.");
                return Result<GetPurchaseOrderDto>.Fail(result.Message ?? "Failed to return purchase order.")
                    .WithFailureType(FailureType.Unexpected);
            
            default:
                _logger.LogError(result.Exception, "Return Purchase Order Request: failed to return purchase order.");
                return Result<GetPurchaseOrderDto>.Fail(result.Message ?? "Failed to return purchase order.")
                    .WithFailureType(FailureType.Unexpected);
        }
        
        return result;
    }

    public async Task<Result<GetPurchaseOrderDto>> UpdatePurchaseOrderStatusAsync(Guid id, PurchaseOrderStatus status, CancellationToken cancellationToken = default)
    {
        Result<GetPurchaseOrderDto> result = await _purchaseOrderService.UpdatePurchaseOrderStatusAsync(id, status, cancellationToken);

        if (result.IsSuccess)
        {
            _logger.LogInformation($"Update Purchase Order Request: purchase order {result.Value?.Id} successfully updated.");
        }
        else
        {
            _logger.LogInformation("Update Purchase Order Request: failed to update purchase order.");
        }
        
        return result;
    }

    public async Task<Result<GetPurchaseOrderDto>> RevertReceivedPurchaseOrderToPendingAsync(Guid id, CancellationToken cancellationToken = default)
    {
        Result<GetPurchaseOrderDto> result = await _purchaseOrderService.RevertReceivedPurchaseOrderToPendingAsync(id, cancellationToken);

        if (result.IsSuccess)
        {
            _logger.LogInformation($"Revert Received Purchase Order Request: purchase order {result.Value?.Id} successfully reverted to pending.");
        }
        else switch (result.FailureType)
        {
            case FailureType.NotFound:
                _logger.LogInformation("Revert Received Purchase Order Request: failed to revert purchase order, not found.");
                break;
            
            case FailureType.BusinessLogic:
                _logger.LogInformation("Revert Received Purchase Order Request: failed to revert purchase order.");
                break;
            
            case FailureType.Exception:
                _logger.LogError(result.Exception, "Revert Received Purchase Order Request: unhandled exception.");
                return Result<GetPurchaseOrderDto>.Fail(result.Message ?? "Failed to revert purchase order.")
                    .WithFailureType(FailureType.Unexpected);
            
            default:
                _logger.LogError(result.Exception, "Revert Received Purchase Order Request: failed to evert purchase order.");
                return Result<GetPurchaseOrderDto>.Fail(result.Message ?? "Failed to revert purchase order.")
                    .WithFailureType(FailureType.Unexpected);
        }
        
        return result;
    }

    public async Task<Result<GetPurchaseOrderDto>> UncancelPurchaseOrderAsync(Guid id, CancellationToken cancellationToken = default)
    {
        Result<GetPurchaseOrderDto> result = await _purchaseOrderService.UncancelPurchaseOrderAsync(id, cancellationToken);

        if (result.IsSuccess)
        {
            _logger.LogInformation($"Uncancel Purchase Order Request: purchase order {result.Value?.Id} successfully uncancelled.");
        }
        else switch (result.FailureType)
        {
            case FailureType.NotFound:
                _logger.LogInformation("Uncancel Purchase Order Request: failed to uncancel, purchase order not found.");
                break;
            
            case FailureType.BusinessLogic:
                _logger.LogInformation("Uncancel Purchase Order Request: failed to uncancel purchase order.");
                break;
            
            case FailureType.Exception:
                _logger.LogError(result.Exception, "Uncancel Purchase Order Request: unhandled exception.");
                return Result<GetPurchaseOrderDto>.Fail(result.Message ?? "Failed to uncancel purchase order.")
                    .WithFailureType(FailureType.Unexpected);
            
            default:
                _logger.LogError(result.Exception, "Uncancel Purchase Order Request: failed to uncancel purchase order.");
                return Result<GetPurchaseOrderDto>.Fail(result.Message ?? "Failed to uncancel purchase order.")
                    .WithFailureType(FailureType.Unexpected);
        }
        
        return result;
    }
}