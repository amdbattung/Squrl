using MediatR;
using Squrl.App.Features.SalesOrders.DTOs;
using Squrl.App.Models;

namespace Squrl.App.Features.SalesOrders.Commands;

public record UpdateSalesOrderCommand(Guid Id, UpdateSalesOrderDto SalesOrder) : IRequest<SalesOrder?>;