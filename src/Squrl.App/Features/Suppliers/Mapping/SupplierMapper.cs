using System.Linq.Expressions;
using Squrl.App.Features.Suppliers.DTOs;
using Squrl.App.Models;

namespace Squrl.App.Features.Suppliers.Mapping;

public class SupplierMapper
{
    public static GetSupplierDto ToDto(Supplier supplier)
    {
        return new GetSupplierDto(
            supplier.Id,
            supplier.Name,
            supplier.Description
        );
    }
    
    public static Expression<Func<Supplier, GetSupplierDto>> ToDtoExpression
    {
        get
        {
            return supplier => new GetSupplierDto(
                supplier.Id,
                supplier.Name,
                supplier.Description
            );
        }
    }
}