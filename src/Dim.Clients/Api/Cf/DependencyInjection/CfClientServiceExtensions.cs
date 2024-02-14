using Dim.Clients.Extensions;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Options;

namespace Dim.Clients.Api.Cf.DependencyInjection;

public static class CfClientServiceExtensions
{
    public static IServiceCollection AddCfClient(this IServiceCollection services, IConfigurationSection section)
    {
        services.AddOptions<CfSettings>()
            .Bind(section)
            .ValidateOnStart();

        var sp = services.BuildServiceProvider();
        var settings = sp.GetRequiredService<IOptions<CfSettings>>();
        services
            .AddCustomHttpClientWithAuthentication<CfClient>(settings.Value.BaseUrl, settings.Value.TokenAddress)
            .AddTransient<ICfClient, CfClient>();

        return services;
    }
}
