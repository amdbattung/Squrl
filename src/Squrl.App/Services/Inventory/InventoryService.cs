using Microsoft.EntityFrameworkCore;
using NodaTime;
using Squrl.App.Common;
using Squrl.App.Data;
using Squrl.App.Enums;
using Squrl.App.Features.Items.DTOs;
using Squrl.App.Features.Items.Mapping;
using Squrl.App.Infrastructure.BackgroundTaskQueue;
using Squrl.App.Infrastructure.ImageProcessor;
using Squrl.App.Infrastructure.ImageStorage;
using Squrl.App.Infrastructure.TransactionManager;
using Squrl.App.Models;

namespace Squrl.App.Services.Inventory;

public class InventoryService : IInventoryService
{
    private readonly DataContext _dataContext;
    private readonly ITransactionManager _transactionManager;
    private readonly IBackgroundTaskQueue _queue;
    private readonly IImageProcessor _imageProcessor;
    private readonly IImageStorage _imageStorage;

    public InventoryService(
        DataContext dataContext,
        ITransactionManager transactionManager,
        IBackgroundTaskQueue queue,
        IImageProcessor imageProcessor,
        IImageStorage imageStorage)
    {
        _dataContext = dataContext;
        _transactionManager = transactionManager;
        _queue = queue;
        _imageProcessor = imageProcessor;
        _imageStorage = imageStorage;
    }
    
    public async Task<Result<GetManyItemsDto>> GetManyItemsAsync(
        string? search = null,
        int pageNumber = 1,
        int pageSize = 10,
        SortDirection orderDirection = SortDirection.Ascending,
        CancellationToken cancellationToken = default)
    {
        IQueryable<Item> query = _dataContext.Items
            .AsNoTracking()
            .Include(i => i.Uom);
        
        if (!string.IsNullOrWhiteSpace(search))
        {
            query = query.Where(i =>
                EF.Functions.Like(i.Name, $"%{search}%"));
        }
        
        if (orderDirection == SortDirection.Descending)
        {
            query = query
                .OrderByDescending(i => EF.Property<Instant>(i, "DateCreated"))
                .ThenBy(i => i.Id);
        }
        else
        {
            query = query
                .OrderBy(i => EF.Property<Instant>(i, "DateCreated"))
                .ThenBy(i => i.Id);
        }
        
        int totalCount = await query.CountAsync(cancellationToken);

        IReadOnlyList<Item> existingItems = await query
            .Skip((pageNumber - 1) * pageSize)
            .Take(pageSize)
            .ToListAsync(cancellationToken);
        
        GetManyItemsDto payload = new(
            pageNumber,
            pageSize,
            totalCount,
            existingItems.Select(ItemMapper.ToDto).ToList());

        return Result<GetManyItemsDto>.Ok(payload);
    }
    
    public async Task<Result<GetItemDto>> CreateItemAsync(
        CreateItemDto item,
        Stream? photo = null,
        CancellationToken cancellationToken = default)
    {
        Func<CancellationToken, Task<Result<GetItemDto>>> action = async ct =>
        {
            Models.UnitOfMeasure? uom = await _dataContext.UnitOfMeasures
                .AsNoTracking()
                .FirstOrDefaultAsync(u => u.Id == item.UomId, ct);
            
            if (uom == null)
            {
                return Result<GetItemDto>.Fail("Selected UOM not found.")
                    .WithFailureType(FailureType.NotFound);
            }
            
            string? uploadedImage = null;
            
            if (photo is not null)
            {
                uploadedImage = await _imageProcessor.SaveAsync(photo, ct);

                if (!string.IsNullOrWhiteSpace(uploadedImage))
                {
                    // If the transaction rolls back, delete the new image.
                    _transactionManager.OnRollback(async () =>
                    {
                        await _imageStorage.DeleteAsync(uploadedImage, CancellationToken.None);
                    });
                }
            }
            
            Item newItem = new()
            {
                Id = Guid.NewGuid(),
                Name = item.Name ?? "",
                Uom = uom,
                Description = item.Description,
                Image = !string.IsNullOrWhiteSpace(uploadedImage)
                    ? uploadedImage
                    : null,
                ListPrice = item.ListPrice,
                RetailPrice = item.RetailPrice,
                CostPrice = item.CostPrice,
                Quantity = item.Quantity ?? 0m,
                LowQuantityAlertThreshold =  item.LowQuantityAlertThreshold,
                Locations = item.Locations ?? new List<string>()
            };
            
            await _dataContext.Items.AddAsync(newItem, ct);
            
            return Result<GetItemDto>.Ok(ItemMapper.ToDto(newItem));
        };
        
        return await _transactionManager.ExecuteAsync(action, cancellationToken);
    }
    
