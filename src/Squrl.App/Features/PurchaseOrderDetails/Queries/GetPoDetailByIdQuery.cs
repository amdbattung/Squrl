using MediatR;
using Squrl.App.Models;

namespace Squrl.App.Features.PurchaseOrderDetails.Queries;

public record GetPoDetailByIdQuery(Guid Id) : IRequest<PurchaseOrderDetail?>;