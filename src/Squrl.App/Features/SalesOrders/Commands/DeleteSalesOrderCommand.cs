using MediatR;
using Squrl.App.Models;

namespace Squrl.App.Features.SalesOrders.Commands;

public record DeleteSalesOrderCommand(Guid Id) : IRequest<SalesOrder?>;