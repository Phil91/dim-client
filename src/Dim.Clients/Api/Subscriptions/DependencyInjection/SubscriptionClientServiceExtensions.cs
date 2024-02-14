using Dim.Clients.Extensions;
using Microsoft.Extensions.DependencyInjection;

namespace Dim.Clients.Api.Subscriptions.DependencyInjection;

public static class SubscriptionClientServiceExtensions
{
    public static IServiceCollection AddSubscriptionClient(this IServiceCollection services)
    {
        services
            .AddCustomHttpClientWithAuthentication<SubscriptionClient>(null, null)
            .AddTransient<ISubscriptionClient, SubscriptionClient>();

        return services;
    }
}