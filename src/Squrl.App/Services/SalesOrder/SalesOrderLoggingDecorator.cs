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
    
    public async Task<Result<GetManySalesOrdersDto>> GetManySalesOrdersAsync(
        string? search = null,
        string? customer = null,
        int pageNumber = 1,
        int pageSize = 10,
        SortDirection orderDirection = SortDirection.Ascending,
        CancellationToken cancellationToken = default)
    {
        Result<GetManySalesOrdersDto> result;

        try
        {
            result = await _salesOrderService.GetManySalesOrdersAsync(
                search,
                customer,
                pageNumber,
                pageSize,
                orderDirection,
                cancellationToken);
        }
        catch (Exception e)
        {
            _logger.LogError(e, "Get Many Sales Orders Request: Unhandled exception.");
            throw;
        }
        
        if (result.IsSuccess)
        {
            _logger.LogInformation("Get Many Sales Orders Request: {Count} sales orders successfully fetched.",
                result.Value?.SalesOrders?.Count);
        }
        else switch (result.FailureType)
        {
            case FailureType.Validation:
            
            case FailureType.NotFound:
            
            case FailureType.BusinessLogic:
                _logger.LogInformation(result.Exception,
                    "Get Many Sales Orders Request: {Message}",
                    result.Message ?? "Failed to retrieve sales orders.");
                break;

            case FailureType.Exception:
            
            case FailureType.Unexpected:
                _logger.LogError(result.Exception,
                    "Get Many Sales Orders Request: Unhandled exception.");
                throw result.Exception
                      ?? new Exception(result.Message ?? "Failed to retrieve sales orders, something went wrong.");

            default:
                _logger.LogError(result.Exception,
                    "Get Many Sales Orders Request: Unhandled failure type {FailureType}.",
                    Enum.GetName(result.FailureType ?? FailureType.Exception));
                throw result.Exception
                      ?? new Exception(result.Message ?? "Failed to retrieve sales orders, something went wrong.");
        }
        
        return result;
    }

    public async Task<Result<GetSalesOrderDto>> CreateSalesOrderAsync(CreateSalesOrderDto salesOrder, CancellationToken cancellationToken = default)
    {
        Result<GetSalesOrderDto> result;

        try
        {
            result = await _salesOrderService.CreateSalesOrderAsync(salesOrder, cancellationToken);
        }
        catch (Exception e)
        {
            _logger.LogError(e, "Create Sales Order Request: Unhandled exception.");
            throw;
        }

        if (result.IsSuccess)
        {
            _logger.LogInformation("Create Sales Order Request: Sales order {Id} successfully created.",
                result.Value?.Id);
        }
        else switch (result.FailureType)
        {
            case FailureType.Validation:
            
            case FailureType.NotFound:
            
            case FailureType.BusinessLogic:
                _logger.LogInformation(result.Exception,
                    "Create Sales Order Request: {Message}",
                    result.Message ?? "Failed to create sales order.");
                break;

            case FailureType.Exception:
                
            case FailureType.Unexpected:
                _logger.LogError(result.Exception,
                    "Create Sales Order Request: Unhandled exception.");
                throw result.Exception
                      ?? new Exception(result.Message ?? "Failed to create sales order, something went wrong.");
            default:
                _logger.LogError(result.Exception,
                    "Create Sales Order Request: Unhandled failure type {FailureType}.",
                    Enum.GetName(result.FailureType ?? FailureType.Exception));
                throw result.Exception
                      ?? new Exception(result.Message ?? "Failed to create sales order, something went wrong.");
        }
    
        return result;
    }

    public async Task<Result<GetSalesOrderDto>> GetSalesOrderByIdAsync(Guid id, CancellationToken cancellationToken = default)
    {
        Result<GetSalesOrderDto> result;

        try
        {
            result = await _salesOrderService.GetSalesOrderByIdAsync(id, cancellationToken);
        }
        catch (Exception e)
        {
            _logger.LogError(e, "Get Sales Order By ID Request: unhandled exception.");
            throw;
        }

        if (result.IsSuccess)
        {
            _logger.LogInformation("Get Sales Order By ID Request: sales order {Id} successfully fetched.",
                result.Value?.Id);
        }
        else switch (result.FailureType)
        {
            case FailureType.Validation:
            
            case FailureType.NotFound:
            
            case FailureType.BusinessLogic:
                _logger.LogInformation(result.Exception,
                    "Get Sales Order By ID Request: {Message}",
                    result.Message ?? "Failed to retrieve sales order.");
                break;

            case FailureType.Exception:
            
            case FailureType.Unexpected:
                _logger.LogError(result.Exception,
                    "Get Sales Order By ID Request: unhandled exception.");
                throw result.Exception
                      ?? new Exception(result.Message ?? "Failed to retrieve sales order, something went wrong.");
                
            default:
                _logger.LogError(result.Exception,
                    "Get Sales Order By ID Request: unhandled failure type {FailureType}.",
                    Enum.GetName(result.FailureType ?? FailureType.Exception));
                throw result.Exception
                      ?? new Exception(result.Message ?? "Failed to retrieve sales order, something went wrong.");
        }
        
        return result;
    }

    public async Task<Result<GetSalesOrderDto>> UpdateSalesOrderAsync(Guid id, UpdateSalesOrderDto salesOrder, CancellationToken cancellationToken = default)
    {
        Result<GetSalesOrderDto> result;

        try
        {
            result = await _salesOrderService.UpdateSalesOrderAsync(id, salesOrder, cancellationToken);
        }
        catch (Exception e)
        {
            _logger.LogError(e, "Update Sales Order Request: unhandled exception.");
            throw;
        }

        if (result.IsSuccess)
        {
            _logger.LogInformation("Update Sales Order Request: sales order {Id} successfully updated.",
                result.Value?.Id);
        }
        else switch (result.FailureType)
        {
            case FailureType.Validation:
            
            case FailureType.NotFound:
            
            case FailureType.BusinessLogic:
                _logger.LogInformation(result.Exception,
                    "Update Sales Order Request: {Message}",
                    result.Message ?? "Failed to update sales order.");
                break;

            case FailureType.Exception:
            
            case FailureType.Unexpected:
                _logger.LogError(result.Exception,
                    "Update Sales Order Request: unhandled exception.");
                throw result.Exception
                      ?? new Exception(result.Message ?? "Failed to update sales order, something went wrong.");
                
            default:
                _logger.LogError(result.Exception,
                    "Update Sales Order Request: unhandled failure type {FailureType}.",
                    Enum.GetName(result.FailureType ?? FailureType.Exception));
                throw result.Exception
                      ?? new Exception(result.Message ?? "Failed to update sales order, something went wrong.");
        }
        
        return result;
    }

    public async Task<Result<GetSalesOrderDto>> DeleteSalesOrderAsync(Guid id, CancellationToken cancellationToken = default)
    {
        Result<GetSalesOrderDto> result;

        try
        {
            result = await _salesOrderService.DeleteSalesOrderAsync(id, cancellationToken);
        }
        catch (Exception e)
        {
            _logger.LogError(e, "Delete Sales Order Request: unhandled exception.");
            throw;
        }

        if (result.IsSuccess)
        {
            _logger.LogInformation("Delete Sales Order Request: sales order {Id} successfully deleted.",
                result.Value?.Id);
        }
        else switch (result.FailureType)
        {
            case FailureType.Validation:
            
            case FailureType.NotFound:
            
            case FailureType.BusinessLogic:
                _logger.LogInformation(result.Exception,
                    "Delete Sales Order Request: {Message}",
                    result.Message ?? "Failed to delete sales order.");
                break;

            case FailureType.Exception:
            
            case FailureType.Unexpected:
                _logger.LogError(result.Exception,
                    "Delete Sales Order Request: unhandled exception.");
                throw result.Exception
                      ?? new Exception(result.Message ?? "Failed to delete sales order, something went wrong.");
                
            default:
                _logger.LogError(result.Exception,
                    "Delete Sales Order Request: unhandled failure type {FailureType}.",
                    Enum.GetName(result.FailureType ?? FailureType.Exception));
                throw result.Exception
                      ?? new Exception(result.Message ?? "Failed to delete sales order, something went wrong.");
        }
        
        return result;
    }

    public async Task<Result<GetManySoDetailsDto>> GetSoDetailsAsync(Guid salesOrderId, SortDirection orderDirection = SortDirection.Ascending,
        CancellationToken cancellationToken = default)
    {
        Result<GetManySoDetailsDto> result;

        try
        {
            result = await _salesOrderService.GetSoDetailsAsync(
                salesOrderId,
                orderDirection,
                cancellationToken);
        }
        catch (Exception e)
        {
            _logger.LogError(e, "Get Many SO Details Request: unhandled exception.");
            throw;
        }

        if (result.IsSuccess)
        {
            _logger.LogInformation("Get Many SO Details Request: {Count} SO details successfully fetched.",
                result.Value?.SoDetails?.Count);
        }
        else switch (result.FailureType)
        {
            case FailureType.Validation:
            
            case FailureType.NotFound:
            
            case FailureType.BusinessLogic:
                _logger.LogInformation(result.Exception,
                    "Get Many SO Details Request: {Message}",
                    result.Message ?? "Failed to retrieve sales orders.");
                break;

            case FailureType.Exception:
            
            case FailureType.Unexpected:
                _logger.LogError(result.Exception,
                    "Get Many SO Details Request: Unhandled exception.");
                throw result.Exception
                      ?? new Exception(result.Message ?? "Failed to retrieve SO details, something went wrong.");

            default:
                _logger.LogError(result.Exception,
                    "Get Many SO Details Request: Unhandled failure type {FailureType}.",
                    Enum.GetName(result.FailureType ?? FailureType.Exception));
                throw result.Exception
                      ?? new Exception(result.Message ?? "Failed to retrieve SO details, something went wrong.");
        }
        
        return result;
    }

    public async Task<Result<GetSoDetailDto>> AddSoDetailAsync(Guid salesOrderId, CreateSoDetailDto soDetail, CancellationToken cancellationToken = default)
    {
        Result<GetSoDetailDto> result;

        try
        {
            result = await _salesOrderService.AddSoDetailAsync(salesOrderId, soDetail, cancellationToken);
        }
        catch (Exception e)
        {
            _logger.LogError(e, "Create SO Detail Request: unhandled exception.");
            throw;
        }

        if (result.IsSuccess)
        {
            _logger.LogInformation("Create SO Detail Request: SO detail {Id} successfully created.",
                result.Value?.Id);
        }
        else switch (result.FailureType)
        {
            case FailureType.Validation:
            
            case FailureType.NotFound:
            
            case FailureType.BusinessLogic:
                _logger.LogInformation(result.Exception,
                    "Create SO Detail Request: {Message}",
                    result.Message ?? "Failed to create SO detail.");
                break;
            
            case FailureType.Exception:

            case FailureType.Unexpected:
                _logger.LogError(result.Exception,
                    "Create SO Detail Request: unhandled exception.");
                throw result.Exception
                      ?? new Exception(result.Message ?? "Failed to create SO detail, something went wrong.");
                
            default:
                _logger.LogError(result.Exception,
                    "Create SO Detail Request: unhandled failure type {FailureType}.",
                    Enum.GetName(result.FailureType ?? FailureType.Exception));
                throw result.Exception
                      ?? new Exception(result.Message ?? "Failed to create SO detail, something went wrong.");
        }
    
        return result;
    }

    public async Task<Result<GetSoDetailDto>> GetSoDetailByIdAsync(Guid id, CancellationToken cancellationToken = default)
    {
        Result<GetSoDetailDto> result;

        try
        {
            result = await _salesOrderService.GetSoDetailByIdAsync(id, cancellationToken);
        }
        catch (Exception e)
        {
            _logger.LogError(e, "Get SO Detail By ID Request: unhandled exception.");
            throw;
        }

        if (result.IsSuccess)
        {
            _logger.LogInformation("Get SO Detail By ID Request: SO detail {Id} successfully fetched.",
                result.Value?.Id);
        }
        else switch (result.FailureType)
        {
            case FailureType.Validation:
            
            case FailureType.NotFound:
            
            case FailureType.BusinessLogic:
                _logger.LogInformation(result.Exception,
                    "Get SO Detail By ID Request: {Message}",
                    result.Message ?? "Failed to retrieve SO detail.");
                break;
            
            case FailureType.Exception:
            
            case FailureType.Unexpected:
                _logger.LogError(result.Exception,
                    "Get SO Detail By ID Request: unhandled exception.");
                throw result.Exception
                      ?? new Exception(result.Message ?? "Failed to retrieve PO detail, something went wrong.");
                
            default:
                _logger.LogError(result.Exception,
                    "Get SO Detail By ID Request: unhandled failure type {FailureType}.",
                    Enum.GetName(result.FailureType ?? FailureType.Exception));
                throw result.Exception
                      ?? new Exception(result.Message ?? "Failed to retrieve SO detail, something went wrong.");
        }
        
        return result;
    }

    public async Task<Result<GetSoDetailDto>> UpdateSoDetailAsync(Guid id, UpdateSoDetailDto soDetail, CancellationToken cancellationToken = default)
    {
        Result<GetSoDetailDto> result;

        try
        {
            result = await _salesOrderService.UpdateSoDetailAsync(id, soDetail, cancellationToken);
        }
        catch (Exception e)
        {
            _logger.LogError(e, "Update SO Detail Request: unhandled exception.");
            throw;
        }

        if (result.IsSuccess)
        {
            _logger.LogInformation("Update SO Detail Request: SO detail {Id} successfully updated.",
                result.Value?.Id);
        }
        else switch (result.FailureType)
        {
            case FailureType.Validation:
            
            case FailureType.NotFound:
            
            case FailureType.BusinessLogic:
                _logger.LogInformation(result.Exception,
                    "Update SO Detail Request: {Message}",
                    result.Message ?? "Failed to update SO detail.");
                break;
            
            case FailureType.Exception:

            case FailureType.Unexpected:
                _logger.LogError(result.Exception,
                    "Update SO Detail Request: unhandled exception.");
                throw result.Exception
                      ?? new Exception(result.Message ?? "Failed to update SO detail, something went wrong.");
                
            default:
                _logger.LogError(result.Exception,
                    "Update SO Detail Request: unhandled failure type {FailureType}.",
                    Enum.GetName(result.FailureType ?? FailureType.Exception));
                throw result.Exception
                      ?? new Exception(result.Message ?? "Failed to update SO detail, something went wrong.");
        }
        
        return result;
    }

    public async Task<Result<GetSoDetailDto>> RemoveSoDetailAsync(Guid id, CancellationToken cancellationToken = default)
    {
        Result<GetSoDetailDto> result;

        try
        {
            result = await _salesOrderService.RemoveSoDetailAsync(id, cancellationToken);
        }
        catch (Exception e)
        {
            _logger.LogError(e, "Delete SO Detail Request: unhandled exception.");
            throw;
        }

        if (result.IsSuccess)
        {
            _logger.LogInformation("Delete SO Detail Request: SO detail {Id} successfully deleted.",
                result.Value?.Id);
        }
        else switch (result.FailureType)
        {
            case FailureType.Validation:
            
            case FailureType.NotFound:
            
            case FailureType.BusinessLogic:
                _logger.LogInformation(result.Exception,
                    "Delete SO Detail Request: {Message}",
                    result.Message ?? "Failed to delete SO detail.");
                break;
            
            case FailureType.Exception:
            
            case FailureType.Unexpected:
                _logger.LogError(result.Exception,
                    "Delete SO Detail Request: unhandled exception.");
                throw result.Exception
                      ?? new Exception(result.Message ?? "Failed to delete SO detail, something went wrong.");
                
            default:
                _logger.LogError(result.Exception,
                    "Delete SO Detail Request: unhandled failure type {FailureType}.",
                    Enum.GetName(result.FailureType ?? FailureType.Exception));
                throw result.Exception
                      ?? new Exception(result.Message ?? "Failed to delete SO detail, something went wrong.");
        }
        
        return result;
    }

    public async Task<Result<bool>> ExportSoDetailsAsCsvAsync(Guid salesOrderId, Stream destination, CancellationToken cancellationToken = default)
    {
        Result<bool> result;

        try
        {
            result = await _salesOrderService.ExportSoDetailsAsCsvAsync(salesOrderId, destination, cancellationToken);
        }
        catch (Exception e)
        {
            _logger.LogError(e, "Export SO Details As CSV Request: unhandled exception.");
            throw;
        }

        if (result.IsSuccess)
        {
            _logger.LogInformation("Export SO Details As CSV Request: SO details successfully exported.");
        }
        else switch (result.FailureType)
        {
            case FailureType.Validation:
            
            case FailureType.NotFound:
            
            case FailureType.BusinessLogic:
                _logger.LogInformation(result.Exception,
                    "Export SO Details As CSV Request: {Message}",
                    result.Message ?? "Failed to export SO details.");
                break;
            
            case FailureType.Exception:
            
            case FailureType.Unexpected:
                _logger.LogError(result.Exception,
                    "Export SO Details As CSV Request: unhandled exception.");
                throw result.Exception
                      ?? new Exception(result.Message ?? "Failed to export SO details, something went wrong.");
                
            default:
                _logger.LogError(result.Exception,
                    "Export SO Details As CSV Request: unhandled failure type {FailureType}.",
                    Enum.GetName(result.FailureType ?? FailureType.Exception));
                throw result.Exception
                      ?? new Exception(result.Message ?? "Failed to export SO details, something went wrong.");
        }
        
        return result;
    }
}