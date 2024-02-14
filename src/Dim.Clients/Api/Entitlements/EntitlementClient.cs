using Dim.Clients.Extensions;
using Dim.Clients.Token;
using System.Net.Http.Json;

namespace Dim.Clients.Api.Entitlements;

public class EntitlementClient(ITokenService tokenService) : IEntitlementClient
{
    public async Task AssignEntitlements(AuthSettings authSettings, Guid subAccountId, CancellationToken cancellationToken)
    {
        var client = await tokenService.GetAuthorizedClient<EntitlementClient>(authSettings, cancellationToken).ConfigureAwait(false);
        var data = new CreateSubAccountRequest(
                new List<SubaccountServicePlan>
                {
                    new(Enumerable.Repeat(new AssignmentInfo(true, null, subAccountId), 1), "cis", "local"),
                    new(Enumerable.Repeat(new AssignmentInfo(true, null, subAccountId), 1), "decentralized-identity-management-app", "standard"),
                    new(Enumerable.Repeat(new AssignmentInfo(null, 1, subAccountId), 1), "decentralized-identity-management", "standard"),
                    new(Enumerable.Repeat(new AssignmentInfo(true, null, subAccountId), 1), "auditlog-viewer", "free")
                }
            );

        await client.PutAsJsonAsync("/entitlements/v1/subaccountServicePlans", data, JsonSerializerExtensions.Options, cancellationToken)
            .CatchingIntoServiceExceptionFor("assign-entitlements", HttpAsyncResponseMessageExtension.RecoverOptions.INFRASTRUCTURE).ConfigureAwait(false);
    }
}