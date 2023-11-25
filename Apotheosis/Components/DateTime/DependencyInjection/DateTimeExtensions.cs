using Apotheosis.Components.DateTime.Interfaces;
using Apotheosis.Components.DateTime.Services;
using Microsoft.Extensions.DependencyInjection;

namespace Apotheosis.Components.DateTime.DependencyInjection;


public static class DateTimeExtensions
{
    /// <summary>
    /// Adds client related services.
    /// </summary>
    /// <param name="services"> The instances of <see cref="IServiceCollection"/>.</param>
    public static void AddDateTimeServices(
        this IServiceCollection services)
    {
        services.AddScoped<IDateTimeService, DateTimeService>();
    }
}