    public async Task<Result<GetItemDto>> GetItemByIdAsync(Guid id, CancellationToken cancellationToken = default)
    {
        Item? existingItem = await _dataContext.Items
            .AsNoTracking()
            .Include(i => i.Uom)
            .Where(i => i.Id == id)
            .FirstOrDefaultAsync(cancellationToken);
        
        if (existingItem == null)
        {
            return Result<GetItemDto>.Fail("Item not found.")
                .WithFailureType(FailureType.NotFound);
        }

        return Result<GetItemDto>.Ok(ItemMapper.ToDto(existingItem));
    }
    
    public async Task<Result<GetItemDto>> UpdateItemAsync(
        Guid id,
        UpdateItemDto item,
        Stream? photo = null,
        CancellationToken cancellationToken = default)
    {
        Func<CancellationToken, Task<Result<GetItemDto>>> action = async ct =>
        {
            Item? existingItem = await _dataContext.Items
                .Include(i => i.Uom)
                .FirstOrDefaultAsync(i => i.Id == id, ct);
            
            if (existingItem == null)
            {
                return Result<GetItemDto>.Fail("Item not found.")
                    .WithFailureType(FailureType.NotFound);
            }

            Models.UnitOfMeasure? uom = null;

            if (item.UomId != null
                && item.UomId != existingItem.Uom.Id)
            {
                uom = await _dataContext.UnitOfMeasures
                    .FirstOrDefaultAsync(u => u.Id == item.UomId, ct);
            
                if (uom == null)
                {
                    return Result<GetItemDto>.Fail("Selected UOM not found.")
                        .WithFailureType(FailureType.NotFound);
                }
            }
            
            string? uploadedImage = null;
            string? oldImage = null;
            
            if (photo is not null)
            {
                uploadedImage = await _imageProcessor.SaveAsync(photo, ct);
                
                if (!string.IsNullOrWhiteSpace(uploadedImage))
                {
                    oldImage = existingItem.Image;

                    // If the transaction rolls back, delete the new image.
                    _transactionManager.OnRollback(async () =>
                    {
                        await _imageStorage.DeleteAsync(uploadedImage, CancellationToken.None);
                    });
                }
                else
                {
                    uploadedImage = null;
                }
            }
            
            existingItem.Name = item.Name ?? existingItem.Name;
            existingItem.Uom = uom ?? existingItem.Uom;
            existingItem.Description = item.Description ?? existingItem.Description;
            existingItem.Image = uploadedImage ?? existingItem.Image;
            existingItem.ListPrice = item.ListPrice ?? existingItem.ListPrice;
            existingItem.RetailPrice = item.RetailPrice ?? existingItem.RetailPrice;
            existingItem.CostPrice = item.CostPrice ?? existingItem.CostPrice;
            existingItem.Quantity = item.Quantity ?? existingItem.Quantity;
            existingItem.LowQuantityAlertThreshold = item.LowQuantityAlertThreshold ?? existingItem.LowQuantityAlertThreshold;
            existingItem.Locations = item.Locations ?? existingItem.Locations;
            
            // If a replacement was uploaded, delete the old image.
            if (!string.IsNullOrWhiteSpace(oldImage))
            {
                string imageToDelete = oldImage;

                _transactionManager.OnCommit(async () =>
                {
                    await _queue.QueueAsync(async c =>
                    {
                        await _imageStorage.DeleteAsync(imageToDelete, c);
                    });
                });
            }
            
            return Result<GetItemDto>.Ok(ItemMapper.ToDto(existingItem));
        };
        
        return await _transactionManager.ExecuteAsync(action, cancellationToken);
    }
    
