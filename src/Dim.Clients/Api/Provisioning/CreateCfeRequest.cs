using System.Text.Json.Serialization;

namespace Dim.Clients.Api.Provisioning;

public record CreateCfeRequest(
    [property: JsonPropertyName("environmentType")] string EnvironmentType,
    [property: JsonPropertyName("parameters")] Dictionary<string, string> Parameters,
    [property: JsonPropertyName("landscapeLabel")] string LandscapeLabel,
    [property: JsonPropertyName("planName")] string PlanName,
    [property: JsonPropertyName("serviceName")] string ServiceName,
    [property: JsonPropertyName("user")] string User
);

public record CreateCfeResponse(
    [property: JsonPropertyName("id")] Guid Id
);