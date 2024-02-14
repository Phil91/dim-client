using Dim.Clients.Api.Directories.DependencyInjection;
using Dim.Clients.Extensions;
using Dim.Clients.Token;
using Microsoft.Extensions.Options;
using System.Net.Http.Json;
using System.Text.Json;

namespace Dim.Clients.Api.Directories;

public class DirectoryClient(ITokenService tokenService, IOptions<DirectorySettings> settings)
    : IDirectoryClient
{
    private readonly DirectorySettings _settings = settings.Value;

    public async Task<Guid> CreateDirectory(string description, string bpnl, Guid parentId, CancellationToken cancellationToken)
    {
        var client = await tokenService.GetAuthorizedClient<DirectoryClient>(_settings, cancellationToken).ConfigureAwait(false);
        var directory = new DirectoryRequest(
            description,
            Enumerable.Repeat("phil.schneider@digitalnativesolutions.de", 1),
            bpnl,
            new Dictionary<string, IEnumerable<string>>()
            {
                { "cloud_management_service", new[] { "Created by API - Don't change it" } }
            }
        );

        var result = await client.PostAsJsonAsync($"/accounts/v1/directories?parentId={parentId}", directory, JsonSerializerExtensions.Options, cancellationToken)
            .CatchingIntoServiceExceptionFor("create-directory", HttpAsyncResponseMessageExtension.RecoverOptions.INFRASTRUCTURE).ConfigureAwait(false);
        try
        {
            var response = await result.Content
                .ReadFromJsonAsync<DirectoryResponse>(JsonSerializerExtensions.Options, cancellationToken)
                .ConfigureAwait(false);

            if (response == null)
            {
                throw new ServiceException("Directory response must not be null");
            }

            return response.Id;
        }
        catch (JsonException je)
        {
            throw new ServiceException(je.Message);
        }
    }
}
