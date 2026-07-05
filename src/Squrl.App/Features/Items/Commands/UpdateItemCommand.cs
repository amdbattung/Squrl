using MediatR;
using Squrl.App.Features.Items.DTOs;
using Squrl.App.Models;

namespace Squrl.App.Features.Items.Commands;

public record UpdateItemCommand(Guid Id, UpdateItemDto Item) : IRequest<Item?>;