using MediatR;
using Squrl.App.Features.UnitOfMeasures.DTOs;
using Squrl.App.Models;

namespace Squrl.App.Features.UnitOfMeasures.Commands;

public record CreateUomCommand(CreateUomDto Uom) : IRequest<UnitOfMeasure?>;