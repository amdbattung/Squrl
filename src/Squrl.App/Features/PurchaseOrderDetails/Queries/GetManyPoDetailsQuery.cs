using MediatR;
using Squrl.App.Enums;
using Squrl.App.Models;

namespace Squrl.App.Features.PurchaseOrderDetails.Queries;

public record GetManyPoDetailsQuery(
    string? Query = null,
    Guid? PurchaseOrderId = null,
    int? PageNumber = null,
    int? PageSize = null,
    SortDirection OrderDirection = SortDirection.Ascending)
    : IRequest<(IReadOnlyList<PurchaseOrderDetail> Value, int? PageNumber, int? PageSize, int TotalCount)>;