using FluentValidation;
using FluentValidation.Results;
using MediatR;
using Squrl.App.Common;
using Squrl.App.Enums;
using Squrl.App.Features.Items.Commands;
using Squrl.App.Features.Items.DTOs;
using Squrl.App.Features.Items.Mapping;
using Squrl.App.Features.Items.Queries;
using Squrl.App.Models;

namespace Squrl.App.Services.Inventory;

public class InventoryService : IInventoryService
{
    private readonly IMediator _mediator;
    private readonly IValidator<CreateItemDto> _createItemValidator;
    private readonly IValidator<UpdateItemDto> _updateItemValidator;

    public InventoryService(IMediator mediator,
        IValidator<CreateItemDto> createItemValidator,
        IValidator<UpdateItemDto> updateItemValidator)
    {
        _mediator = mediator;
        _createItemValidator = createItemValidator;
        _updateItemValidator = updateItemValidator;
    }
    
    public async Task<Result<GetManyItemsDto>> GetManyItemsAsync(string? query = null, int? pageNumber = null, int? pageSize = null, CancellationToken cancellationToken = default)
    {
        try
        {
            pageNumber = pageNumber >= 1 ? pageNumber : null;
            pageSize = pageSize is >= 1 and <= 50 ? pageSize : null;
        
            var result = await _mediator.Send(new GetManyItemsQuery(query, pageNumber, pageSize), cancellationToken);

            GetManyItemsDto payload = new GetManyItemsDto(
                result.PageSize,
                result.PageNumber,
                result.ItemCount,
                result.Value.Select(ItemMapper.ToDto).ToList());
            
            return Result<GetManyItemsDto>.Ok(payload);
        }
        catch (OperationCanceledException)
        {
            throw;
        }
        catch (Exception e)
        {
            return Result<GetManyItemsDto>.Fail("Failed to retrieve items.")
                .WithException(e)
                .WithFailureType(FailureType.Exception);
        }
    }
    
    public async Task<Result<GetItemDto>> CreateItemAsync(CreateItemDto item, CancellationToken cancellationToken = default)
    {
        try
        {
            item.Name = item.Name?.Trim();
            item.Description = string.IsNullOrWhiteSpace(item.Description) ? null : item.Description.Trim();
            item.Locations = item.Locations?
                .Select(i => i.Trim())
                .Where(i => !string.IsNullOrWhiteSpace(i))
                .ToList();
            
            ValidationResult validationResult = await _createItemValidator.ValidateAsync(item, cancellationToken);
        
            if (!validationResult.IsValid)
            {
                return Result<GetItemDto>.Fail(validationResult.Errors
                    .Select(e => e.ErrorMessage)
                    .ToArray())
                    .WithFailureType(FailureType.Validation);
            }
        
            Item? result = await _mediator.Send(new CreateItemCommand(item), cancellationToken);
        
            if (result == null)
            {
                return Result<GetItemDto>.Fail("Failed to create item.")
                    .WithFailureType(FailureType.BusinessLogic);
            }
            
            return Result<GetItemDto>.Ok(ItemMapper.ToDto(result));
        }
        catch (OperationCanceledException)
        {
            throw;
        }
        catch (Exception e)
        {
            return Result<GetItemDto>.Fail("Failed to create item.")
                .WithException(e)
                .WithFailureType(FailureType.Exception);
        }
    }
    
    public async Task<Result<GetItemDto>> GetItemByIdAsync(Guid id, CancellationToken cancellationToken = default)
    {
        try
        {
            if (id == Guid.Empty)
            {
                return Result<GetItemDto>.Fail("Null or invalid ID.")
                    .WithFailureType(FailureType.Validation);
            }
        
            Item? result = await _mediator.Send(new GetItemByIdQuery(id), cancellationToken);
        
            if (result == null)
            {
                return Result<GetItemDto>.Fail("Item not found.")
                    .WithFailureType(FailureType.NotFound);
            }
        
            return Result<GetItemDto>.Ok(ItemMapper.ToDto(result));
        }
        catch (OperationCanceledException)
        {
            throw;
        }
        catch (Exception e)
        {
            return Result<GetItemDto>.Fail("Failed to retrieve item.")
                .WithException(e)
                .WithFailureType(FailureType.Exception);
        }
    }
    
