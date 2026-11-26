using Microsoft.Extensions.Options;
using Squrl.App.Common;

namespace Squrl.App.Services.Settings;

public interface ISettingsService
{
    UserSettings Current { get; }
    event EventHandler<UserSettings>? SettingsChanged;
    Task<Result> UpdateAsync(UserSettings settings, CancellationToken cancellationToken = default);
}