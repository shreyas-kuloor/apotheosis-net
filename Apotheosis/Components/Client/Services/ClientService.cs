using Apotheosis.Components.AiChat.Interfaces;
using Apotheosis.Components.Client.Interfaces;
using Apotheosis.Components.Logging.Interfaces;
using Microsoft.Extensions.Logging;
using NetCord.Gateway;

namespace Apotheosis.Components.Client.Services;

public sealed class ClientService(
    GatewayClient discordClient,
    ILogService<ClientService> logger,
    IInteractionHandler interactionHandler,
    IAiChatThreadMessageHandler aiChatThreadMessageHandler)
    : IClientService
{
    public async Task RunAsync()
    {
        discordClient.Log += Log;

        await discordClient.StartAsync();
        
        await interactionHandler.InitializeAsync();
        
        aiChatThreadMessageHandler.InitializeAsync();

        await Task.Delay(Timeout.Infinite);
    }

    private ValueTask Log(LogMessage message)
    {
        var logLevel = message.Severity switch
        {
            LogSeverity.Error => LogLevel.Error,
            LogSeverity.Info => LogLevel.Information,
            _ => LogLevel.Information
        };
        logger.Log(logLevel, message.Exception, message.Message);
        return default;
    }
}
