using Squrl.App.Common;
using Squrl.App.Enums;

namespace Squrl.App.Services.Import;

public sealed class ImportErrorHandlingDecorator : IImportService
{
    private readonly IImportService _importService;

    public ImportErrorHandlingDecorator(IImportService importService)
    {
        _importService = importService;
    }

    public async Task<Result<ImportInfo>> ImportItemsFromCsvAsync(Stream csv, CancellationToken cancellationToken = default)
    {
        try
        {
            Result<ImportInfo> result = await _importService
                .ImportItemsFromCsvAsync(csv, cancellationToken);
            
            return result.Exception is null
                ? result
                : result.WithoutException();
        }
        catch (OperationCanceledException)
        {
            throw;
        }
        catch (Exception)
        {
            return Result<ImportInfo>
                .Fail("Failed to import items, something went wrong.")
                .WithFailureType(FailureType.Unexpected);
        }
    }

    public async Task<Result<ImportInfo>> ImportUomsFromCsvAsync(Stream csv, CancellationToken cancellationToken = default)
    {
        try
        {
            Result<ImportInfo> result = await _importService
                .ImportUomsFromCsvAsync(csv, cancellationToken);
            
            return result.Exception is null
                ? result
                : result.WithoutException();
        }
        catch (OperationCanceledException)
        {
            throw;
        }
        catch (Exception)
        {
            return Result<ImportInfo>
                .Fail("Failed to import UOMs, something went wrong.")
                .WithFailureType(FailureType.Unexpected);
        }
    }

    public async Task<Result<ImportInfo>> ImportSuppliersFromCsvAsync(Stream csv, CancellationToken cancellationToken = default)
    {
        try
        {
            Result<ImportInfo> result = await _importService
                .ImportSuppliersFromCsvAsync(csv, cancellationToken);
            
            return result.Exception is null
                ? result
                : result.WithoutException();
        }
        catch (OperationCanceledException)
        {
            throw;
        }
        catch (Exception)
        {
            return Result<ImportInfo>
                .Fail("Failed to import suppliers, something went wrong.")
                .WithFailureType(FailureType.Unexpected);
        }
    }
}