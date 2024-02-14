using Dim.Clients.Api.Services;
using Dim.Clients.Extensions;
using Dim.Clients.Token;
using System.Net.Http.Json;

namespace Dim.Clients.Api.Subscriptions;

public class SubscriptionClient(ITokenService tokenService) : ISubscriptionClient
{
    public async Task SubscribeApplication(string authUrl, BindingItem bindingData, string applicationName, string planName, CancellationToken cancellationToken)
    {
        var authSettings = new AuthSettings
        {
            TokenAddress = $"{authUrl}/oauth/token",
            ClientId = bindingData.Credentials.Uaa.ClientId,
            ClientSecret = bindingData.Credentials.Uaa.ClientSecret
        };
        var client = await tokenService.GetAuthorizedClient<SubscriptionClient>(authSettings, cancellationToken).ConfigureAwait(false);
        var data = new
        {
            planName = planName
        };

        await client.PostAsJsonAsync($"{bindingData.Credentials.Endpoints.SaasRegistryServiceUrl}/saas-manager/v1/applications/{applicationName}/subscription", data, JsonSerializerExtensions.Options, cancellationToken)
            .CatchingIntoServiceExceptionFor("subscribe-application", HttpAsyncResponseMessageExtension.RecoverOptions.INFRASTRUCTURE).ConfigureAwait(false);
    }
}
