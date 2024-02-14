using System.Text.Json.Serialization;

namespace Dim.Clients.Api.Cf;

public record ServicePlanResponse(
    [property: JsonPropertyName("resources")] IEnumerable<ServicePlanResources> Resources
);

public record ServicePlanResources(
    [property: JsonPropertyName("guid")] Guid Id,
    [property: JsonPropertyName("name")] string Name,
    [property: JsonPropertyName("broker_catalog")] BrokerCatalog? BrokerCatalog
);

public record BrokerCatalog(
    [property: JsonPropertyName("id")] string Id,
    [property: JsonPropertyName("metadata")] BrokerCatalogMetadata? BrokerCatalogMetadata
);

public record BrokerCatalogMetadata(
    [property: JsonPropertyName("auto_subscription")] AutoSupscription? AutoSubscription
);

public record AutoSupscription(
    [property: JsonPropertyName("app_name")] string? AppName
);

public record ServiceInstanceResponse(
    [property: JsonPropertyName("resources")] IEnumerable<ServiceInstanceResource> Resources
);

public record ServiceInstanceResource(
    [property: JsonPropertyName("guid")] Guid Id,
    [property: JsonPropertyName("name")] string Name,
    [property: JsonPropertyName("type")] string Type
);

public record CreateServiceCredentialBindingRequest(
    [property: JsonPropertyName("type")] string Type,
    [property: JsonPropertyName("name")] string Name,
    [property: JsonPropertyName("relationships")] ServiceCredentialRelationships Relationships
);

public record ServiceCredentialRelationships(
    [property: JsonPropertyName("service_instance")] DimServiceInstance ServiceInstance
);

public record DimServiceInstance(
    [property: JsonPropertyName("data")] DimData Data
);

public record CreateDimServiceInstance(
    [property: JsonPropertyName("type")] string Type,
    [property: JsonPropertyName("name")] string Name,
    [property: JsonPropertyName("relationships")] DimRelationships Relationships
);

public record DimRelationships(
    [property: JsonPropertyName("space")] DimSpace Space,
    [property: JsonPropertyName("service_plan")] DimServicePlan ServicePlan
);

public record DimServicePlan(
    [property: JsonPropertyName("data")] DimData Data
);

public record DimSpace(
    [property: JsonPropertyName("data")] DimData Data
);

public record DimData(
    [property: JsonPropertyName("guid")] Guid Id
);

public record DimServiceInstanceRequest(
    [property: JsonPropertyName("guid")] Guid Id
);