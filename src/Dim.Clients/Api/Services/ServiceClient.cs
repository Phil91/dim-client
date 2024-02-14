using Dim.Clients.Api.SubAccounts;
using Dim.Clients.Extensions;
using Dim.Clients.Token;
using System.Net.Http.Json;
using System.Text.Json;

namespace Dim.Clients.Api.Services;

public class ServiceClient(ITokenService tokenService) : IServiceClient
{
    public async Task<CreateServiceInstanceResponse> CreateServiceInstance(ServiceManagementBindingResponse saBinding, CancellationToken cancellationToken)
    {
        var serviceAuth = new AuthSettings
        {
            TokenAddress = $"{saBinding.Url}/oauth/token",
            ClientId = saBinding.ClientId,
            ClientSecret = saBinding.ClientSecret
        };
        var client = await tokenService.GetAuthorizedClient<ServiceClient>(serviceAuth, cancellationToken).ConfigureAwait(false);
        var directory = new CreateServiceInstanceRequest(
            "cis-local-instance",
            "cis",
            "local",
            new Dictionary<string, string>
            {
                { "grantType", "clientCredentials" }
            }
        );

        var result = await client.PostAsJsonAsync($"{saBinding.SmUrl}/v1/service_instances?async=false", directory, JsonSerializerExtensions.Options, cancellationToken)
            .CatchingIntoServiceExceptionFor("create-service-instance", HttpAsyncResponseMessageExtension.RecoverOptions.INFRASTRUCTURE).ConfigureAwait(false);
        try
        {
            var response = await result.Content
                .ReadFromJsonAsync<CreateServiceInstanceResponse>(JsonSerializerExtensions.Options, cancellationToken)
                .ConfigureAwait(false);
            if (response == null)
            {
                throw new ServiceException("Response was empty", true);
            }

            return response;
        }
        catch (JsonException je)
        {
            throw new ServiceException(je.Message);
        }
    }

    public async Task<CreateServiceBindingResponse> CreateServiceBinding(ServiceManagementBindingResponse saBinding, string serviceInstanceId, CancellationToken cancellationToken)
    {
        var serviceAuth = new AuthSettings
        {
            TokenAddress = $"{saBinding.Url}/oauth/token",
            ClientId = saBinding.ClientId,
            ClientSecret = saBinding.ClientSecret
        };
        var client = await tokenService.GetAuthorizedClient<ServiceClient>(serviceAuth, cancellationToken).ConfigureAwait(false);
        var data = new CreateServiceBindingRequest(
            "cis-local-binding",
            serviceInstanceId
        );

        var result = await client.PostAsJsonAsync($"{saBinding.SmUrl}/v1/service_bindings?async=false", data, JsonSerializerExtensions.Options, cancellationToken)
            .CatchingIntoServiceExceptionFor("create-service-binding", HttpAsyncResponseMessageExtension.RecoverOptions.INFRASTRUCTURE).ConfigureAwait(false);
        try
        {
            var response = await result.Content
                .ReadFromJsonAsync<CreateServiceBindingResponse>(JsonSerializerExtensions.Options, cancellationToken)
                .ConfigureAwait(false);
            if (response == null)
            {
                throw new ServiceException("Response was empty", true);
            }

            return response;
        }
        catch (JsonException je)
        {
            throw new ServiceException(je.Message);
        }
    }

    public async Task<BindingItem> GetServiceBinding(ServiceManagementBindingResponse saBinding, string serviceBindingName, CancellationToken cancellationToken)
    {
        var serviceAuth = new AuthSettings
        {
            TokenAddress = $"{saBinding.Url}/oauth/token",
            ClientId = saBinding.ClientId,
            ClientSecret = saBinding.ClientSecret
        };
        var client = await tokenService.GetAuthorizedClient<ServiceClient>(serviceAuth, cancellationToken).ConfigureAwait(false);
        var result = await client.GetAsync($"{saBinding.SmUrl}/v1/service_bindings?fieldQuery=name eq '{serviceBindingName}'", cancellationToken)
            .CatchingIntoServiceExceptionFor("get-service-binding", HttpAsyncResponseMessageExtension.RecoverOptions.INFRASTRUCTURE).ConfigureAwait(false);
        try
        {
            var response = await result.Content
                .ReadFromJsonAsync<GetBindingResponse>(JsonSerializerExtensions.Options, cancellationToken)
                .ConfigureAwait(false);
            if (response == null)
            {
                throw new ServiceException("Response was empty", true);
            }

            if (response.Items.Count() != 1)
            {
                throw new ServiceException($"There must be exactly one binding for {serviceBindingName}");
            }

            return response.Items.Single();
        }
        catch (JsonException je)
        {
            throw new ServiceException(je.Message);
        }
    }
}
