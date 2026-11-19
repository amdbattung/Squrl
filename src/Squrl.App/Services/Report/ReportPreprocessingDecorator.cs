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
    
    public async Task<Result<bool>> GenerateCustomerItemSalesAsWordAsync(
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

        return await _reportService
            .GenerateCustomerItemSalesAsWordAsync(customer, destination, template, cancellationToken);
    }

    public async Task<Result<bool>> GenerateCustomerItemSalesAsPdfAsync(
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

        return await _reportService
            .GenerateCustomerItemSalesAsPdfAsync(customer, destination, template, cancellationToken);
    }

    public async Task<Result<bool>> GenerateSupplierItemPurchasesAsWordAsync(
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

        return await _reportService
            .GenerateSupplierItemPurchasesAsWordAsync(supplierId, destination, template, cancellationToken);
    }

    public async Task<Result<bool>> GenerateSupplierItemPurchasesAsPdfAsync(
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

        return await _reportService
            .GenerateSupplierItemPurchasesAsPdfAsync(supplierId, destination, template, cancellationToken);
    }

    public async Task<Result<bool>> GenerateItemSalesHistoryAsWordAsync(
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

        return await _reportService
            .GenerateItemSalesHistoryAsWordAsync(itemId, destination, template, cancellationToken);
    }

    public async Task<Result<bool>> GenerateItemSalesHistoryAsPdfAsync(
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

        return await _reportService
            .GenerateItemSalesHistoryAsPdfAsync(itemId, destination, template, cancellationToken);
    }

    public async Task<Result<bool>> GenerateItemPurchaseHistoryAsWordAsync(
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

        return await _reportService
            .GenerateItemPurchaseHistoryAsWordAsync(itemId, destination, template, cancellationToken);
    }

    public async Task<Result<bool>> GenerateItemPurchaseHistoryAsPdfAsync(
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

        return await _reportService
            .GenerateItemPurchaseHistoryAsPdfAsync(itemId, destination, template, cancellationToken);
    }
}