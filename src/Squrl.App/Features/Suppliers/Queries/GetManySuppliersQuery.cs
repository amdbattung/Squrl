using MediatR;
using Squrl.App.Enums;
using Squrl.App.Models;

namespace Squrl.App.Features.Suppliers.Queries;

public record GetManySuppliersQuery(
    string? Query = null,
    int? PageNumber = null,
    int? PageSize = null,
    SortDirection OrderDirection = SortDirection.Ascending)
    : IRequest<(IReadOnlyList<Supplier> Value, int PageNumber, int PageSize, int TotalCount)>;