using Apotheosis.Components.Converse.Configuration;
using Apotheosis.Components.Converse.Interfaces;
using Apotheosis.Components.Converse.Services;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;

namespace Apotheosis.Components.Converse.DependencyInjection;

public static class ConverseExtensions
{
    /// <summary>
    /// Adds converse command related services.
    /// </summary>
    /// <param name="services"> The instances of <see cref="IServiceCollection"/>.</param>
    /// <param name="converseSection">An instance of <see cref="IConfigurationSection"/>.</param>
    public static void AddConverseServices(
        this IServiceCollection services,
        IConfigurationSection converseSection)
    {
        var converseSettings = converseSection.Get<ConverseSettings>()!;
        services.AddSingleton(converseSettings);
        services.AddScoped<IConverseService, ConverseService>();
    }
}