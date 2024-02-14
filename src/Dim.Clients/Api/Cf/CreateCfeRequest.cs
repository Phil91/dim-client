using System.Text.Json.Serialization;

namespace Dim.Clients.Api.Cf;

public record CreateSpaceRequest(
    [property: JsonPropertyName("name")] string Name,
    [property: JsonPropertyName("relationships")] SpaceRelationship Relationship
);

public record SpaceRelationship(    
    [property: JsonPropertyName("organization")] SpaceOrganization Organization
);

public record SpaceOrganization(
    [property: JsonPropertyName("data")] SpaceRelationshipData Data
);

public record SpaceRelationshipData(
    [property: JsonPropertyName("guid")] Guid Id
);

public record CreateSpaceResponse(
    [property: JsonPropertyName("guid")] Guid Id
);
