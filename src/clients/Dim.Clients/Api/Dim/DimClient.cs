/********************************************************************************
 * Copyright (c) 2024 BMW Group AG
 *
 * See the NOTICE file(s) distributed with this work for additional
 * information regarding copyright ownership.
 *
 * This program and the accompanying materials are made available under the
 * terms of the Apache License, Version 2.0 which is available at
 * https://www.apache.org/licenses/LICENSE-2.0.
 *
 * Unless required by applicable law or agreed to in writing, software
 * distributed under the License is distributed on an "AS IS" BASIS, WITHOUT
 * WARRANTIES OR CONDITIONS OF ANY KIND, either express or implied. See the
 * License for the specific language governing permissions and limitations
 * under the License.
 *
 * SPDX-License-Identifier: Apache-2.0
 ********************************************************************************/

using Dim.Clients.Extensions;
using Dim.Clients.Token;
using Org.Eclipse.TractusX.Portal.Backend.Framework.ErrorHandling;
using Org.Eclipse.TractusX.Portal.Backend.Framework.HttpClientExtensions;
using System.Net.Http.Json;
using System.Text.Json;

namespace Dim.Clients.Api.Dim;

public class DimClient : IDimClient
{
    private readonly IBasicAuthTokenService _basicAuthTokenService;
    private readonly IHttpClientFactory _clientFactory;

    public DimClient(IBasicAuthTokenService basicAuthTokenService, IHttpClientFactory clientFactory)
    {
        _basicAuthTokenService = basicAuthTokenService;
        _clientFactory = clientFactory;
    }

    public async Task<CreateCompanyIdentityResponse> CreateCompanyIdentity(BasicAuthSettings dimBasicAuth, string hostingUrl, string baseUrl, string tenantName, bool isIssuer, CancellationToken cancellationToken)
    {
        var client = await _basicAuthTokenService.GetBasicAuthorizedClient<DimClient>(dimBasicAuth, cancellationToken).ConfigureAwait(false);
        var data = new CreateCompanyIdentityRequest(new Payload(
            hostingUrl,
            new Bootstrap("Holder with IATP", "Holder IATP", Enumerable.Repeat("IATP", 1)),
            isIssuer ?
                Enumerable.Empty<Key>() :
                new Key[]
                {
                    new("SIGNING"),
                    new("SIGNING_VC"),
                }));
        var result = await client.PostAsJsonAsync($"{baseUrl}/api/v2.0.0/companyIdentities", data, JsonSerializerExtensions.Options, cancellationToken)
            .CatchingIntoServiceExceptionFor("create-company-identity", HttpAsyncResponseMessageExtension.RecoverOptions.INFRASTRUCTURE,
                async m =>
                {
                    var message = await m.Content.ReadAsStringAsync().ConfigureAwait(false);
                    return (false, message);
                }).ConfigureAwait(false);
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
        var client = _clientFactory.CreateClient("didDocumentDownload");
        using var result = await client.GetStreamAsync(url, cancellationToken).ConfigureAwait(false);
        var document = await JsonDocument.ParseAsync(result, cancellationToken: cancellationToken).ConfigureAwait(false);
        return document;
    }

    public async Task<string> CreateApplication(BasicAuthSettings dimAuth, string dimBaseUrl, string tenantName, CancellationToken cancellationToken)
    {
        var client = await _basicAuthTokenService.GetBasicAuthorizedClient<DimClient>(dimAuth, cancellationToken).ConfigureAwait(false);
        var data = new CreateApplicationRequest(new ApplicationPayload(
            "catena-x-portal",
            $"Catena-X Portal MIW for {tenantName}",
            6));
        var result = await client.PostAsJsonAsync($"{dimBaseUrl}/api/v2.0.0/applications", data, JsonSerializerExtensions.Options, cancellationToken)
            .CatchingIntoServiceExceptionFor("create-application", HttpAsyncResponseMessageExtension.RecoverOptions.INFRASTRUCTURE,
                async m =>
                {
                    var message = await m.Content.ReadAsStringAsync().ConfigureAwait(false);
                    return (false, message);
                }).ConfigureAwait(false);
        try
        {
            var response = await result.Content
                .ReadFromJsonAsync<CreateApplicationResponse>(JsonSerializerExtensions.Options, cancellationToken)
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

    public async Task<string> GetApplication(BasicAuthSettings dimAuth, string dimBaseUrl, string applicationId, CancellationToken cancellationToken)
    {
        var client = await _basicAuthTokenService.GetBasicAuthorizedClient<DimClient>(dimAuth, cancellationToken).ConfigureAwait(false);
        var result = await client.GetAsync($"{dimBaseUrl}/api/v2.0.0/applications/{applicationId}", cancellationToken)
            .CatchingIntoServiceExceptionFor("get-application", HttpAsyncResponseMessageExtension.RecoverOptions.INFRASTRUCTURE,
                async m =>
                {
                    var message = await m.Content.ReadAsStringAsync().ConfigureAwait(false);
                    return (false, message);
                }).ConfigureAwait(false);
        try
        {
            var response = await result.Content
                .ReadFromJsonAsync<ApplicationResponse>(JsonSerializerExtensions.Options, cancellationToken)
                .ConfigureAwait(false);
            if (response == null)
            {
                throw new ServiceException("Response must not be null");
            }

            return response.ApplicaitonKey;
        }
        catch (JsonException je)
        {
            throw new ServiceException(je.Message);
        }
    }

    public async Task AssignApplicationToCompany(BasicAuthSettings dimAuth, string dimBaseUrl, string applicationKey, Guid companyId, CancellationToken cancellationToken)
    {
        var client = await _basicAuthTokenService.GetBasicAuthorizedClient<DimClient>(dimAuth, cancellationToken).ConfigureAwait(false);
        var data = new CompanyIdentityPatch(new ApplicationUpdates(Enumerable.Repeat(applicationKey, 1)));
        await client.PatchAsJsonAsync($"{dimBaseUrl}/api/v2.0.0/companyIdentities/{companyId}", data, JsonSerializerExtensions.Options, cancellationToken)
            .CatchingIntoServiceExceptionFor("assign-application", HttpAsyncResponseMessageExtension.RecoverOptions.INFRASTRUCTURE,
                async m =>
                {
                    var message = await m.Content.ReadAsStringAsync().ConfigureAwait(false);
                    return (false, message);
                }).ConfigureAwait(false);
    }
}
