using Squrl.App.Common;
using Squrl.App.Features.PurchaseOrderDetails.DTOs;
using Squrl.App.Features.PurchaseOrders.DTOs;

namespace Squrl.App.Services.PurchaseOrder;

public interface IPurchaseOrderService
{
    Task<Result<GetManyPurchaseOrdersDto>> GetManyPurchaseOrdersAsync(string? query = null, int? pageNumber = null, int? pageSize = null, CancellationToken cancellationToken = default);
    Task<Result<GetPurchaseOrderDto>> CreatePurchaseOrderAsync(CreatePurchaseOrderDto purchaseOrder, CancellationToken cancellationToken = default);
    Task<Result<GetPurchaseOrderDto>> GetPurchaseOrderByIdAsync(Guid id, CancellationToken cancellationToken = default);
    Task<Result<GetPurchaseOrderDto>> UpdatePurchaseOrderAsync(Guid id, UpdatePurchaseOrderDto purchaseOrder, CancellationToken cancellationToken = default);
    Task<Result<GetPurchaseOrderDto>> DeletePurchaseOrderAsync(Guid id, CancellationToken cancellationToken = default);

    // TODO:
    // Task<Result<GetManyPoDetailsDto>> GetPoDetailsAsync(Guid purchaseOrderId, CancellationToken cancellationToken = default);
    Task<Result<GetManyPoDetailsDto>> GetManyPoDetailsAsync(string? query = null, int? pageNumber = null, int? pageSize = null, CancellationToken cancellationToken = default);
    Task<Result<GetPoDetailDto>> AddPoDetailAsync(Guid purchaseOrderId, CreatePoDetailDto poDetail, CancellationToken cancellationToken = default);
    Task<Result<GetPoDetailDto>> GetPoDetailByIdAsync(Guid id, CancellationToken cancellationToken = default);
    Task<Result<GetPoDetailDto>> UpdatePoDetailAsync(Guid id, UpdatePoDetailDto poDetail, CancellationToken cancellationToken = default);
    Task<Result<GetPoDetailDto>> RemovePoDetailAsync(Guid id, CancellationToken cancellationToken = default);
}