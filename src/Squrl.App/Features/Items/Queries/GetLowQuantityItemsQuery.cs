using MediatR;
using Squrl.App.Models;

namespace Squrl.App.Features.Items.Queries;

public record GetLowQuantityItemsQuery : IRequest<IReadOnlyList<Item>>;