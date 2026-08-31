using MediatR;
using Squrl.App.Features.PurchaseOrders.DTOs;
using Squrl.App.Models;

namespace Squrl.App.Features.PurchaseOrders.Commands;

public record CreatePurchaseOrderCommand(CreatePurchaseOrderDto PurchaseOrder) : IRequest<PurchaseOrder?>;