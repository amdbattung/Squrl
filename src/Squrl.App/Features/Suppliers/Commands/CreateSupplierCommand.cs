using MediatR;
using Squrl.App.Features.Suppliers.DTOs;
using Squrl.App.Models;

namespace Squrl.App.Features.Suppliers.Commands;

public record CreateSupplierCommand(CreateSupplierDto Supplier) : IRequest<Supplier?>;