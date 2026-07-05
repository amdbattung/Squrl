using MediatR;
using Squrl.App.Models;

namespace Squrl.App.Features.Items.Commands;

public record DeleteItemCommand(Guid Id) : IRequest<Item?>;