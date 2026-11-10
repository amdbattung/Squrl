using Squrl.App.Common;
using Squrl.App.Enums;
using Squrl.App.Features.PurchaseOrderDetails.DTOs;
using Squrl.App.Features.PurchaseOrders.DTOs;

namespace Squrl.App.Services.PurchaseOrder;

public class PurchaseOrderErrorHandlingDecorator : IPurchaseOrderService
{
    private readonly IPurchaseOrderService _purchaseOrderService;

    public PurchaseOrderErrorHandlingDecorator(IPurchaseOrderService purchaseOrderService)
    {
        _purchaseOrderService = purchaseOrderService;
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
        try
        {
            Result<GetManyPurchaseOrdersDto> result = await _purchaseOrderService
                .GetManyPurchaseOrdersAsync(
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

            return result.Exception is null
                ? result
                : result.WithoutException();
        }
        catch (OperationCanceledException)
        {
            throw;
        }
        catch (Exception)
        {
            return Result<GetManyPurchaseOrdersDto>
                .Fail("Failed to retrieve purchase orders, something went wrong.")
                .WithFailureType(FailureType.Unexpected);
        }
    }

    public async Task<Result<GetPurchaseOrderDto>> CreatePurchaseOrderAsync(CreatePurchaseOrderDto purchaseOrder, CancellationToken cancellationToken = default)
    {
        try
        {
            Result<GetPurchaseOrderDto> result = await _purchaseOrderService
                .CreatePurchaseOrderAsync(purchaseOrder, cancellationToken);
            
            return result.Exception is null
                ? result
                : result.WithoutException();
        }
        catch (OperationCanceledException)
        {
            throw;
        }
        catch (Exception)
        {
            return Result<GetPurchaseOrderDto>
                .Fail("Failed to create purchase order, something went wrong.")
                .WithFailureType(FailureType.Unexpected);
        }
    }

    public async Task<Result<GetPurchaseOrderDto>> GetPurchaseOrderByIdAsync(Guid id, CancellationToken cancellationToken = default)
    {
        try
        {
            Result<GetPurchaseOrderDto> result = await _purchaseOrderService
                .GetPurchaseOrderByIdAsync(id, cancellationToken);
            
            return result.Exception is null
                ? result
                : result.WithoutException();
        }
        catch (OperationCanceledException)
        {
            throw;
        }
        catch (Exception)
        {
            return Result<GetPurchaseOrderDto>
                .Fail("Failed to retrieve purchase order, something went wrong.")
                .WithFailureType(FailureType.Unexpected);
        }
    }

    public async Task<Result<GetPurchaseOrderDto>> UpdatePurchaseOrderAsync(
        Guid id,
        UpdatePurchaseOrderDto purchaseOrder,
        CancellationToken cancellationToken = default)
    {
        try
        {
            Result<GetPurchaseOrderDto> result = await _purchaseOrderService
                .UpdatePurchaseOrderAsync(id, purchaseOrder, cancellationToken);
            
            return result.Exception is null
                ? result
                : result.WithoutException();
        }
        catch (OperationCanceledException)
        {
            throw;
        }
        catch (Exception)
        {
            return Result<GetPurchaseOrderDto>
                .Fail("Failed to update purchase order, something went wrong.")
                .WithFailureType(FailureType.Unexpected);
        }
    }

    public async Task<Result<GetPurchaseOrderDto>> DeletePurchaseOrderAsync(Guid id, CancellationToken cancellationToken = default)
    {
        try
        {
            Result<GetPurchaseOrderDto> result = await _purchaseOrderService
                .DeletePurchaseOrderAsync(id, cancellationToken);
            
            return result.Exception is null
                ? result
                : result.WithoutException();
        }
        catch (OperationCanceledException)
        {
            throw;
        }
        catch (Exception)
        {
            return Result<GetPurchaseOrderDto>
                .Fail("Failed to delete purchase order, something went wrong.")
                .WithFailureType(FailureType.Unexpected);
        }
    }

    public async Task<Result<GetManyPoDetailsDto>> GetPoDetailsAsync(
        Guid purchaseOrderId,
        SortDirection orderDirection = SortDirection.Ascending,
        CancellationToken cancellationToken = default)
    {
        try
        {
            Result<GetManyPoDetailsDto> result = await _purchaseOrderService
                .GetPoDetailsAsync(purchaseOrderId, orderDirection, cancellationToken);
            
            return result.Exception is null
                ? result
                : result.WithoutException();
        }
        catch (OperationCanceledException)
        {
            throw;
        }
        catch (Exception)
        {
            return Result<GetManyPoDetailsDto>
                .Fail("Failed to retrieve PO details, something went wrong.")
                .WithFailureType(FailureType.Unexpected);
        }
    }

    public async Task<Result<GetPoDetailDto>> AddPoDetailAsync(Guid purchaseOrderId, CreatePoDetailDto poDetail, CancellationToken cancellationToken = default)
    {
        try
        {
            Result<GetPoDetailDto> result = await _purchaseOrderService
                .AddPoDetailAsync(purchaseOrderId, poDetail, cancellationToken);
            
            return result.Exception is null
                ? result
                : result.WithoutException();
        }
        catch (OperationCanceledException)
        {
            throw;
        }
        catch (Exception)
        {
            return Result<GetPoDetailDto>
                .Fail("Failed to create PO detail, something went wrong.")
                .WithFailureType(FailureType.Unexpected);
        }
    }

    public async Task<Result<GetPoDetailDto>> GetPoDetailByIdAsync(Guid id, CancellationToken cancellationToken = default)
    {
        try
        {
            Result<GetPoDetailDto> result = await _purchaseOrderService
                .GetPoDetailByIdAsync(id, cancellationToken);
            
            return result.Exception is null
                ? result
                : result.WithoutException();
        }
        catch (OperationCanceledException)
        {
            throw;
        }
        catch (Exception)
        {
            return Result<GetPoDetailDto>
                .Fail("Failed to retrieve PO detail, something went wrong.")
                .WithFailureType(FailureType.Unexpected);
        }
    }

    public async Task<Result<GetPoDetailDto>> UpdatePoDetailAsync(Guid id, UpdatePoDetailDto poDetail, CancellationToken cancellationToken = default)
    {
        try
        {
            Result<GetPoDetailDto> result = await _purchaseOrderService
                .UpdatePoDetailAsync(id, poDetail, cancellationToken);
            
            return result.Exception is null
                ? result
                : result.WithoutException();
        }
        catch (OperationCanceledException)
        {
            throw;
        }
        catch (Exception)
        {
            return Result<GetPoDetailDto>
                .Fail("Failed to update PO detail, something went wrong.")
                .WithFailureType(FailureType.Unexpected);
        }
    }

    public async Task<Result<GetPoDetailDto>> RemovePoDetailAsync(Guid id, CancellationToken cancellationToken = default)
    {
        try
        {
            Result<GetPoDetailDto> result = await _purchaseOrderService
                .RemovePoDetailAsync(id, cancellationToken);
            
            return result.Exception is null
                ? result
                : result.WithoutException();
        }
        catch (OperationCanceledException)
        {
            throw;
        }
        catch (Exception)
        {
            return Result<GetPoDetailDto>
                .Fail("Failed to delete PO detail, something went wrong.")
                .WithFailureType(FailureType.Unexpected);
        }
    }

    public async Task<Result<GetPurchaseOrderDto>> ReceivePurchaseOrderAsync(Guid id, CancellationToken cancellationToken = default)
    {
        try
        {
            Result<GetPurchaseOrderDto> result = await _purchaseOrderService
                .ReceivePurchaseOrderAsync(id, cancellationToken);
            
            return result.Exception is null
                ? result
                : result.WithoutException();
        }
        catch (OperationCanceledException)
        {
            throw;
        }
        catch (Exception)
        {
            return Result<GetPurchaseOrderDto>
                .Fail("Failed to receive purchase order, something went wrong.")
                .WithFailureType(FailureType.Unexpected);
        }
    }

    public async Task<Result<GetPurchaseOrderDto>> CancelPurchaseOrderAsync(Guid id, CancellationToken cancellationToken = default)
    {
        try
        {
            Result<GetPurchaseOrderDto> result = await _purchaseOrderService
                .CancelPurchaseOrderAsync(id, cancellationToken);
            
            return result.Exception is null
                ? result
                : result.WithoutException();
        }
        catch (OperationCanceledException)
        {
            throw;
        }
        catch (Exception)
        {
            return Result<GetPurchaseOrderDto>
                .Fail("Failed to cancel purchase order, something went wrong.")
                .WithFailureType(FailureType.Unexpected);
        }
    }

    public async Task<Result<GetPurchaseOrderDto>> ReturnPurchaseOrderAsync(Guid id, CancellationToken cancellationToken = default)
    {
        try
        {
            Result<GetPurchaseOrderDto> result = await _purchaseOrderService
                .ReturnPurchaseOrderAsync(id, cancellationToken);
            
            return result.Exception is null
                ? result
                : result.WithoutException();
        }
        catch (OperationCanceledException)
        {
            throw;
        }
        catch (Exception)
        {
            return Result<GetPurchaseOrderDto>
                .Fail("Failed to return purchase order, something went wrong.")
                .WithFailureType(FailureType.Unexpected);
        }
    }

    public async Task<Result<GetPurchaseOrderDto>> UpdatePurchaseOrderStatusAsync(Guid id, PurchaseOrderStatus status, CancellationToken cancellationToken = default)
    {
        try
        {
            Result<GetPurchaseOrderDto> result = await _purchaseOrderService
                .UpdatePurchaseOrderStatusAsync(id, status, cancellationToken);
            
            return result.Exception is null
                ? result
                : result.WithoutException();
        }
        catch (OperationCanceledException)
        {
            throw;
        }
        catch (Exception)
        {
            return Result<GetPurchaseOrderDto>
                .Fail("Failed to update purchase order, something went wrong.")
                .WithFailureType(FailureType.Unexpected);
        }
    }

    public async Task<Result<GetPurchaseOrderDto>> RevertReceivedPurchaseOrderToPendingAsync(Guid id, CancellationToken cancellationToken = default)
    {
        try
        {
            Result<GetPurchaseOrderDto> result = await _purchaseOrderService
                .RevertReceivedPurchaseOrderToPendingAsync(id, cancellationToken);
            
            return result.Exception is null
                ? result
                : result.WithoutException();
        }
        catch (OperationCanceledException)
        {
            throw;
        }
        catch (Exception)
        {
            return Result<GetPurchaseOrderDto>
                .Fail("Failed to revert purchase order, something went wrong.")
                .WithFailureType(FailureType.Unexpected);
        }
    }

    public async Task<Result<GetPurchaseOrderDto>> UncancelPurchaseOrderAsync(Guid id, CancellationToken cancellationToken = default)
    {
        try
        {
            Result<GetPurchaseOrderDto> result = await _purchaseOrderService
                .UncancelPurchaseOrderAsync(id, cancellationToken);
            
            return result.Exception is null
                ? result
                : result.WithoutException();
        }
        catch (OperationCanceledException)
        {
            throw;
        }
        catch (Exception)
        {
            return Result<GetPurchaseOrderDto>
                .Fail("Failed to uncancel purchase order, something went wrong.")
                .WithFailureType(FailureType.Unexpected);
        }
    }

    public async Task<Result<bool>> ExportPoDetailsAsCsvAsync(Guid purchaseOrderId, Stream destination, CancellationToken cancellationToken = default)
    {
        try
        {
            Result<bool> result = await _purchaseOrderService
                .ExportPoDetailsAsCsvAsync(purchaseOrderId, destination, cancellationToken);
            
            return result.Exception is null
                ? result
                : result.WithoutException();
        }
        catch (OperationCanceledException)
        {
            throw;
        }
        catch (Exception)
        {
            return Result<bool>
                .Fail("Failed to export PO detail, something went wrong.")
                .WithFailureType(FailureType.Unexpected);
        }
    }
}