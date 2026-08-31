using MediatR;
using Squrl.App.Models;

namespace Squrl.App.Features.PurchaseOrderDetails.Commands;

public record DeletePoDetailCommand(Guid Id) : IRequest<PurchaseOrderDetail?>;