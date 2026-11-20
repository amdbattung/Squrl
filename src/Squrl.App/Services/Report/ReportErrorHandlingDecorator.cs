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

    public async Task<Result> GenerateCustomerItemSalesAsWordAsync(
        string customer,
        Stream destination,
        Stream? template = null,
        CancellationToken cancellationToken = default)
    {
        try
        {
            Result result = await _reportService
                .GenerateCustomerItemSalesAsWordAsync(
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
            return Result
                .Fail("Failed to generate report, something went wrong.")
                .WithFailureType(FailureType.Unexpected);
        }
    }

    public async Task<Result> GenerateCustomerItemSalesAsPdfAsync(
        string customer,
        Stream destination,
        Stream? template = null,
        CancellationToken cancellationToken = default)
    {
        try
        {
            Result result = await _reportService
                .GenerateCustomerItemSalesAsPdfAsync(
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
            return Result
                .Fail("Failed to generate report, something went wrong.")
                .WithFailureType(FailureType.Unexpected);
        }
    }

    public async Task<Result> GenerateSupplierItemPurchasesAsWordAsync(
        Guid supplierId,
        Stream destination,
        Stream? template = null,
        CancellationToken cancellationToken = default)
    {
        try
        {
            Result result = await _reportService
                .GenerateSupplierItemPurchasesAsWordAsync(
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
            return Result
                .Fail("Failed to generate report, something went wrong.")
                .WithFailureType(FailureType.Unexpected);
        }
    }

    public async Task<Result> GenerateSupplierItemPurchasesAsPdfAsync(
        Guid supplierId,
        Stream destination,
        Stream? template = null,
        CancellationToken cancellationToken = default)
    {
        try
        {
            Result result = await _reportService
                .GenerateSupplierItemPurchasesAsPdfAsync(
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
            return Result
                .Fail("Failed to generate report, something went wrong.")
                .WithFailureType(FailureType.Unexpected);
        }
    }

    public async Task<Result> GenerateItemSalesHistoryAsWordAsync(
        Guid itemId,
        Stream destination,
        Stream? template = null,
        CancellationToken cancellationToken = default)
    {
        try
        {
            Result result = await _reportService
                .GenerateItemSalesHistoryAsWordAsync(
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
            return Result
                .Fail("Failed to generate report, something went wrong.")
                .WithFailureType(FailureType.Unexpected);
        }
    }

    public async Task<Result> GenerateItemSalesHistoryAsPdfAsync(
        Guid itemId,
        Stream destination,
        Stream? template = null,
        CancellationToken cancellationToken = default)
    {
        try
        {
            Result result = await _reportService
                .GenerateItemSalesHistoryAsPdfAsync(
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
            return Result
                .Fail("Failed to generate report, something went wrong.")
                .WithFailureType(FailureType.Unexpected);
        }
    }

    public async Task<Result> GenerateItemPurchaseHistoryAsWordAsync(
        Guid itemId,
        Stream destination,
        Stream? template = null,
        CancellationToken cancellationToken = default)
    {
        try
        {
            Result result = await _reportService
                .GenerateItemPurchaseHistoryAsWordAsync(
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
            return Result
                .Fail("Failed to generate report, something went wrong.")
                .WithFailureType(FailureType.Unexpected);
        }
    }

    public async Task<Result> GenerateItemPurchaseHistoryAsPdfAsync(
        Guid itemId,
        Stream destination,
        Stream? template = null,
        CancellationToken cancellationToken = default)
    {
        try
        {
            Result result = await _reportService
                .GenerateItemPurchaseHistoryAsPdfAsync(
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
            return Result
                .Fail("Failed to generate report, something went wrong.")
                .WithFailureType(FailureType.Unexpected);
        }
    }
}