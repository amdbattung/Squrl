using FluentValidation;
using FluentValidation.Results;
using Squrl.App.Common;
using Squrl.App.Enums;
using Squrl.App.Features.SalesOrderDetails.DTOs;
using Squrl.App.Features.SalesOrders.DTOs;

namespace Squrl.App.Services.SalesOrder;

public class SalesOrderPreprocessingDecorator : ISalesOrderService
{
    private readonly ISalesOrderService _salesOrderService;
    private readonly IValidator<CreateSalesOrderDto> _createSalesOrderValidator;
    private readonly IValidator<UpdateSalesOrderDto> _updateSalesOrderValidator;
    private readonly IValidator<CreateSoDetailDto> _createSoDetailValidator;
    private readonly IValidator<UpdateSoDetailDto> _updateSoDetailValidator;

    public SalesOrderPreprocessingDecorator(ISalesOrderService salesOrderService,
        IValidator<CreateSalesOrderDto> createSalesOrderValidator,
        IValidator<UpdateSalesOrderDto> updateSalesOrderValidator,
        IValidator<CreateSoDetailDto> createSoDetailValidator,
        IValidator<UpdateSoDetailDto> updateSoDetailValidator)
    {
        _salesOrderService = salesOrderService;
        _createSalesOrderValidator = createSalesOrderValidator;
        _updateSalesOrderValidator = updateSalesOrderValidator;
        _createSoDetailValidator = createSoDetailValidator;
        _updateSoDetailValidator = updateSoDetailValidator;
    }
    
    public async Task<Result<GetManySalesOrdersDto>> GetManySalesOrdersAsync(
        string? search = null,
        string? customer = null,
        int pageNumber = 1,
        int pageSize = 10,
        SortDirection orderDirection = SortDirection.Ascending,
        CancellationToken cancellationToken = default)
    {
        search = search?.Trim();
        pageNumber = pageNumber >= 1 ? pageNumber : 1;
        pageSize = pageSize is >= 1 and <= 50 ? pageSize : 10;
        
        Result<GetManySalesOrdersDto> result = await _salesOrderService
            .GetManySalesOrdersAsync(
                search,
                customer,
                pageNumber,
                pageSize,
                orderDirection,
                cancellationToken);

        return result;
    }

    public async Task<Result<GetSalesOrderDto>> CreateSalesOrderAsync(CreateSalesOrderDto salesOrder, CancellationToken cancellationToken = default)
    {
        ValidationResult soValidationResult = await _createSalesOrderValidator
            .ValidateAsync(salesOrder, cancellationToken);
        
        if (!soValidationResult.IsValid)
        {
            return Result<GetSalesOrderDto>.Fail(soValidationResult.Errors
                    .Select(e => e.ErrorMessage)
                    .ToArray())
                .WithFailureType(FailureType.Validation);
        }
        
        Result<GetSalesOrderDto> result = await _salesOrderService
            .CreateSalesOrderAsync(salesOrder, cancellationToken);
        
        return result;
    }

    public async Task<Result<GetSalesOrderDto>> GetSalesOrderByIdAsync(Guid id, CancellationToken cancellationToken = default)
    {
        if (id == Guid.Empty)
        {
            return Result<GetSalesOrderDto>.Fail("Null or invalid ID.")
                .WithFailureType(FailureType.Validation);
        }
        
        Result<GetSalesOrderDto> result = await _salesOrderService
            .GetSalesOrderByIdAsync(id, cancellationToken);
        
        return result;
    }

    public async Task<Result<GetSalesOrderDto>> UpdateSalesOrderAsync(Guid id, UpdateSalesOrderDto salesOrder, CancellationToken cancellationToken = default)
    {
        if (id == Guid.Empty)
        {
            return Result<GetSalesOrderDto>.Fail("Null or invalid ID.")
                .WithFailureType(FailureType.Validation);
        }
            
        ValidationResult soValidationResult = await _updateSalesOrderValidator
            .ValidateAsync(salesOrder, cancellationToken);
            
        if (!soValidationResult.IsValid)
        {
            return Result<GetSalesOrderDto>.Fail(soValidationResult.Errors
                    .Select(e => e.ErrorMessage)
                    .ToArray())
                .WithFailureType(FailureType.Validation);
        }
        
        Result<GetSalesOrderDto> result = await _salesOrderService
            .UpdateSalesOrderAsync(id, salesOrder, cancellationToken);
        
        return result;
    }

