using MediatR;
using Squrl.App.Common;
using Squrl.App.Models;

namespace Squrl.App.Features.Items.Commands;

public record UpdateItemStockCommand(Guid ItemId, StockOperation Operation, decimal Value, bool AllowNegative = false) : IRequest<Item?>;