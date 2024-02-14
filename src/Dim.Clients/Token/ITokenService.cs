namespace Dim.Clients.Token;

public interface ITokenService
{
    Task<HttpClient> GetAuthorizedClient<T>(AuthSettings settings, CancellationToken cancellationToken);
    Task<HttpClient> GetAuthorizedLegacyClient<T>(AuthSettings settings, CancellationToken cancellationToken);
}
