using Squrl.App.Common;

namespace Squrl.App.Services.Report;

public interface IReportService
{
    Task<Result<bool>> GenerateCustomerItemSalesAsWordAsync(
        string customer,
        Stream destination,
        Stream? template = null,
        CancellationToken cancellationToken = default);
    
    Task<Result<bool>> GenerateCustomerItemSalesAsPdfAsync(
        string customer,
        Stream destination,
        Stream? template = null,
        CancellationToken cancellationToken = default);
    
    Task<Result<bool>> GenerateSupplierItemPurchasesAsWordAsync(
        Guid supplierId,
        Stream destination,
        Stream? template = null,
        CancellationToken cancellationToken = default);
    
    Task<Result<bool>> GenerateSupplierItemPurchasesAsPdfAsync(
        Guid supplierId,
        Stream destination,
        Stream? template = null,
        CancellationToken cancellationToken = default);
    
    Task<Result<bool>> GenerateItemSalesHistoryAsWordAsync(
        Guid itemId,
        Stream destination,
        Stream? template = null,
        CancellationToken cancellationToken = default);
    
    Task<Result<bool>> GenerateItemSalesHistoryAsPdfAsync(
        Guid itemId,
        Stream destination,
        Stream? template = null,
        CancellationToken cancellationToken = default);
    
    Task<Result<bool>> GenerateItemPurchaseHistoryAsWordAsync(
        Guid itemId,
        Stream destination,
        Stream? template = null,
        CancellationToken cancellationToken = default);
    
    Task<Result<bool>> GenerateItemPurchaseHistoryAsPdfAsync(
        Guid itemId,
        Stream destination,
        Stream? template = null,
        CancellationToken cancellationToken = default);
}