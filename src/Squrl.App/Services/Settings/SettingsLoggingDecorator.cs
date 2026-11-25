using Squrl.App.Common;
using Squrl.App.Enums;

namespace Squrl.App.Services.Settings;

public sealed class SettingsLoggingDecorator : ISettingsService
{
    private readonly ISettingsService _settingsService;
    private readonly ILogger<SettingsLoggingDecorator> _logger;
    
    public UserSettings Current => _settingsService.Current;

    public event EventHandler<UserSettings>? SettingsChanged
    {
        add => _settingsService.SettingsChanged += value;
        remove => _settingsService.SettingsChanged -= value;
    }
    
    public SettingsLoggingDecorator(ISettingsService settingsService,
        ILogger<SettingsLoggingDecorator> logger)
    {
        _settingsService = settingsService;
        _logger = logger;
    }
    
    public async Task<Result> UpdateAsync(UserSettings settings, CancellationToken cancellationToken = default)
    {
        Result result;

        try
        {
            result = await _settingsService.UpdateAsync(settings, cancellationToken);
        }
        catch (Exception e)
        {
            _logger.LogError(e, "Update Settings Request: unhandled exception.");
            throw;
        }

        if (result.IsSuccess)
        {
            _logger.LogInformation("Update Settings Request: settings successfully updated.");
        }
        else switch (result.FailureType)
        {
            case FailureType.Validation:
            
            case FailureType.NotFound:
            
            case FailureType.BusinessLogic:
                _logger.LogInformation(result.Exception,
                    "Update Settings Request: {Message}",
                    result.Message ?? "Failed to update settings.");
                break;
            
            case FailureType.Exception:
            
            case FailureType.Unexpected:
                _logger.LogError(result.Exception,
                    "Update Settings Request: unhandled exception.");
                throw result.Exception
                      ?? new Exception(result.Message ?? "Failed to update settings, something went wrong.");
                
            default:
                _logger.LogError(result.Exception,
                    "Update Settings Request: unhandled failure type {FailureType}.",
                    Enum.GetName(result.FailureType ?? FailureType.Exception));
                throw result.Exception
                      ?? new Exception(result.Message ?? "Failed to update settings, something went wrong.");
        }
        
        return result;
    }
}