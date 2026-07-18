using MediatR;
using Squrl.App.Models;

namespace Squrl.App.Features.PurchaseOrders.Queries;

public record GetPurchaseOrderByIdQuery(Guid Id) : IRequest<PurchaseOrder?>;