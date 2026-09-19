using Squrl.App.Common;
using Squrl.App.Enums;
using Squrl.App.Features.SalesOrderDetails.DTOs;
using Squrl.App.Features.SalesOrders.DTOs;

namespace Squrl.App.Services.SalesOrder;

public class SalesOrderLoggingDecorator : ISalesOrderService
{
    private readonly ISalesOrderService _salesOrderService;
    private readonly ILogger<SalesOrderLoggingDecorator> _logger;

    public SalesOrderLoggingDecorator(ISalesOrderService salesOrderService,
        ILogger<SalesOrderLoggingDecorator> logger)
    {
        _salesOrderService = salesOrderService;
        _logger = logger;
    }
    
    public async Task<Result<GetManySalesOrdersDto>> GetManySalesOrdersAsync(string? query = null,
        string? customer = null,
        int? pageNumber = null,
        int? pageSize = null,
        SortDirection orderDirection = SortDirection.Ascending,
        CancellationToken cancellationToken = default)
    {
        Result<GetManySalesOrdersDto> result = await _salesOrderService.GetManySalesOrdersAsync(
            query,
            customer,
            pageNumber,
            pageSize,
            orderDirection,
            cancellationToken);
        
        if (result.IsSuccess)
        {
            _logger.LogInformation($"Get Many Sales Orders Request: {result.Value?.SalesOrders?.Count} sales orders successfully fetched.");
        }
        else if (result.FailureType == FailureType.Exception)
        {
            _logger.LogError(result.Exception, "Get Many Sales Orders Request: unhandled exception.");
            return Result<GetManySalesOrdersDto>.Fail(result.Message ?? "Failed to retrieve sales orders.")
                .WithFailureType(FailureType.Unexpected);
        }
        else
        {
            _logger.LogError(result.Exception, "Get Many Sales Orders Request: failed to fetch sales orders.");
            return Result<GetManySalesOrdersDto>.Fail(result.Message ?? "Failed to retrieve sales orders.")
                .WithFailureType(FailureType.Unexpected);
        }
        
        return result;
    }

    public async Task<Result<GetSalesOrderDto>> CreateSalesOrderAsync(CreateSalesOrderDto salesOrder, CancellationToken cancellationToken = default)
    {
        Result<GetSalesOrderDto> result = await _salesOrderService.CreateSalesOrderAsync(salesOrder, cancellationToken);

        if (result.IsSuccess)
        {
            _logger.LogInformation($"Create Sales Order Request: sales order {result.Value?.Id} successfully created.");
        }
        else switch (result.FailureType)
        {
            case FailureType.Validation:
                _logger.LogInformation($"Create Sales Order Request: failed to create sales order {result.Value?.Id}, invalid sales order.");
                break;
            
            case FailureType.BusinessLogic:
                _logger.LogError(result.Exception, "Create Sales Order Request: failed to create sales order.");
                break;
            
            case FailureType.Exception:
                _logger.LogError(result.Exception, "Create Sales Order Request: unhandled exception.");
                return Result<GetSalesOrderDto>.Fail(result.Message ?? "Failed to create sales order.")
                    .WithFailureType(FailureType.Unexpected);
            
            default:
                _logger.LogError(result.Exception, "Create Sales Order Request: failed to create sales order.");
                return Result<GetSalesOrderDto>.Fail(result.Message ?? "Failed to create sales order.")
                    .WithFailureType(FailureType.Unexpected);
        }
    
        return result;
    }

    public async Task<Result<GetSalesOrderDto>> GetSalesOrderByIdAsync(Guid id, CancellationToken cancellationToken = default)
    {
        Result<GetSalesOrderDto> result = await _salesOrderService.GetSalesOrderByIdAsync(id, cancellationToken);

        if (result.IsSuccess)
        {
            _logger.LogInformation($"Get Sales Order By ID Request: sales order {result.Value?.Id} successfully fetched.");
        }
        else switch (result.FailureType)
        {
            case FailureType.Validation:
                _logger.LogInformation("Get Sales Order By ID Request: failed to fetch, invalid request.");
                break;
            
            case FailureType.NotFound:
                _logger.LogInformation("Get Sales Order By ID Request: failed to fetch, sales order not found.");
                break;
            
            case FailureType.Exception:
                _logger.LogError(result.Exception, "Get Sales Order By ID Request: unhandled exception.");
                return Result<GetSalesOrderDto>.Fail(result.Message ?? "Failed to retrieve sales order.")
                    .WithFailureType(FailureType.Unexpected);
            
            default:
                _logger.LogError(result.Exception, "Get Sales Order By ID Request: failed to fetch sales order.");
                return Result<GetSalesOrderDto>.Fail(result.Message ?? "Failed to retrieve sales order.")
                    .WithFailureType(FailureType.Unexpected);
        }
        
        return result;
    }

