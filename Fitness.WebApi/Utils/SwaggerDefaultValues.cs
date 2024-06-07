using Microsoft.AspNetCore.Mvc.ApiExplorer;
using Microsoft.OpenApi.Any;
using Microsoft.OpenApi.Models;
using Swashbuckle.AspNetCore.SwaggerGen;
using System.Linq;

public class SwaggerDefaultValues : IOperationFilter
{
    public void Apply(OpenApiOperation operation, OperationFilterContext context)
    {
        var apiDescription = context.ApiDescription;

        operation.Deprecated = apiDescription.IsDeprecated();

        if (operation.Parameters == null)
        {
            return;
        }

        foreach (var parameter in operation.Parameters)
        {
            var description = apiDescription.ParameterDescriptions.First(p => p.Name == parameter.Name);

            parameter.Description ??= description.ModelMetadata?.Description;

            if (parameter.Schema.Default == null && description.DefaultValue != null)
            {
              
                switch (description.ModelMetadata.ModelType.Name)
                {
                    case "String":
                        parameter.Schema.Default = new OpenApiString(description.DefaultValue.ToString());
                        break;
                    case "Int32":
                        parameter.Schema.Default = new OpenApiInteger((int)description.DefaultValue);
                        break;
                    case "Boolean":
                        parameter.Schema.Default = new OpenApiBoolean((bool)description.DefaultValue);
                        break;
                       
                }
            }

            parameter.Required |= description.IsRequired;
        }
    }
}
