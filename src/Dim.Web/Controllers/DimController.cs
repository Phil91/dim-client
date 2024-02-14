using Dim.Clients.Services;
using Dim.Web.Extensions;
using Dim.Web.Models;
using System.Text.Json;

namespace Dim.Web.Controllers;

/// <summary>
/// Creates a new instance of <see cref="DimController"/>
/// </summary>
public static class DimController
{
    public static RouteGroupBuilder MapDimApi(this RouteGroupBuilder group)
    {
        var policyHub = group.MapGroup("/dim");

        policyHub.MapPost("setup-dim", (string bpnl, string companyName, CancellationToken cancellationToken, ISetupDimService setupDimService) => setupDimService.SaAndInstanceSetup(bpnl, companyName, cancellationToken))
            .WithSwaggerDescription("Gets the keys for the attributes",
                "Example: Post: api/dim/setup-dim",
                "The busines partner number of the company",
                "The company name")
            .Produces(StatusCodes.Status200OK, typeof(JsonDocument), Constants.JsonContentType);

        return group;
    }
}
