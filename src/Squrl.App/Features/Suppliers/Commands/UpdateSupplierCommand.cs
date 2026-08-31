using MediatR;
using Squrl.App.Features.Suppliers.DTOs;
using Squrl.App.Models;

namespace Squrl.App.Features.Suppliers.Commands;

public record UpdateSupplierCommand(Guid Id, UpdateSupplierDto Supplier) : IRequest<Supplier?>;