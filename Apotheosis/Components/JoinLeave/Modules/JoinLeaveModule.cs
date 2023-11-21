using Discord;
using Discord.Interactions;

namespace Apotheosis.Components.JoinLeave.Modules;

public class JoinLeaveModule : InteractionModuleBase<SocketInteractionContext>
{
    [SlashCommand("join", "Invites the bot to join your voice channel.")]
    public async Task JoinVoiceAsync()
    {
        if (Context.Guild.CurrentUser.IsStreaming)
        {
            await RespondAsync("I'm currently speaking in a voice channel, please try again after I'm done!", ephemeral: true);
        }
        
        var voiceChannel = (Context.User as IVoiceState)?.VoiceChannel;

        if (voiceChannel == null)
        {
            await RespondAsync(
                "You are not in a voice channel. Please join a voice channel and then re-run the command.",
                ephemeral: true);
            return;
        }
        
        await voiceChannel.ConnectAsync();
        await RespondAsync("Joined voice channel!", ephemeral: true);
    }
    
    [SlashCommand("leave", "Makes the bot leave your voice channel.")]
    public async Task LeaveVoiceAsync()
    {
        if (Context.Guild.CurrentUser.IsStreaming)
        {
            await RespondAsync("I'm currently speaking in a voice channel, please try again after I'm done!", ephemeral: true);
        }
        
        var voiceChannel = (Context.User as IVoiceState)?.VoiceChannel;

        if (voiceChannel == null)
        {
            await RespondAsync(
                "You are not in a voice channel. Please join a voice channel and then re-run the command.",
                ephemeral: true);
            return;
        }

        var botVoiceChannel = (Context.Guild.CurrentUser as IVoiceState)?.VoiceChannel;

        if (botVoiceChannel == null)
        {
            await RespondAsync(
                "I am not in a voice channel. This command can only be used to make me leave a voice channel.",
                ephemeral: true);
            return;
        }

        await botVoiceChannel.DisconnectAsync();
        await RespondAsync("Left voice channel!", ephemeral: true);
    }
}