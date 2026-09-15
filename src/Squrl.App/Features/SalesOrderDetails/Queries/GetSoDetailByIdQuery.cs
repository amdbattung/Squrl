using MediatR;
using Squrl.App.Models;

namespace Squrl.App.Features.SalesOrderDetails.Queries;

public record GetSoDetailByIdQuery(Guid Id) : IRequest<SalesOrderDetail?>;