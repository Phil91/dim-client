using System.Text.Json.Serialization;

namespace Dim.Clients.Api.Cf;

public record AddSpaceRoleToUserRequest(
    [property: JsonPropertyName("type")] string Type,
    [property: JsonPropertyName("relationships")] SpaceRoleRelationship Relationship
);

public record SpaceRoleRelationship(
    [property: JsonPropertyName("user")] RelationshipUser User,
    [property: JsonPropertyName("space")] SpaceRoleSpace Space
);

public record RelationshipUser(
    [property: JsonPropertyName("data")] UserData Data
);

public record UserData(
    [property: JsonPropertyName("username")] string Username,
    [property: JsonPropertyName("origin")] string Origin
);

public record SpaceRoleSpace(
    [property: JsonPropertyName("data")] SpaceRoleData Data
);

public record SpaceRoleData(
    [property: JsonPropertyName("guid")] Guid Id
);
