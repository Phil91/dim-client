using Dim.Clients.Api.Services;

namespace Dim.Clients.Api.Provisioning;

public interface IProvisioningClient
{
    Task<Guid> CreateCloudFoundryEnvironment(string authUrl, BindingItem bindingData, string bpnl, string user, CancellationToken cancellationToken);
}