using Squrl.App.Common;
using Squrl.App.Enums;

namespace Squrl.App.Services.Report;

public class ReportErrorHandlingDecorator : IReportService
{
    private readonly IReportService _reportService;

    public ReportErrorHandlingDecorator(IReportService reportService)
    {
        _reportService = reportService;
    }

    public async Task<Result<bool>> GenerateCustomerItemSalesAsWord(
        string customer,
        Stream destination,
        Stream? template = null,
        CancellationToken cancellationToken = default)
    {
        try
        {
            Result<bool> result = await _reportService
                .GenerateCustomerItemSalesAsWord(
                    customer,
                    destination,
                    template,
                    cancellationToken);
            
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
            return Result<bool>
                .Fail("Failed to generate report, something went wrong.")
                .WithFailureType(FailureType.Unexpected);
        }
    }

    public async Task<Result<bool>> GenerateCustomerItemSalesAsPdf(
        string customer,
        Stream destination,
        Stream? template = null,
        CancellationToken cancellationToken = default)
    {
        try
        {
            Result<bool> result = await _reportService
                .GenerateCustomerItemSalesAsPdf(
                    customer,
                    destination,
                    template,
                    cancellationToken);
            
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
            return Result<bool>
                .Fail("Failed to generate report, something went wrong.")
                .WithFailureType(FailureType.Unexpected);
        }
    }

    public async Task<Result<bool>> GenerateSupplierItemPurchasesAsWord(
        Guid supplierId,
        Stream destination,
        Stream? template = null,
        CancellationToken cancellationToken = default)
    {
        try
        {
            Result<bool> result = await _reportService
                .GenerateSupplierItemPurchasesAsWord(
                    supplierId,
                    destination,
                    template,
                    cancellationToken);
            
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
            return Result<bool>
                .Fail("Failed to generate report, something went wrong.")
                .WithFailureType(FailureType.Unexpected);
        }
    }

    public async Task<Result<bool>> GenerateSupplierItemPurchasesAsPdf(
        Guid supplierId,
        Stream destination,
        Stream? template = null,
        CancellationToken cancellationToken = default)
    {
        try
        {
            Result<bool> result = await _reportService
                .GenerateSupplierItemPurchasesAsPdf(
                    supplierId,
                    destination,
                    template,
                    cancellationToken);
            
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
            return Result<bool>
                .Fail("Failed to generate report, something went wrong.")
                .WithFailureType(FailureType.Unexpected);
        }
    }

    public async Task<Result<bool>> GenerateItemSalesHistoryAsWord(
        Guid itemId,
        Stream destination,
        Stream? template = null,
        CancellationToken cancellationToken = default)
    {
        try
        {
            Result<bool> result = await _reportService
                .GenerateItemSalesHistoryAsWord(
                    itemId,
                    destination,
                    template,
                    cancellationToken);
            
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
            return Result<bool>
                .Fail("Failed to generate report, something went wrong.")
                .WithFailureType(FailureType.Unexpected);
        }
    }

    public async Task<Result<bool>> GenerateItemSalesHistoryAsPdf(
        Guid itemId,
        Stream destination,
        Stream? template = null,
        CancellationToken cancellationToken = default)
    {
        try
        {
            Result<bool> result = await _reportService
                .GenerateItemSalesHistoryAsPdf(
                    itemId,
                    destination,
                    template,
                    cancellationToken);
            
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
            return Result<bool>
                .Fail("Failed to generate report, something went wrong.")
                .WithFailureType(FailureType.Unexpected);
        }
    }
}