    public async Task<Result<GetSalesOrderDto>> DeleteSalesOrderAsync(Guid id, CancellationToken cancellationToken = default)
    {
        if (id == Guid.Empty)
        {
            return Result<GetSalesOrderDto>.Fail("Null or invalid ID.")
                .WithFailureType(FailureType.Validation);
        }
        
        Result<GetSalesOrderDto> result = await _salesOrderService
            .DeleteSalesOrderAsync(id, cancellationToken);
        
        return result;
    }

    public async Task<Result<GetManySoDetailsDto>> GetSoDetailsAsync(Guid salesOrderId, SortDirection orderDirection = SortDirection.Ascending,
        CancellationToken cancellationToken = default)
    {
        if (salesOrderId == Guid.Empty)
        {
            return Result<GetManySoDetailsDto>.Fail("Null or invalid ID.")
                .WithFailureType(FailureType.Validation);
        }
        
        Result<GetManySoDetailsDto> result = await _salesOrderService
            .GetSoDetailsAsync(salesOrderId, orderDirection, cancellationToken);
        
        return result;
    }

    public async Task<Result<GetSoDetailDto>> AddSoDetailAsync(Guid salesOrderId, CreateSoDetailDto soDetail, CancellationToken cancellationToken = default)
    {
        if (salesOrderId == Guid.Empty)
        {
            return Result<GetSoDetailDto>.Fail("Null or invalid ID.")
                .WithFailureType(FailureType.Validation);
        }
        
        ValidationResult validationResult = await _createSoDetailValidator
            .ValidateAsync(soDetail, cancellationToken);
            
        if (!validationResult.IsValid)
        {
            return Result<GetSoDetailDto>.Fail(validationResult.Errors
                    .Select(e => e.ErrorMessage)
                    .ToArray())
                .WithFailureType(FailureType.Validation);
        }
        
        Result<GetSoDetailDto> result = await _salesOrderService
            .AddSoDetailAsync(salesOrderId, soDetail, cancellationToken);
        
        return result;
    }

    public async Task<Result<GetSoDetailDto>> GetSoDetailByIdAsync(Guid id, CancellationToken cancellationToken = default)
    {
        if (id == Guid.Empty)
        {
            return Result<GetSoDetailDto>.Fail("Null or invalid ID.")
                .WithFailureType(FailureType.Validation);
        }
        
        Result<GetSoDetailDto> result = await _salesOrderService
            .GetSoDetailByIdAsync(id, cancellationToken);
        
        return result;
    }

    public async Task<Result<GetSoDetailDto>> UpdateSoDetailAsync(Guid id, UpdateSoDetailDto soDetail, CancellationToken cancellationToken = default)
    {
        if (id == Guid.Empty)
        {
            return Result<GetSoDetailDto>.Fail("Null or invalid ID.")
                .WithFailureType(FailureType.Validation);
        }
        
        ValidationResult validationResult = await _updateSoDetailValidator
            .ValidateAsync(soDetail, cancellationToken);
            
        if (!validationResult.IsValid)
        {
            return Result<GetSoDetailDto>.Fail(validationResult.Errors
                    .Select(e => e.ErrorMessage)
                    .ToArray())
                .WithFailureType(FailureType.Validation);
        }
        
        Result<GetSoDetailDto> result = await _salesOrderService
            .UpdateSoDetailAsync(id, soDetail, cancellationToken);
        
        return result;
    }

    public async Task<Result<GetSoDetailDto>> RemoveSoDetailAsync(Guid id, CancellationToken cancellationToken = default)
    {
        if (id == Guid.Empty)
        {
            return Result<GetSoDetailDto>.Fail("Null or invalid ID.")
                .WithFailureType(FailureType.Validation);
        }
        
        Result<GetSoDetailDto> result = await _salesOrderService
            .RemoveSoDetailAsync(id, cancellationToken);
        
        return result;
    }
}