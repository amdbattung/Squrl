using Squrl.App.Common;
using Squrl.App.Enums;

namespace Squrl.App.Services.Report;

public class ReportPreprocessingDecorator : IReportService
{
    private readonly IReportService _reportService;
    private readonly long _maxTemplateSize;

    public ReportPreprocessingDecorator(IReportService reportService)
    {
        _reportService = reportService;
        _maxTemplateSize = 10 * 1024L * 1024L; // 10 MB
    }
    
    public async Task<Result<bool>> GenerateCustomerItemSalesAsWord(
        string customer,
        Stream destination,
        Stream? template = null,
        CancellationToken cancellationToken = default)
    {
        customer = customer.Trim();
        
        if (string.IsNullOrWhiteSpace(customer))
        {
            return Result<bool>.Fail("Null or invalid customer.")
                .WithFailureType(FailureType.Validation);
        }
        
        if (!destination.CanWrite)
        {
            return Result<bool>.Fail("Destination must be writable.")
                .WithFailureType(FailureType.Validation);
        }

        if (template is not null)
        {
            if (!template.CanRead)
            {
                return Result<bool>.Fail("Template is not readable.")
                    .WithFailureType(FailureType.Validation);
            }

            if (template.CanSeek)
            {
                if (template.Length > _maxTemplateSize)
                {
                    return Result<bool>.Fail("Template is too large.")
                        .WithFailureType(FailureType.Validation);
                }

                template.Position = 0;
            }
        }

        Result<bool> result = await _reportService
            .GenerateCustomerItemSalesAsWord(customer, destination, template, cancellationToken);
        
        return result;
    }

    public async Task<Result<bool>> GenerateCustomerItemSalesAsPdf(
        string customer,
        Stream destination,
        Stream? template = null,
        CancellationToken cancellationToken = default)
    {
        customer = customer.Trim();
        
        if (string.IsNullOrWhiteSpace(customer))
        {
            return Result<bool>.Fail("Null or invalid customer.")
                .WithFailureType(FailureType.Validation);
        }
        
        if (!destination.CanWrite)
        {
            return Result<bool>.Fail("Destination must be writable.")
                .WithFailureType(FailureType.Validation);
        }

        if (template is not null)
        {
            if (!template.CanRead)
            {
                return Result<bool>.Fail("Template is not readable.")
                    .WithFailureType(FailureType.Validation);
            }

            if (template.CanSeek)
            {
                if (template.Length > _maxTemplateSize)
                {
                    return Result<bool>.Fail("Template is too large.")
                        .WithFailureType(FailureType.Validation);
                }

                template.Position = 0;
            }
        }

        Result<bool> result = await _reportService
            .GenerateCustomerItemSalesAsPdf(customer, destination, template, cancellationToken);
        
        return result;
    }

    public async Task<Result<bool>> GenerateSupplierItemPurchasesAsWord(
        Guid supplierId,
        Stream destination,
        Stream? template = null,
        CancellationToken cancellationToken = default)
    {
        if (supplierId == Guid.Empty)
        {
            return Result<bool>.Fail("Null or invalid ID.")
                .WithFailureType(FailureType.Validation);
        }
        
        if (!destination.CanWrite)
        {
            return Result<bool>.Fail("Destination must be writable.")
                .WithFailureType(FailureType.Validation);
        }

        if (template is not null)
        {
            if (!template.CanRead)
            {
                return Result<bool>.Fail("Template is not readable.")
                    .WithFailureType(FailureType.Validation);
            }

            if (template.CanSeek)
            {
                if (template.Length > _maxTemplateSize)
                {
                    return Result<bool>.Fail("Template is too large.")
                        .WithFailureType(FailureType.Validation);
                }

                template.Position = 0;
            }
        }

        Result<bool> result = await _reportService
            .GenerateSupplierItemPurchasesAsWord(supplierId, destination, template, cancellationToken);
        
        return result;
    }

    public async Task<Result<bool>> GenerateSupplierItemPurchasesAsPdf(
        Guid supplierId,
        Stream destination,
        Stream? template = null,
        CancellationToken cancellationToken = default)
    {
        if (supplierId == Guid.Empty)
        {
            return Result<bool>.Fail("Null or invalid ID.")
                .WithFailureType(FailureType.Validation);
        }
        
        if (!destination.CanWrite)
        {
            return Result<bool>.Fail("Destination must be writable.")
                .WithFailureType(FailureType.Validation);
        }

        if (template is not null)
        {
            if (!template.CanRead)
            {
                return Result<bool>.Fail("Template is not readable.")
                    .WithFailureType(FailureType.Validation);
            }

            if (template.CanSeek)
            {
                if (template.Length > _maxTemplateSize)
                {
                    return Result<bool>.Fail("Template is too large.")
                        .WithFailureType(FailureType.Validation);
                }

                template.Position = 0;
            }
        }

        Result<bool> result = await _reportService
            .GenerateSupplierItemPurchasesAsPdf(supplierId, destination, template, cancellationToken);
        
        return result;
    }

