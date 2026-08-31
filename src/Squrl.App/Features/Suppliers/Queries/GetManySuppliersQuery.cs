using MediatR;
using Squrl.App.Models;

namespace Squrl.App.Features.Suppliers.Queries;

public record GetManySuppliersQuery(string? Query = null, int? PageNumber = null, int? PageSize = null)
           : IRequest<(IReadOnlyList<Supplier> Value, int PageNumber, int PageSize, int TotalCount)>;