    public async Task<Result<GetItemDto>> DeleteItemAsync(Guid id, CancellationToken cancellationToken = default)
    {
        Func<CancellationToken, Task<Result<GetItemDto>>> action = async ct =>
        {
            Item? existingItem = await _dataContext.Items
                .Include(i => i.Uom)
                .FirstOrDefaultAsync(i => i.Id == id, ct);
        
            if (existingItem == null)
            {
                return Result<GetItemDto>.Fail("Item not found.")
                    .WithFailureType(FailureType.NotFound);
            }
        
            _dataContext.Items.Remove(existingItem);

            if (!string.IsNullOrWhiteSpace(existingItem.Image))
            {
                _transactionManager.OnCommit(async () =>
                {
                    await _queue.QueueAsync(async c =>
                    {
                        await _imageStorage.DeleteAsync(existingItem.Image, c);
                    });
                });
            }
            
            return Result<GetItemDto>.Ok(ItemMapper.ToDto(existingItem));
        };
        
        return await _transactionManager.ExecuteAsync(action, cancellationToken);
    }
    
    #region Stock Control
    public async Task<Result<GetItemDto>> AddStocksAsync(Guid itemId, decimal quantity, CancellationToken cancellationToken = default)
    {
        Func<CancellationToken, Task<Result<GetItemDto>>> action = async ct =>
        {
            if (quantity <= 0m)
            {
                return Result<GetItemDto>.Fail("Invalid quantity.")
                    .WithFailureType(FailureType.Validation);
            }

            int affected = await _dataContext.Database.ExecuteSqlInterpolatedAsync(
                $"""
                 UPDATE Items
                 SET Quantity = Quantity + {quantity}
                 WHERE Id = {itemId}
                 """,
                ct);

            if (affected <= 0)
            {
                return Result<GetItemDto>.Fail("Item not found.")
                    .WithFailureType(FailureType.NotFound);
            }
        
            Item? existingItem = await _dataContext.Items
                .AsNoTracking()
                .Include(i => i.Uom)
                .FirstOrDefaultAsync(i => i.Id == itemId, ct);
        
            if (existingItem == null)
            {
                return Result<GetItemDto>.Fail("Item not found.")
                    .WithFailureType(FailureType.NotFound);
            }
        
            return Result<GetItemDto>.Ok(ItemMapper.ToDto(existingItem));
        };
        
        return await _transactionManager.ExecuteAsync(action, cancellationToken);
    }
    
    public async Task<Result<GetItemDto>> RemoveStocksAsync(Guid itemId, decimal quantity, CancellationToken cancellationToken = default)
    {
        Func<CancellationToken, Task<Result<GetItemDto>>> action = async ct =>
        {
            if (quantity <= 0m)
            {
                return Result<GetItemDto>.Fail("Invalid quantity.")
                    .WithFailureType(FailureType.Validation);
            }

            int affected = await _dataContext.Database.ExecuteSqlInterpolatedAsync(
                $"""
                 UPDATE Items
                 SET Quantity = Quantity - {quantity}
                 WHERE Id = {itemId}
                    AND CAST(Quantity AS REAL) >= {quantity}
                 """,
                ct);

            if (affected <= 0)
            {
                bool itemExists = await _dataContext.Items
                    .AnyAsync(i => i.Id == itemId, ct);

                if (!itemExists)
                {
                    return Result<GetItemDto>.Fail("Item not found.")
                        .WithFailureType(FailureType.NotFound);
                }

                return Result<GetItemDto>.Fail("Quantity exceeds stocks on-hand.")
                    .WithFailureType(FailureType.BusinessLogic);
            }
        
            Item? existingItem = await _dataContext.Items
                .AsNoTracking()
                .Include(i => i.Uom)
                .FirstOrDefaultAsync(i => i.Id == itemId, ct);
        
            if (existingItem == null)
            {
                return Result<GetItemDto>.Fail("Item not found.")
                    .WithFailureType(FailureType.NotFound);
            }
        
            return Result<GetItemDto>.Ok(ItemMapper.ToDto(existingItem));
        };
        
        return await _transactionManager.ExecuteAsync(action, cancellationToken);
    }
    
