using Apotheosis.Components.Client.Configuration;
using Apotheosis.Components.Client.Interfaces;
using Discord;
using Discord.WebSocket;
using Microsoft.Extensions.Logging;
using Microsoft.Extensions.Options;

namespace Apotheosis.Components.Client.Services;

public sealed class ClientService: IClientService
{
    private readonly ClientSettings _clientSettings;
    private readonly DiscordSocketClient _discordSocketClient;
    private readonly ILogger<ClientService> _logger;
    private readonly IInteractionHandler _interactionHandler;

    public ClientService(
        IOptions<ClientSettings> clientOptions, 
        DiscordSocketClient discordSocketClient, 
        ILogger<ClientService> logger,
        IInteractionHandler interactionHandler)
    {
        _clientSettings = clientOptions.Value;
        _discordSocketClient = discordSocketClient;
        _logger = logger;
        _interactionHandler = interactionHandler;
    }

    public async Task RunAsync()
    {
        _discordSocketClient.Log += Log;

        await _interactionHandler.InitializeAsync();

        await _discordSocketClient.LoginAsync(TokenType.Bot, _clientSettings.BotToken);
        await _discordSocketClient.StartAsync();

        await Task.Delay(Timeout.Infinite);
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