    public async Task<Result<GetSalesOrderDto>> UpdateSalesOrderAsync(Guid id, UpdateSalesOrderDto salesOrder, CancellationToken cancellationToken = default)
    {
        Result<GetSalesOrderDto> result = await _salesOrderService.UpdateSalesOrderAsync(id, salesOrder, cancellationToken);

        if (result.IsSuccess)
        {
            _logger.LogInformation($"Update Sales Order Request: sales order {result.Value?.Id} successfully updated.");
        }
        else switch (result.FailureType)
        {
            case FailureType.Validation:
                _logger.LogInformation("Update Sales Order Request: failed to update, invalid request.");
                break;
            
            case FailureType.NotFound:
                _logger.LogInformation("Update Sales Order Request: failed to update, sales order not found.");
                break;
            
            case FailureType.Exception:
                _logger.LogError(result.Exception, "Update Sales Order Request: unhandled exception.");
                return Result<GetSalesOrderDto>.Fail(result.Message ?? "Failed to update sales order.")
                    .WithFailureType(FailureType.Unexpected);
            
            default:
                _logger.LogError(result.Exception, "Update Sales Order Request: failed to update sales order.");
                return Result<GetSalesOrderDto>.Fail(result.Message ?? "Failed to update sales order.")
                    .WithFailureType(FailureType.Unexpected);
        }
        
        return result;
    }

    public async Task<Result<GetSalesOrderDto>> DeleteSalesOrderAsync(Guid id, CancellationToken cancellationToken = default)
    {
        Result<GetSalesOrderDto> result = await _salesOrderService.DeleteSalesOrderAsync(id, cancellationToken);

        if (result.IsSuccess)
        {
            _logger.LogInformation($"Delete Sales Order Request: sales order {result.Value?.Id} successfully deleted.");
        }
        else switch (result.FailureType)
        {
            case FailureType.Validation:
                _logger.LogInformation("Delete Sales Order Request: failed to delete, invalid request.");
                break;
            
            case FailureType.NotFound:
                _logger.LogInformation("Delete Sales Order Request: failed to update, sales order not found.");
                break;
            
            case FailureType.Exception:
                _logger.LogError(result.Exception, "Delete Sales Order Request: unhandled exception.");
                return Result<GetSalesOrderDto>.Fail(result.Message ?? "Failed to delete sales order.")
                    .WithFailureType(FailureType.Unexpected);
            
            default:
                _logger.LogError(result.Exception, "Delete Sales Order Request: failed to update sales order.");
                return Result<GetSalesOrderDto>.Fail(result.Message ?? "Failed to delete sales order.")
                    .WithFailureType(FailureType.Unexpected);
        }
        
        return result;
    }

    public async Task<Result<GetManySoDetailsDto>> GetSoDetailsAsync(Guid salesOrderId, SortDirection orderDirection = SortDirection.Ascending,
        CancellationToken cancellationToken = default)
    {
        Result<GetManySoDetailsDto> result = await _salesOrderService.GetSoDetailsAsync(
            salesOrderId,
            orderDirection,
            cancellationToken);

        if (result.IsSuccess)
        {
            _logger.LogInformation($"Get Many SO Details Request: {result.Value?.SoDetails?.Count} SO details successfully fetched.");
        }
        else if (result.FailureType == FailureType.Exception)
        {
            _logger.LogError(result.Exception, "Get Many SO Details Request: unhandled exception.");
            return Result<GetManySoDetailsDto>.Fail(result.Message ?? "Failed to retrieve SO details.")
                .WithFailureType(FailureType.Unexpected);
        }
        else
        {
            _logger.LogError(result.Exception, "Get Many SO Details Request: failed to fetch SO details.");
            return Result<GetManySoDetailsDto>.Fail(result.Message ?? "Failed to retrieve SO details.")
                .WithFailureType(FailureType.Unexpected);
        }
        
        return result;
    }

    public async Task<Result<GetSoDetailDto>> AddSoDetailAsync(Guid salesOrderId, CreateSoDetailDto soDetail, CancellationToken cancellationToken = default)
    {
        Result<GetSoDetailDto> result = await _salesOrderService.AddSoDetailAsync(salesOrderId, soDetail, cancellationToken);

        if (result.IsSuccess)
        {
            _logger.LogInformation($"Create SO Detail Request: SO detail {result.Value?.Id} successfully created.");
        }
        else switch (result.FailureType)
        {
            case FailureType.Validation:
                _logger.LogInformation($"Create SO Detail Request: failed to create SO detail {result.Value?.Id}, invalid SO detail.");
                break;
            
            case FailureType.BusinessLogic:
                _logger.LogError(result.Exception, "Create SO Detail Request: failed to create SO detail.");
                break;
            
            case FailureType.Exception:
                _logger.LogError(result.Exception, "Create SO Detail Request: unhandled exception.");
                return Result<GetSoDetailDto>.Fail(result.Message ?? "Failed to create SO detail.")
                    .WithFailureType(FailureType.Unexpected);
            
            default:
                _logger.LogError(result.Exception, "Create SO Detail Request: failed to create SO detail.");
                return Result<GetSoDetailDto>.Fail(result.Message ?? "Failed to create SO detail.")
                    .WithFailureType(FailureType.Unexpected);
        }
    
        return result;
    }

