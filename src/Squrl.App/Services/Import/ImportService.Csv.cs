using DocumentFormat.OpenXml.Wordprocessing;
using FluentValidation.Results;
using Microsoft.EntityFrameworkCore;
using nietras.SeparatedValues;
using Squrl.App.Common;
using Squrl.App.Features.Items.DTOs;
using Squrl.App.Features.Suppliers.DTOs;
using Squrl.App.Features.UnitOfMeasures.DTOs;
using Item = Squrl.App.Models.Item;

namespace Squrl.App.Services.Import;

public partial class ImportService
{
    public async Task<Result<ImportInfo>> ImportItemsFromCsvAsync(Stream csvStream, CancellationToken cancellationToken = default)
    {
        using StreamReader reader = new(csvStream, leaveOpen: true);
        
        using SepReader csv = Sep.New(',')
            .Reader(options => options with {
                HasHeader = false,
                Unescape = true,
                Trim = SepTrim.All
            }).From(reader);
        
        int totalRows = 0;
        int successfulRows = 0;
        int failedRows = 0;
        List<ImportError> importErrors = new();
        
        List<(int RowNumber, Item Item)> items = new();

        var rowNumber = 0;
        
        Dictionary<string, Models.UnitOfMeasure> uomCache = new(StringComparer.OrdinalIgnoreCase);

        foreach (SepReader.Row row in csv)
        {
            cancellationToken.ThrowIfCancellationRequested();
            rowNumber++;
            totalRows++;

            try
            {
                string name = row[0].ToString().Trim();
                string description = row[1].ToString().Trim();
                decimal quantity = decimal.Parse(row[2].ToString());
                string uomCode = row[3].ToString().Trim();

                decimal? listPrice = decimal.TryParse(row[4].ToString(), out decimal lp)
                    ? lp
                    : null;

                decimal? retailPrice = decimal.TryParse(row[5].ToString(), out decimal rp)
                    ? rp
                    : null;

                decimal? costPrice = decimal.TryParse(row[6].ToString(), out decimal cp)
                    ? cp
                    : null;
                
                decimal? lowQuantityAlertThreshold =
                    decimal.TryParse(row[7].ToString(), out decimal threshold)
                        ? threshold
                        : null;

                List<string> locations = row[8].ToString()
                    .Split(',', StringSplitOptions.TrimEntries
                                | StringSplitOptions.RemoveEmptyEntries)
                    .ToList();
                
                if (!uomCache.TryGetValue(uomCode, out Models.UnitOfMeasure? uom))
                {
                    uom = await _dataContext.UnitOfMeasures
                              .FirstOrDefaultAsync(u => u.Code == uomCode, cancellationToken)
                          ?? throw new InvalidOperationException($"UOM '{uomCode}' not found.");

                    uomCache[uomCode] = uom;
                }

                // Map CSV row to DTO
                CreateItemDto dto = new()
                {
                    Name = name,
                    UomId = uom.Id,
                    Description = description.Length > 0 ? description : null,
                    ListPrice = listPrice,
                    RetailPrice = retailPrice,
                    CostPrice = costPrice,
                    Quantity = quantity,
                    LowQuantityAlertThreshold = lowQuantityAlertThreshold,
                    Locations = locations
                };

                ValidationResult? validationResult = await _createItemValidator
                    .ValidateAsync(dto, cancellationToken);

                if (!validationResult.IsValid)
                {
                    failedRows++;

                    importErrors.Add(new()
                    {
                        RowNumber = rowNumber,
                        Message = string.Join(
                            "; ",
                            validationResult.Errors.Select(x => x.ErrorMessage))
                    });

                    continue;
                }

                // Map validated DTO to entity
                Item item = new()
                {
                    Id = Guid.NewGuid(),
                    Name = dto.Name,
                    Uom = uom,
                    Description = dto.Description,
                    ListPrice = dto.ListPrice,
                    RetailPrice = dto.RetailPrice,
                    CostPrice = dto.CostPrice,
                    Quantity = dto.Quantity ?? 0m,
                    LowQuantityAlertThreshold =  dto.LowQuantityAlertThreshold,
                    Locations = dto.Locations
                };

                items.Add((rowNumber, item));
            }
            catch (Exception e) when (e is FormatException
                                          or OverflowException
                                          or InvalidOperationException)
            {
                failedRows++;

                importErrors.Add(new()
                {
                    RowNumber = rowNumber,
                    Message = e.Message
                });
            }
        }

        try
        {
            _dataContext.Items.AddRange(
                items.Select(x => x.Item));

            await _dataContext.SaveChangesAsync(cancellationToken);

            successfulRows += items.Count;
        }
        catch (DbUpdateException)
        {
            // Detach entities from the failed batch.
            foreach (var item in items)
            {
                _dataContext.Entry(item.Item).State =
                    EntityState.Detached;
            }

            // Retry individually so valid records are still committed.
            foreach (var item in items)
            {
                cancellationToken.ThrowIfCancellationRequested();

                try
                {
                    _dataContext.Items.Add(item.Item);

                    await _dataContext.SaveChangesAsync(cancellationToken);

                    successfulRows++;
                }
                catch (DbUpdateException e) when (IsUniqueConstraintViolation(e))
                {
                    _dataContext.Entry(item.Item).State = EntityState.Detached;

                    failedRows++;

                    importErrors.Add(new()
                    {
                        RowNumber = item.RowNumber,
                        Message = "An item with the same name already exists."
                    });
                }
                catch (DbUpdateException)
                {
                    _dataContext.Entry(item.Item).State = EntityState.Detached;

                    failedRows++;

                    importErrors.Add(new()
                    {
                        RowNumber = item.RowNumber,
                        Message = "Failed to save item."
                    });
                }
            }
        }

        ImportInfo importInfo = new()
        {
            TotalRows = totalRows,
            SuccessfulRows = successfulRows,
            FailedRows = failedRows,
            Errors = importErrors
        };

        return Result<ImportInfo>.Ok(importInfo);
    }

