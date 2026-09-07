using Squrl.App.Common;
using Squrl.App.Enums;
using Squrl.App.Features.Suppliers.DTOs;

namespace Squrl.App.Services.Supplier;

public interface ISupplierService
{
    Task<Result<GetManySuppliersDto>> GetManySuppliersAsync(
        string? query = null,
        int? pageNumber = null,
        int? pageSize = null,
        SortDirection orderDirection = SortDirection.Ascending,
        CancellationToken cancellationToken = default);
    Task<Result<GetSupplierDto>> CreateSupplierAsync(CreateSupplierDto supplier, CancellationToken cancellationToken = default);
    Task<Result<GetSupplierDto>> GetSupplierByIdAsync(Guid id, CancellationToken cancellationToken = default);
    Task<Result<GetSupplierDto>> UpdateSupplierAsync(Guid id, UpdateSupplierDto supplier, CancellationToken cancellationToken = default);
    Task<Result<GetSupplierDto>> DeleteSupplierAsync(Guid id, CancellationToken cancellationToken = default);
}