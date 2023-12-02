using Apotheosis.Components.Client.Interfaces;
using Apotheosis.Components.Client.Services;
using Microsoft.Extensions.DependencyInjection;
using NetCord;
using NetCord.Gateway;
using NetCord.Services.ApplicationCommands;

namespace Apotheosis.Components.Client.DependencyInjection;

public static class ClientServicesExtensions
{
    /// <summary>
    /// Adds client related services.
    /// </summary>
    /// <param name="services"> The instances of <see cref="IServiceCollection"/>.</param>
    /// <param name="botToken">The discord bot token.</param>
    public static void AddClientServices(this IServiceCollection services, string botToken)
    {
        services.AddSingleton<GatewayClient>(_ =>
            new GatewayClient(
                new Token(TokenType.Bot, botToken), 
                new GatewayClientConfiguration { Intents = GatewayIntents.AllNonPrivileged | GatewayIntents.MessageContent }));
        services.AddSingleton<IClientService, ClientService>();
        services.AddSingleton(_ => new ApplicationCommandService<SlashCommandContext>());
        services.AddSingleton<IInteractionHandler, InteractionHandler>();
    }
}
