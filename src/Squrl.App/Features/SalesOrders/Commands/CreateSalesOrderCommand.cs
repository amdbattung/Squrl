using MediatR;
using Squrl.App.Features.SalesOrders.DTOs;
using Squrl.App.Models;

namespace Squrl.App.Features.SalesOrders.Commands;

public record CreateSalesOrderCommand(CreateSalesOrderDto SalesOrder) : IRequest<SalesOrder?>;