    public async Task<Result<ImportInfo>> ImportUomsFromCsvAsync(Stream csvStream, CancellationToken cancellationToken = default)
    {
        using StreamReader reader = new(csvStream, leaveOpen: true);
        
        using SepReader csv = Sep.New(',')
            .Reader(options => options with {
                HasHeader = false,
                Unescape = true,
                Trim = SepTrim.All
            }).From(reader);
        
        int totalRows = 0;
        int successfulRows = 0;
        int failedRows = 0;
        List<ImportError> importErrors = new();
        
        List<(int RowNumber, Models.UnitOfMeasure Uom)> uoms = new();

        var rowNumber = 0;

        foreach (SepReader.Row row in csv)
        {
            cancellationToken.ThrowIfCancellationRequested();
            rowNumber++;
            totalRows++;

            try
            {
                // Map CSV row to DTO
                CreateUomDto dto = new()
                {
                    Name = row[0].ToString().Trim(),
                    Code = row[1].ToString().Trim(),
                    Description = row[2].ToString().Trim()
                        is { Length: > 0 } description
                        ? description
                        : null
                };
                
                ValidationResult? validationResult = await _createUomValidator
                    .ValidateAsync(dto, cancellationToken);

                if (!validationResult.IsValid)
                {
                    failedRows++;

                    importErrors.Add(new()
                    {
                        RowNumber = rowNumber,
                        Message = string.Join(
                            "; ",
                            validationResult.Errors.Select(x => x.ErrorMessage))
                    });

                    continue;
                }

                // Map validated DTO to entity
                Models.UnitOfMeasure uom = new()
                {
                    Id = Guid.NewGuid(),
                    Name = dto.Name,
                    Code = dto.Code,
                    Description = dto.Description
                };

                uoms.Add((rowNumber, uom));
            }
            catch (Exception e) when (e is FormatException or OverflowException)
            {
                failedRows++;

                importErrors.Add(new()
                {
                    RowNumber = rowNumber,
                    Message = e.Message
                });
            }
        }

        try
        {
            _dataContext.UnitOfMeasures.AddRange(
                uoms.Select(x => x.Uom));

            await _dataContext.SaveChangesAsync(cancellationToken);

            successfulRows += uoms.Count;
        }
        catch (DbUpdateException)
        {
            // Detach entities from the failed batch.
            foreach (var item in uoms)
            {
                _dataContext.Entry(item.Uom).State =
                    EntityState.Detached;
            }

            // Retry individually so valid records are still committed.
            foreach (var item in uoms)
            {
                cancellationToken.ThrowIfCancellationRequested();

                try
                {
                    _dataContext.UnitOfMeasures.Add(item.Uom);

                    await _dataContext.SaveChangesAsync(cancellationToken);

                    successfulRows++;
                }
                catch (DbUpdateException e) when (IsUniqueConstraintViolation(e))
                {
                    _dataContext.Entry(item.Uom).State = EntityState.Detached;

                    failedRows++;

                    importErrors.Add(new()
                    {
                        RowNumber = item.RowNumber,
                        Message = "A UOM with the same name and/or code already exists."
                    });
                }
                catch (DbUpdateException)
                {
                    _dataContext.Entry(item.Uom).State = EntityState.Detached;

                    failedRows++;

                    importErrors.Add(new()
                    {
                        RowNumber = item.RowNumber,
                        Message = "Failed to save UOM."
                    });
                }
            }
        }

        ImportInfo importInfo = new()
        {
            TotalRows = totalRows,
            SuccessfulRows = successfulRows,
            FailedRows = failedRows,
            Errors = importErrors
        };

        return Result<ImportInfo>.Ok(importInfo);
    }

