using MediatR;
using Squrl.App.Models;

namespace Squrl.App.Features.Suppliers.Commands;

public record DeleteSupplierCommand(Guid Id) : IRequest<Supplier?>;