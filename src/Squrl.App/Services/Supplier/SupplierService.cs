using Microsoft.EntityFrameworkCore;
using NodaTime;
using Squrl.App.Common;
using Squrl.App.Data;
using Squrl.App.Enums;
using Squrl.App.Features.Suppliers.DTOs;
using Squrl.App.Features.Suppliers.Mapping;
using Squrl.App.Infrastructure.DataExporter;
using Squrl.App.Infrastructure.TransactionManager;

namespace Squrl.App.Services.Supplier;

public partial class SupplierService : ISupplierService
{
    private readonly DataContext _dataContext;
    private readonly ITransactionManager _transactionManager;
    private readonly ICsvExporter<Models.Supplier> _csvExporter;

    public SupplierService(DataContext dataContext,
        ITransactionManager transactionManager,
        ICsvExporter<Models.Supplier> csvExporter)
    {
        _dataContext = dataContext;
        _transactionManager = transactionManager;
        _csvExporter = csvExporter;
    }

    public async Task<Result<GetManySuppliersDto>> GetManySuppliersAsync(
        string? search = null,
        int pageNumber = 1,
        int pageSize = 10,
        SortDirection orderDirection = SortDirection.Ascending,
        CancellationToken cancellationToken = default)
    {
        IQueryable<Models.Supplier> query = _dataContext.Suppliers
            .AsNoTracking();
        
        if (!string.IsNullOrWhiteSpace(search))
        {
            query = query.Where(s =>
                EF.Functions.Like(s.Name, $"%{search}%"));
        }
        
        if (orderDirection == SortDirection.Descending)
        {
            query = query
                .OrderByDescending(s => EF.Property<Instant>(s, "DateCreated"))
                .ThenBy(s => s.Id);
        }
        else
        {
            query = query
                .OrderBy(s => EF.Property<Instant>(s, "DateCreated"))
                .ThenBy(s => s.Id);
        }
        
        int totalCount = await query.CountAsync(cancellationToken);

        IReadOnlyList<Models.Supplier> existingSuppliers = await query
            .Skip((pageNumber - 1) * pageSize)
            .Take(pageSize)
            .ToListAsync(cancellationToken);
        
        GetManySuppliersDto payload = new(
            pageNumber,
            pageSize,
            totalCount,
            existingSuppliers.Select(SupplierMapper.ToDto).ToList());

        return Result<GetManySuppliersDto>.Ok(payload);
    }

    public async Task<Result<GetSupplierDto>> CreateSupplierAsync(CreateSupplierDto supplier, CancellationToken cancellationToken = default)
    {
        return await _transactionManager.ExecuteAsync(async ct =>
                      {
                          Models.Supplier newSupplier = new()
                          {
                              Id = Guid.NewGuid(),
                              Name = supplier.Name ?? "",
                              Description = supplier.Description,
                          };
        
                          await _dataContext.Suppliers.AddAsync(newSupplier, ct);
            
                          return Result<GetSupplierDto>.Ok(SupplierMapper.ToDto(newSupplier));
                      }, cancellationToken)
               ?? throw new InvalidOperationException("Supplier could not be persisted.");
    }

    public async Task<Result<GetSupplierDto>> GetSupplierByIdAsync(Guid id, CancellationToken cancellationToken = default)
    {
        Models.Supplier? existingSupplier = await _dataContext.Suppliers
            .AsNoTracking()
            .Where(s => s.Id == id)
            .FirstOrDefaultAsync(cancellationToken);

        if (existingSupplier == null)
        {
            return Result<GetSupplierDto>.Fail("Supplier not found.")
                .WithFailureType(FailureType.NotFound);
        }
        
        return Result<GetSupplierDto>.Ok(SupplierMapper.ToDto(existingSupplier));
    }

    public async Task<Result<GetSupplierDto>> UpdateSupplierAsync(Guid id, UpdateSupplierDto supplier, CancellationToken cancellationToken = default)
    {
        Func<CancellationToken, Task<Result<GetSupplierDto>>> action = async ct =>
        {
            Models.Supplier? existingSupplier = await _dataContext.Suppliers
                .FirstOrDefaultAsync(s => s.Id == id, ct);
        
            if (existingSupplier == null)
            {
                return Result<GetSupplierDto>.Fail("Supplier not found.")
                    .WithFailureType(FailureType.NotFound);
            }
        
            existingSupplier.Name = supplier.Name ?? existingSupplier.Name;
            existingSupplier.Description = supplier.Description ?? existingSupplier.Description;

            return Result<GetSupplierDto>.Ok(SupplierMapper.ToDto(existingSupplier));
        };
        
        return await _transactionManager.ExecuteAsync(action, cancellationToken);
    }

    public async Task<Result<GetSupplierDto>> DeleteSupplierAsync(Guid id, CancellationToken cancellationToken = default)
    {
        Func<CancellationToken, Task<Result<GetSupplierDto>>> action = async ct =>
        {
            Models.Supplier? existingSupplier = await _dataContext.Suppliers
                .FirstOrDefaultAsync(s => s.Id == id, ct);

            if (existingSupplier == null)
            {
                return Result<GetSupplierDto>.Fail("Supplier not found.")
                    .WithFailureType(FailureType.NotFound);
            }

            _dataContext.Suppliers.Remove(existingSupplier);

            return Result<GetSupplierDto>.Ok(SupplierMapper.ToDto(existingSupplier));
        };
            
        return await _transactionManager.ExecuteAsync(action, cancellationToken);
    }
}