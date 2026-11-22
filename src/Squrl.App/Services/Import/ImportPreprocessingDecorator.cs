using Squrl.App.Common;
using Squrl.App.Enums;

namespace Squrl.App.Services.Import;

public class ImportPreprocessingDecorator : IImportService
{
    private readonly IImportService _importService;

    public ImportPreprocessingDecorator(IImportService importService)
    {
        _importService = importService;
    }

    public async Task<Result<ImportInfo>> ImportItemsFromCsvAsync(Stream csv, CancellationToken cancellationToken = default)
    {
        if (!csv.CanRead)
        {
            return Result<ImportInfo>.Fail("CSV is not readable.")
                .WithFailureType(FailureType.Validation);
        }
        
        long maxTemplateSize = 5 * 1024L * 1024L; // 5 MB

        if (csv.CanSeek)
        {
            if (csv.Length > maxTemplateSize)
            {
                return Result<ImportInfo>.Fail("CSV is too large.")
                    .WithFailureType(FailureType.Validation);
            }

            csv.Position = 0;
        }

        return await _importService
            .ImportItemsFromCsvAsync(csv, cancellationToken);
    }

    public async Task<Result<ImportInfo>> ImportUomsFromCsvAsync(Stream csv, CancellationToken cancellationToken = default)
    {
        if (!csv.CanRead)
        {
            return Result<ImportInfo>.Fail("CSV is not readable.")
                .WithFailureType(FailureType.Validation);
        }
        
        long maxTemplateSize = 5 * 1024L * 1024L; // 5 MB

        if (csv.CanSeek)
        {
            if (csv.Length > maxTemplateSize)
            {
                return Result<ImportInfo>.Fail("CSV is too large.")
                    .WithFailureType(FailureType.Validation);
            }

            csv.Position = 0;
        }

        return await _importService
            .ImportUomsFromCsvAsync(csv, cancellationToken);
    }

    public async Task<Result<ImportInfo>> ImportSuppliersFromCsvAsync(Stream csv, CancellationToken cancellationToken = default)
    {
        if (!csv.CanRead)
        {
            return Result<ImportInfo>.Fail("CSV is not readable.")
                .WithFailureType(FailureType.Validation);
        }
        
        long maxTemplateSize = 5 * 1024L * 1024L; // 5 MB

        if (csv.CanSeek)
        {
            if (csv.Length > maxTemplateSize)
            {
                return Result<ImportInfo>.Fail("CSV is too large.")
                    .WithFailureType(FailureType.Validation);
            }

            csv.Position = 0;
        }

        return await _importService
            .ImportSuppliersFromCsvAsync(csv, cancellationToken);
    }
}