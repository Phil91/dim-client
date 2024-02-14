using Dim.Clients.Extensions;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Options;

namespace Dim.Clients.Api.Entitlements.DependencyInjection;

public static class EntitlementClientServiceExtensions
{
    public static IServiceCollection AddEntitlementClient(this IServiceCollection services, IConfigurationSection section)
    {
        services.AddOptions<EntitlementSettings>()
            .Bind(section)
            .ValidateOnStart();

        var sp = services.BuildServiceProvider();
        var settings = sp.GetRequiredService<IOptions<EntitlementSettings>>();
        services
            .AddCustomHttpClientWithAuthentication<EntitlementClient>(settings.Value.BaseUrl, null)
            .AddTransient<IEntitlementClient, EntitlementClient>();

        return services;
    }
}