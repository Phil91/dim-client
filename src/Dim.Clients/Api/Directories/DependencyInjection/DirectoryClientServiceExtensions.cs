using Dim.Clients.Extensions;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Options;

namespace Dim.Clients.Api.Directories.DependencyInjection;

public static class DirectoryClientServiceExtensions
{
    public static IServiceCollection AddDirectoryClient(this IServiceCollection services, IConfigurationSection section)
    {
        services.AddOptions<DirectorySettings>()
            .Bind(section)
            .ValidateOnStart();

        var sp = services.BuildServiceProvider();
        var settings = sp.GetRequiredService<IOptions<DirectorySettings>>();
        services
            .AddCustomHttpClientWithAuthentication<DirectoryClient>(settings.Value.BaseUrl, settings.Value.TokenAddress)
            .AddTransient<IDirectoryClient, DirectoryClient>();

        return services;
    }
}