    public async Task<Result<GetItemDto>> UpdateItemAsync(Guid id, UpdateItemDto item, CancellationToken cancellationToken = default)
    {
        try
        {
            if (id == Guid.Empty)
            {
                return Result<GetItemDto>.Fail("Null or invalid ID.")
                    .WithFailureType(FailureType.Validation);
            }
            
            item.Name = item.Name?.Trim();
            item.Description = string.IsNullOrWhiteSpace(item.Description) ? null : item.Description.Trim();
            item.Locations = item.Locations?
                .Select(i => i.Trim())
                .Where(i => !string.IsNullOrWhiteSpace(i))
                .ToList();
        
            ValidationResult validationResult = await _updateItemValidator.ValidateAsync(item, cancellationToken);
        
            if (!validationResult.IsValid)
            {
                return Result<GetItemDto>.Fail(validationResult.Errors
                    .Select(e => e.ErrorMessage)
                    .ToArray())
                    .WithFailureType(FailureType.Validation);
            }
        
            Item? result = await _mediator.Send(new UpdateItemCommand(id, item), cancellationToken);
        
            if (result == null)
            {
                return Result<GetItemDto>.Fail("Item not found.")
                    .WithFailureType(FailureType.NotFound);
            }
        
            return Result<GetItemDto>.Ok(ItemMapper.ToDto(result));
        }
        catch (OperationCanceledException)
        {
            throw;
        }
        catch (Exception e)
        {
            return Result<GetItemDto>.Fail("Failed to update item.")
                .WithException(e)
                .WithFailureType(FailureType.Exception);
        }
    }
    
    public async Task<Result<GetItemDto>> DeleteItemAsync(Guid id, CancellationToken cancellationToken = default)
    {
        try
        {
            if (id == Guid.Empty)
            {
                return Result<GetItemDto>.Fail("Null or invalid ID.")
                    .WithFailureType(FailureType.Validation);
            }
        
            Item? result = await _mediator.Send(new DeleteItemCommand(id), cancellationToken);
        
            if (result == null)
            {
                return Result<GetItemDto>.Fail("Item not found.")
                    .WithFailureType(FailureType.NotFound);
            }
        
            return Result<GetItemDto>.Ok(ItemMapper.ToDto(result));
        }
        catch (OperationCanceledException)
        {
            throw;
        }
        catch (Exception e)
        {
            return Result<GetItemDto>.Fail("Failed to delete item.")
                .WithException(e)
                .WithFailureType(FailureType.Exception);
        }
    }
    
    #region Stock Control
    public async Task<Result<GetItemDto>> AddStocksAsync(Guid itemId, decimal quantity, CancellationToken cancellationToken = default)
    {
        try
        {
            if (itemId == Guid.Empty)
            {
                return Result<GetItemDto>.Fail("Null or invalid ID.")
                    .WithFailureType(FailureType.Validation);
            }
        
            if (quantity <= 0m)
            {
                return Result<GetItemDto>.Fail("Invalid quantity.")
                    .WithFailureType(FailureType.Validation);
            }
        
            Item? result = await _mediator.Send(new UpdateItemStockCommand(itemId, StockOperation.Add, quantity), cancellationToken);
        
            if (result == null)
            {
                return Result<GetItemDto>.Fail("Item not found.")
                    .WithFailureType(FailureType.NotFound);
            }
            
            return Result<GetItemDto>.Ok(ItemMapper.ToDto(result));
        }
        catch (OperationCanceledException)
        {
            throw;
        }
        catch (Exception e)
        {
            return Result<GetItemDto>.Fail("Failed to add stocks.")
                .WithException(e)
                .WithFailureType(FailureType.Exception);
        }
    }
    
