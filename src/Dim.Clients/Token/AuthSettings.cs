using System.ComponentModel.DataAnnotations;

namespace Dim.Clients.Token;

public class AuthSettings
{
    [Required(AllowEmptyStrings = false)]
    public string ClientId { get; set; } = null!;

    [Required(AllowEmptyStrings = false)]
    public string ClientSecret { get; set; } = null!;

    [Required(AllowEmptyStrings = false)]
    public string TokenAddress { get; set; } = null!;
}
