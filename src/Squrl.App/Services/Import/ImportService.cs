using FluentValidation;
using Microsoft.Data.Sqlite;
using Microsoft.EntityFrameworkCore;
using Squrl.App.Data;
using Squrl.App.Features.Items.DTOs;
using Squrl.App.Features.Suppliers.DTOs;
using Squrl.App.Features.UnitOfMeasures.DTOs;
using Squrl.App.Infrastructure.DecimalFormatter;
using Squrl.App.Models;
using Squrl.App.Services.Alert;
using Squrl.App.Services.Settings;

namespace Squrl.App.Services.Import;

public sealed partial class ImportService : IImportService
{
    private readonly DataContext _dataContext;
    private readonly ISettingsService _settingsService;
    private readonly IAlertService _alertService;
    private readonly IDecimalFormatter _decimalFormatter;
    private readonly IValidator<CreateItemDto> _createItemValidator;
    private readonly IValidator<CreateUomDto> _createUomValidator;
    private readonly IValidator<CreateSupplierDto> _createSupplierValidator;

    public ImportService(DataContext dataContext,
        ISettingsService settingsService,
        IAlertService alertService,
        IDecimalFormatter decimalFormatter,
        IValidator<CreateItemDto> createItemValidator,
        IValidator<CreateUomDto> createUomValidator,
        IValidator<CreateSupplierDto> createSupplierValidator)
    {
        _dataContext = dataContext;
        _settingsService = settingsService;
        _alertService = alertService;
        _decimalFormatter = decimalFormatter;
        _createItemValidator = createItemValidator;
        _createUomValidator = createUomValidator;
        _createSupplierValidator = createSupplierValidator;
    }
    
    private static bool IsUniqueConstraintViolation(DbUpdateException e)
    {
        if (e.InnerException is not SqliteException sqliteException)
        {
            return false;
        }

        return sqliteException.SqliteExtendedErrorCode is
            2067 or // SQLITE_CONSTRAINT_UNIQUE
            1555;    // SQLITE_CONSTRAINT_PRIMARYKEY
    }
    
    private async Task AlertHelper(Item item, CancellationToken cancellationToken)
    {
        if (item.LowQuantityAlertThreshold is not { } threshold)
        {
            return;
        }

        if (item.Quantity > threshold)
        {
            return;
        }

        await _alertService.SetAlertAsync(
            item.Id,
            $"Low stock warning for item {item.Name}. " +
            $"Current stock of {_decimalFormatter.FormatQuantity(item.Quantity)} " +
            $"is on or below the threshold of {_decimalFormatter
                .RemoveTrailingZeros(threshold)}.",
            cancellationToken);
    }
}