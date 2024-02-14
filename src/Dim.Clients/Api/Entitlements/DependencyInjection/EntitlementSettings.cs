using System.ComponentModel.DataAnnotations;

namespace Dim.Clients.Api.Entitlements.DependencyInjection;

public class EntitlementSettings
{
    [Required]
    public string BaseUrl { get; set; } = null!;
}
