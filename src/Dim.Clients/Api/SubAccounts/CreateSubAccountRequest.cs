using System.Text.Json.Serialization;

namespace Dim.Clients.Api.SubAccounts;

public record CreateSubAccountRequest(
    [property: JsonPropertyName("betaEnabled")] bool BetaEnabled,
    [property: JsonPropertyName("description")] string Description,
    [property: JsonPropertyName("displayName")] string DisplayName,
    [property: JsonPropertyName("labels")] Dictionary<string, IEnumerable<string>> Labels,
    [property: JsonPropertyName("origin")] string Origin,
    [property: JsonPropertyName("parentGUID")] Guid ParentId,
    [property: JsonPropertyName("region")] string Region,
    [property: JsonPropertyName("subaccountAdmins")] IEnumerable<string> SubaccountAdmins,
    [property: JsonPropertyName("subdomain")] string Subdomain,
    [property: JsonPropertyName("usedForProduction")] UsedForProduction UsedForProduction
);

public record CreateSubaccountResponse(
    [property: JsonPropertyName("guid")] Guid Id
);

public record ServiceManagementBindingResponse(
    [property: JsonPropertyName("clientid")] string ClientId,
    [property: JsonPropertyName("clientsecret")] string ClientSecret,
    [property: JsonPropertyName("sm_url")] string SmUrl,
    [property: JsonPropertyName("url")] string Url
);
