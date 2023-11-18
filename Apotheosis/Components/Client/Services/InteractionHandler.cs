using System.Reflection;
using Apotheosis.Components.Client.Interfaces;
using Discord;
using Discord.Interactions;
using Discord.WebSocket;
using Microsoft.Extensions.Logging;

namespace Apotheosis.Components.Client.Services;

public class InteractionHandler : IInteractionHandler
{
    private readonly DiscordSocketClient _discordSocketClient;
    private readonly InteractionService _interactionService;
    private readonly ILogger<InteractionHandler> _logger;
    private readonly IServiceProvider _services;
    
    public InteractionHandler(
        DiscordSocketClient discordSocketClient,
        InteractionService interactionService,
        ILogger<InteractionHandler> logger,
        IServiceProvider serviceProvider)
    {
        _discordSocketClient = discordSocketClient;
        _interactionService = interactionService;
        _logger = logger;
        _services = serviceProvider;
    }
    
    public async Task InitializeAsync()
    {
        _discordSocketClient.Ready += ReadyAsync;
        _interactionService.Log += Log;

        await _interactionService.AddModulesAsync(Assembly.GetEntryAssembly(), _services);

        _discordSocketClient.InteractionCreated += HandleInteractionAsync;
    }

    private async Task ReadyAsync()
    {
        await _interactionService.RegisterCommandsGloballyAsync();
    }

    private async Task HandleInteractionAsync(SocketInteraction interaction)
    {
        try
        {
            var context = new SocketInteractionContext(_discordSocketClient, interaction);
            
            var result = await _interactionService.ExecuteCommandAsync(context, _services);

            if (!result.IsSuccess)
                switch (result.Error)
                {
                    case InteractionCommandError.UnmetPrecondition:
                        break;
                    case null:
                    case InteractionCommandError.UnknownCommand:
                    case InteractionCommandError.ConvertFailed:
                    case InteractionCommandError.BadArgs:
                    case InteractionCommandError.Exception:
                    case InteractionCommandError.Unsuccessful:
                    case InteractionCommandError.ParseFailed:
                    default:
                        break;
                }
        }
        catch
        {
            if (interaction.Type is InteractionType.ApplicationCommand)
                await interaction.GetOriginalResponseAsync().ContinueWith(
                    async (msg) => await msg.Result.DeleteAsync());
        }
    }
    
    private Task Log(LogMessage message)
    {
        var logLevel = message.Severity switch
        {
            LogSeverity.Critical => LogLevel.Critical,
            LogSeverity.Error => LogLevel.Error,
            LogSeverity.Warning => LogLevel.Warning,
            LogSeverity.Info => LogLevel.Information,
            LogSeverity.Verbose => LogLevel.Trace,
            LogSeverity.Debug => LogLevel.Debug,
            _ => LogLevel.Information
        };
        _logger.Log(logLevel, message.Exception, "[{Source}] {Message}", message.Source, message.Message);
        return Task.CompletedTask;
    }
}