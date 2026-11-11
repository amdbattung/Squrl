using FluentValidation;
using FluentValidation.Results;
using Squrl.App.Common;
using Squrl.App.Enums;
using Squrl.App.Features.PurchaseOrderDetails.DTOs;
using Squrl.App.Features.PurchaseOrders.DTOs;

namespace Squrl.App.Services.PurchaseOrder;

public class PurchaseOrderPreprocessingDecorator : IPurchaseOrderService
{
    private readonly IPurchaseOrderService _purchaseOrderService;
    private readonly IValidator<CreatePurchaseOrderDto> _createPurchaseOrderValidator;
    private readonly IValidator<UpdatePurchaseOrderDto> _updatePurchaseOrderValidator;
    private readonly IValidator<CreatePoDetailDto> _createPoDetailValidator;
    private readonly IValidator<UpdatePoDetailDto> _updatePoDetailValidator;

    public PurchaseOrderPreprocessingDecorator(IPurchaseOrderService purchaseOrderService,
        IValidator<CreatePurchaseOrderDto> createPurchaseOrderValidator,
        IValidator<UpdatePurchaseOrderDto> updatePurchaseOrderValidator,
        IValidator<CreatePoDetailDto> createPoDetailValidator,
        IValidator<UpdatePoDetailDto> updatePoDetailValidator)
    {
        _purchaseOrderService = purchaseOrderService;
        _createPurchaseOrderValidator = createPurchaseOrderValidator;
        _updatePurchaseOrderValidator = updatePurchaseOrderValidator;
        _createPoDetailValidator = createPoDetailValidator;
        _updatePoDetailValidator = updatePoDetailValidator;
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
        search = search?.Trim();
        pageNumber = pageNumber >= 1 ? pageNumber : 1;
        pageSize = pageSize is >= 1 and <= 50 ? pageSize : 10;
        
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

        return result;
    }

    public async Task<Result<GetPurchaseOrderDto>> CreatePurchaseOrderAsync(CreatePurchaseOrderDto purchaseOrder, CancellationToken cancellationToken = default)
    {
        purchaseOrder.Status ??= PurchaseOrderStatus.Pending;
            
        ValidationResult poValidationResult = await _createPurchaseOrderValidator
            .ValidateAsync(purchaseOrder, cancellationToken);
            
        if (!poValidationResult.IsValid)
        {
            return Result<GetPurchaseOrderDto>.Fail(poValidationResult.Errors
                    .Select(e => e.ErrorMessage)
                    .ToArray())
                .WithFailureType(FailureType.Validation);
        }
        
        Result<GetPurchaseOrderDto> result = await _purchaseOrderService
            .CreatePurchaseOrderAsync(purchaseOrder, cancellationToken);
        
        return result;
    }

    public async Task<Result<GetPurchaseOrderDto>> GetPurchaseOrderByIdAsync(Guid id, CancellationToken cancellationToken = default)
    {
        if (id == Guid.Empty)
        {
            return Result<GetPurchaseOrderDto>.Fail("Null or invalid ID.")
                .WithFailureType(FailureType.Validation);
        }
        
        Result<GetPurchaseOrderDto> result = await _purchaseOrderService
            .GetPurchaseOrderByIdAsync(id, cancellationToken);
        
        return result;
    }

    public async Task<Result<GetPurchaseOrderDto>> UpdatePurchaseOrderAsync(
        Guid id,
        UpdatePurchaseOrderDto purchaseOrder,
        CancellationToken cancellationToken = default)
    {
        if (id == Guid.Empty)
        {
            return Result<GetPurchaseOrderDto>.Fail("Null or invalid ID.")
                .WithFailureType(FailureType.Validation);
        }
            
        ValidationResult poValidationResult = await _updatePurchaseOrderValidator
            .ValidateAsync(purchaseOrder, cancellationToken);
            
        if (!poValidationResult.IsValid)
        {
            return Result<GetPurchaseOrderDto>.Fail(poValidationResult.Errors
                    .Select(e => e.ErrorMessage)
                    .ToArray())
                .WithFailureType(FailureType.Validation);
        }
        
        Result<GetPurchaseOrderDto> result = await _purchaseOrderService
            .UpdatePurchaseOrderAsync(id, purchaseOrder, cancellationToken);
        
        return result;
    }

