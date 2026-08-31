using MediatR;
using Squrl.App.Models;

namespace Squrl.App.Features.Suppliers.Queries;

public record GetSupplierByIdQuery(Guid Id) : IRequest<Supplier?>;