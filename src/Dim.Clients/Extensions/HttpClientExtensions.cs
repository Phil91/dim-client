using Microsoft.Extensions.DependencyInjection;

namespace Dim.Clients.Extensions;

public static class HttpClientExtensions
{
    public static IServiceCollection AddCustomHttpClientWithAuthentication<T>(this IServiceCollection services, string? baseAddress, string? authAddress) where T : class
    {
        services.AddHttpClient(typeof(T).Name, c =>
        {
            if (baseAddress != null)
            {
                c.BaseAddress = new Uri(baseAddress);
            }
        });

        services.AddHttpClient($"{typeof(T).Name}Auth", c =>
        {
            if (authAddress != null)
            {
                c.BaseAddress = new Uri(authAddress);
            }
        });
        return services;
    }
}
