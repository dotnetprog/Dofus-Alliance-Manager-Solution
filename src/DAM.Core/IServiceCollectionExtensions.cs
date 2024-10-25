using DAM.Core.Abstractions.Services;
using DAM.Core.Behaviors;
using DAM.Core.Requests.Commands;
using DAM.Core.Services;
using FluentValidation;
using MediatR;
using Microsoft.Extensions.DependencyInjection;
using System.Reflection;

namespace DAM.Core
{
    public static class IServiceCollectionExtensions
    {
        public static IServiceCollection ConfigureDAMCore(this IServiceCollection service, string webappurl, params Assembly[] assemblies)
        {
            var currentAssembly = typeof(AddScreenPostCommand).Assembly;
            var a = new List<Assembly>();
            a.Add(currentAssembly);
            a.AddRange(assemblies);
            service.AddMediatR(cfg => cfg.RegisterServicesFromAssemblies(a.ToArray()))
                   .AddTransient(typeof(IPipelineBehavior<,>), typeof(ValidationBehavior<,>))
                   .AddValidatorsFromAssembly(currentAssembly, ServiceLifetime.Singleton);
            service.AddSingleton<IWebAppMetadataService, SimpleWebAppMedataService>((sp) => new SimpleWebAppMedataService(webappurl));


            return service;
        }
    }
}
