using FluentValidation;
using FluentValidation.Results;
using Microsoft.EntityFrameworkCore;
using nietras.SeparatedValues;
using Squrl.App.Common;
using Squrl.App.Data;
using Squrl.App.Features.Suppliers.DTOs;

namespace Squrl.App.Services.Import;

public class ImportService : IImportService
{
    private readonly DataContext _dataContext;
    private readonly IValidator<CreateSupplierDto> _createSupplierValidator;

    public ImportService(DataContext dataContext,
        IValidator<CreateSupplierDto> createSupplierValidator)
    {
        _dataContext = dataContext;
        _createSupplierValidator = createSupplierValidator;
    }
    
    public async Task<Result<ImportInfo>> ImportSuppliersAsync(
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
                
                ValidationResult? validation = await _createSupplierValidator
                    .ValidateAsync(dto, cancellationToken);

                if (!validation.IsValid)
                {
                    failedRows++;

                    importErrors.Add(new ImportError
                    {
                        RowNumber = rowNumber,
                        Message = string.Join(
                            "; ",
                            validation.Errors.Select(x => x.ErrorMessage))
                    });

                    continue;
                }

                // Map validated DTO to entity
                Models.Supplier supplier = new Models.Supplier
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

                importErrors.Add(new ImportError
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
                catch (DbUpdateException)
                {
                    _dataContext.Entry(item.Supplier).State = EntityState.Detached;

                    failedRows++;

                    importErrors.Add(new ImportError
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