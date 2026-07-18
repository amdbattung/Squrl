using MediatR;
using Squrl.App.Models;

namespace Squrl.App.Features.PurchaseOrders.Commands;

public record DeletePurchaseOrderCommand(Guid Id) : IRequest<PurchaseOrder?>;