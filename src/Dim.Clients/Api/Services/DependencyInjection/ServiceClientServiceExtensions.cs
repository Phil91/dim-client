using Dim.Clients.Extensions;
using Microsoft.Extensions.DependencyInjection;

namespace Dim.Clients.Api.Services.DependencyInjection;

public static class ServiceClientServiceExtensions
{
    public static IServiceCollection AddServiceClient(this IServiceCollection services)
    {
        services
            .AddCustomHttpClientWithAuthentication<ServiceClient>(null, null)
            .AddTransient<IServiceClient, ServiceClient>();

        return services;
    }
}