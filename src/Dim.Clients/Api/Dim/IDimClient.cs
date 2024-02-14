using Dim.Clients.Token;
using System.Text.Json;

namespace Dim.Clients.Api.Dim;

public interface IDimClient
{
    Task<CreateCompanyIdentityResponse> CreateCompanyIdentity(AuthSettings dimAuth, object baseUrl, string bpnl, CancellationToken cancellationToken);
    Task<JsonDocument> GetDidDocument(string url, CancellationToken cancellationToken);
}
