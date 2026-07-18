using MediatR;
using Squrl.App.Models;

namespace Squrl.App.Features.PurchaseOrders.Queries;

public record GetManyPurchaseOrdersQuery(string? Query = null, int? PageNumber = null, int? PageSize = null)
    : IRequest<(IReadOnlyList<PurchaseOrder> Value, int PageNumber, int PageSize, int TotalCount)>;