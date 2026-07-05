using MediatR;
using Squrl.App.Models;

namespace Squrl.App.Features.Items.Queries;

public record GetManyItemsQuery(string? Query = null, int? PageNumber = null, int? PageSize = null)
    : IRequest<(IReadOnlyList<Item> Value, int PageNumber, int PageSize, int ItemCount)>;