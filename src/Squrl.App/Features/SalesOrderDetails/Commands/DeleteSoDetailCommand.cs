using MediatR;
using Squrl.App.Models;

namespace Squrl.App.Features.SalesOrderDetails.Commands;

public record DeleteSoDetailCommand(Guid Id) : IRequest<SalesOrderDetail?>;