using Microsoft.OpenApi.Any;
using Microsoft.OpenApi.Models;
using Swashbuckle.AspNetCore.SwaggerGen;

namespace ReklamationAPI.Swagger
{
    /// <summary>
    /// A class for setting the example values for primitive properties in the Swagger UI.
    /// At the time of creation the Example Schema seems to result in the default value rather than a placeholder.
    /// See https://github.com/swagger-api/swagger-ui/issues/3920 for the issue.
    /// </summary>
    public class ParameterDefaults : IOperationFilter
    {
        public void Apply(OpenApiOperation operation, OperationFilterContext context)
        {
            if (operation.Parameters == null)
                return;

            foreach (var parameter in operation.Parameters)
            {
                if (parameter.Schema.Default != null)
                    continue;
                switch (parameter.Schema.Type)
                {
                    case "integer":
                        switch (parameter.Name)
                        {
                            case "id":
                                parameter.Schema.Example = new OpenApiInteger(0);
                                break;
                            case "productId":
                                parameter.Schema.Example = new OpenApiInteger(1);
                                break;
                            default:
                                break;
                        }
                        break;


                    case "string":
                        switch (parameter.Name)
                        {
                            case "customerName":
                                parameter.Schema.Example = new OpenApiString("John Doe");
                                break;
                            case "customerEmail":
                                parameter.Schema.Example = new OpenApiString("john.doe@example.com");
                                break;
                            case "date":
                                parameter.Schema.Example = new OpenApiString("2023-05-28");
                                break;
                            case "status":
                                parameter.Schema.Example = new OpenApiString("Open");
                                break;
                            case "description":
                                parameter.Schema.Example = new OpenApiString("Das Produkt ist defekt.");
                                break;
                            default:
                                break;
                        }
                        break;


                    default:
                        break;
                }

            }
        }
    }
}
