using Apotheosis.Core.Features.Audio.Interfaces;

namespace Apotheosis.Core.Features.Audio.Handlers;

[GatewayEvent(nameof(GatewayClient.VoiceStateUpdate))]
public sealed class VoiceChannelEmptyHandler(
    GatewayClient discordClient,
    IVoiceClientService voiceClientService)
    : IGatewayEventHandler<VoiceState>
{
    public async ValueTask HandleAsync(VoiceState voiceState)
    {
        if (voiceState.User == null || voiceState.User.IsBot)
        {
            return;
        }

        if (voiceState.ChannelId is null)
        {
            var botUser = await discordClient.Rest.GetCurrentUserAsync();

            var guild = discordClient.Cache.Guilds[voiceState.GuildId.GetValueOrDefault()];

            if (!guild.VoiceStates.TryGetValue(botUser.Id, out var botVoiceState))
            {
                return;
            }

            var voiceStates = guild.VoiceStates
                .Where(vs => vs.Value.ChannelId == botVoiceState.ChannelId)
                .Where(vs => vs.Value.User != null && !vs.Value.User.IsBot);

            if (!voiceStates.Any())
            {
                if (voiceClientService.TryRemoveVoiceClient(botVoiceState.ChannelId.GetValueOrDefault(), out var voiceClient))
                {
                    await voiceClient!.CloseAsync();
                    await discordClient.UpdateVoiceStateAsync(new VoiceStateProperties(guild.Id, null));
                    voiceClient.Dispose();
                }
            }
        }
    }
}