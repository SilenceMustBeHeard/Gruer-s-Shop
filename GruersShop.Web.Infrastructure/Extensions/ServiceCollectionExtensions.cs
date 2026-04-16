using Microsoft.Extensions.DependencyInjection;
using System.Reflection;

namespace GruersShop.Web.Infrastructure.Extensions;

public static class ServiceCollectionExtensions
{
    private const string ServiceSuffix = "Service";
    private const string RepositorySuffix = "Repository";
    private const string InterfacePrefix = "I";

    public static IServiceCollection RegisterServices(
        this IServiceCollection services,
        Assembly assembly)
    {
        var serviceTypes = assembly
            .GetTypes()
            .Where(t =>
                t.IsClass &&
                !t.IsAbstract &&
                t.Name.EndsWith(ServiceSuffix))
            .ToList();

        foreach (var implementation in serviceTypes)
        {
            var serviceInterface = implementation.GetInterfaces()
                .FirstOrDefault(i =>
                    i.Name == $"I{implementation.Name}");

            if (serviceInterface == null)
            {
              
                continue;
            }

            services.AddScoped(serviceInterface, implementation);
        }

        return services;
    }

    public static IServiceCollection RegisterRepositories(
        this IServiceCollection services,
        Assembly assembly)
    {
        var repoTypes = assembly
            .GetTypes()
            .Where(t =>
                t.IsClass &&
                !t.IsAbstract &&
                t.Name.EndsWith(RepositorySuffix) &&
                !t.Name.StartsWith("Base"))
            .ToList();

        foreach (var implementation in repoTypes)
        {
            var repoInterface = implementation.GetInterfaces()
                .FirstOrDefault(i =>
                    i.Name == $"I{implementation.Name}");

            if (repoInterface == null)
            {
                // ⚠️ skip instead of crash
                continue;
            }

            services.AddScoped(repoInterface, implementation);
        }

        return services;
    }
}