    public async Task<Result<GetItemDto>> RemoveStocksAsync(Guid itemId, decimal quantity, CancellationToken cancellationToken = default)
    {
        try
        {
            if (itemId == Guid.Empty)
            {
                return Result<GetItemDto>.Fail("Null or invalid ID.")
                    .WithFailureType(FailureType.Validation);
            }
        
            if (quantity <= 0m)
            {
                return Result<GetItemDto>.Fail("Invalid quantity.")
                    .WithFailureType(FailureType.Validation);
            }
        
            Item? result = await _mediator.Send(new UpdateItemStockCommand(itemId, StockOperation.Subtract, quantity), cancellationToken);

            if (result == null)
            {
                return Result<GetItemDto>.Fail("Item not found.")
                    .WithFailureType(FailureType.NotFound);
            }
            
            return Result<GetItemDto>.Ok(ItemMapper.ToDto(result));
        }
        catch (OperationCanceledException)
        {
            throw;
        }
        catch (Exception e)
        {
            return Result<GetItemDto>.Fail("Failed to remove stocks.")
                .WithException(e)
                .WithFailureType(FailureType.Exception);
        }
    }
    
    public async Task<Result<GetItemDto>> DynamicStockUpdateAsync(Guid itemId, string quantity, CancellationToken cancellationToken = default)
    {
        // A powerful method that accepts an operation symbol placed as
        // the first character of the 'quantity' argument to determine
        // which arithmetic operation to perform to update an item's
        // quantity. Dropping the operation symbol entirely will replace
        // the existing quantity with the new one.
        
        try
        {
            if (itemId == Guid.Empty)
            {
                return Result<GetItemDto>.Fail("Null or invalid ID.")
                    .WithFailureType(FailureType.Validation);
            }
        
            if (string.IsNullOrWhiteSpace(quantity))
            {
                return Result<GetItemDto>.Fail("Invalid quantity.")
                    .WithFailureType(FailureType.Validation);
            }
            
            quantity = quantity.Trim();
            StockOperation operation;
            decimal value;
        
            if (char.IsDigit(quantity[0]))
            {
                if (!decimal.TryParse(quantity, out value))
                {
                    return Result<GetItemDto>.Fail("Invalid quantity.")
                        .WithFailureType(FailureType.Validation);
                }
            
                if (value < 0m)
                {
                    return Result<GetItemDto>.Fail("Invalid negative quantity.")
                        .WithFailureType(FailureType.Validation);
                }
            
                operation = StockOperation.Set;
            }
            else
            {
                switch (quantity[0])
                {
                    case '+':
                        operation = StockOperation.Add;
                        break;
            
                    case '−' or '-':
                        operation = StockOperation.Subtract;
                        break;
            
                    case '×' or '*' or 'x' or 'X':
                        operation = StockOperation.Multiply;
                        break;
            
                    case '÷' or '/':
                        operation = StockOperation.Divide;
                        break;
                    
                    case '=':
                        operation = StockOperation.Set;
                        break;
                
                    default:
                        return Result<GetItemDto>.Fail("Invalid operator.")
                            .WithFailureType(FailureType.Validation);
                }

                if (!decimal.TryParse(quantity.AsSpan(1), out value))
                {
                    return Result<GetItemDto>.Fail("Invalid quantity.")
                        .WithFailureType(FailureType.Validation);
                }

                if (operation == StockOperation.Set)
                {
                    if (value < 0m)
                    {
                        return Result<GetItemDto>.Fail("Invalid negative quantity.")
                            .WithFailureType(FailureType.Validation);
                    }
                }
                else if (value <= 0m)
                {
                    return Result<GetItemDto>.Fail("Invalid quantity.")
                        .WithFailureType(FailureType.Validation);
                }
            }
        
            Item? result = await _mediator.Send(new UpdateItemStockCommand(itemId, operation, value), cancellationToken);

            if (result == null)
            {
                return Result<GetItemDto>.Fail("Item not found.")
                    .WithFailureType(FailureType.NotFound);
            }
        
            return Result<GetItemDto>.Ok(ItemMapper.ToDto(result));
        }
        catch (OperationCanceledException)
        {
            throw;
        }
        catch (Exception e)
        {
            return Result<GetItemDto>.Fail("Failed to update stocks.")
                .WithException(e)
                .WithFailureType(FailureType.Exception);
        }
    }
    #endregion
}