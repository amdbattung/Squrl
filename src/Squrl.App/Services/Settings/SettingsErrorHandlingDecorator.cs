using Squrl.App.Common;
using Squrl.App.Enums;

namespace Squrl.App.Services.Settings;

public sealed class SettingsErrorHandlingDecorator : ISettingsService
{
    private readonly ISettingsService _settingsService;

    public UserSettings Current => _settingsService.Current;

    public event EventHandler<UserSettings>? SettingsChanged
    {
        add => _settingsService.SettingsChanged += value;
        remove => _settingsService.SettingsChanged -= value;
    }
    
    public SettingsErrorHandlingDecorator(ISettingsService settingsService)
    {
        _settingsService = settingsService;
    }

    public async Task<Result> UpdateAsync(UserSettings settings, CancellationToken cancellationToken = default)
    {
        try
        {
            Result result = await _settingsService
                .UpdateAsync(settings, cancellationToken);
            
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
            return Result
                .Fail("Failed to update settings, something went wrong.")
                .WithFailureType(FailureType.Unexpected);
        }
    }
}