using Squrl.App.Common;
using Squrl.App.Enums;
using Squrl.App.Features.SalesOrderDetails.DTOs;
using Squrl.App.Features.SalesOrders.DTOs;

namespace Squrl.App.Services.SalesOrder;

public class SalesOrderErrorHandlingDecorator : ISalesOrderService
{
    private readonly ISalesOrderService _salesOrderService;

    public SalesOrderErrorHandlingDecorator(ISalesOrderService salesOrderService)
    {
        _salesOrderService = salesOrderService;
    }
    
    public async Task<Result<GetManySalesOrdersDto>> GetManySalesOrdersAsync(
        string? search = null,
        string? customer = null,
        int pageNumber = 1,
        int pageSize = 10,
        SortDirection orderDirection = SortDirection.Ascending,
        CancellationToken cancellationToken = default)
    {
        try
        {
            Result<GetManySalesOrdersDto> result = await _salesOrderService
                .GetManySalesOrdersAsync(
                    search,
                    customer,
                    pageNumber,
                    pageSize,
                    orderDirection,
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
            return Result<GetManySalesOrdersDto>
                .Fail("Failed to retrieve sales orders, something went wrong.")
                .WithFailureType(FailureType.Unexpected);
        }
    }

    public async Task<Result<GetSalesOrderDto>> CreateSalesOrderAsync(CreateSalesOrderDto salesOrder, CancellationToken cancellationToken = default)
    {
        try
        {
            Result<GetSalesOrderDto> result = await _salesOrderService
                .CreateSalesOrderAsync(salesOrder, cancellationToken);
            
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
            return Result<GetSalesOrderDto>
                .Fail("Failed to create sales order, something went wrong.")
                .WithFailureType(FailureType.Unexpected);
        }
    }

    public async Task<Result<GetSalesOrderDto>> GetSalesOrderByIdAsync(Guid id, CancellationToken cancellationToken = default)
    {
        try
        {
            Result<GetSalesOrderDto> result = await _salesOrderService
                .GetSalesOrderByIdAsync(id, cancellationToken);
            
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
            return Result<GetSalesOrderDto>
                .Fail("Failed to retrieve sales order, something went wrong.")
                .WithFailureType(FailureType.Unexpected);
        }
    }

    public async Task<Result<GetSalesOrderDto>> UpdateSalesOrderAsync(Guid id, UpdateSalesOrderDto salesOrder, CancellationToken cancellationToken = default)
    {
        try
        {
            Result<GetSalesOrderDto> result = await _salesOrderService
                .UpdateSalesOrderAsync(id, salesOrder, cancellationToken);
            
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
            return Result<GetSalesOrderDto>
                .Fail("Failed to update sales order, something went wrong.")
                .WithFailureType(FailureType.Unexpected);
        }
    }

    public async Task<Result<GetSalesOrderDto>> DeleteSalesOrderAsync(Guid id, CancellationToken cancellationToken = default)
    {
        try
        {
            Result<GetSalesOrderDto> result = await _salesOrderService
                .DeleteSalesOrderAsync(id, cancellationToken);
            
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
            return Result<GetSalesOrderDto>
                .Fail("Failed to delete sales order, something went wrong.")
                .WithFailureType(FailureType.Unexpected);
        }
    }

    public async Task<Result<GetManySoDetailsDto>> GetSoDetailsAsync(
        Guid salesOrderId,
        SortDirection orderDirection = SortDirection.Ascending,
        CancellationToken cancellationToken = default)
    {
        try
        {
            Result<GetManySoDetailsDto> result = await _salesOrderService
                .GetSoDetailsAsync(salesOrderId, orderDirection, cancellationToken);
            
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
            return Result<GetManySoDetailsDto>
                .Fail("Failed to retrieve SO details, something went wrong.")
                .WithFailureType(FailureType.Unexpected);
        }
    }

    public async Task<Result<GetSoDetailDto>> AddSoDetailAsync(Guid salesOrderId, CreateSoDetailDto soDetail, CancellationToken cancellationToken = default)
    {
        try
        {
            Result<GetSoDetailDto> result = await _salesOrderService
                .AddSoDetailAsync(salesOrderId, soDetail, cancellationToken);
            
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
            return Result<GetSoDetailDto>
                .Fail("Failed to create SO detail, something went wrong.")
                .WithFailureType(FailureType.Unexpected);
        }
    }

    public async Task<Result<GetSoDetailDto>> GetSoDetailByIdAsync(Guid id, CancellationToken cancellationToken = default)
    {
        try
        {
            Result<GetSoDetailDto> result = await _salesOrderService
                .GetSoDetailByIdAsync(id, cancellationToken);
            
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
            return Result<GetSoDetailDto>
                .Fail("Failed to retrieve SO detail, something went wrong.")
                .WithFailureType(FailureType.Unexpected);
        }
    }

    public async Task<Result<GetSoDetailDto>> UpdateSoDetailAsync(Guid id, UpdateSoDetailDto soDetail, CancellationToken cancellationToken = default)
    {
        try
        {
            Result<GetSoDetailDto> result = await _salesOrderService
                .UpdateSoDetailAsync(id, soDetail, cancellationToken);
            
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
            return Result<GetSoDetailDto>
                .Fail("Failed to update SO detail, something went wrong.")
                .WithFailureType(FailureType.Unexpected);
        }
    }

    public async Task<Result<GetSoDetailDto>> RemoveSoDetailAsync(Guid id, CancellationToken cancellationToken = default)
    {
        try
        {
            Result<GetSoDetailDto> result = await _salesOrderService
                .RemoveSoDetailAsync(id, cancellationToken);
            
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
            return Result<GetSoDetailDto>
                .Fail("Failed to delete SO detail, something went wrong.")
                .WithFailureType(FailureType.Unexpected);
        }
    }

    public async Task<Result<bool>> ExportSoDetailsAsCsvAsync(Guid salesOrderId, Stream destination, CancellationToken cancellationToken = default)
    {
        try
        {
            Result<bool> result = await _salesOrderService
                .ExportSoDetailsAsCsvAsync(salesOrderId, destination, cancellationToken);
            
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
                .Fail("Failed to export SO detail, something went wrong.")
                .WithFailureType(FailureType.Unexpected);
        }
    }

    public async Task<Result<bool>> ExportSoDetailsAsExcelAsync(Guid salesOrderId, Stream destination, CancellationToken cancellationToken = default)
    {
        try
        {
            Result<bool> result = await _salesOrderService
                .ExportSoDetailsAsExcelAsync(salesOrderId, destination, cancellationToken);
            
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
                .Fail("Failed to export SO detail, something went wrong.")
                .WithFailureType(FailureType.Unexpected);
        }
    }
}