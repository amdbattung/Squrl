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
        Result<GetManyPurchaseOrdersDto> result;

        try
        {
            result = await _purchaseOrderService.GetManyPurchaseOrdersAsync(
                search,
                supplierId,
                isPending,
                isReceived,
                isCanceled,
                hasFailed,
                isReturned,
                pageNumber,
                pageSize,
                orderDirection,
                cancellationToken);
        }
        catch (Exception e)
        {
            _logger.LogError(e, "Get Many Purchase Orders Request: unhandled exception.");
            throw;
        }

        if (result.IsSuccess)
        {
            _logger.LogInformation("Get Many Purchase Orders Request: {Count} purchase orders successfully fetched.",
                result.Value?.PurchaseOrders?.Count);
        }
        else switch (result.FailureType)
        {
            case FailureType.Validation:
            
            case FailureType.NotFound:
            
            case FailureType.BusinessLogic:
                _logger.LogInformation(result.Exception,
                    "Get Many Purchase Orders Request: {Message}",
                    result.Message ?? "Failed to retrieve purchase orders.");
                break;

            case FailureType.Exception:
            
            case FailureType.Unexpected:
                _logger.LogError(result.Exception,
                    "Get Many Purchase Orders Request: Unhandled exception.");
                throw result.Exception
                      ?? new Exception(result.Message ?? "Failed to retrieve purchase orders, something went wrong.");

            default:
                _logger.LogError(result.Exception,
                    "Get Many Suppliers Request: Unhandled failure type {FailureType}.",
                    Enum.GetName(result.FailureType ?? FailureType.Exception));
                throw result.Exception
                      ?? new Exception(result.Message ?? "Failed to retrieve purchase orders, something went wrong.");
        }
        
        return result;
    }

    public async Task<Result<GetPurchaseOrderDto>> CreatePurchaseOrderAsync(CreatePurchaseOrderDto purchaseOrder, CancellationToken cancellationToken = default)
    {
        Result<GetPurchaseOrderDto> result;

        try
        {
            result = await _purchaseOrderService.CreatePurchaseOrderAsync(purchaseOrder, cancellationToken);
        }
        catch (Exception e)
        {
            _logger.LogError(e, "Create Purchase Order Request: unhandled exception.");
            throw;
        }

        if (result.IsSuccess)
        {
            _logger.LogInformation("Create Purchase Order Request: purchase order {Id} successfully created.",
                result.Value?.Id);
        }
        else switch (result.FailureType)
        {
            case FailureType.Validation:
            
            case FailureType.NotFound:
            
            case FailureType.BusinessLogic:
                _logger.LogInformation(result.Exception,
                    "Create Purchase Order Request: {Message}",
                    result.Message ?? "Failed to create purchase order.");
                break;
            
            case FailureType.Exception:
            
            case FailureType.Unexpected:
                _logger.LogError(result.Exception,
                    "Create Purchase Order Request: unhandled exception.");
                throw result.Exception
                      ?? new Exception(result.Message ?? "Failed to create purchase order, something went wrong.");
                
            default:
                _logger.LogError(result.Exception,
                    "Create Purchase Order Request: unhandled failure type {FailureType}.",
                    Enum.GetName(result.FailureType ?? FailureType.Exception));
                throw result.Exception
                      ?? new Exception(result.Message ?? "Failed to create purchase order, something went wrong.");
        }
    
        return result;
    }

    public async Task<Result<GetPurchaseOrderDto>> GetPurchaseOrderByIdAsync(Guid id, CancellationToken cancellationToken = default)
    {
        Result<GetPurchaseOrderDto> result;

        try
        {
            result = await _purchaseOrderService.GetPurchaseOrderByIdAsync(id, cancellationToken);
        }
        catch (Exception e)
        {
            _logger.LogError(e, "Get Purchase Order By ID Request: unhandled exception.");
            throw;
        }

        if (result.IsSuccess)
        {
            _logger.LogInformation("Get Purchase Order By ID Request: purchase order {Id} successfully fetched.",
                result.Value?.Id);
        }
        else switch (result.FailureType)
        {
            case FailureType.Validation:
            
            case FailureType.NotFound:
            
            case FailureType.BusinessLogic:
                _logger.LogInformation(result.Exception,
                    "Get Purchase Order By ID Request: {Message}",
                    result.Message ?? "Failed to retrieve purchase order.");
                break;
            
            case FailureType.Exception:

            case FailureType.Unexpected:
                _logger.LogError(result.Exception,
                    "Get Purchase Order By ID Request: unhandled exception.");
                throw result.Exception
                      ?? new Exception(result.Message ?? "Failed to retrieve purchase order, something went wrong.");
                
            default:
                _logger.LogError(result.Exception,
                    "Get Purchase Order By ID Request: unhandled failure type {FailureType}.",
                    Enum.GetName(result.FailureType ?? FailureType.Exception));
                throw result.Exception
                      ?? new Exception(result.Message ?? "Failed to retrieve purchase order, something went wrong.");
        }
        
        return result;
    }

    public async Task<Result<GetPurchaseOrderDto>> UpdatePurchaseOrderAsync(Guid id, UpdatePurchaseOrderDto purchaseOrder,
        CancellationToken cancellationToken = default)
    {
        Result<GetPurchaseOrderDto> result;

        try
        {
            result = await _purchaseOrderService.UpdatePurchaseOrderAsync(id, purchaseOrder, cancellationToken);
        }
        catch (Exception e)
        {
            _logger.LogError(e, "Update Purchase Order Request: unhandled exception.");
            throw;
        }

        if (result.IsSuccess)
        {
            _logger.LogInformation("Update Purchase Order Request: purchase order {Id} successfully updated.",
                result.Value?.Id);
        }
        else switch (result.FailureType)
        {
            case FailureType.Validation:
            
            case FailureType.NotFound:
            
            case FailureType.BusinessLogic:
                _logger.LogInformation(result.Exception,
                    "Update Purchase Order Request: {Message}",
                    result.Message ?? "Failed to update purchase order.");
                break;
            
            case FailureType.Exception:

            case FailureType.Unexpected:
                _logger.LogError(result.Exception,
                    "Update Purchase Order Request: unhandled exception.");
                throw result.Exception
                      ?? new Exception(result.Message ?? "Failed to update purchase order, something went wrong.");
                
            default:
                _logger.LogError(result.Exception,
                    "Update Purchase Order Request: unhandled failure type {FailureType}.",
                    Enum.GetName(result.FailureType ?? FailureType.Exception));
                throw result.Exception
                      ?? new Exception(result.Message ?? "Failed to update purchase order, something went wrong.");
        }
        
        return result;
    }

    public async Task<Result<GetPurchaseOrderDto>> DeletePurchaseOrderAsync(Guid id, CancellationToken cancellationToken = default)
    {
        Result<GetPurchaseOrderDto> result;

        try
        {
            result = await _purchaseOrderService.DeletePurchaseOrderAsync(id, cancellationToken);
        }
        catch (Exception e)
        {
            _logger.LogError(e, "Delete Purchase Order Request: unhandled exception.");
            throw;
        }

        if (result.IsSuccess)
        {
            _logger.LogInformation("Delete Purchase Order Request: purchase order {Id} successfully deleted.",
                result.Value?.Id);
        }
        else switch (result.FailureType)
        {
            case FailureType.Validation:
            
            case FailureType.NotFound:
            
            case FailureType.BusinessLogic:
                _logger.LogInformation(result.Exception,
                    "Delete Purchase Order Request: {Message}",
                    result.Message ?? "Failed to delete purchase order.");
                break;
            
            case FailureType.Exception:

            case FailureType.Unexpected:
                _logger.LogError(result.Exception,
                    "Delete Purchase Order Request: unhandled exception.");
                throw result.Exception
                      ?? new Exception(result.Message ?? "Failed to delete purchase order, something went wrong.");
                
            default:
                _logger.LogError(result.Exception,
                    "Delete Purchase Order Request: unhandled failure type {FailureType}.",
                    Enum.GetName(result.FailureType ?? FailureType.Exception));
                throw result.Exception
                      ?? new Exception(result.Message ?? "Failed to delete purchase order, something went wrong.");
        }
        
        return result;
    }

    public async Task<Result<GetManyPoDetailsDto>> GetPoDetailsAsync(
        Guid purchaseOrderId,
        SortDirection orderDirection = SortDirection.Ascending,
        CancellationToken cancellationToken = default)
    {
        Result<GetManyPoDetailsDto> result;

        try
        {
            result = await _purchaseOrderService.GetPoDetailsAsync(
                purchaseOrderId,
                orderDirection,
                cancellationToken);
        }
        catch (Exception e)
        {
            _logger.LogError(e, "Get Many PO Details Request: unhandled exception.");
            throw;
        }

        if (result.IsSuccess)
        {
            _logger.LogInformation("Get Many PO Details Request: {Count} PO details successfully fetched.",
                result.Value?.PoDetails?.Count);
        }
        else switch (result.FailureType)
        {
            case FailureType.Validation:
            
            case FailureType.NotFound:
            
            case FailureType.BusinessLogic:
                _logger.LogInformation(result.Exception,
                    "Get Many PO Details Request: {Message}",
                    result.Message ?? "Failed to retrieve PO details.");
                break;

            case FailureType.Exception:
            
            case FailureType.Unexpected:
                _logger.LogError(result.Exception,
                    "Get Many PO Details Request: Unhandled exception.");
                throw result.Exception
                      ?? new Exception(result.Message ?? "Failed to retrieve PO details, something went wrong.");

            default:
                _logger.LogError(result.Exception,
                    "Get Many PO Details Request: Unhandled failure type {FailureType}.",
                    Enum.GetName(result.FailureType ?? FailureType.Exception));
                throw result.Exception
                      ?? new Exception(result.Message ?? "Failed to retrieve PO details, something went wrong.");
        }
        
        return result;
    }

    public async Task<Result<GetPoDetailDto>> AddPoDetailAsync(Guid purchaseOrderId, CreatePoDetailDto poDetail, CancellationToken cancellationToken = default)
    {
        Result<GetPoDetailDto> result;

        try
        {
            result = await _purchaseOrderService.AddPoDetailAsync(purchaseOrderId, poDetail, cancellationToken);
        }
        catch (Exception e)
        {
            _logger.LogError(e, "Create PO Detail Request: unhandled exception.");
            throw;
        }

        if (result.IsSuccess)
        {
            _logger.LogInformation("Create PO Detail Request: PO detail {Id} successfully created.",
                result.Value?.Id);
        }
        else switch (result.FailureType)
        {
            case FailureType.Validation:
            
            case FailureType.NotFound:
            
            case FailureType.BusinessLogic:
                _logger.LogInformation(result.Exception,
                    "Create PO Detail Request: {Message}",
                    result.Message ?? "Failed to create PO detail.");
                break;
            
            case FailureType.Exception:
            
            case FailureType.Unexpected:
                _logger.LogError(result.Exception,
                    "Create PO Detail Request: unhandled exception.");
                throw result.Exception
                      ?? new Exception(result.Message ?? "Failed to create PO detail, something went wrong.");
                
            default:
                _logger.LogError(result.Exception,
                    "Create PO Detail Request: unhandled failure type {FailureType}.",
                    Enum.GetName(result.FailureType ?? FailureType.Exception));
                throw result.Exception
                      ?? new Exception(result.Message ?? "Failed to create PO detail, something went wrong.");
        }
    
        return result;
    }

    public async Task<Result<GetPoDetailDto>> GetPoDetailByIdAsync(Guid id, CancellationToken cancellationToken = default)
    {
        Result<GetPoDetailDto> result;

        try
        {
            result = await _purchaseOrderService.GetPoDetailByIdAsync(id, cancellationToken);
        }
        catch (Exception e)
        {
            _logger.LogError(e, "Get PO Detail By ID Request: unhandled exception.");
            throw;
        }

        if (result.IsSuccess)
        {
            _logger.LogInformation("Get PO Detail By ID Request: PO detail {Id} successfully fetched.",
                result.Value?.Id);
        }
        else switch (result.FailureType)
        {
            case FailureType.Validation:
            
            case FailureType.NotFound:
            
            case FailureType.BusinessLogic:
                _logger.LogInformation(result.Exception,
                    "Get PO Detail By ID Request: {Message}",
                    result.Message ?? "Failed to retrieve PO detail.");
                break;
            
            case FailureType.Exception:

            case FailureType.Unexpected:
                _logger.LogError(result.Exception,
                    "Get PO Detail By ID Request: unhandled exception.");
                throw result.Exception
                      ?? new Exception(result.Message ?? "Failed to retrieve PO detail, something went wrong.");
                
            default:
                _logger.LogError(result.Exception,
                    "Get PO Detail By ID Request: unhandled failure type {FailureType}.",
                    Enum.GetName(result.FailureType ?? FailureType.Exception));
                throw result.Exception
                      ?? new Exception(result.Message ?? "Failed to retrieve PO detail, something went wrong.");
        }
        
        return result;
    }

    public async Task<Result<GetPoDetailDto>> UpdatePoDetailAsync(Guid id, UpdatePoDetailDto poDetail, CancellationToken cancellationToken = default)
    {
        Result<GetPoDetailDto> result;

        try
        {
            result = await _purchaseOrderService.UpdatePoDetailAsync(id, poDetail, cancellationToken);
        }
        catch (Exception e)
        {
            _logger.LogError(e, "Update PO Detail Request: unhandled exception.");
            throw;
        }

        if (result.IsSuccess)
        {
            _logger.LogInformation("Update PO Detail Request: PO detail {Id} successfully updated.",
                result.Value?.Id);
        }
        else switch (result.FailureType)
        {
            case FailureType.Validation:
            
            case FailureType.NotFound:
            
            case FailureType.BusinessLogic:
                _logger.LogInformation(result.Exception,
                    "Update PO Detail Request: {Message}",
                    result.Message ?? "Failed to update PO detail.");
                break;
            
            case FailureType.Exception:

            case FailureType.Unexpected:
                _logger.LogError(result.Exception,
                    "Update PO Detail Request: unhandled exception.");
                throw result.Exception
                      ?? new Exception(result.Message ?? "Failed to update PO detail, something went wrong.");
                
            default:
                _logger.LogError(result.Exception,
                    "Update PO Detail Request: unhandled failure type {FailureType}.",
                    Enum.GetName(result.FailureType ?? FailureType.Exception));
                throw result.Exception
                      ?? new Exception(result.Message ?? "Failed to update PO detail, something went wrong.");
        }
        
        return result;
    }

    public async Task<Result<GetPoDetailDto>> RemovePoDetailAsync(Guid id, CancellationToken cancellationToken = default)
    {
        Result<GetPoDetailDto> result;

        try
        {
            result = await _purchaseOrderService.RemovePoDetailAsync(id, cancellationToken);
        }
        catch (Exception e)
        {
            _logger.LogError(e, "Delete PO Detail Request: unhandled exception.");
            throw;
        }

        if (result.IsSuccess)
        {
            _logger.LogInformation("Delete PO Detail Request: PO detail {Id} successfully deleted.",
                result.Value?.Id);
        }
        else switch (result.FailureType)
        {
            case FailureType.Validation:
            
            case FailureType.NotFound:
            
            case FailureType.BusinessLogic:
                _logger.LogInformation(result.Exception,
                    "Delete PO Detail Request: {Message}",
                    result.Message ?? "Failed to delete PO detail.");
                break;
            
            case FailureType.Exception:

            case FailureType.Unexpected:
                _logger.LogError(result.Exception,
                    "Delete PO Detail Request: unhandled exception.");
                throw result.Exception
                      ?? new Exception(result.Message ?? "Failed to delete PO detail, something went wrong.");
                
            default:
                _logger.LogError(result.Exception,
                    "Delete PO Detail Request: unhandled failure type {FailureType}.",
                    Enum.GetName(result.FailureType ?? FailureType.Exception));
                throw result.Exception
                      ?? new Exception(result.Message ?? "Failed to delete PO detail, something went wrong.");
        }
        
        return result;
    }

    public async Task<Result<GetPurchaseOrderDto>> ReceivePurchaseOrderAsync(Guid id, CancellationToken cancellationToken = default)
    {
        Result<GetPurchaseOrderDto> result;

        try
        {
            result = await _purchaseOrderService.ReceivePurchaseOrderAsync(id, cancellationToken);
        }
        catch (Exception e)
        {
            _logger.LogError(e, "Receive Purchase Order Request: unhandled exception.");
            throw;
        }

        if (result.IsSuccess)
        {
            _logger.LogInformation("Receive Purchase Order Request: purchase order {Id} successfully received.",
                result.Value?.Id);
        }
        else switch (result.FailureType)
        {
            case FailureType.Validation:
            
            case FailureType.NotFound:
            
            case FailureType.BusinessLogic:
                _logger.LogInformation(result.Exception,
                    "Receive Purchase Order Request: {Message}",
                    result.Message ?? "Failed to receive purchase order.");
                break;
            
            case FailureType.Exception:

            case FailureType.Unexpected:
                _logger.LogError(result.Exception,
                    "Receive Purchase Order Request: unhandled exception.");
                throw result.Exception
                      ?? new Exception(result.Message ?? "Failed to receive purchase order, something went wrong.");
                
            default:
                _logger.LogError(result.Exception,
                    "Receive Purchase Order Request: unhandled failure type {FailureType}.",
                    Enum.GetName(result.FailureType ?? FailureType.Exception));
                throw result.Exception
                      ?? new Exception(result.Message ?? "Failed to receive purchase order, something went wrong.");
        }
        
        return result;
    }

    public async Task<Result<GetPurchaseOrderDto>> CancelPurchaseOrderAsync(Guid id, CancellationToken cancellationToken = default)
    {
        Result<GetPurchaseOrderDto> result;

        try
        {
            result = await _purchaseOrderService.CancelPurchaseOrderAsync(id, cancellationToken);
        }
        catch (Exception e)
        {
            _logger.LogError(e, "Cancel Purchase Order Request: unhandled exception.");
            throw;
        }

        if (result.IsSuccess)
        {
            _logger.LogInformation("Cancel Purchase Order Request: purchase order {Id} successfully canceled.",
                result.Value?.Id);
        }
        else switch (result.FailureType)
        {
            case FailureType.Validation:
            
            case FailureType.NotFound:
            
            case FailureType.BusinessLogic:
                _logger.LogInformation(result.Exception,
                    "Cancel Purchase Order Request: {Message}",
                    result.Message ?? "Failed to cancel purchase order.");
                break;
            
            case FailureType.Exception:

            case FailureType.Unexpected:
                _logger.LogError(result.Exception,
                    "Cancel Purchase Order Request: unhandled exception.");
                throw result.Exception
                      ?? new Exception(result.Message ?? "Failed to cancel purchase order, something went wrong.");
                
            default:
                _logger.LogError(result.Exception,
                    "Cancel Purchase Order Request: unhandled failure type {FailureType}.",
                    Enum.GetName(result.FailureType ?? FailureType.Exception));
                throw result.Exception
                      ?? new Exception(result.Message ?? "Failed to cancel purchase order, something went wrong.");
        }
        
        return result;
    }

    public async Task<Result<GetPurchaseOrderDto>> ReturnPurchaseOrderAsync(Guid id, CancellationToken cancellationToken = default)
    {
        Result<GetPurchaseOrderDto> result;

        try
        {
            result = await _purchaseOrderService.ReturnPurchaseOrderAsync(id, cancellationToken);
        }
        catch (Exception e)
        {
            _logger.LogError(e, "Cancel Purchase Order Request: unhandled exception.");
            throw;
        }

        if (result.IsSuccess)
        {
            _logger.LogInformation("Return Purchase Order Request: purchase order {Id} successfully returned.",
                result.Value?.Id);
        }
        else switch (result.FailureType)
        {
            case FailureType.Validation:
            
            case FailureType.NotFound:
            
            case FailureType.BusinessLogic:
                _logger.LogInformation(result.Exception,
                    "Return Purchase Order Request: {Message}",
                    result.Message ?? "Failed to return purchase order.");
                break;
            
            case FailureType.Exception:

            case FailureType.Unexpected:
                _logger.LogError(result.Exception,
                    "Return Purchase Order Request: unhandled exception.");
                throw result.Exception
                      ?? new Exception(result.Message ?? "Failed to return purchase order, something went wrong.");
                
            default:
                _logger.LogError(result.Exception,
                    "Return Purchase Order Request: unhandled failure type {FailureType}.",
                    Enum.GetName(result.FailureType ?? FailureType.Exception));
                throw result.Exception
                      ?? new Exception(result.Message ?? "Failed to return purchase order, something went wrong.");
        }
        
        return result;
    }

    public async Task<Result<GetPurchaseOrderDto>> UpdatePurchaseOrderStatusAsync(Guid id, PurchaseOrderStatus status, CancellationToken cancellationToken = default)
    {
        Result<GetPurchaseOrderDto> result;

        try
        {
            result = await _purchaseOrderService.UpdatePurchaseOrderStatusAsync(id, status, cancellationToken);
        }
        catch (Exception e)
        {
            _logger.LogError(e, "Update Purchase Order Status Request: unhandled exception.");
            throw;
        }

        if (result.IsSuccess)
        {
            _logger.LogInformation("Update Purchase Order Status Request: purchase order {Id} successfully updated.",
                result.Value?.Id);
        }
        else switch (result.FailureType)
        {
            case FailureType.Validation:
            
            case FailureType.NotFound:
            
            case FailureType.BusinessLogic:
                _logger.LogInformation(result.Exception,
                    "Update Purchase Order Status Request: {Message}",
                    result.Message ?? "Failed to update purchase order.");
                break;
            
            case FailureType.Exception:

            case FailureType.Unexpected:
                _logger.LogError(result.Exception,
                    "Update Purchase Order Status Request: unhandled exception.");
                throw result.Exception
                      ?? new Exception(result.Message ?? "Failed to update purchase order, something went wrong.");
                
            default:
                _logger.LogError(result.Exception,
                    "Update Purchase Order Status Request: unhandled failure type {FailureType}.",
                    Enum.GetName(result.FailureType ?? FailureType.Exception));
                throw result.Exception
                      ?? new Exception(result.Message ?? "Failed to update purchase order, something went wrong.");
        }
        
        return result;
    }

    public async Task<Result<GetPurchaseOrderDto>> RevertReceivedPurchaseOrderToPendingAsync(Guid id, CancellationToken cancellationToken = default)
    {
        Result<GetPurchaseOrderDto> result;

        try
        {
            result = await _purchaseOrderService.RevertReceivedPurchaseOrderToPendingAsync(id, cancellationToken);
        }
        catch (Exception e)
        {
            _logger.LogError(e, "Revert Received Purchase Order Request: unhandled exception.");
            throw;
        }

        if (result.IsSuccess)
        {
            _logger.LogInformation("Revert Received Purchase Order Request: " +
                                   "purchase order {Id} successfully reverted to pending.",
                result.Value?.Id);
        }
        else switch (result.FailureType)
        {
            case FailureType.Validation:
            
            case FailureType.NotFound:
            
            case FailureType.BusinessLogic:
                _logger.LogInformation(result.Exception,
                    "Revert Received Purchase Order Request: {Message}",
                    result.Message ?? "Failed to revert purchase order.");
                break;
            
            case FailureType.Exception:

            case FailureType.Unexpected:
                _logger.LogError(result.Exception,
                    "Revert Received Purchase Order Request: unhandled exception.");
                throw result.Exception
                      ?? new Exception(result.Message ?? "Failed to revert purchase order, something went wrong.");
                
            default:
                _logger.LogError(result.Exception,
                    "Revert Received Purchase Order Request: unhandled failure type {FailureType}.",
                    Enum.GetName(result.FailureType ?? FailureType.Exception));
                throw result.Exception
                      ?? new Exception(result.Message ?? "Failed to revert purchase order, something went wrong.");
        }
        
        return result;
    }

    public async Task<Result<GetPurchaseOrderDto>> UncancelPurchaseOrderAsync(Guid id, CancellationToken cancellationToken = default)
    {
        Result<GetPurchaseOrderDto> result;

        try
        {
            result = await _purchaseOrderService.UncancelPurchaseOrderAsync(id, cancellationToken);
        }
        catch (Exception e)
        {
            _logger.LogError(e, "Uncancel Purchase Order Request: unhandled exception.");
            throw;
        }

        if (result.IsSuccess)
        {
            _logger.LogInformation("Uncancel Purchase Order Request: purchase order {Id} successfully uncanceled.", result.Value?.Id);
        }
        else switch (result.FailureType)
        {
            case FailureType.Validation:
            
            case FailureType.NotFound:
            
            case FailureType.BusinessLogic:
                _logger.LogInformation(result.Exception,
                    "Uncancel Purchase Order Request: {Message}",
                    result.Message ?? "Failed to uncancel purchase order.");
                break;
            
            case FailureType.Exception:

            case FailureType.Unexpected:
                _logger.LogError(result.Exception,
                    "Uncancel Purchase Order Request: unhandled exception.");
                throw result.Exception
                      ?? new Exception(result.Message ?? "Failed to uncancel purchase order, something went wrong.");
                
            default:
                _logger.LogError(result.Exception,
                    "Uncancel Purchase Order Request: unhandled failure type {FailureType}.",
                    Enum.GetName(result.FailureType ?? FailureType.Exception));
                throw result.Exception
                      ?? new Exception(result.Message ?? "Failed to uncancel purchase order, something went wrong.");
        }
        
        return result;
    }

    public async Task<Result<bool>> ExportPoDetailsAsCsvAsync(Guid purchaseOrderId, Stream destination, CancellationToken cancellationToken = default)
    {
        Result<bool> result;

        try
        {
            result = await _purchaseOrderService
                .ExportPoDetailsAsCsvAsync(purchaseOrderId, destination, cancellationToken);
        }
        catch (Exception e)
        {
            _logger.LogError(e, "Export PO Details As CSV Request: unhandled exception.");
            throw;
        }

        if (result.IsSuccess)
        {
            _logger.LogInformation("Export PO Details As CSV Request: PO details successfully exported.");
        }
        else switch (result.FailureType)
        {
            case FailureType.Validation:
            
            case FailureType.NotFound:
            
            case FailureType.BusinessLogic:
                _logger.LogInformation(result.Exception,
                    "Export PO Details As CSV Request: {Message}",
                    result.Message ?? "Failed to export PO details.");
                break;
            
            case FailureType.Exception:

            case FailureType.Unexpected:
                _logger.LogError(result.Exception,
                    "Export PO Details As CSV Request: unhandled exception.");
                throw result.Exception
                      ?? new Exception(result.Message ?? "Failed to export PO detail, something went wrong.");
                
            default:
                _logger.LogError(result.Exception,
                    "Export PO Details As CSV Request: unhandled failure type {FailureType}.",
                    Enum.GetName(result.FailureType ?? FailureType.Exception));
                throw result.Exception
                      ?? new Exception(result.Message ?? "Failed to export PO detail, something went wrong.");
        }
        
        return result;
    }
}