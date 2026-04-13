using Microsoft.OpenApi.Models;
using Swashbuckle.AspNetCore.SwaggerGen;

namespace TaskService.Api.Filters;

public class RequiredNullableSchemaFilter : ISchemaFilter
{
    public void Apply(OpenApiSchema schema, SchemaFilterContext context)
    {
        if (schema.Properties is null)
            return;

        foreach (var property in schema.Properties)
        {
            if (!schema.Required.Contains(property.Key))
                schema.Required.Add(property.Key);
        }
    }
}
