using Dim.Clients.Token;

namespace Dim.Clients.Api.SubAccounts;

public interface ISubAccountClient
{
    Task<Guid> CreateSubaccount(AuthSettings authSettings, string adminMail, string bpnl, string companyName, Guid directoryId, CancellationToken cancellationToken);
    Task<ServiceManagementBindingResponse> CreateServiceManagerBindings(AuthSettings authSettings, Guid subAccountId, CancellationToken cancellationToken);
}