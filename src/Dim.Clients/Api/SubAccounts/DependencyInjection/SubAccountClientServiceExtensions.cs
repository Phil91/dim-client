using Dim.Clients.Extensions;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Options;

namespace Dim.Clients.Api.SubAccounts.DependencyInjection;

public static class SubAccountClientServiceExtensions
{
    public static IServiceCollection AddSubAccountClient(this IServiceCollection services, IConfigurationSection section)
    {
        services.AddOptions<SubAccountSettings>()
            .Bind(section)
            .ValidateOnStart();

        var sp = services.BuildServiceProvider();
        var settings = sp.GetRequiredService<IOptions<SubAccountSettings>>();
        services
            .AddCustomHttpClientWithAuthentication<SubAccountClient>(settings.Value.BaseUrl, null)
            .AddTransient<ISubAccountClient, SubAccountClient>();

        return services;
    }
}