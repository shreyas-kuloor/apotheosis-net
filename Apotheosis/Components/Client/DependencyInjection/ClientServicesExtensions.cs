using Apotheosis.Components.Client.Configuration;
using Apotheosis.Components.Client.Interfaces;
using Apotheosis.Components.Client.Services;
using Discord.Interactions;
using Discord.WebSocket;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;

namespace Apotheosis.Components.Client.DependencyInjection;

public static class ClientServicesExtensions
{
    /// <summary>
    /// Adds client related services.
    /// </summary>
    /// <param name="services"> The instances of <see cref="IServiceCollection"/>.</param>
    /// <param name="clientSection">An instance of <see cref="IConfigurationSection"/>.</param>
    public static void AddClientServices(this IServiceCollection services, IConfigurationSection clientSection)
    {
        services.Configure<ClientSettings>(clientSection);
        services.AddSingleton<DiscordSocketClient>();
        services.AddSingleton(
            x => new InteractionService(
                x.GetRequiredService<DiscordSocketClient>()));
        services.AddSingleton<IInteractionHandler, InteractionHandler>();
    }
}
