using MediatR;
using Squrl.App.Models;

namespace Squrl.App.Features.SalesOrders.Queries;

public record GetSalesOrderByIdQuery(Guid Id) : IRequest<SalesOrder?>;