﻿using Apotheosis.Core.Features.Audio.Interfaces;
using Apotheosis.Core.Features.FeatureFlags.Configuration;
using Apotheosis.Core.Features.Logging.Interfaces;
using Microsoft.Extensions.Logging;
using NetCord.Gateway.Voice;
using NetCord.Rest;

namespace Apotheosis.Core.Features.JoinLeave.Modules;

public sealed class JoinLeaveModule(
    IVoiceClientService voiceClientService,
    ILogService<JoinLeaveModule> logger,
    IOptions<FeatureFlagSettings> featureFlagOptions) : ApplicationCommandModule<SlashCommandContext>
{
    readonly FeatureFlagSettings featureFlagSettings = featureFlagOptions.Value;

    [SlashCommand("join", "Invites the bot to join your voice channel.")]
    public async Task JoinVoiceAsync()
    {
        if (!featureFlagSettings.JoinEnabled)
        {
            await RespondAsync(InteractionCallback.Message(
                new InteractionMessageProperties()
                .WithContent("This command is currently disabled. Please try again later!")
                .WithFlags(MessageFlags.Ephemeral)));
            return;
        }

        logger.Log(LogLevel.Information, null, $"{Context.User.GlobalName} used /join");

        var client = Context.Client;
        var currentUser = await client.Rest.GetCurrentUserAsync();
        var guild = Context.Guild!;

        if (!guild.VoiceStates.TryGetValue(Context.User.Id, out var userVoiceState))
        {
            await RespondAsync(InteractionCallback.Message(new InteractionMessageProperties
            {
                Content = "You are not in a voice channel. Please join a voice channel and then re-run the command.",
                Flags = MessageFlags.Ephemeral
            }));
            return;
        }

        var voiceChannelId = userVoiceState.ChannelId.GetValueOrDefault();
        if (guild.VoiceStates.TryGetValue(currentUser.Id, out var botVoiceState))
        {
            var botVoiceChannelId = botVoiceState.ChannelId.GetValueOrDefault();
            if (voiceChannelId != botVoiceChannelId)
            {
                await RespondAsync(InteractionCallback.Message(new InteractionMessageProperties
                {
                    Content = "I'm currently in another voice channel right now. Please wait until I'm done to invite me to speak.",
                    Flags = MessageFlags.Ephemeral
                }));
                return;
            }
        }

        if (!voiceClientService.TryGetVoiceClient(voiceChannelId, out var voiceClient))
        {
            voiceClient = await client.JoinVoiceChannelAsync(
                guild.Id,
                voiceChannelId);

            await voiceClient.StartAsync();

            voiceClientService.StoreVoiceClient(voiceChannelId, voiceClient);

            await RespondAsync(InteractionCallback.Message(new InteractionMessageProperties
            {
                Content = "Joined voice channel!",
                Flags = MessageFlags.Ephemeral
            }));
            return;
        }

        await RespondAsync(InteractionCallback.Message(new InteractionMessageProperties
        {
            Content = "I'm already in your voice channel!",
            Flags = MessageFlags.Ephemeral
        }));
    }

    [SlashCommand("leave", "Makes the bot leave your voice channel.")]
    public async Task LeaveVoiceAsync()
    {
        if (!featureFlagSettings.LeaveEnabled)
        {
            await RespondAsync(InteractionCallback.Message(
                new InteractionMessageProperties()
                .WithContent("This command is currently disabled. Please try again later!")
                .WithFlags(MessageFlags.Ephemeral)));
            return;
        }

        logger.Log(LogLevel.Information, null, $"{Context.User.GlobalName} used /leave");

        var client = Context.Client;
        var currentUser = await client.Rest.GetCurrentUserAsync();
        var guild = Context.Guild!;

        if (!guild.VoiceStates.TryGetValue(Context.User.Id, out var userVoiceState))
        {
            await RespondAsync(InteractionCallback.Message(new InteractionMessageProperties
            {
                Content = "You are not in a voice channel. Please join a voice channel and then re-run the command.",
                Flags = MessageFlags.Ephemeral
            }));
            return;
        }

        if (!guild.VoiceStates.TryGetValue(currentUser.Id, out var botVoiceState))
        {
            await RespondAsync(InteractionCallback.Message(new InteractionMessageProperties
            {
                Content = "I am not in a voice channel. This command can only be used to make me leave a voice channel.",
                Flags = MessageFlags.Ephemeral
            }));
            return;
        }

        var userVoiceChannelId = userVoiceState.ChannelId.GetValueOrDefault();
        var botVoiceChannelId = botVoiceState.ChannelId.GetValueOrDefault();

        if (userVoiceChannelId != botVoiceChannelId)
        {
            await RespondAsync(InteractionCallback.Message(new InteractionMessageProperties
            {
                Content = "You must be in the same voice channel as me to make me leave.",
                Flags = MessageFlags.Ephemeral
            }));
            return;
        }

        if (voiceClientService.TryRemoveVoiceClient(botVoiceChannelId, out var voiceClient))
        {
            await voiceClient!.CloseAsync();
            await client.UpdateVoiceStateAsync(new VoiceStateProperties(guild.Id, null));
            voiceClient.Dispose();
            await RespondAsync(InteractionCallback.Message(new InteractionMessageProperties
            {
                Content = "Left voice channel!",
                Flags = MessageFlags.Ephemeral
            }));
        }
    }
}