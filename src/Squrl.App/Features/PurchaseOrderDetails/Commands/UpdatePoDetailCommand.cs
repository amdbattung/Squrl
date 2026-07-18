using MediatR;
using Squrl.App.Features.PurchaseOrderDetails.DTOs;
using Squrl.App.Models;

namespace Squrl.App.Features.PurchaseOrderDetails.Commands;

public record UpdatePoDetailCommand(Guid Id, UpdatePoDetailDto PoDetail) : IRequest<PurchaseOrderDetail?>;