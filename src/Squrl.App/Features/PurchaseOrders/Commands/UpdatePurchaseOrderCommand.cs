using MediatR;
using Squrl.App.Features.PurchaseOrders.DTOs;
using Squrl.App.Models;

namespace Squrl.App.Features.PurchaseOrders.Commands;

public record UpdatePurchaseOrderCommand(Guid Id, UpdatePurchaseOrderDto PurchaseOrder) : IRequest<PurchaseOrder?>;