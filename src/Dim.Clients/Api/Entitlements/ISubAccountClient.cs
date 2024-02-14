using Dim.Clients.Token;

namespace Dim.Clients.Api.Entitlements;

public interface IEntitlementClient
{
    Task AssignEntitlements(AuthSettings authSettings, Guid subAccountId, CancellationToken cancellationToken);
}