using System.Reflection;
using Apotheosis.Core.Components.Client.Interfaces;
using NetCord;
using NetCord.Gateway;
using NetCord.Rest;
using NetCord.Services.ApplicationCommands;

namespace Apotheosis.Core.Components.Client.Services;

public sealed class InteractionHandler(
    GatewayClient discordClient,
    IServiceProvider provider)
    : IInteractionHandler
{
    private readonly ApplicationCommandService<SlashCommandContext> _applicationCommandService = new();
    
    public async Task InitializeAsync()
    {
        _applicationCommandService.AddModules(Assembly.GetEntryAssembly()!);
        await discordClient.ReadyAsync;
        await _applicationCommandService.CreateCommandsAsync(discordClient.Rest, discordClient.ApplicationId);

        discordClient.InteractionCreate += HandleInteractionAsync;
    }

    private async ValueTask HandleInteractionAsync(Interaction interaction)
    {
        if (interaction is SlashCommandInteraction slashCommandInteraction)
        {
            try
            {
                await _applicationCommandService.ExecuteAsync(new SlashCommandContext(slashCommandInteraction, discordClient), provider);
            }
            catch (Exception ex)
            {
                await interaction.SendResponseAsync(InteractionCallback.Message($"Error: {ex.Message}"));
            }
        }
    }
}