    public async Task<Result<GetPurchaseOrderDto>> DeletePurchaseOrderAsync(Guid id, CancellationToken cancellationToken = default)
    {
        if (id == Guid.Empty)
        {
            return Result<GetPurchaseOrderDto>.Fail("Null or invalid ID.")
                .WithFailureType(FailureType.Validation);
        }
        
        Result<GetPurchaseOrderDto> result = await _purchaseOrderService
            .DeletePurchaseOrderAsync(id, cancellationToken);
        
        return result;
    }

    public async Task<Result<GetManyPoDetailsDto>> GetPoDetailsAsync(
        Guid purchaseOrderId,
        SortDirection orderDirection = SortDirection.Ascending,
        CancellationToken cancellationToken = default)
    {
        if (purchaseOrderId == Guid.Empty)
        {
            return Result<GetManyPoDetailsDto>.Fail("Null or invalid ID.")
                .WithFailureType(FailureType.Validation);
        }
        
        Result<GetManyPoDetailsDto> result = await _purchaseOrderService
            .GetPoDetailsAsync(purchaseOrderId, orderDirection, cancellationToken);
        
        return result;
    }

    public async Task<Result<GetPoDetailDto>> AddPoDetailAsync(Guid purchaseOrderId, CreatePoDetailDto poDetail, CancellationToken cancellationToken = default)
    {
        if (purchaseOrderId == Guid.Empty)
        {
            return Result<GetPoDetailDto>.Fail("Null or invalid ID.")
                .WithFailureType(FailureType.Validation);
        }
        
        ValidationContext<CreatePoDetailDto> validationContext = new(poDetail)
        {
            RootContextData =
            {
                ["SkipPurchaseOrderIdValidation"] = true
            }
        };

        ValidationResult validationResult = await _createPoDetailValidator
            .ValidateAsync(validationContext, cancellationToken);
            
        if (!validationResult.IsValid)
        {
            return Result<GetPoDetailDto>.Fail(validationResult.Errors
                    .Select(e => e.ErrorMessage)
                    .ToArray())
                .WithFailureType(FailureType.Validation);
        }
        
        Result<GetPoDetailDto> result = await _purchaseOrderService
            .AddPoDetailAsync(purchaseOrderId, poDetail, cancellationToken);
        
        return result;
    }

    public async Task<Result<GetPoDetailDto>> GetPoDetailByIdAsync(Guid id, CancellationToken cancellationToken = default)
    {
        if (id == Guid.Empty)
        {
            return Result<GetPoDetailDto>.Fail("Null or invalid ID.")
                .WithFailureType(FailureType.Validation);
        }
        
        Result<GetPoDetailDto> result = await _purchaseOrderService
            .GetPoDetailByIdAsync(id, cancellationToken);
        
        return result;
    }

    public async Task<Result<GetPoDetailDto>> UpdatePoDetailAsync(Guid id, UpdatePoDetailDto poDetail, CancellationToken cancellationToken = default)
    {
        if (id == Guid.Empty)
        {
            return Result<GetPoDetailDto>.Fail("Null or invalid ID.")
                .WithFailureType(FailureType.Validation);
        }
        
        ValidationResult validationResult = await _updatePoDetailValidator
            .ValidateAsync(poDetail, cancellationToken);
            
        if (!validationResult.IsValid)
        {
            return Result<GetPoDetailDto>.Fail(validationResult.Errors
                    .Select(e => e.ErrorMessage)
                    .ToArray())
                .WithFailureType(FailureType.Validation);
        }
        
        Result<GetPoDetailDto> result = await _purchaseOrderService
            .UpdatePoDetailAsync(id, poDetail, cancellationToken);
        
        return result;
    }

    public async Task<Result<GetPoDetailDto>> RemovePoDetailAsync(Guid id, CancellationToken cancellationToken = default)
    {
        if (id == Guid.Empty)
        {
            return Result<GetPoDetailDto>.Fail("Null or invalid ID.")
                .WithFailureType(FailureType.Validation);
        }
        
        Result<GetPoDetailDto> result = await _purchaseOrderService
            .RemovePoDetailAsync(id, cancellationToken);
        
        return result;
    }

