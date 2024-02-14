using Dim.Clients.Api.Cf.DependencyInjection;
using Dim.Clients.Extensions;
using Dim.Clients.Token;
using Microsoft.Extensions.Options;
using System.Net.Http.Json;
using System.Text.Json;

namespace Dim.Clients.Api.Cf;

public class CfClient(ITokenService tokenService, IOptions<CfSettings> settings) : ICfClient
{
    private readonly CfSettings _settings = settings.Value;

    public async Task<Guid> CreateCloudFoundrySpace(string bpnl, Guid cfEnvironmentId, CancellationToken cancellationToken)
    {
        var client = await tokenService.GetAuthorizedLegacyClient<CfClient>(_settings, cancellationToken).ConfigureAwait(false);
        var data = new CreateSpaceRequest(
            $"{bpnl}-space",
            new SpaceRelationship(new SpaceOrganization(new SpaceRelationshipData(cfEnvironmentId)))
        );

        var result = await client.PostAsJsonAsync("/v3/spaces", data, JsonSerializerExtensions.Options, cancellationToken)
            .CatchingIntoServiceExceptionFor("create-cfe", HttpAsyncResponseMessageExtension.RecoverOptions.INFRASTRUCTURE).ConfigureAwait(false);
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

    public async Task AddSpaceRoleToUser(string type, string user, Guid spaceId, CancellationToken cancellationToken)
    {
        var client = await tokenService.GetAuthorizedLegacyClient<CfClient>(_settings, cancellationToken).ConfigureAwait(false);
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
        var client = await tokenService.GetAuthorizedLegacyClient<CfClient>(_settings, cancellationToken).ConfigureAwait(false);
        var result = await client.GetAsync("/v3/service_plans", cancellationToken)
            .CatchingIntoServiceExceptionFor("get-service-plan", HttpAsyncResponseMessageExtension.RecoverOptions.INFRASTRUCTURE).ConfigureAwait(false);
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

    public async Task CreateDimServiceInstance(Guid spaceId, Guid servicePlanId, CancellationToken cancellationToken)
    {
        var client = await tokenService.GetAuthorizedLegacyClient<CfClient>(_settings, cancellationToken).ConfigureAwait(false);
        var data = new CreateDimServiceInstance(
            "managed",
            "dim-instance", //TODO: clarify
            new DimRelationships(
                new DimSpace(new DimData(spaceId)),
                new DimServicePlan(new DimData(servicePlanId)))
        );
        
        await client.PostAsJsonAsync("/v3/service_instances", data, JsonSerializerExtensions.Options, cancellationToken)
            .CatchingIntoServiceExceptionFor("create-dim-si", HttpAsyncResponseMessageExtension.RecoverOptions.INFRASTRUCTURE).ConfigureAwait(false);
    }

    public async Task<Guid> GetServiceInstances(CancellationToken cancellationToken)
    {
        var client = await tokenService.GetAuthorizedLegacyClient<CfClient>(_settings, cancellationToken).ConfigureAwait(false);
        var result = await client.GetAsync("/v3/service_instances", cancellationToken)
            .CatchingIntoServiceExceptionFor("get-si", HttpAsyncResponseMessageExtension.RecoverOptions.INFRASTRUCTURE).ConfigureAwait(false);
        try
        {
            var response = await result.Content
                .ReadFromJsonAsync<ServiceInstanceResponse>(JsonSerializerExtensions.Options, cancellationToken)
                .ConfigureAwait(false);

            var resources = response.Resources.Where(x => x is { Name: "dim-instance", Type: "managed" });
            if (response == null || resources.Count() != 1)
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
    
    public async Task CreateServiceInstanceBindings(CancellationToken cancellationToken)
    {
        var serviceInstanceId = await GetServiceInstances(cancellationToken).ConfigureAwait(false);
        var client = await tokenService.GetAuthorizedLegacyClient<CfClient>(_settings, cancellationToken).ConfigureAwait(false);
        var data = new CreateServiceCredentialBindingRequest(
            "key",
            "dim-key01",
            new ServiceCredentialRelationships(
                new DimServiceInstance(new DimData(serviceInstanceId)))
        );
        await client.PostAsJsonAsync("/v3/service_credential_bindings", data, JsonSerializerOptions.Default, cancellationToken)
            .CatchingIntoServiceExceptionFor("create-si-bindings", HttpAsyncResponseMessageExtension.RecoverOptions.INFRASTRUCTURE).ConfigureAwait(false);
    }
}