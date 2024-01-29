using Apotheosis.Core.Components.Converse.Configuration;
using Apotheosis.Core.Components.Converse.Interfaces;
using Apotheosis.Core.Components.Converse.Services;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;

namespace Apotheosis.Core.Components.Converse.Extensions;

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