    public async Task<Result<bool>> GenerateItemSalesHistoryAsWord(
        Guid itemId,
        Stream destination,
        Stream? template = null,
        CancellationToken cancellationToken = default)
    {
        if (itemId == Guid.Empty)
        {
            return Result<bool>.Fail("Null or invalid ID.")
                .WithFailureType(FailureType.Validation);
        }
        
        if (!destination.CanWrite)
        {
            return Result<bool>.Fail("Destination must be writable.")
                .WithFailureType(FailureType.Validation);
        }

        if (template is not null)
        {
            if (!template.CanRead)
            {
                return Result<bool>.Fail("Template is not readable.")
                    .WithFailureType(FailureType.Validation);
            }

            if (template.CanSeek)
            {
                if (template.Length > _maxTemplateSize)
                {
                    return Result<bool>.Fail("Template is too large.")
                        .WithFailureType(FailureType.Validation);
                }

                template.Position = 0;
            }
        }

        Result<bool> result = await _reportService
            .GenerateItemSalesHistoryAsWord(itemId, destination, template, cancellationToken);
        
        return result;
    }

    public async Task<Result<bool>> GenerateItemSalesHistoryAsPdf(
        Guid itemId,
        Stream destination,
        Stream? template = null,
        CancellationToken cancellationToken = default)
    {
        if (itemId == Guid.Empty)
        {
            return Result<bool>.Fail("Null or invalid ID.")
                .WithFailureType(FailureType.Validation);
        }
        
        if (!destination.CanWrite)
        {
            return Result<bool>.Fail("Destination must be writable.")
                .WithFailureType(FailureType.Validation);
        }

        if (template is not null)
        {
            if (!template.CanRead)
            {
                return Result<bool>.Fail("Template is not readable.")
                    .WithFailureType(FailureType.Validation);
            }

            if (template.CanSeek)
            {
                if (template.Length > _maxTemplateSize)
                {
                    return Result<bool>.Fail("Template is too large.")
                        .WithFailureType(FailureType.Validation);
                }

                template.Position = 0;
            }
        }

        Result<bool> result = await _reportService
            .GenerateItemSalesHistoryAsPdf(itemId, destination, template, cancellationToken);
        
        return result;
    }

    public async Task<Result<bool>> GenerateItemPurchaseHistoryAsWord(
        Guid itemId,
        Stream destination,
        Stream? template = null,
        CancellationToken cancellationToken = default)
    {
        if (itemId == Guid.Empty)
        {
            return Result<bool>.Fail("Null or invalid ID.")
                .WithFailureType(FailureType.Validation);
        }
        
        if (!destination.CanWrite)
        {
            return Result<bool>.Fail("Destination must be writable.")
                .WithFailureType(FailureType.Validation);
        }

        if (template is not null)
        {
            if (!template.CanRead)
            {
                return Result<bool>.Fail("Template is not readable.")
                    .WithFailureType(FailureType.Validation);
            }

            if (template.CanSeek)
            {
                if (template.Length > _maxTemplateSize)
                {
                    return Result<bool>.Fail("Template is too large.")
                        .WithFailureType(FailureType.Validation);
                }

                template.Position = 0;
            }
        }

        Result<bool> result = await _reportService
            .GenerateItemPurchaseHistoryAsWord(itemId, destination, template, cancellationToken);
        
        return result;
    }

    public async Task<Result<bool>> GenerateItemPurchaseHistoryAsPdf(
        Guid itemId,
        Stream destination,
        Stream? template = null,
        CancellationToken cancellationToken = default)
    {
        if (itemId == Guid.Empty)
        {
            return Result<bool>.Fail("Null or invalid ID.")
                .WithFailureType(FailureType.Validation);
        }
        
        if (!destination.CanWrite)
        {
            return Result<bool>.Fail("Destination must be writable.")
                .WithFailureType(FailureType.Validation);
        }

        if (template is not null)
        {
            if (!template.CanRead)
            {
                return Result<bool>.Fail("Template is not readable.")
                    .WithFailureType(FailureType.Validation);
            }

            if (template.CanSeek)
            {
                if (template.Length > _maxTemplateSize)
                {
                    return Result<bool>.Fail("Template is too large.")
                        .WithFailureType(FailureType.Validation);
                }

                template.Position = 0;
            }
        }

        Result<bool> result = await _reportService
            .GenerateItemPurchaseHistoryAsPdf(itemId, destination, template, cancellationToken);
        
        return result;
    }
}