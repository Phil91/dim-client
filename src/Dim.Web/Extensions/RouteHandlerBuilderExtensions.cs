namespace Dim.Web.Extensions;

public static class RouteHandlerBuilderExtensions
{
    public static RouteHandlerBuilder WithSwaggerDescription(this RouteHandlerBuilder builder, string summary, string description, params string[] parameterDescriptions) =>
        builder.WithOpenApi(op =>
        {
            op.Summary = summary;
            op.Description = description;
            for (var i = 0; i < parameterDescriptions.Length; i++)
            {
                if (i < op.Parameters.Count)
                {
                    op.Parameters[i].Description = parameterDescriptions[i];
                }
            }

            return op;
        });
}
