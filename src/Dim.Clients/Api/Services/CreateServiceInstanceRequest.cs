using System.Text.Json.Serialization;

namespace Dim.Clients.Api.Services;

public record CreateServiceInstanceRequest(
    [property: JsonPropertyName("name")] string Name,
    [property: JsonPropertyName("service_offering_name")] string ServiceOfferingName,
    [property: JsonPropertyName("service_plan_name")] string ServicePlanName,
    [property: JsonPropertyName("parameters")] Dictionary<string, string> Parameters
);

public record CreateServiceInstanceResponse(
    [property: JsonPropertyName("id")] string Id,
    [property: JsonPropertyName("name")] string Name
);

public record CreateServiceBindingRequest(
    [property: JsonPropertyName("name")] string Name,
    [property: JsonPropertyName("service_instance_id")] string ServiceInstanceId
);

public record CreateServiceBindingResponse(
    [property: JsonPropertyName("id")] string Id,
    [property: JsonPropertyName("name")] string Name
);

public record GetBindingResponse(
    [property: JsonPropertyName("items")] IEnumerable<BindingItem> Items
);

public record BindingItem(
    [property: JsonPropertyName("name")] string Name,
    [property: JsonPropertyName("service_instance_id")] Guid ServiceInstanceId,
    [property: JsonPropertyName("credentials")] BindingCredentials Credentials
);

public record BindingCredentials(
    [property: JsonPropertyName("endpoints")] GetBindingEndpoints Endpoints,
    [property: JsonPropertyName("grant_type")] string GrantType,
    [property: JsonPropertyName("uaa")] GetBindingUaa Uaa
);

public record GetBindingUaa(
    [property: JsonPropertyName("clientid")] string ClientId,
    [property: JsonPropertyName("clientsecret")] string ClientSecret
);

public record GetBindingEndpoints(
    [property: JsonPropertyName("provisioning_service_url")] string ProvisioningServiceUrl,
    [property: JsonPropertyName("saas_registry_service_url")] string SaasRegistryServiceUrl
);
