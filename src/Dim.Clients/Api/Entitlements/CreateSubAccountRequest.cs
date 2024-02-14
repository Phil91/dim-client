using System.Text.Json.Serialization;

namespace Dim.Clients.Api.Entitlements;

public record CreateSubAccountRequest(
    [property: JsonPropertyName("subaccountServicePlans")] IEnumerable<SubaccountServicePlan> SubaccountServicePlans
);

public record SubaccountServicePlan(
    [property: JsonPropertyName("assignmentInfo")] IEnumerable<AssignmentInfo> AssignmentInfo,
    [property: JsonPropertyName("serviceName")] string ServiceName,
    [property: JsonPropertyName("servicePlanName")] string ServicePlanName
);

public record AssignmentInfo(
    [property: JsonPropertyName("enable")] bool? Enabled,
    [property: JsonPropertyName("amount")] int? Amount,
    [property: JsonPropertyName("subaccountGUID")] Guid SubaccountId
);
