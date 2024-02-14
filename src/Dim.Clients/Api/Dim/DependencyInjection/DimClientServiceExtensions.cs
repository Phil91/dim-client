using Dim.Clients.Extensions;
using Microsoft.Extensions.DependencyInjection;

namespace Dim.Clients.Api.Dim.DependencyInjection;

public static class DimClientServiceExtensions
{
    public static IServiceCollection AddDimClient(this IServiceCollection services)
    {
        services
            .AddCustomHttpClientWithAuthentication<DimClient>(null, null)
            .AddTransient<IDimClient, DimClient>();

        return services;
    }
}
