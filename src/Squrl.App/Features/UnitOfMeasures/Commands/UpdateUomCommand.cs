using MediatR;
using Squrl.App.Features.UnitOfMeasures.DTOs;
using Squrl.App.Models;

namespace Squrl.App.Features.UnitOfMeasures.Commands;

public record UpdateUomCommand(Guid Id, UpdateUomDto Uom) : IRequest<UnitOfMeasure?>;