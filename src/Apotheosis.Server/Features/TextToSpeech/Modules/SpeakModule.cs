﻿using Apotheosis.Core.Features.Audio.Interfaces;
using Apotheosis.Core.Features.TextToSpeech.Interfaces;
using Apotheosis.Server.Features.Audio.Services;
using Microsoft.Extensions.Logging;
using NetCord.Gateway.Voice;

namespace Apotheosis.Server.Features.TextToSpeech.Modules;

public sealed class SpeakModule(
    IAudioStreamService audioStreamService,
    ILogger<SpeakModule> logger,
    ITextToSpeechService textToSpeechService,
    VoiceClientService voiceClientService,
    IOptions<FeatureFlagSettings> featureFlagOptions)
    : ApplicationCommandModule<SlashCommandContext>
{
    readonly FeatureFlagSettings _featureFlagSettings = featureFlagOptions.Value;

    [SlashCommand("speak", "Invite the bot to speak a prompt using a voice of your choice.")]
    public async Task SpeakAsync([SlashCommandParameter(Name = "voice_name")] string voiceName, string prompt)
    {
        if (!_featureFlagSettings.SpeakEnabled)
        {
            await RespondAsync(InteractionCallback.Message(
                new InteractionMessageProperties()
                .WithContent("This command is currently disabled. Please try again later!")
                .WithFlags(MessageFlags.Ephemeral)));
            return;
        }

        logger.LogInformation("{User} used /speak {VoiceName} {Prompt}", Context.User.GlobalName, voiceName, prompt);

        var voices = await textToSpeechService.GetVoicesAsync();
        var voiceId = voices.FirstOrDefault(v => v.VoiceName?.Equals(voiceName, StringComparison.OrdinalIgnoreCase) ?? false)?.VoiceId;

        if (string.IsNullOrWhiteSpace(voiceId))
        {
            await RespondAsync(InteractionCallback.Message(
                new InteractionMessageProperties
                {
                    Content = $"{voiceName} is not valid. Please try again with a valid voice name. A list of voices can be retrieved with the /voices command",
                    Flags = MessageFlags.Ephemeral
                }));
            return;
        }

        var client = Context.Client;
        var guild = Context.Guild!;

        await RespondAsync(InteractionCallback.DeferredMessage(MessageFlags.Ephemeral));
        var voiceStream = await textToSpeechService.GenerateSpeechFromPromptAsync(prompt, voiceId);
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
                    new($"{voiceName}_speech.mp3", voiceStream)
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
            Content = $"Playing speech with prompt \"{prompt}\" using voice \"{voiceName}\"",
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

        if (voiceClientService.TryRemoveVoiceClient(voiceChannelId, out var removedVoiceClient))
        {
            await removedVoiceClient!.CloseAsync();
            await client.UpdateVoiceStateAsync(new VoiceStateProperties(guild.Id, null));
            removedVoiceClient.Dispose();
        }
    }

    [SlashCommand("voices", "Get the list of all supported voices for the /speak and /converse commands.")]
    public async Task GetVoicesAsync()
    {
        if (!_featureFlagSettings.VoicesEnabled)
        {
            await RespondAsync(InteractionCallback.Message(
                new InteractionMessageProperties()
                .WithContent("This command is currently disabled. Please try again later!")
                .WithFlags(MessageFlags.Ephemeral)));
            return;
        }

        logger.LogInformation("{User} used /voices", Context.User.GlobalName);

        var voices = await textToSpeechService.GetVoicesAsync();

        var voicesString = $"**Available Voices:** {string.Join(", ", voices.Select(v => v.VoiceName))}";
        await RespondAsync(InteractionCallback.Message(new InteractionMessageProperties
        {
            Content = voicesString,
            Flags = MessageFlags.Ephemeral
        }));
    }
}