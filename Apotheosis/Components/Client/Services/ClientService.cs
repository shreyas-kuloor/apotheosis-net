using Apotheosis.Components.Client.Configuration;
using Apotheosis.Components.Client.Interfaces;
using Apotheosis.Components.Logging.Interfaces;
using Discord;
using Discord.WebSocket;
using Microsoft.Extensions.Options;

namespace Apotheosis.Components.Client.Services;

public class ClientService: IClientService
{
    private readonly ClientSettings _clientSettings;
    private readonly DiscordSocketClient _discordSocketClient;
    private readonly ILogService _logService;

    public ClientService(IOptions<ClientSettings> clientOptions, DiscordSocketClient discordSocketClient, ILogService logService)
    {
        _clientSettings = clientOptions.Value;
        _discordSocketClient = discordSocketClient;
        _logService = logService;
    }

    public async Task RunAsync()
    {
        _discordSocketClient.Log += _logService.Log;

        await _discordSocketClient.LoginAsync(TokenType.Bot, _clientSettings.BotToken);
        await _discordSocketClient.StartAsync();

        await Task.Delay(Timeout.Infinite);
    }
}
