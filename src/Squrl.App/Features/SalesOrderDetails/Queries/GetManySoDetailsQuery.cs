using MediatR;
using Squrl.App.Enums;
using Squrl.App.Models;

namespace Squrl.App.Features.SalesOrderDetails.Queries;

public record GetManySoDetailsQuery(
    string? Query = null,
    Guid? SalesOrderId = null,
    int? PageNumber = null,
    int? PageSize = null,
    SortDirection OrderDirection = SortDirection.Ascending)
    : IRequest<(IReadOnlyList<SalesOrderDetail> Value, int PageNumber, int PageSize, int TotalCount)>;