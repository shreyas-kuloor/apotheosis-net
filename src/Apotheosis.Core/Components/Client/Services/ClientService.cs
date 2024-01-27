using Apotheosis.Core.Components.AiChat.Interfaces;
using Apotheosis.Core.Components.Audio.Interfaces;
using Apotheosis.Core.Components.Client.Interfaces;
using Apotheosis.Core.Components.EmojiCounter.Interfaces;
using Apotheosis.Core.Components.Logging.Interfaces;
using Microsoft.Extensions.Logging;
using NetCord.Gateway;

namespace Apotheosis.Core.Components.Client.Services;

public sealed class ClientService(
    GatewayClient discordClient,
    ILogService<ClientService> logger,
    IInteractionHandler interactionHandler,
    IAiChatThreadMessageHandler aiChatThreadMessageHandler,
    IVoiceChannelEmptyHandler voiceChannelEmptyHandler,
    IEmojiCounterReactionHandler emojiCounterReactionHandler)
    : IClientService
{
    public async Task RunAsync()
    {
        discordClient.Log += Log;

        await discordClient.StartAsync();
        
        await interactionHandler.InitializeAsync();
        
        aiChatThreadMessageHandler.Initialize();
        
        voiceChannelEmptyHandler.Initialize();

        emojiCounterReactionHandler.Initialize();

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
