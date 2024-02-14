using Dim.Clients.Extensions;
using Dim.Clients.Token;
using System.Net.Http.Json;
using System.Text.Json;

namespace Dim.Clients.Api.SubAccounts;

public class SubAccountClient(ITokenService tokenService)
    : ISubAccountClient
{
    public async Task<Guid> CreateSubaccount(AuthSettings authSettings, string adminMail, string bpnl, string companyName, Guid directoryId, CancellationToken cancellationToken)
    {
        var client = await tokenService.GetAuthorizedClient<SubAccountClient>(authSettings, cancellationToken).ConfigureAwait(false);
        var directory = new CreateSubAccountRequest(
            false,
            $"CX customer sub-account {companyName}",
            $"{bpnl}_{companyName}",
            new Dictionary<string, IEnumerable<string>>
            {
                { "cloud_management_service", new[] { "Created by API - Don't change it" } },
                { "bpnl", new[] { bpnl } },
                { "companyName", new[] { companyName } }
            },
            "API",
            directoryId,
            "eu10",
            Enumerable.Repeat(adminMail, 1),
            "test01",
            UsedForProduction.USED_FOR_PRODUCTION
        );

        var result = await client.PostAsJsonAsync("/accounts/v1/subaccounts", directory, JsonSerializerExtensions.Options, cancellationToken)
            .CatchingIntoServiceExceptionFor("create-subaccount", HttpAsyncResponseMessageExtension.RecoverOptions.INFRASTRUCTURE).ConfigureAwait(false);
        try
        {
            var response = await result.Content
                .ReadFromJsonAsync<CreateSubaccountResponse>(JsonSerializerExtensions.Options, cancellationToken)
                .ConfigureAwait(false);

            if (response == null)
            {
                throw new ServiceException("Response must not be null");
            }

            return response.Id;
        }
        catch (JsonException je)
        {
            throw new ServiceException(je.Message);
        }
    }

    public async Task<ServiceManagementBindingResponse> CreateServiceManagerBindings(AuthSettings authSettings, Guid subAccountId, CancellationToken cancellationToken)
    {
        var client = await tokenService.GetAuthorizedClient<SubAccountClient>(authSettings, cancellationToken).ConfigureAwait(false);
        var data = new
        {
            name = "accessServiceManager"
        };

        var result = await client.PostAsJsonAsync($"/accounts/v2/subaccounts/{subAccountId}/serviceManagerBindings", data, JsonSerializerExtensions.Options, cancellationToken)
            .CatchingIntoServiceExceptionFor("create-subaccount", HttpAsyncResponseMessageExtension.RecoverOptions.INFRASTRUCTURE).ConfigureAwait(false);
        try
        {
            var response = await result.Content
                .ReadFromJsonAsync<ServiceManagementBindingResponse>(JsonSerializerExtensions.Options, cancellationToken)
                .ConfigureAwait(false);

            if (response == null)
            {
                throw new ServiceException("Response must not be null");
            }

            return response;
        }
        catch (JsonException je)
        {
            throw new ServiceException(je.Message);
        }
    }

    public async Task<ServiceManagementBindingResponse> AssignEntitlements(AuthSettings authSettings, Guid subAccountId, CancellationToken cancellationToken)
    {
        var client = await tokenService.GetAuthorizedClient<SubAccountClient>(authSettings, cancellationToken).ConfigureAwait(false);
        var data = new
        {
            name = "accessServiceManager"
        };

        var result = await client.PostAsJsonAsync($"/accounts/v2/subaccounts/{subAccountId}/serviceManagerBindings", data, JsonSerializerExtensions.Options, cancellationToken)
            .CatchingIntoServiceExceptionFor("create-subaccount", HttpAsyncResponseMessageExtension.RecoverOptions.INFRASTRUCTURE).ConfigureAwait(false);
        try
        {
            var response = await result.Content
                .ReadFromJsonAsync<ServiceManagementBindingResponse>(JsonSerializerExtensions.Options, cancellationToken)
                .ConfigureAwait(false);

            if (response == null)
            {
                throw new ServiceException("Response must not be null");
            }

            return response;
        }
        catch (JsonException je)
        {
            throw new ServiceException(je.Message);
        }
    }
}