    public async Task<Result<GetPurchaseOrderDto>> ReceivePurchaseOrderAsync(Guid id, CancellationToken cancellationToken = default)
    {
        if (id == Guid.Empty)
        {
            return Result<GetPurchaseOrderDto>.Fail("Null or invalid ID.")
                .WithFailureType(FailureType.Validation);
        }
        
        Result<GetPurchaseOrderDto> result = await _purchaseOrderService
            .ReceivePurchaseOrderAsync(id, cancellationToken);
        
        return result;
    }

    public async Task<Result<GetPurchaseOrderDto>> CancelPurchaseOrderAsync(Guid id, CancellationToken cancellationToken = default)
    {
        if (id == Guid.Empty)
        {
            return Result<GetPurchaseOrderDto>.Fail("Null or invalid ID.")
                .WithFailureType(FailureType.Validation);
        }
        
        Result<GetPurchaseOrderDto> result = await _purchaseOrderService
            .CancelPurchaseOrderAsync(id, cancellationToken);
        
        return result;
    }

    public async Task<Result<GetPurchaseOrderDto>> ReturnPurchaseOrderAsync(Guid id, CancellationToken cancellationToken = default)
    {
        if (id == Guid.Empty)
        {
            return Result<GetPurchaseOrderDto>.Fail("Null or invalid ID.")
                .WithFailureType(FailureType.Validation);
        }
        
        Result<GetPurchaseOrderDto> result = await _purchaseOrderService
            .ReturnPurchaseOrderAsync(id, cancellationToken);
        
        return result;
    }

    public async Task<Result<GetPurchaseOrderDto>> UpdatePurchaseOrderStatusAsync(Guid id, PurchaseOrderStatus status, CancellationToken cancellationToken = default)
    {
        Result<GetPurchaseOrderDto> result = await _purchaseOrderService
            .UpdatePurchaseOrderStatusAsync(id, status, cancellationToken);
        
        return result;
    }

    public async Task<Result<GetPurchaseOrderDto>> RevertReceivedPurchaseOrderToPendingAsync(Guid id, CancellationToken cancellationToken = default)
    {
        if (id == Guid.Empty)
        {
            return Result<GetPurchaseOrderDto>.Fail("Null or invalid ID.")
                .WithFailureType(FailureType.Validation);
        }
        
        Result<GetPurchaseOrderDto> result = await _purchaseOrderService
            .RevertReceivedPurchaseOrderToPendingAsync(id, cancellationToken);
        
        return result;
    }

    public async Task<Result<GetPurchaseOrderDto>> UncancelPurchaseOrderAsync(Guid id, CancellationToken cancellationToken = default)
    {
        if (id == Guid.Empty)
        {
            return Result<GetPurchaseOrderDto>.Fail("Null or invalid ID.")
                .WithFailureType(FailureType.Validation);
        }
        
        Result<GetPurchaseOrderDto> result = await _purchaseOrderService
            .UncancelPurchaseOrderAsync(id, cancellationToken);
        
        return result;
    }

    public async Task<Result<bool>> ExportPoDetailsAsCsvAsync(Guid purchaseOrderId, Stream destination, CancellationToken cancellationToken = default)
    {
        if (purchaseOrderId == Guid.Empty)
        {
            return Result<bool>.Fail("Null or invalid ID.")
                .WithFailureType(FailureType.Validation);
        }
        
        if (!destination.CanWrite)
        {
            return Result<bool>.Fail("Destination must be writable.")
                .WithFailureType(FailureType.Validation);
        }
        
        Result<bool> result = await _purchaseOrderService
            .ExportPoDetailsAsCsvAsync(purchaseOrderId, destination, cancellationToken);
        
        return result;
    }

    public async Task<Result<bool>> ExportPoDetailsAsExcelAsync(Guid purchaseOrderId, Stream destination,
        CancellationToken cancellationToken = default)
    {
        if (purchaseOrderId == Guid.Empty)
        {
            return Result<bool>.Fail("Null or invalid ID.")
                .WithFailureType(FailureType.Validation);
        }
        
        if (!destination.CanWrite)
        {
            return Result<bool>.Fail("Destination must be writable.")
                .WithFailureType(FailureType.Validation);
        }
        
        Result<bool> result = await _purchaseOrderService
            .ExportPoDetailsAsExcelAsync(purchaseOrderId, destination, cancellationToken);
        
        return result;
    }
}