using MediatR;
using Squrl.App.Models;

namespace Squrl.App.Features.PurchaseOrderDetails.Queries;

public record GetManyPoDetailsQuery(string? Query = null, int? PageNumber = null, int? PageSize = null)
    : IRequest<(IReadOnlyList<PurchaseOrderDetail> Value, int PageNumber, int PageSize, int ItemCount)>;