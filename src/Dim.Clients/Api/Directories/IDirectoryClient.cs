namespace Dim.Clients.Api.Directories;

public interface IDirectoryClient
{
    Task<Guid> CreateDirectory(string description, string bpnl, Guid parentId, CancellationToken cancellationToken);
}
