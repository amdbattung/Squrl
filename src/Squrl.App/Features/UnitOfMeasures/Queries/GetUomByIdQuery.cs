using MediatR;
using Squrl.App.Models;

namespace Squrl.App.Features.UnitOfMeasures.Queries;

public record GetUomByIdQuery(Guid Id) : IRequest<UnitOfMeasure?>;