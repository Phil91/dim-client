using Dim.Clients.Extensions;
using Microsoft.Extensions.DependencyInjection;

namespace Dim.Clients.Api.Provisioning.DependencyInjection;

public static class ProvisioningClientServiceExtensions
{
    public static IServiceCollection AddProvisioningClient(this IServiceCollection services)
    {
        services
            .AddCustomHttpClientWithAuthentication<ProvisioningClient>(null, null)
            .AddTransient<IProvisioningClient, ProvisioningClient>();

        return services;
    }
}