using Dim.Clients.Api.Services;
using Dim.Clients.Extensions;
using Dim.Clients.Token;
using System.Net.Http.Json;
using System.Text.Json;

namespace Dim.Clients.Api.Provisioning;

public class ProvisioningClient(ITokenService tokenService) : IProvisioningClient
{
    public async Task<Guid> CreateCloudFoundryEnvironment(string authUrl, BindingItem bindingData, string bpnl, string user, CancellationToken cancellationToken)
    {
        var authSettings = new AuthSettings
        {
            TokenAddress = $"{authUrl}/oauth/token",
            ClientId = bindingData.Credentials.Uaa.ClientId,
            ClientSecret = bindingData.Credentials.Uaa.ClientSecret
        };
        var client = await tokenService.GetAuthorizedClient<ProvisioningClient>(authSettings, cancellationToken).ConfigureAwait(false);
        var data = new CreateCfeRequest(
            "cloudfoundry",
            new Dictionary<string, string>
            {
                { "instance_name", bpnl }
            },
            "cf-eu10",
            "standard",
            "cloudfoundry",
            user
        );

        var result = await client.PostAsJsonAsync($"{bindingData.Credentials.Endpoints.ProvisioningServiceUrl}/provisioning/v1/environments", data, JsonSerializerExtensions.Options, cancellationToken)
            .CatchingIntoServiceExceptionFor("create-cf-env", HttpAsyncResponseMessageExtension.RecoverOptions.INFRASTRUCTURE).ConfigureAwait(false);
        try
        {
            var response = await result.Content
                .ReadFromJsonAsync<CreateCfeResponse>(JsonSerializerExtensions.Options, cancellationToken)
                .ConfigureAwait(false);
            if (response == null)
            {
                throw new ServiceException("Response was empty", true);
            }

            return response.Id;
        }
        catch (JsonException je)
        {
            throw new ServiceException(je.Message);
        }
    }
}
