using Dim.Clients.Api.Cf;
using Dim.Clients.Api.Dim;
using Dim.Clients.Api.Entitlements;
using Dim.Clients.Api.Provisioning;
using Dim.Clients.Api.Services;
using Dim.Clients.Api.SubAccounts;
using Dim.Clients.Api.Subscriptions;
using Dim.Clients.Token;
using Microsoft.Extensions.Options;
using System.Text.Json;

namespace Dim.Clients.Services;

public class SetupDimService : ISetupDimService
{
    private readonly ISubAccountClient _subAccountClient;
    private readonly IServiceClient _serviceClient;
    private readonly IEntitlementClient _entitlementClient;
    private readonly ISubscriptionClient _subscriptionClient;
    private readonly IProvisioningClient _provisioningClient;
    private readonly ICfClient _cfClient;
    private readonly IDimClient _dimClient;
    private readonly SetupDimSettings _settings;

    public SetupDimService(
        ISubAccountClient subAccountClient,
        IServiceClient serviceClient,
        ISubscriptionClient subscriptionClient,
        IEntitlementClient entitlementClient,
        IProvisioningClient provisioningClient,
        ICfClient cfClient,
        IDimClient dimClient,
        IOptions<SetupDimSettings> options)
    {
        _subAccountClient = subAccountClient;
        _serviceClient = serviceClient;
        _entitlementClient = entitlementClient;
        _subscriptionClient = subscriptionClient;
        _provisioningClient = provisioningClient;
        _cfClient = cfClient;
        _dimClient = dimClient;
        _settings = options.Value;
    }

    public async Task<JsonDocument> SaAndInstanceSetup(string bpnl, string companyName, CancellationToken cancellationToken)
    {
        var parentDirectoryId = _settings.RootDirectoryId;
        var adminMail = _settings.AdminMail;
        var subAccountAuth = new AuthSettings
        {
            TokenAddress = $"{_settings.AuthUrl}/oauth/token",
            ClientId = _settings.ClientidCisCentral,
            ClientSecret = _settings.ClientsecretCisCentral
        };
        var subAccountId = await _subAccountClient.CreateSubaccount(subAccountAuth, adminMail, bpnl, companyName, parentDirectoryId, cancellationToken).ConfigureAwait(false);
        Thread.Sleep(30000);
        var saBinding = await _subAccountClient.CreateServiceManagerBindings(subAccountAuth, subAccountId, cancellationToken).ConfigureAwait(false);
        await _entitlementClient.AssignEntitlements(subAccountAuth, subAccountId, cancellationToken).ConfigureAwait(false);
        var serviceInstance = await _serviceClient.CreateServiceInstance(saBinding, cancellationToken).ConfigureAwait(false);
        var serviceBinding = await _serviceClient.CreateServiceBinding(saBinding, serviceInstance.Id, cancellationToken).ConfigureAwait(false);
        var bindingResponse = await _serviceClient.GetServiceBinding(saBinding, serviceBinding.Name, cancellationToken).ConfigureAwait(false);
        
        await _subscriptionClient.SubscribeApplication(saBinding.Url, bindingResponse, "decentralized-identity-management-app", "standard", cancellationToken).ConfigureAwait(false);
        
        var cfEnvironmentId = await _provisioningClient.CreateCloudFoundryEnvironment(saBinding.Url, bindingResponse, bpnl, adminMail, cancellationToken)
            .ConfigureAwait(false);
        var spaceId = await _cfClient.CreateCloudFoundrySpace(bpnl, cfEnvironmentId, cancellationToken).ConfigureAwait(false);
        await _cfClient.AddSpaceRoleToUser("space_manager", adminMail, spaceId, cancellationToken).ConfigureAwait(false);
        await _cfClient.AddSpaceRoleToUser("space_developer", adminMail, spaceId, cancellationToken).ConfigureAwait(false);
        
        var servicePlanId = await _cfClient.GetServicePlan("decentralized-identity-management", "standard", cancellationToken).ConfigureAwait(false);
        await _cfClient.CreateDimServiceInstance(spaceId, servicePlanId, cancellationToken).ConfigureAwait(false);
        await _cfClient.CreateServiceInstanceBindings(cancellationToken).ConfigureAwait(false);

        // TODO: get the data of the dim service instance
        var dimAuth = new AuthSettings
        {
            TokenAddress = "",
            ClientId = "",
            ClientSecret = ""
        };
        var dimBaseUrl = "";
        var result = await _dimClient.CreateCompanyIdentity(dimAuth, dimBaseUrl, bpnl, cancellationToken).ConfigureAwait(false);
        var didDocument = await _dimClient.GetDidDocument(result.DownloadUrl, cancellationToken).ConfigureAwait(false);
        return didDocument;
    }
}
