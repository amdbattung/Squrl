using Squrl.App.Common;
using Squrl.App.Enums;
using Squrl.App.Features.UnitOfMeasures.DTOs;

namespace Squrl.App.Services.UnitOfMeasure;

public interface IUomService
{
    Task<Result<GetManyUomsDto>> GetManyUomsAsync(
        string? query = null,
        int? pageNumber = null,
        int? pageSize = null,
        SortDirection orderDirection = SortDirection.Ascending,
        CancellationToken cancellationToken = default);
    Task<Result<GetUomDto>> CreateUomAsync(CreateUomDto uom, CancellationToken cancellationToken = default);
    Task<Result<GetUomDto>> GetUomByIdAsync(Guid id, CancellationToken cancellationToken = default);
    Task<Result<GetUomDto>> UpdateUomAsync(Guid id, UpdateUomDto uom, CancellationToken cancellationToken = default);
    Task<Result<GetUomDto>> DeleteUomAsync(Guid id, CancellationToken cancellationToken = default);
}