using System.Reflection;
using Apotheosis.Components.Client.Interfaces;
using Apotheosis.Components.Logging.Interfaces;
using Discord;
using Discord.Interactions;
using Discord.WebSocket;

namespace Apotheosis.Components.Client.Services;

public class InteractionHandler : IInteractionHandler
{
    private readonly DiscordSocketClient _discordSocketClient;
    private readonly InteractionService _interactionService;
    private readonly ILogService _logService;
    private readonly IServiceProvider _services;
    
    public InteractionHandler(
        DiscordSocketClient discordSocketClient,
        InteractionService interactionService,
        ILogService logService,
        IServiceProvider serviceProvider)
    {
        _discordSocketClient = discordSocketClient;
        _interactionService = interactionService;
        _logService = logService;
        _services = serviceProvider;
    }
    
    public async Task InitializeAsync()
    {
        _discordSocketClient.Ready += () => _interactionService.RegisterCommandsGloballyAsync();
        _interactionService.Log += _logService.Log;

        await _interactionService.AddModulesAsync(Assembly.GetEntryAssembly(), _services);

        _discordSocketClient.InteractionCreated += HandleInteractionAsync;
    }

    public Task StopAsync()
    {
        _interactionService.Dispose();
        return Task.CompletedTask;
    }

    private async Task HandleInteractionAsync(SocketInteraction interaction)
    {
        try
        {
            var context = new SocketInteractionContext(_discordSocketClient, interaction);
            
            var result = await _interactionService.ExecuteCommandAsync(context, _services);

            if (!result.IsSuccess)
                await context.Channel.SendMessageAsync(result.ToString());
        }
        catch
        {
            if (interaction.Type is InteractionType.ApplicationCommand)
                await interaction.GetOriginalResponseAsync().ContinueWith(msg => msg.Result.DeleteAsync());
        }
    }
}