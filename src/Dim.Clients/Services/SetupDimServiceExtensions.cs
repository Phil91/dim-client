using Dim.Clients.Api.Cf.DependencyInjection;
using Dim.Clients.Api.Dim.DependencyInjection;
using Dim.Clients.Api.Entitlements.DependencyInjection;
using Dim.Clients.Api.Provisioning.DependencyInjection;
using Dim.Clients.Api.Services.DependencyInjection;
using Dim.Clients.Api.SubAccounts.DependencyInjection;
using Dim.Clients.Api.Subscriptions.DependencyInjection;
using Dim.Clients.Token;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;

namespace Dim.Clients.Services;

public static class SetupDimServiceExtensions
{
    public static IServiceCollection AddSetupDim(this IServiceCollection services, IConfiguration config)
    {
        services.AddOptions<SetupDimSettings>()
            .Bind(config.GetSection("Dim"))
            .ValidateOnStart();

        services
            .AddTransient<ITokenService, TokenService>()
            .AddTransient<ISetupDimService, SetupDimService>()
            .AddSubAccountClient(config.GetSection("SubAccount"))
            .AddEntitlementClient(config.GetSection("Entitlement"))
            .AddServiceClient()
            .AddSubscriptionClient()
            .AddProvisioningClient()
            .AddCfClient(config.GetSection("Cf"))
            .AddDimClient();

        return services;
    }
}