    public async Task<Result<GetSoDetailDto>> GetSoDetailByIdAsync(Guid id, CancellationToken cancellationToken = default)
    {
        Result<GetSoDetailDto> result = await _salesOrderService.GetSoDetailByIdAsync(id, cancellationToken);

        if (result.IsSuccess)
        {
            _logger.LogInformation($"Get SO Detail By ID Request: SO detail {result.Value?.Id} successfully fetched.");
        }
        else switch (result.FailureType)
        {
            case FailureType.Validation:
                _logger.LogInformation("Get SO Detail By ID Request: failed to fetch, invalid request.");
                break;
            
            case FailureType.NotFound:
                _logger.LogInformation("Get SO Detail By ID Request: failed to fetch, SO detail not found.");
                break;
            
            case FailureType.Exception:
                _logger.LogError(result.Exception, "Get SO Detail By ID Request: unhandled exception.");
                return Result<GetSoDetailDto>.Fail(result.Message ?? "Failed to retrieve SO detail.")
                    .WithFailureType(FailureType.Unexpected);
            
            default:
                _logger.LogError(result.Exception, "Get SO Detail By ID Request: failed to fetch SO detail.");
                return Result<GetSoDetailDto>.Fail(result.Message ?? "Failed to retrieve SO detail.")
                    .WithFailureType(FailureType.Unexpected);
        }
        
        return result;
    }

    public async Task<Result<GetSoDetailDto>> UpdateSoDetailAsync(Guid id, UpdateSoDetailDto soDetail, CancellationToken cancellationToken = default)
    {
        Result<GetSoDetailDto> result = await _salesOrderService.UpdateSoDetailAsync(id, soDetail, cancellationToken);

        if (result.IsSuccess)
        {
            _logger.LogInformation($"Update SO Detail Request: SO detail {result.Value?.Id} successfully updated.");
        }
        else switch (result.FailureType)
        {
            case FailureType.Validation:
                _logger.LogInformation("Update SO Detail Request: failed to update, invalid request.");
                break;
            
            case FailureType.NotFound:
                _logger.LogInformation("Update SO Detail Request: failed to update, SO detail not found.");
                break;
            
            case FailureType.Exception:
                _logger.LogError(result.Exception, "Update SO Detail Request: unhandled exception.");
                return Result<GetSoDetailDto>.Fail(result.Message ?? "Failed to update SO detail.")
                    .WithFailureType(FailureType.Unexpected);
            
            default:
                _logger.LogError(result.Exception, "Update SO Detail Request: failed to update SO detail.");
                return Result<GetSoDetailDto>.Fail(result.Message ?? "Failed to update SO detail.")
                    .WithFailureType(FailureType.Unexpected);
        }
        
        return result;
    }

    public async Task<Result<GetSoDetailDto>> RemoveSoDetailAsync(Guid id, CancellationToken cancellationToken = default)
    {
        Result<GetSoDetailDto> result = await _salesOrderService.RemoveSoDetailAsync(id, cancellationToken);

        if (result.IsSuccess)
        {
            _logger.LogInformation($"Delete SO Detail Request: SO detail {result.Value?.Id} successfully deleted.");
        }
        else switch (result.FailureType)
        {
            case FailureType.Validation:
                _logger.LogInformation("Delete SO Detail Request: failed to delete, invalid request.");
                break;
            
            case FailureType.NotFound:
                _logger.LogInformation("Delete SO Detail Request: failed to update, SO detail not found.");
                break;
            
            case FailureType.Exception:
                _logger.LogError(result.Exception, "Delete SO Detail Request: unhandled exception.");
                return Result<GetSoDetailDto>.Fail(result.Message ?? "Failed to delete SO detail.")
                    .WithFailureType(FailureType.Unexpected);
            
            default:
                _logger.LogError(result.Exception, "Delete SO Detail Request: failed to update SO detail.");
                return Result<GetSoDetailDto>.Fail(result.Message ?? "Failed to delete SO detail.")
                    .WithFailureType(FailureType.Unexpected);
        }
        
        return result;
    }
}