using System.Text.Json;
using System.Text.Json.Serialization;

namespace Dim.Clients.Extensions;

public static class JsonSerializerExtensions
{ 
    public static readonly JsonSerializerOptions Options = new()
    {
        PropertyNamingPolicy = JsonNamingPolicy.CamelCase,
        Converters =
        {
            new JsonStringEnumConverter(allowIntegerValues: false)
        },
        DefaultIgnoreCondition = JsonIgnoreCondition.WhenWritingNull
    };
}