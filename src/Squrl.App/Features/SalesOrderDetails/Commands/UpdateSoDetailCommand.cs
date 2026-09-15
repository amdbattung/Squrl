using MediatR;
using Squrl.App.Features.SalesOrderDetails.DTOs;
using Squrl.App.Models;

namespace Squrl.App.Features.SalesOrderDetails.Commands;

public record UpdateSoDetailCommand(Guid Id, UpdateSoDetailDto SoDetail) : IRequest<SalesOrderDetail?>;