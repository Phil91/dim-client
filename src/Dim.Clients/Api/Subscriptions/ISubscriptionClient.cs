using Dim.Clients.Api.Services;

namespace Dim.Clients.Api.Subscriptions;

public interface ISubscriptionClient
{
    Task SubscribeApplication(string authUrl, BindingItem bindingData, string applicationName, string planName, CancellationToken cancellationToken);
}