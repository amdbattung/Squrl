using Microsoft.EntityFrameworkCore;
using NodaTime;
using Squrl.App.Common;
using Squrl.App.Data;
using Squrl.App.Enums;
using Squrl.App.Features.UnitOfMeasures.DTOs;
using Squrl.App.Features.UnitOfMeasures.Mapping;
using Squrl.App.Infrastructure.DataExporter;
using Squrl.App.Infrastructure.TransactionManager;

namespace Squrl.App.Services.UnitOfMeasure;

public partial class UomService : IUomService
{
    private readonly DataContext _dataContext;
    private readonly ITransactionManager _transactionManager;
    private readonly ICsvExporter<Models.UnitOfMeasure> _csvExporter;

    public UomService(DataContext dataContext,
        ITransactionManager transactionManager,
        ICsvExporter<Models.UnitOfMeasure> csvExporter)
    {
        _dataContext = dataContext;
        _transactionManager = transactionManager;
        _csvExporter = csvExporter;
    }

    public async Task<Result<GetManyUomsDto>> GetManyUomsAsync(
        string? search = null,
        int pageNumber = 1,
        int pageSize = 10,
        SortDirection orderDirection = SortDirection.Ascending,
        CancellationToken cancellationToken = default)
    {
        IQueryable<Models.UnitOfMeasure> query = _dataContext.UnitOfMeasures
            .AsNoTracking();
        
        if (!string.IsNullOrWhiteSpace(search))
        {
            string pattern = $"%{search}%";

            query = query.Where(u =>
                EF.Functions.Like(u.Name, pattern) ||
                EF.Functions.Like(u.Code, pattern));
        }
        
        if (orderDirection == SortDirection.Descending)
        {
            query = query
                .OrderByDescending(p => EF.Property<Instant>(p, "DateCreated"))
                .ThenBy(p => p.Id);
        }
        else
        {
            query = query
                .OrderBy(p => EF.Property<Instant>(p, "DateCreated"))
                .ThenBy(p => p.Id);
        }
        
        int totalCount = await query.CountAsync(cancellationToken);

        IReadOnlyList<Models.UnitOfMeasure> existingUnitOfMeasures = await query
            .Skip((pageNumber - 1) * pageSize)
            .Take(pageSize)
            .ToListAsync(cancellationToken);
        
        GetManyUomsDto payload = new(
            pageNumber,
            pageSize,
            totalCount,
            existingUnitOfMeasures.Select(UomMapper.ToDto).ToList());

        return Result<GetManyUomsDto>.Ok(payload);
    }

    public async Task<Result<GetUomDto>> CreateUomAsync(CreateUomDto uom, CancellationToken cancellationToken = default)
    {
        return await _transactionManager.ExecuteAsync(async ct =>
               {
                   Models.UnitOfMeasure newUom = new()
                   {
                       Id = Guid.NewGuid(),
                       Name = uom.Name ?? "",
                       Code = uom.Code ?? "",
                       Description = uom.Description,
                   };
        
                   await _dataContext.UnitOfMeasures.AddAsync(newUom, ct);

                   return Result<GetUomDto>.Ok(UomMapper.ToDto(newUom));
               }, cancellationToken)
               ?? throw new InvalidOperationException("UOM could not be persisted.");
    }

    public async Task<Result<GetUomDto>> GetUomByIdAsync(Guid id, CancellationToken cancellationToken = default)
    {
        Models.UnitOfMeasure? existingUom = await _dataContext.UnitOfMeasures
            .AsNoTracking()
            .Where(u => u.Id == id)
            .FirstOrDefaultAsync(cancellationToken);
        
        if (existingUom == null)
        {
            return Result<GetUomDto>.Fail("UOM not found.")
                .WithFailureType(FailureType.NotFound);
        }

        return Result<GetUomDto>.Ok(UomMapper.ToDto(existingUom));
    }

    public async Task<Result<GetUomDto>> UpdateUomAsync(Guid id, UpdateUomDto uom, CancellationToken cancellationToken = default)
    {
        Func<CancellationToken, Task<Result<GetUomDto>>> action = async ct =>
        {
            Models.UnitOfMeasure? existingUom = await _dataContext.UnitOfMeasures
                .FirstOrDefaultAsync(u => u.Id == id, ct);
        
            if (existingUom == null)
            {
                return Result<GetUomDto>.Fail("UOM not found.")
                    .WithFailureType(FailureType.NotFound);
            }
        
            existingUom.Name = uom.Name ?? existingUom.Name;
            existingUom.Code = uom.Code ?? existingUom.Code;
            existingUom.Description = uom.Description ?? existingUom.Description;
        
            return Result<GetUomDto>.Ok(UomMapper.ToDto(existingUom));
        };
        
        return await _transactionManager.ExecuteAsync(action, cancellationToken);
    }

    public async Task<Result<GetUomDto>> DeleteUomAsync(Guid id, CancellationToken cancellationToken = default)
    {
        Func<CancellationToken, Task<Result<GetUomDto>>> action = async ct =>
        {
            Models.UnitOfMeasure? existingUom = await _dataContext.UnitOfMeasures
                .FirstOrDefaultAsync(u => u.Id == id, ct);
        
            if (existingUom == null)
            {
                return Result<GetUomDto>.Fail("UOM not found.")
                    .WithFailureType(FailureType.NotFound);
            }
        
            _dataContext.UnitOfMeasures.Remove(existingUom);

            return Result<GetUomDto>.Ok(UomMapper.ToDto(existingUom));
        };
        
        return await _transactionManager.ExecuteAsync(action, cancellationToken);
    }
}