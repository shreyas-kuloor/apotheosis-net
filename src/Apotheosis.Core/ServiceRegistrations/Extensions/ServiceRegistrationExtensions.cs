using System.Reflection;
using Microsoft.Extensions.DependencyInjection;

namespace Apotheosis.Core.ServiceRegistrations.Extensions;

public static  class ServiceRegistrationExtensions
{
    public static void AddServicesWithAttributes(this IServiceCollection services, Assembly assembly)
    {
        var scopedTypes = assembly.GetTypes().Where(t => t.GetCustomAttribute<ScopedAttribute>() != null);
        var singletonTypes = assembly.GetTypes().Where(t => t.GetCustomAttribute<SingletonAttribute>() != null);

        foreach (var scopedType in scopedTypes)
        {
            var typeToRegister = scopedType.GetInterfaces().FirstOrDefault() ?? scopedType;

            services.AddScoped(typeToRegister, scopedType);
        }

        foreach (var singletonType in singletonTypes)
        {
            var typeToRegister = singletonType.GetInterfaces().FirstOrDefault() ?? singletonType;

            services.AddSingleton(typeToRegister, singletonType);
        }
    }
}
