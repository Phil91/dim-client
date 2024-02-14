using System.Text.Json;

namespace Dim.Clients.Services;

public interface ISetupDimService
{
    Task<JsonDocument> SaAndInstanceSetup(string bpnl, string companyName, CancellationToken cancellationToken);
}