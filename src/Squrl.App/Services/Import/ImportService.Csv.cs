using FluentValidation.Results;
using Microsoft.EntityFrameworkCore;
using nietras.SeparatedValues;
using Squrl.App.Common;
using Squrl.App.Features.Suppliers.DTOs;
using Squrl.App.Features.UnitOfMeasures.DTOs;

namespace Squrl.App.Services.Import;

public partial class ImportService
{
    public Task<Result<ImportInfo>> ImportItemsFromCsvAsync(Stream csv, CancellationToken cancellationToken = default)
    {
        throw new NotImplementedException();
    }

    public async Task<Result<ImportInfo>> ImportUomsFromCsvAsync(Stream csvStream, CancellationToken cancellationToken = default)
    {
        using StreamReader reader = new(csvStream, leaveOpen: true);
        
        using SepReader csv = Sep.New(',')
            .Reader(options => options with {
                HasHeader = false
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
                HasHeader = false
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