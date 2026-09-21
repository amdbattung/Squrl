using System.Linq.Expressions;
using Squrl.App.Features.SalesOrders.DTOs;
using Squrl.App.Models;

namespace Squrl.App.Features.SalesOrders.Mapping;

public class SalesOrderMapper
{
    public static GetSalesOrderDto ToDto(SalesOrder salesOrder)
    {
        return new GetSalesOrderDto(
            salesOrder.Id,
            salesOrder.Customer,
            salesOrder.InvoiceNumber,
            salesOrder.DateOrdered
        );
    }
    
    public static Expression<Func<SalesOrder, GetSalesOrderDto>> ToDtoExpression
    {
        get
        {
            return salesOrder => new GetSalesOrderDto(
                salesOrder.Id,
                salesOrder.Customer,
                salesOrder.InvoiceNumber,
                salesOrder.DateOrdered
            );
        }
    }
}