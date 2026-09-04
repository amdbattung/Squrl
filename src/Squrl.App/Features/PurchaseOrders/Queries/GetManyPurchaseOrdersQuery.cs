using MediatR;
using Squrl.App.Enums;
using Squrl.App.Models;

namespace Squrl.App.Features.PurchaseOrders.Queries;

public record GetManyPurchaseOrdersQuery(
    string? Query = null,
    Guid? SupplierId = null,
    bool IsPending = false,
    bool IsReceived = false,
    bool IsCancelled = false,
    bool HasFailed = false,
    bool IsReturned = false,
    int? PageNumber = null,
    int? PageSize = null,
    SortDirection OrderDirection = SortDirection.Ascending)
    : IRequest<(IReadOnlyList<PurchaseOrder> Value, int? PageNumber, int? PageSize, int TotalCount)>;