    public async Task<Result<ImportInfo>> ImportSuppliersFromCsvAsync(
        Stream csvStream,
        CancellationToken cancellationToken = default)
    {
        using StreamReader reader = new(csvStream, leaveOpen: true);
        
        using SepReader csv = Sep.New(',')
            .Reader(options => options with {
                HasHeader = false,
                Unescape = true,
                Trim = SepTrim.All
            }).From(reader);
        
        int totalRows = 0;
        int successfulRows = 0;
        int failedRows = 0;
        List<ImportError> importErrors = new();
        
        List<(int RowNumber, Models.Supplier Supplier)> suppliers = new();

        var rowNumber = 0;

        foreach (SepReader.Row row in csv)
        {
            cancellationToken.ThrowIfCancellationRequested();
            rowNumber++;
            totalRows++;

            try
            {
                // Map CSV row to DTO
                CreateSupplierDto dto = new()
                {
                    Name = row[0].ToString().Trim(),
                    Description = row[1].ToString().Trim()
                        is { Length: > 0 } description
                        ? description
                        : null
                };
                
                ValidationResult? validationResult = await _createSupplierValidator
                    .ValidateAsync(dto, cancellationToken);

                if (!validationResult.IsValid)
                {
                    failedRows++;

                    importErrors.Add(new()
                    {
                        RowNumber = rowNumber,
                        Message = string.Join(
                            "; ",
                            validationResult.Errors.Select(x => x.ErrorMessage))
                    });

                    continue;
                }

                // Map validated DTO to entity
                Models.Supplier supplier = new()
                {
                    Id = Guid.NewGuid(),
                    Name = dto.Name,
                    Description = dto.Description
                };

                suppliers.Add((rowNumber, supplier));
            }
            catch (Exception e) when (e is FormatException or OverflowException)
            {
                failedRows++;

                importErrors.Add(new()
                {
                    RowNumber = rowNumber,
                    Message = e.Message
                });
            }
        }

        try
        {
            _dataContext.Suppliers.AddRange(
                suppliers.Select(x => x.Supplier));

            await _dataContext.SaveChangesAsync(cancellationToken);

            successfulRows += suppliers.Count;
        }
        catch (DbUpdateException)
        {
            // Detach entities from the failed batch.
            foreach (var item in suppliers)
            {
                _dataContext.Entry(item.Supplier).State =
                    EntityState.Detached;
            }

            // Retry individually so valid records are still committed.
            foreach (var item in suppliers)
            {
                cancellationToken.ThrowIfCancellationRequested();

                try
                {
                    _dataContext.Suppliers.Add(item.Supplier);

                    await _dataContext.SaveChangesAsync(cancellationToken);

                    successfulRows++;
                }
                catch (DbUpdateException e) when (IsUniqueConstraintViolation(e))
                {
                    _dataContext.Entry(item.Supplier).State = EntityState.Detached;

                    failedRows++;

                    importErrors.Add(new()
                    {
                        RowNumber = item.RowNumber,
                        Message = "A supplier with the same name already exists."
                    });
                }
                catch (DbUpdateException)
                {
                    _dataContext.Entry(item.Supplier).State = EntityState.Detached;

                    failedRows++;

                    importErrors.Add(new()
                    {
                        RowNumber = item.RowNumber,
                        Message = "Failed to save supplier."
                    });
                }
            }
        }

        ImportInfo importInfo = new()
        {
            TotalRows = totalRows,
            SuccessfulRows = successfulRows,
            FailedRows = failedRows,
            Errors = importErrors
        };

        return Result<ImportInfo>.Ok(importInfo);
    }
}