﻿using Apotheosis.Core.Features.Audio.Interfaces;
using Apotheosis.Core.Features.Converse.Interfaces;
using Apotheosis.Core.Features.TextToSpeech.Interfaces;
using Apotheosis.Server.Features.Audio.Services;
using Microsoft.Extensions.Logging;
using NetCord.Gateway.Voice;

namespace Apotheosis.Server.Features.Converse.Modules;

public sealed class ConverseModule(
    IAudioStreamService audioStreamService,
    IConverseService converseService,
    ILogger<ConverseModule> logger,
    ITextToSpeechService textToSpeechService,
    VoiceClientService voiceClientService,
    IOptions<FeatureFlagSettings> featureFlagOptions)
    : ApplicationCommandModule<SlashCommandContext>
{
    readonly FeatureFlagSettings _featureFlagSettings = featureFlagOptions.Value;

    [SlashCommand("converse", "Invite the bot to respond to a prompt using a voice of your choice.")]
    public async Task ConverseAsync([SlashCommandParameter(Name = "voice_name")] string voiceName, string prompt)
    {
        if (!_featureFlagSettings.ConverseEnabled)
        {
            await RespondAsync(InteractionCallback.Message(
                new InteractionMessageProperties()
                .WithContent("This command is currently disabled. Please try again later!")
                .WithFlags(MessageFlags.Ephemeral)));
            return;
        }

        await RespondAsync(InteractionCallback.DeferredMessage(MessageFlags.Ephemeral));

        logger.LogInformation("{User} used /converse {VoiceName} {Prompt}", Context.User.GlobalName, voiceName, prompt);
        var voices = await textToSpeechService.GetVoicesAsync();
        var voiceId = voices.FirstOrDefault(v => v.VoiceName?.Equals(voiceName, StringComparison.OrdinalIgnoreCase) ?? false)?.VoiceId;

        if (string.IsNullOrWhiteSpace(voiceId))
        {
            await FollowupAsync(new InteractionMessageProperties
                {
                    Content = $"{voiceName} is not valid. Please try again with a valid voice name. A list of voices can be retrieved with the /voices command",
                    Flags = MessageFlags.Ephemeral
                });
            return;
        }

        var client = Context.Client;
        var guild = Context.Guild!;

        var voiceStream = await converseService.GenerateConverseResponseFromPromptAsync(prompt, voiceName, voiceId, CancellationToken.None);
        if (!guild.VoiceStates.TryGetValue(Context.User.Id, out var userVoiceState))
        {
            await FollowupAsync(new InteractionMessageProperties
            {
                Content = $"Generated speech with prompt \"{prompt}\" using voice \"{voiceName}\"",
                Flags = MessageFlags.Ephemeral
            });
            await FollowupAsync(new InteractionMessageProperties
            {
                Attachments = new List<AttachmentProperties>
                {
                    new($"{voiceName}_response.mp3", voiceStream)
                }
            });
            return;
        }

        var currentUser = await client.Rest.GetCurrentUserAsync();
        var voiceChannelId = userVoiceState.ChannelId.GetValueOrDefault();
        if (guild.VoiceStates.TryGetValue(currentUser.Id, out var botVoiceState))
        {
            var botVoiceChannelId = botVoiceState.ChannelId.GetValueOrDefault();
            if (voiceChannelId != botVoiceChannelId)
            {
                await FollowupAsync(new InteractionMessageProperties
                {
                    Content = "I'm currently in another voice channel right now. Please wait until I'm done to invite me to speak.",
                    Flags = MessageFlags.Ephemeral
                });
                return;
            }
        }

        if (!voiceClientService.TryGetVoiceClient(voiceChannelId, out var voiceClient))
        {
            voiceClient = await client.JoinVoiceChannelAsync(
                guild.Id,
                voiceChannelId);

            voiceClientService.StoreVoiceClient(voiceChannelId, voiceClient);

            await voiceClient.StartAsync();
        }

        await voiceClient!.EnterSpeakingStateAsync(SpeakingFlags.Microphone);

        await FollowupAsync(new InteractionMessageProperties
        {
            Content = $"Playing response to prompt \"{prompt}\" using voice \"{voiceName}\"",
            Flags = MessageFlags.Ephemeral
        });

        await using var outStream = voiceClient.CreateOutputStream();
        using var ffmpeg = audioStreamService.CreateStreamProcessAsync()!;
        await using var input = ffmpeg.StandardInput.BaseStream;
        await using var output = ffmpeg.StandardOutput.BaseStream;
        await using var opusStream = new OpusEncodeStream(outStream, PcmFormat.Short, VoiceChannels.Stereo, OpusApplication.Audio);

        await voiceStream.CopyToAsync(input);
        ffmpeg.StandardInput.Close();
        await output.CopyToAsync(opusStream);
        await opusStream.FlushAsync();

        if (voiceClientService.TryRemoveVoiceClient(botVoiceState!.ChannelId.GetValueOrDefault(), out var removedVoiceClient))
        {
            await removedVoiceClient!.CloseAsync();
            await client.UpdateVoiceStateAsync(new VoiceStateProperties(guild.Id, null));
            removedVoiceClient.Dispose();
        }
    }
}