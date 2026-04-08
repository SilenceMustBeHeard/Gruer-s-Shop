using Microsoft.Extensions.DependencyInjection;
using GruersShop.Services.Common.Exceptions;
using System.Reflection;

namespace GruersShop.Web.Infrastructure.Extensions;

public static class ServiceCollectionExtensions
{
    private const string ServiceSuffix = "Service";
    private const string InterfacePrefix = "I";
    private const string RepositorySuffix = "Repository";
    private const string BasePrefix = "Base";

    public static IServiceCollection RegisterServices(
        this IServiceCollection services,
        Assembly assembly)
    {
        var serviceTypes = assembly
            .GetTypes()
            .Where(t =>
                t.IsClass &&
                !t.IsAbstract &&
                t.Name.EndsWith(ServiceSuffix) &&
                !t.IsGenericType)
            .ToList();

        foreach (var implementation in serviceTypes)
        {
            var expectedInterfaceName = $"{InterfacePrefix}{implementation.Name}";

            var serviceInterface = implementation
                .GetInterfaces()
                .FirstOrDefault(i => i.Name == expectedInterfaceName);

            if (serviceInterface == null)
            {
                throw new InvalidOperationException(
                    $"❌ Service '{implementation.Name}' does not have matching interface '{expectedInterfaceName}'");
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
                !t.IsGenericType &&
                t.Name.EndsWith(RepositorySuffix) &&
                !t.Name.StartsWith(BasePrefix)).ToList();

        foreach (var implementation in repoTypes)
        {
            var expectedInterfaceName = $"{InterfacePrefix}{implementation.Name}";

            var repoInterface = implementation
                .GetInterfaces()
                .FirstOrDefault(i => i.Name == expectedInterfaceName);

            if (repoInterface == null)
            {
                throw new InvalidOperationException(
                    $"❌ Repository '{implementation.Name}' does not have matching interface '{expectedInterfaceName}'");
            }

            services.AddScoped(repoInterface, implementation);
        }

        return services;
    }
}