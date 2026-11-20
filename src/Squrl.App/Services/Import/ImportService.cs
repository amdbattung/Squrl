using FluentValidation;
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
    
    public async Task<Result<bool>> ImportSuppliersAsync(
        Stream csvStream,
        CancellationToken cancellationToken = default)
    {
        using var reader = new StreamReader(csvStream, leaveOpen: true);
        using var csv = Sep.Reader().From(reader);

        var result = new ImportResult();
        var suppliers = new List<(int RowNumber, Models.Supplier Supplier)>();

        var rowNumber = 1; // Header

        foreach (var row in csv)
        {
            cancellationToken.ThrowIfCancellationRequested();
            rowNumber++;
            result.TotalRows++;

            try
            {
                // Map CSV row to DTO
                var dto = new CreateSupplierDto
                {
                    Name = row["Name"].ToString().Trim(),
                    Description = row["Description"].ToString().Trim()
                };

                // Validate using FluentValidation
                var validation = await _createSupplierValidator.ValidateAsync(
                    dto, cancellationToken);

                if (!validation.IsValid)
                {
                    result.FailedRows++;

                    result.Errors.Add(new ImportError
                    {
                        RowNumber = rowNumber,
                        Message = string.Join(
                            "; ",
                            validation.Errors.Select(x => x.ErrorMessage))
                    });

                    continue;
                }

                // Map validated DTO to entity
                var supplier = new Models.Supplier
                {
                    Id = Guid.NewGuid(),
                    Name = dto.Name,
                    Description = dto.Description
                };

                suppliers.Add((rowNumber, supplier));
            }
            catch (Exception ex) when (
                ex is FormatException or OverflowException)
            {
                result.FailedRows++;

                result.Errors.Add(new ImportError
                {
                    RowNumber = rowNumber,
                    Message = ex.Message
                });
            }
        }

        if (suppliers.Count > 0)
        {
            await SaveSuppliersAsync(suppliers, result, cancellationToken);
        }

        return Result<bool>.Ok(true);
    }

    private async Task SaveSuppliersAsync(
        List<(int RowNumber, Models.Supplier Supplier)> suppliers,
        ImportResult result,
        CancellationToken cancellationToken)
    {
        try
        {
            _dataContext.Suppliers.AddRange(
                suppliers.Select(x => x.Supplier));

            await _dataContext.SaveChangesAsync(cancellationToken);

            result.SuccessfulRows += suppliers.Count;
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

                    result.SuccessfulRows++;
                }
                catch (DbUpdateException ex)
                {
                    _dataContext.Entry(item.Supplier).State =
                        EntityState.Detached;

                    result.FailedRows++;

                    result.Errors.Add(new ImportError
                    {
                        RowNumber = item.RowNumber,
                        Message = ex.InnerException?.Message ?? ex.Message
                    });
                }
            }
        }
    }
    
    private class ImportResult
    {
        public int TotalRows { get; set; }
        public int SuccessfulRows { get; set; }
        public int FailedRows { get; set; }

        public List<ImportError> Errors { get; set; } = [];
    }

    private class ImportError
    {
        public int RowNumber { get; set; }
        public string Message { get; set; } = string.Empty;
    }
}