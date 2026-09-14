using MediatR;
using Squrl.App.Enums;
using Squrl.App.Models;

namespace Squrl.App.Features.SalesOrders.Queries;

public record GetManySalesOrdersQuery(
    string? Query = null,
    string? Customer = null,
    int? PageNumber = null,
    int? PageSize = null,
    SortDirection OrderDirection = SortDirection.Ascending)
    : IRequest<(IReadOnlyList<SalesOrder> Value, int PageNumber, int PageSize, int TotalCount)>;