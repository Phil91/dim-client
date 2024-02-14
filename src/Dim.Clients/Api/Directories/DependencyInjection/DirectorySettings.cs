using Dim.Clients.Token;
using System.ComponentModel.DataAnnotations;

namespace Dim.Clients.Api.Directories.DependencyInjection;

public class DirectorySettings : AuthSettings
{
    [Required]
    public string BaseUrl { get; set; } = null!;
}
