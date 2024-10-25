using Microsoft.OpenApi.Models;
using Swashbuckle.AspNetCore.SwaggerGen;

namespace DAM.WebApp.Swagger
{
    public class DeltaOperationFilter : IOperationFilter
    {
        private const string _deltaParam = "Delta";

        public void Apply(OpenApiOperation operation, OperationFilterContext context)
        {
            if (operation.RequestBody == null) return;

            var deltaTypes =
                operation.RequestBody
                    .Content
                    .Where(x => x.Value.Schema.Reference.Id.EndsWith(_deltaParam));

            foreach (var (_, value) in deltaTypes)
            {
                var schema = value.Schema;
                string model = schema.Reference.Id.Substring(0, schema.Reference.Id.Length - _deltaParam.Length);
                schema.Reference.Id = model;
            }
        }
    }
}
