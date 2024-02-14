using System.Text.Json.Serialization;

namespace Dim.Clients.Api.Directories;

public record DirectoryRequest(
    [property: JsonPropertyName("description")] string Description,
    [property: JsonPropertyName("directoryAdmins")] IEnumerable<string> DirectoryAdmins,
    [property: JsonPropertyName("displayName")] string DisplayName,
    [property: JsonPropertyName("labels")] Dictionary<string, IEnumerable<string>> Labels
);

public record DirectoryResponse(
    [property: JsonPropertyName("guid")] Guid Id,
    [property: JsonPropertyName("subdomain")] string Subdomain
);
