using System.ComponentModel.DataAnnotations;

namespace Dim.Clients.Services;

public class SetupDimSettings
{
    [Required(AllowEmptyStrings = false)]
    public string AdminMail { get; set; } = null!;

    [Required]
    public Guid RootDirectoryId { get; set; }

    [Required(AllowEmptyStrings = false)]
    public string AuthUrl { get; set; } = null!;

    [Required(AllowEmptyStrings = false)]
    public string ClientidCisCentral { get; set; } = null!;

    [Required(AllowEmptyStrings = false)]
    public string ClientsecretCisCentral { get; set; } = null!;
}