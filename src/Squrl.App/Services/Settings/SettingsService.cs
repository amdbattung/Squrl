using System.Text.Json;
using FluentValidation;
using FluentValidation.Results;
using Squrl.App.Common;
using Squrl.App.Enums;

namespace Squrl.App.Services.Settings;

public sealed class SettingsService : ISettingsService
{
    private static readonly JsonSerializerOptions JsonOptions = new()
    {
        WriteIndented = true
    };

    private readonly string _filePath;
    private readonly SemaphoreSlim _lock = new(1, 1);
    private readonly InlineValidator<UserSettings> _settingsValidator;

    private UserSettings _current;

    public event EventHandler<UserSettings>? SettingsChanged;

    public UserSettings Current => Volatile.Read(ref _current);

    public SettingsService(string filePath)
    {
        _filePath = filePath;
        
        _settingsValidator = new InlineValidator<UserSettings>();

        _settingsValidator.RuleFor(x => x.CurrencySymbol)
            .NotEmpty()
            .WithName("Currency Symbol")
            .WithMessage("{PropertyName} is required.");

        _settingsValidator.RuleFor(x => x.ItemQuantityPrecision)
            .InclusiveBetween(0, 6)
            .WithName("Quantity Precision")
            .WithMessage("{PropertyName} must be between 0 and 6.");
        
        _current = Load();
    }

    private UserSettings Load()
    {
        if (!File.Exists(_filePath))
        {
            return new UserSettings();
        }

        string json = File.ReadAllText(_filePath);

        return JsonSerializer.Deserialize<UserSettings>(json)
               ?? new UserSettings();
    }

    public async Task<Result> UpdateAsync(UserSettings settings, CancellationToken cancellationToken = default)
    {
        ValidationResult validationResult = await _settingsValidator
            .ValidateAsync(settings, cancellationToken);
        
        if (!validationResult.IsValid)
        {
            return Result.Fail(validationResult.Errors
                    .Select(e => e.ErrorMessage)
                    .ToArray())
                .WithFailureType(FailureType.Validation);
        }

        await _lock.WaitAsync(cancellationToken);

        try
        {
            string json = JsonSerializer.Serialize(settings, JsonOptions);

            string tempFile = _filePath + ".tmp";

            await File.WriteAllTextAsync(
                tempFile,
                json,
                cancellationToken);

            File.Move(tempFile, _filePath, overwrite: true);

            Volatile.Write(ref _current, settings);
        }
        finally
        {
            _lock.Release();
        }

        SettingsChanged?.Invoke(this, settings);
        
        return Result.Ok();
    }
}