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

using Dim.Clients.Api.Cf.DependencyInjection;
using Dim.Clients.Extensions;
using Dim.Clients.Token;
using Microsoft.Extensions.Options;
using Org.Eclipse.TractusX.Portal.Backend.Framework.ErrorHandling;
using Org.Eclipse.TractusX.Portal.Backend.Framework.HttpClientExtensions;
using System.Net.Http.Json;
using System.Text.Json;

namespace Dim.Clients.Api.Cf;

public class CfClient : ICfClient
{
    private readonly CfSettings _settings;
    private readonly IBasicAuthTokenService _basicAuthTokenService;

    public CfClient(IBasicAuthTokenService basicAuthTokenService, IOptions<CfSettings> settings)
    {
        _basicAuthTokenService = basicAuthTokenService;
        _settings = settings.Value;
    }

    public async Task<Guid> CreateCloudFoundrySpace(string tenantName, CancellationToken cancellationToken)
    {
        var client = await _basicAuthTokenService.GetBasicAuthorizedLegacyClient<CfClient>(_settings, cancellationToken).ConfigureAwait(false);
        var cfEnvironmentId = await GetEnvironmentId(tenantName, cancellationToken, client).ConfigureAwait(false);
        var data = new CreateSpaceRequest(
            $"{tenantName}-space",
            new SpaceRelationship(new SpaceOrganization(new SpaceRelationshipData(cfEnvironmentId)))
        );

        var result = await client.PostAsJsonAsync("/v3/spaces", data, JsonSerializerExtensions.Options, cancellationToken)
            .CatchingIntoServiceExceptionFor("create-cfe", HttpAsyncResponseMessageExtension.RecoverOptions.ALLWAYS).ConfigureAwait(false);
        try
        {
            var response = await result.Content
                .ReadFromJsonAsync<CreateSpaceResponse>(JsonSerializerExtensions.Options, cancellationToken)
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

    private static async Task<Guid> GetEnvironmentId(string tenantName, CancellationToken cancellationToken, HttpClient client)
    {
        var environmentsResponse = await client.GetAsync("/v3/organizations", cancellationToken)
            .CatchingIntoServiceExceptionFor("get-organizations", HttpAsyncResponseMessageExtension.RecoverOptions.INFRASTRUCTURE);
        var environments = await environmentsResponse.Content
            .ReadFromJsonAsync<GetEnvironmentsResponse>(JsonSerializerExtensions.Options, cancellationToken)
            .ConfigureAwait(false);

        var tenantEnvironment = environments.Resources.Where(x => x.Name == tenantName);
        if (tenantEnvironment.Count() > 1)
        {
            throw new ConflictException($"There should only be one cf environment for tenant {tenantName}");
        }

        return tenantEnvironment.Single().EnvironmentId;
    }

    public async Task AddSpaceRoleToUser(string type, string user, Guid spaceId, CancellationToken cancellationToken)
    {
        var client = await _basicAuthTokenService.GetBasicAuthorizedLegacyClient<CfClient>(_settings, cancellationToken).ConfigureAwait(false);
        var data = new AddSpaceRoleToUserRequest(
            type,
            new SpaceRoleRelationship(
                new RelationshipUser(new UserData(user, "sap.ids")),
                new SpaceRoleSpace(new SpaceRoleData(spaceId))
            )
        );

        await client.PostAsJsonAsync("/v3/roles", data, JsonSerializerExtensions.Options, cancellationToken)
            .CatchingIntoServiceExceptionFor("add-space-roles", HttpAsyncResponseMessageExtension.RecoverOptions.INFRASTRUCTURE).ConfigureAwait(false);
    }

    public async Task<Guid> GetServicePlan(string servicePlanName, string servicePlanType, CancellationToken cancellationToken)
    {
        var client = await _basicAuthTokenService.GetBasicAuthorizedLegacyClient<CfClient>(_settings, cancellationToken).ConfigureAwait(false);
        var result = await client.GetAsync("/v3/service_plans", cancellationToken)
            .CatchingIntoServiceExceptionFor("get-service-plan", HttpAsyncResponseMessageExtension.RecoverOptions.ALLWAYS).ConfigureAwait(false);
        try
        {
            var response = await result.Content
                .ReadFromJsonAsync<ServicePlanResponse>(JsonSerializerExtensions.Options, cancellationToken)
                .ConfigureAwait(false);

            if (response == null)
            {
                throw new ServiceException("response should never be null here");
            }

            var servicePlans = response.Resources.Where(x => x.Name == servicePlanType &&
                                                   x.BrokerCatalog?.BrokerCatalogMetadata?.AutoSubscription?.AppName == servicePlanName);
            if (response == null || servicePlans.Count() != 1)
            {
                throw new ServiceException($"There must be exactly one service plan with name {servicePlanName} and type {servicePlanType}");
            }

            return servicePlans.Single().Id;
        }
        catch (JsonException je)
        {
            throw new ServiceException(je.Message);
        }
    }

    public async Task CreateDimServiceInstance(string tenantName, Guid spaceId, Guid servicePlanId, CancellationToken cancellationToken)
    {
        var client = await _basicAuthTokenService.GetBasicAuthorizedLegacyClient<CfClient>(_settings, cancellationToken).ConfigureAwait(false);
        var data = new CreateDimServiceInstance(
            "managed",
            $"{tenantName}-dim-instance",
            new DimRelationships(
                new DimSpace(new DimData(spaceId)),
                new DimServicePlan(new DimData(servicePlanId)))
        );

        await client.PostAsJsonAsync("/v3/service_instances", data, JsonSerializerExtensions.Options, cancellationToken)
            .CatchingIntoServiceExceptionFor("create-dim-si", HttpAsyncResponseMessageExtension.RecoverOptions.ALLWAYS).ConfigureAwait(false);
    }

    private async Task<Guid> GetServiceInstances(string tenantName, Guid? spaceId, CancellationToken cancellationToken)
    {
        var client = await _basicAuthTokenService.GetBasicAuthorizedLegacyClient<CfClient>(_settings, cancellationToken).ConfigureAwait(false);
        var result = await client.GetAsync("/v3/service_instances", cancellationToken)
            .CatchingIntoServiceExceptionFor("get-si", HttpAsyncResponseMessageExtension.RecoverOptions.INFRASTRUCTURE).ConfigureAwait(false);
        try
        {
            var response = await result.Content
                .ReadFromJsonAsync<ServiceInstanceResponse>(JsonSerializerExtensions.Options, cancellationToken)
                .ConfigureAwait(false);
            if (response == null)
            {
                throw new ServiceException("Response must not be null");
            }

            var name = $"{tenantName}-dim-instance";
            var resources = response.Resources.Where(x => x.Name == name && x.Type == "managed" && (spaceId == null || x.Relationships.Space.Data.Id == spaceId.Value));
            if (resources.Count() != 1)
            {
                throw new ServiceException($"There must be exactly one service instance");
            }

            return resources.Single().Id;
        }
        catch (JsonException je)
        {
            throw new ServiceException(je.Message);
        }
    }

    public async Task CreateServiceInstanceBindings(string tenantName, Guid spaceId, CancellationToken cancellationToken)
    {
        var serviceInstanceId = await GetServiceInstances(tenantName, spaceId, cancellationToken).ConfigureAwait(false);
        var client = await _basicAuthTokenService.GetBasicAuthorizedLegacyClient<CfClient>(_settings, cancellationToken).ConfigureAwait(false);
        var data = new CreateServiceCredentialBindingRequest(
            "key",
            $"{tenantName}-dim-key01",
            new ServiceCredentialRelationships(
                new DimServiceInstance(new DimData(serviceInstanceId)))
        );
        await client.PostAsJsonAsync("/v3/service_credential_bindings", data, JsonSerializerOptions.Default, cancellationToken)
            .CatchingIntoServiceExceptionFor("create-si-bindings", HttpAsyncResponseMessageExtension.RecoverOptions.ALLWAYS);
    }

    public async Task<Guid> GetServiceBinding(string tenantName, Guid spaceId, string bindingName, CancellationToken cancellationToken)
    {
        var client = await _basicAuthTokenService.GetBasicAuthorizedLegacyClient<CfClient>(_settings, cancellationToken).ConfigureAwait(false);
        var serviceInstanceId = await GetServiceInstances(tenantName, spaceId, cancellationToken).ConfigureAwait(false);
        var result = await client.GetAsync($"/v3/service_credential_bindings?names={bindingName}", cancellationToken)
            .CatchingIntoServiceExceptionFor("get-credential-binding", HttpAsyncResponseMessageExtension.RecoverOptions.INFRASTRUCTURE).ConfigureAwait(false);
        try
        {
            var response = await result.Content
                .ReadFromJsonAsync<ServiceCredentialBindingResponse>(JsonSerializerExtensions.Options, cancellationToken)
                .ConfigureAwait(false);
            if (response == null)
            {
                throw new ServiceException("Response must not be null");
            }

            var resources = response.Resources.Where(x => x.Relationships.ServiceInstance.Data.Id == serviceInstanceId);
            if (resources.Count() != 1)
            {
                throw new ServiceException($"There must be exactly one service credential binding");
            }

            return resources.Single().Id;
        }
        catch (JsonException je)
        {
            throw new ServiceException(je.Message);
        }
    }

    public async Task<ServiceCredentialBindingDetailResponse> GetServiceBindingDetails(Guid id, CancellationToken cancellationToken)
    {
        var client = await _basicAuthTokenService.GetBasicAuthorizedLegacyClient<CfClient>(_settings, cancellationToken).ConfigureAwait(false);
        var result = await client.GetAsync($"/v3/service_credential_bindings/{id}/details", cancellationToken)
            .CatchingIntoServiceExceptionFor("get-credential-binding-name", HttpAsyncResponseMessageExtension.RecoverOptions.INFRASTRUCTURE).ConfigureAwait(false);
        try
        {
            var response = await result.Content
                .ReadFromJsonAsync<ServiceCredentialBindingDetailResponse>(JsonSerializerExtensions.Options, cancellationToken)
                .ConfigureAwait(false);

            if (response == null)
            {
                throw new ServiceException($"There must be exactly one service instance");
            }

            return response;
        }
        catch (JsonException je)
        {
            throw new ServiceException(je.Message);
        }
    }
}
