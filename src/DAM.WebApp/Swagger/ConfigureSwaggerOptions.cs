using Asp.Versioning.ApiExplorer;
using Microsoft.Extensions.Options;
using Microsoft.OpenApi.Models;
using Swashbuckle.AspNetCore.SwaggerGen;
using System.Reflection;

namespace DAM.WebApp.Swagger
{
    public class ConfigureSwaggerOptions : IConfigureOptions<SwaggerGenOptions>
    {
        private readonly IApiVersionDescriptionProvider _provider;

        public ConfigureSwaggerOptions(IApiVersionDescriptionProvider provider)
        {
            _provider = provider;
        }

        #region IConfigureOptions<SwaggerGenOptions>

        public void Configure(SwaggerGenOptions options)
        {
            // Generate swagger.json docs for each version of APIs
            foreach (var description in _provider.ApiVersionDescriptions)
            {
                options.SwaggerDoc(description.GroupName,
                                   new OpenApiInfo
                                   {
                                       Title = $"API v{description.ApiVersion}",
                                       Version = description.ApiVersion.MajorVersion.ToString()
                                   });
            }
            options.CustomOperationIds((apidesc) => $"{apidesc.HttpMethod}{apidesc.RelativePath}");
            // Add $odata parameters workaround
            options.OperationFilter<ODataOperationFilter>();
            options.OperationFilter<DeltaOperationFilter>();
            options.DocInclusionPredicate((docName, apiDesc) =>
            {
                if (!apiDesc.TryGetMethodInfo(out MethodInfo methodInfo)) return false;
                if (apiDesc.RelativePath.Contains("v{version}")) return false;

                return methodInfo.DeclaringType.GetCustomAttributes<Microsoft.AspNetCore.Mvc.ApiControllerAttribute>(true).Any()
                || methodInfo.DeclaringType.GetCustomAttributes<Microsoft.AspNetCore.OData.Routing.Attributes.ODataRouteComponentAttribute>(true).Any();
            });
            options.AddSecurityDefinition("Bearer", new OpenApiSecurityScheme
            {
                In = ParameterLocation.Header,
                Description = "Please enter a valid token",
                Name = "Authorization",
                Type = SecuritySchemeType.Http,
                BearerFormat = "JWT",
                Scheme = "Bearer"
            });
            options.AddSecurityRequirement(new OpenApiSecurityRequirement
            {
                {
                    new OpenApiSecurityScheme
                    {
                        Reference = new OpenApiReference
                        {
                            Type=ReferenceType.SecurityScheme,
                            Id="Bearer"
                        }
                    },
                    new string[]{}
                        }
            });

        }

        #endregion
    }

}
