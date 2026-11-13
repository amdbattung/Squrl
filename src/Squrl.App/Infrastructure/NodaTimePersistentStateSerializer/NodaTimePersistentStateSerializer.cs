using System.Buffers;
using System.Text.Json;
using Microsoft.AspNetCore.Components;
using NodaTime;
using NodaTime.Serialization.SystemTextJson;

namespace Squrl.App.Infrastructure.NodaTimePersistentStateSerializer;

public sealed class NodaTimePersistentStateSerializer<T> : PersistentComponentStateSerializer<T>
{
    private readonly JsonSerializerOptions _options;

    public NodaTimePersistentStateSerializer()
    {
        _options = new JsonSerializerOptions();
        _options.ConfigureForNodaTime(DateTimeZoneProviders.Tzdb);
    }

    public override void Persist(T value, IBufferWriter<byte> writer)
    {
        using Utf8JsonWriter jsonWriter = new Utf8JsonWriter(writer);

        JsonSerializer.Serialize(jsonWriter, value, _options);

        jsonWriter.Flush();
    }

    public override T Restore(ReadOnlySequence<byte> data)
    {
        return JsonSerializer.Deserialize<T>(
            data.ToArray(),
            _options) ?? throw new InvalidOperationException();
    }
}