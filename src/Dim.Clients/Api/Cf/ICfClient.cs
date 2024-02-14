namespace Dim.Clients.Api.Cf;

public interface ICfClient
{
    Task<Guid> CreateCloudFoundrySpace(string bpnl, Guid cfEnvironmentId, CancellationToken cancellationToken);
    Task AddSpaceRoleToUser(string type, string user, Guid spaceId, CancellationToken cancellationToken);
    Task<Guid> GetServicePlan(string servicePlanName, string servicePlanType, CancellationToken cancellationToken);
    Task CreateDimServiceInstance(Guid spaceId, Guid servicePlanId, CancellationToken cancellationToken);
    Task CreateServiceInstanceBindings(CancellationToken cancellationToken);
}