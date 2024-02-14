using Dim.Clients.Token;
using System.ComponentModel.DataAnnotations;

namespace Dim.Clients.Api.Provisioning.DependencyInjection;

public class ProvisioningSettings : AuthSettings
{
    [Required]
    public string BaseUrl { get; set; } = null!;
}
