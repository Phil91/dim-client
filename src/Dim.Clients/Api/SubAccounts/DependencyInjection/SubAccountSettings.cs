using System.ComponentModel.DataAnnotations;

namespace Dim.Clients.Api.SubAccounts.DependencyInjection;

public class SubAccountSettings
{
    [Required]
    public string BaseUrl { get; set; } = null!;
}
