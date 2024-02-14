using System.Text.Json.Serialization;

namespace Dim.Clients.Api.Dim;

public record CreateCompanyIdentityRequest(
    [property: JsonPropertyName("payload")] Payload Payload
);

public record Payload(
    [property: JsonPropertyName("network")] Network Network,
    [property: JsonPropertyName("hostingUrl")] string HostingUrl,
    [property: JsonPropertyName("services")] IEnumerable<object> Services,
    [property: JsonPropertyName("keys")] IEnumerable<Key> Keys,
    [property: JsonPropertyName("description")] string Description,
    [property: JsonPropertyName("name")] string Name
);

public record Key(
    [property: JsonPropertyName("type")] string Type
);

public record Network(
    [property: JsonPropertyName("didMethod")] string DidMethod,
    [property: JsonPropertyName("type")] string Type
);

public record CreateCompanyIdentityResponse(
    [property: JsonPropertyName("did")] string Did,
    [property: JsonPropertyName("companyId")] Guid CompanyId,
    [property: JsonPropertyName("downloadURL")] string DownloadUrl
);