    public async Task<Result<GetItemDto>> DynamicStockUpdateAsync(Guid itemId, string quantity, CancellationToken cancellationToken = default)
    {
        // A powerful method that accepts an operation symbol placed as
        // the first character of the 'quantity' argument to determine
        // which arithmetic operation to perform to update an item's
        // quantity. Dropping the operation symbol entirely will replace
        // the existing quantity with the new one.
        
        Func<CancellationToken, Task<Result<GetItemDto>>> action = async ct =>
        {
            quantity = quantity.Trim();
            decimal value;
            int affected;
            
            // Determine OPERATION and Extract QUANTITY.
            // SHORTCUT: Check whether first character is a digit/numerical (which means no explicit operator).
            if (char.IsDigit(quantity[0]))
            {
                // No explicit operator means we do a SET operation using the value.
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
                    
                affected = await _dataContext.Database.ExecuteSqlInterpolatedAsync(
                    $"""
                     UPDATE Items
                     SET Quantity = {value}
                     WHERE Id = {itemId}
                     """,
                    ct);
            }
            // MAIN LOGIC.
            else switch (quantity[0])
            {
                // Validate and determine operation.
                case '+':
                    // Extract quantity by excluding the first character (operator).
                    if (!decimal.TryParse(quantity.AsSpan(1), out value))
                    {
                        return Result<GetItemDto>.Fail("Invalid quantity.")
                            .WithFailureType(FailureType.Validation);
                    }
                        
                    return await AddStocksAsync(itemId, value, ct);
                
                case '−' or '-':
                    if (!decimal.TryParse(quantity.AsSpan(1), out value))
                    {
                        return Result<GetItemDto>.Fail("Invalid quantity.")
                            .WithFailureType(FailureType.Validation);
                    }

                    return await RemoveStocksAsync(itemId, value, ct);
                
                case '×' or '*' or 'x' or 'X':
                    if (!decimal.TryParse(quantity.AsSpan(1), out value)
                        || value <= 0m)
                    {
                        return Result<GetItemDto>.Fail("Invalid quantity.")
                            .WithFailureType(FailureType.Validation);
                    }
                        
                    affected = await _dataContext.Database.ExecuteSqlInterpolatedAsync(
                        $"""
                         UPDATE Items
                         SET Quantity = Quantity * {value}
                         WHERE Id = {itemId}
                         """,
                        ct);

                    break;
                
                case '÷' or '/':
                    if (!decimal.TryParse(quantity.AsSpan(1), out value)
                        || value <= 0m)
                    {
                        return Result<GetItemDto>.Fail("Invalid quantity.")
                            .WithFailureType(FailureType.Validation);
                    }
                    
                    affected = await _dataContext.Database.ExecuteSqlInterpolatedAsync(
                        $"""
                         UPDATE Items
                         SET Quantity = Quantity / {value}
                         WHERE Id = {itemId}
                         """,
                        ct);

                    break;
                
                case '=':
                    if (!decimal.TryParse(quantity.AsSpan(1), out value))
                    {
                        return Result<GetItemDto>.Fail("Invalid quantity.")
                            .WithFailureType(FailureType.Validation);
                    }
                    
                    if (value < 0m)
                    {
                        return Result<GetItemDto>.Fail("Invalid negative quantity.")
                            .WithFailureType(FailureType.Validation);
                    }
                    
                    affected = await _dataContext.Database.ExecuteSqlInterpolatedAsync(
                        $"""
                         UPDATE Items
                         SET Quantity = {value}
                         WHERE Id = {itemId}
                         """,
                        ct);
                    
                    break;
                
                default:
                    return Result<GetItemDto>.Fail("Invalid operator.")
                        .WithFailureType(FailureType.Validation);
            }
            
            if (affected <= 0)
            {
                return Result<GetItemDto>.Fail("Item not found.")
                    .WithFailureType(FailureType.NotFound);
            }
            
            Item? existingItem = await _dataContext.Items
                .AsNoTracking()
                .Include(i => i.Uom)
                .FirstOrDefaultAsync(i => i.Id == itemId, ct);
            
            if (existingItem == null)
            {
                return Result<GetItemDto>.Fail("Item not found.")
                    .WithFailureType(FailureType.NotFound);
            }
            
            return Result<GetItemDto>.Ok(ItemMapper.ToDto(existingItem));
        };
        
        return await _transactionManager.ExecuteAsync(action, cancellationToken);
    }
    #endregion
}