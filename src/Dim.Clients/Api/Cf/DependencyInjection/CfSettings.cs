using Dim.Clients.Token;
using System.ComponentModel.DataAnnotations;

namespace Dim.Clients.Api.Cf.DependencyInjection;

public class CfSettings : AuthSettings
{
    [Required]
    public string BaseUrl { get; set; } = null!;
}
