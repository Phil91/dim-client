using Dim.Clients.Api.SubAccounts;

namespace Dim.Clients.Api.Services;

public interface IServiceClient
{
    Task<CreateServiceInstanceResponse> CreateServiceInstance(ServiceManagementBindingResponse saBinding, CancellationToken cancellationToken);
    Task<CreateServiceBindingResponse> CreateServiceBinding(ServiceManagementBindingResponse saBinding, string serviceInstanceId, CancellationToken cancellationToken);
    Task<BindingItem> GetServiceBinding(ServiceManagementBindingResponse saBinding, string serviceBindingName, CancellationToken cancellationToken);
}