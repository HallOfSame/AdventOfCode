using System.Text.Json;
using System.Text.Json.Serialization;
using Helpers.Interfaces;
using Helpers.Maps;

namespace Helpers.Structure;

public class JsonStateCopier : IStateCopier
{
    private readonly JsonSerializerOptions options = new()
    {
        ReferenceHandler = ReferenceHandler.Preserve,
        Converters =
        {
            new CoordinateConverter()
        }
    };

    public TState Copy<TState>(TState state)
    {
        var serialized = JsonSerializer.Serialize(state, options);
        return JsonSerializer.Deserialize<TState>(serialized, options)!;
    }
}