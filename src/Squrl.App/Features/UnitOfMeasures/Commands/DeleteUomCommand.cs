using MediatR;
using Squrl.App.Models;

namespace Squrl.App.Features.UnitOfMeasures.Commands;

public record DeleteUomCommand(Guid Id) : IRequest<UnitOfMeasure?>;