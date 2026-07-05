using MediatR;
using Squrl.App.Models;

namespace Squrl.App.Features.UnitOfMeasures.Queries;

public record GetManyUomsQuery(string? Query = null, int? PageNumber = null, int? PageSize = null)
    : IRequest<(IReadOnlyList<UnitOfMeasure> Value, int PageNumber, int PageSize, int TotalCount)>;