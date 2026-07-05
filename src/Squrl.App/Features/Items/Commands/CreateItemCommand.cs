using MediatR;
using Squrl.App.Features.Items.DTOs;
using Squrl.App.Models;

namespace Squrl.App.Features.Items.Commands;

public record CreateItemCommand(CreateItemDto Item) : IRequest<Item?>;