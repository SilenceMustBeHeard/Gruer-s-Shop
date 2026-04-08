using Microsoft.Extensions.DependencyInjection;
using GruersShop.Services.Common.Exceptions;
using System;
using System.Collections.Generic;
using System.Reflection;
using System.Text;

namespace GruersShop.Web.Infrastructure.Extensions;

public static class ServiceCollectionExtensions
{
    private const string ServiceSuffix = "Service";
    private const string InterfacePreffix = "I";
    private const string RepoTypeSuffix = "Repository";
    private const string BaseRepoTypePreffix = "Base";

    public static IServiceCollection RegisterServices(
        this IServiceCollection services,
        Assembly serviceAssembly)
    {
        Type[] serviceClasses = serviceAssembly
            .GetTypes()
            .Where(t =>
                t.IsClass &&
                !t.IsAbstract &&
                t.Name.EndsWith(ServiceSuffix))
            .ToArray();

        Type[] serviceInterfaces = serviceAssembly
            .GetTypes()
            .Where(t =>
                t.IsInterface &&
                t.Name.StartsWith(InterfacePreffix) &&
                t.Name.EndsWith(ServiceSuffix))
            .ToArray();

        foreach (Type implementation in serviceClasses)
        {
            Type? serviceInterface = implementation.GetInterfaces()
                .FirstOrDefault(i => i.Name == $"{InterfacePreffix}{implementation.Name}");

            if (serviceInterface == null)
            {
                // ✅ Use ExceptionMessages.RepoInterfaceNotFound
                throw new InvalidOperationException(
                    string.Format(ExceptionMessages.RepoInterfaceNotFound, implementation.Name));
            }

            services.AddScoped(serviceInterface, implementation);
        }

        return services;
    }

    public static IServiceCollection RegisterRepositories(
        this IServiceCollection serviceCollection,
        Assembly repositoryAssembly)
    {
        Type[] repoClasses = repositoryAssembly
            .GetTypes()
            .Where(r => r.Name.EndsWith(RepoTypeSuffix)
                && !r.IsInterface
                && !r.IsAbstract)
            .ToArray();

        foreach (Type implementation in repoClasses)
        {
            Type? repoInterface = implementation.GetInterfaces()
                .FirstOrDefault(i => i.Name == $"{InterfacePreffix}{implementation.Name}");

            if (repoInterface == null)
            {
               
                throw new InvalidOperationException(
                    string.Format(ExceptionMessages.RepoInterfaceNotFound, implementation.Name));
            }

            serviceCollection.AddScoped(repoInterface, implementation);
        }

        return serviceCollection;
    }
}