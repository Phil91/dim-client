using Dim.Clients.Extensions;
using Dim.Clients.Token;
using System.Net.Http.Json;
using System.Text.Json;

namespace Dim.Clients.Api.Dim;

public class DimClient(ITokenService tokenService, IHttpClientFactory clientFactory) : IDimClient
{
    public async Task<CreateCompanyIdentityResponse> CreateCompanyIdentity(AuthSettings dimAuth, object baseUrl, string bpnl, CancellationToken cancellationToken)
    {
        var client = await tokenService.GetAuthorizedClient<DimClient>(dimAuth, cancellationToken).ConfigureAwait(false);
        var data = new CreateCompanyIdentityRequest(new Payload(
            new Network("web", "test"),
            "https://example.org/.well-known/did.json",
            Enumerable.Empty<object>(),
            new Key[]
            {
                new("SIGNING"),
                new("SIGNING_VC"),
            },
            "bpnl",
            bpnl));
        var result = await client.PostAsJsonAsync($"{baseUrl}/api/v2.0.0/companyIdentities", data, JsonSerializerExtensions.Options, cancellationToken)
            .CatchingIntoServiceExceptionFor("create-company-identity", HttpAsyncResponseMessageExtension.RecoverOptions.INFRASTRUCTURE).ConfigureAwait(false);
        try
        {
            var response = await result.Content
                .ReadFromJsonAsync<CreateCompanyIdentityResponse>(JsonSerializerExtensions.Options, cancellationToken)
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

    public async Task<JsonDocument> GetDidDocument(string url, CancellationToken cancellationToken)
    {
        var client = clientFactory.CreateClient("didDocumentDownload");
        using var result = await client.GetStreamAsync(url, cancellationToken).ConfigureAwait(false);
        var document = await JsonDocument.ParseAsync(result, cancellationToken: cancellationToken).ConfigureAwait(false);
        return document;
    }
}
