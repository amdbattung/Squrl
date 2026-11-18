using Squrl.App.Common;

namespace Squrl.App.Services.Report;

public interface IReportService
{
    Task<Result<bool>> GenerateCustomerItemSalesAsWord(
        string customer,
        Stream destination,
        Stream? template = null,
        CancellationToken cancellationToken = default);
    
    Task<Result<bool>> GenerateCustomerItemSalesAsPdf(
        string customer,
        Stream destination,
        Stream? template = null,
        CancellationToken cancellationToken = default);
    
    Task<Result<bool>> GenerateSupplierItemPurchasesAsWord(
        Guid supplierId,
        Stream destination,
        Stream? template = null,
        CancellationToken cancellationToken = default);
    
    Task<Result<bool>> GenerateSupplierItemPurchasesAsPdf(
        Guid supplierId,
        Stream destination,
        Stream? template = null,
        CancellationToken cancellationToken = default);
    
    Task<Result<bool>> GenerateItemSalesHistoryAsWord(
        Guid itemId,
        Stream destination,
        Stream? template = null,
        CancellationToken cancellationToken = default);
    
    Task<Result<bool>> GenerateItemSalesHistoryAsPdf(
        Guid itemId,
        Stream destination,
        Stream? template = null,
        CancellationToken cancellationToken = default);
}